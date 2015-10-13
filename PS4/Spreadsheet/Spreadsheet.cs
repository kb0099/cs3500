﻿using System;
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
        public Spreadsheet() : this(null, s => true, s => s, "default")
        {
        }

        /// <summary>
        /// In addition to creating an empty Spreadsheet it will allow custom Validator,
        /// Normalizer, and VersionString to be sent at the time of construction.
        /// </summary>
        public Spreadsheet(Func<string, bool> ValidityDelegate, Func<string, string> NormalizeDelegate, string VersionString)
            : this(null, ValidityDelegate, NormalizeDelegate, VersionString)
        {
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
              : base(ValidityDelegate, NormalizeDelegate, VersionString)
        {
            Changed = false;
            cells = new Dictionary<string, Cell>();
            dGraph = new DependencyGraph();
            if (PathToFile != null)
            {
                XDocument xmlDoc;
                try
                {
                    xmlDoc = XDocument.Load(PathToFile);
                }
                catch(System.IO.FileNotFoundException fnfe) {
                    throw new SpreadsheetReadWriteException($"The file {PathToFile} could not be located. \n{fnfe.Message}");
                }
                catch (System.Xml.XmlException xex)
                {
                    throw new SpreadsheetReadWriteException($"Error while reading: {PathToFile}.\n{xex.Message}");
                }
                
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
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
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
            ValidateName(name);
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
            ValidateName(name);     // ensures whether name is valid
            if (cells.Keys.Contains(name))
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
        /// The direct dependents(this should be dependees if following previous specificatoin ps3/ps2 etc) 
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
        private static string VAR_PATTERN = @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$";

        /// <summary>
        /// Represents a helper method to throw exception.
        /// </summary>
        /// <param name="name"></param>
        private void ValidateName(string name)
        {
            if (name == null || !Regex.IsMatch(name, VAR_PATTERN))
                throw new InvalidNameException();
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
            if (!cells.ContainsKey(name))
                cells.Add(name, null);
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
                return new HashSet<string>(GetCellsToRecalculate(name));   // could throw circular exception for Formula type                
            }
            catch (CircularException ce)
            {
                // undo the changes : this is basically setting to old content
                if (oldContent != null)
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
