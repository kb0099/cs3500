using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Xml.Linq;

namespace SS
{
    /// <summary>
    /// A Spreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Creates an empty Spreadsheet
        /// In a new spreadsheet, the contents of every cell is the empty string.
        /// The zero-argument constructor will create an empty spreadsheet that
        /// imposes no extra validity conditions, normalizes every cell name 
        /// to itself, and has version "default".
        /// </summary>
        public Spreadsheet() : this(s => true, s => s, "default")
        {
        }

        /// <summary>
        /// In addition to creating an empty Spreadsheet it will allow custom Validator,
        /// Normalizer, and VersionString to be sent at the time of construction.
        /// </summary>
        public Spreadsheet(Func<string, bool> ValidityDelegate, Func<string, string> NormalizeDelegate, string VersionString)
            : base(ValidityDelegate, NormalizeDelegate, VersionString)
        {
            Changed = false;
            cells = new Dictionary<string, Cell>(900);      // Initialize to an equivalent of 30x30 grids
            dGraph = new DependencyGraph();
        }

        /// <summary>
        /// It will allow the user to provide a string representing a path to a file (first parameter), 
        /// a validity delegate (second parameter), a normalization delegate (third parameter), and a 
        /// version (fourth parameter). It will read a saved spreadsheet from a file (see the Save method) 
        /// and use it to construct a new spreadsheet. The new spreadsheet will use the provided validity 
        /// delegate, normalization delegate, and version.
        /// Error Checking/Handling
        /// If anything goes wrong when reading the file, the constructor should throw a SpreadsheetReadWriteException with an explanatory message.
        /// If the version of the saved spreadsheet does not match the version parameter provided to the constructor, an exception should be thrown.
        /// If any of the names contained in the saved spreadsheet are invalid, an exception should be thrown.
        /// If any invalid fomulas or circular dependencies are encountered, an exception should be thrown.
        /// If there are any problems opening, reading, or closing the file, an exception should be thrown.
        /// There are doubtless other things that can go wrong and should be handled appropriately.
        /// </summary>
        public Spreadsheet(string PathToFile, Func<string, bool> ValidityDelegate, Func<string, string> NormalizeDelegate, string VersionString)
              : this(ValidityDelegate, NormalizeDelegate, VersionString)
        {
            XDocument xmlDoc;
            string name = string.Empty, contents = string.Empty;      // currently processing cell's name and contents.
            try
            {
                xmlDoc = XDocument.Load(PathToFile);
                //verify version
                if (!GetSavedVersion(PathToFile).Equals(VersionString))
                    throw new SpreadsheetReadWriteException("The version of the saved spreadsheet does not match the version parameter provided to the constructor");

                //fill in the spreadsheet data
                foreach (XElement cell in xmlDoc.Descendants("cell"))
                {
                    name = cell.Element("name").Value;
                    contents = cell.Element("contents").Value;

                    // Add those values
                    SetContentsOfCell(name, contents);
                }
            }
            catch (System.IO.FileNotFoundException fnfe)
            {
                throw new SpreadsheetReadWriteException($"The provided file path, {PathToFile}, is invalid.\n{fnfe.Message}");
            }
            catch (System.Xml.XmlException xex)
            {
                throw new SpreadsheetReadWriteException($"Error while reading: {PathToFile}.\n{xex.Message}");
            }
            catch (CircularException ce)
            {
                throw new SpreadsheetReadWriteException($"Circular dependency detected while reading: {PathToFile}\nAt cell: {name}, contents: {contents}\n{ce.Message}");
            }
            catch (SpreadsheetReadWriteException srwe)
            {
                throw new SpreadsheetReadWriteException(srwe.Message);      // Re-use the existing descriptive message
            }
            catch (Exception ex)    // Spec says:  There are doubtless other things that can go wrong and should be handled appropriately!
            {
                throw new SpreadsheetReadWriteException($"Error while re-creating spreadsheet object from file: {PathToFile}\n{ex.Message}");
            }


        }
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary> 
        public override string GetSavedVersion(string filename)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(filename);
                if (xmlDoc.Root.Name != "spreadsheet") throw new Exception();
                return xmlDoc.Root.Attribute("version").Value;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error reading the file: " + filename + ", while trying to get version info.");
            }
        }


        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                XElement spreadsheet = new XElement("spreadsheet", new XAttribute("version", Version),          /* root element */
                  from name in GetNamesOfAllNonemptyCells()                 /* We are saving only the non-empty cells! */
                  select new XElement("cell",
                    new XElement("name", name),                             /* <name> element */
                    new XElement("contents", cells[name].XMLContent))       /* <contents> element */
                  );
                spreadsheet.Save(filename);
                Changed = false;        // Now it has been saved!
            }
            catch (System.Xml.XmlException)
            {
                throw new SpreadsheetReadWriteException($"Error occurred while creating a XML file in: {filename}");
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException($"Unexpected error occurred while saving: {filename}\n{ex.GetType()}\n{ex.Message}\nMake sure file path is correct and not write protected!");
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            Validate(name);
            name = Normalize(name);
            if (cells.ContainsKey(name))
            {
                return cells[name].Value;
            }
            return string.Empty;
        }


        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary> 
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
                throw new ArgumentNullException("The content of a cell cannot be null!");
            Validate(name);
            name = Normalize(name);
            double d;
            if (Double.TryParse(content, out d))
            {
                return SetCellContents(name, d);
            }
            else if (content.StartsWith("="))
            {
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            }
            else
            {
                return SetCellContents(name, content);
            }
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells()
        {
            // The logic here is that if the value is not equal to empty string, 
            // we will return the key corrseponding to that.
            foreach (var kv in cells)
            {
                if (!Object.Equals("", kv.Value.Content))
                    yield return kv.Key;
            }
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(String name)
        {
            Validate(name);     // ensures whether name is valid
            name = Normalize(name);
            if (cells.ContainsKey(name))
                return cells[name].Content;
            else
                return string.Empty;          // if the cell doesn't exist it should contain empty string. 
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, double number)
        {
            return SetContentsHelper(name, number);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, String text)
        {
            return SetContentsHelper(name, text);
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, Formula formula)
        {
            return SetContentsHelper(name, formula);
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents(this should be dependees if following previous specification ps3/ps2 etc) 
        /// of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<String> GetDirectDependents(String name)
        {
            if (name == null)
                throw new ArgumentNullException("Cell name cannot be null!");
            if (!Regex.IsMatch(name, VAR_PATTERN))
                throw new InvalidNameException();
            return dGraph.GetDependees(name);    
        }



        // ============================================================================================
        // ======================= Private Members ====================================================
        // ============================================================================================

        /// <summary>
        /// Represents the cells in a Spreadsheet
        /// Maps cell name to cell object.
        /// </summary>
        private Dictionary<string, Cell> cells;

        /// <summary>
        /// Represents a dependency among cells
        /// </summary>
        private DependencyGraph dGraph;

        /// <summary>
        /// pattern for a variable as per assignment specs
        /// </summary>
        private static string VAR_PATTERN = @"^[a-zA-Z]+\d+$";

        /// <summary>
        /// Represents a helper to validate a name, throws exception if not valid.
        /// Variables in the formula class are valid as long as they consist of a letter or underscore followed by zero or more letters, underscores.
        /// Variables for a Spreadsheet are only valid if they are one or more letters followed by one or more digits(numbers). 
        /// </summary>
        /// <param name="name"></param>
        private void Validate(string name)
        {
            if (name == null || !Regex.IsMatch(name, VAR_PATTERN) || !IsValid(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// Tries to get the value of the named cell as a number.
        /// </summary>
        /// <param name="name">Name of the cell</param>
        /// <returns></returns>
        private double Lookup(string name)
        {
            // Any name that comes in should be arleady normalized and validated
            try
            {
                return (double)(cells[name].Value);
            }
            catch
            {
                throw new ArgumentException($"Unable to get value of cell: {name} as a double.");
            }
        }

        /// <summary>
        /// A helper method for SetContents method(s).
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <param name="content">The content of the cell.</param>
        /// <returns>Dependees of the cell name.</returns>
        private ISet<string> SetContentsHelper(string name, object content)
        {
            // first save the old content (in case we need to undo the changes), and try making the changes 
            object oldContent = GetCellContents(name);
            HashSet<string> oldDependents = new HashSet<string>(dGraph.GetDependents(name));        //direct dependents

            // set to new cell content
            cells[name] = new Cell(name, content);

            // remove old direct dependents
            foreach (var od in oldDependents) { dGraph.RemoveDependency(name, od); }

            // if it is a formula object, 
            // add new direct dependents
            if (content.GetType() == typeof(Formula))
            {
                foreach (var nd in ((Formula)content).GetVariables()) { dGraph.AddDependency(name, nd); }
            }

            try
            {
                Changed = true;
                var dependees =  new HashSet<string>(GetCellsToRecalculate(name));   // could throw circular exception for Formula type  
                foreach (var dpndee in dependees)
                    cells[dpndee].Recalc(Lookup);
                return dependees;
            }
            catch (CircularException ce)
            {
                // undo the changes : this is basically setting to old content
                if (oldContent != null && !Object.Equals(string.Empty, oldContent))
                    SetContentsHelper(name, oldContent);
                throw ce;                   // re-throw the same exception ce
            }
        }

        private class Cell
        {
            /// <summary>
            /// Represents the name of the cell, as required by specification.
            /// Name once set by constructor cannot be changed.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Represents the content (not the value) of the cell
            /// It can be either String, Double, or Formula
            /// </summary>
            public object Content { get; set; }

            /// <summary>
            /// Represents the value of the cell
            /// It can be either String, Double, or FormulaError
            /// If a cell's contents is a string, its value is that string.
            /// If a cell's contents is a double, its value is that double.
            /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
            /// as reported by the Evaluate method of the Formula class. 
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// Indicates the Value needs to be Re-calculated.
            /// Must be called explicitly whenever new formula is placed.
            /// </summary>
            public void Recalc(Func<string, double> lookup)
            {
                Value = Content.GetType() == typeof(Formula) ? ((Formula)Content).Evaluate(lookup) : Content;
            }

            /// <summary>           
            /// If the cell contains a string, it should be written as the contents.  
            /// If the cell contains a double d, d.ToString() should be written as the contents.  
            /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
            /// </summary>
            public string XMLContent
            {
                get
                {
                    return (Content.GetType() == typeof(Formula) ? "=" : "") + Content.ToString();
                }
            }
            public Cell(string name) : this(name, "")
            {
            }
            public Cell(string name, Object content)
            {
                Name = name;
                Content = content;
            }
        }
    }
}
