using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;

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
        public Spreadsheet():base(s=>true, s=>s, "default")
        {
        }

        /// <summary>
        /// In addition to creating an empty Spreadsheet it will allow custom Validator,
        /// Normalizer, and VersionString to be sent at the time of construction.
        /// </summary>
        public Spreadsheet(Func<string, bool> ValidityDelegate, Func<string, string> NormalizeDelegate, string VersionString)
            :base(ValidityDelegate, NormalizeDelegate, VersionString)
        {        
        }

        /// <summary>
        /// It will allow the user to provide a string representing a path to a file (first parameter), 
        /// a validity delegate (second parameter), a normalization delegate (third parameter), and a 
        /// version (fourth parameter). It will read a saved spreadsheet from a file (see the Save method) 
        /// and use it to construct a new spreadsheet. The new spreadsheet will use the provided validity 
        /// delegate, normalization delegate, and version.
        /// </summary>
        public Spreadsheet(string PathToFile, Func<string, bool> ValidityDelegate, Func<string, string> NormalizeDelegate, string VersionString)
              : base(ValidityDelegate, NormalizeDelegate, VersionString)
        {
            Changed = false;
            cells = new Dictionary<string, Cell>();
            dGraph = new DependencyGraph();
        }
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get;  protected set;}

        public override object GetCellContents(string name)
        {
            throw new NotImplementedException();
        }

        public override object GetCellValue(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            throw new NotImplementedException();
        }

        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }

        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            throw new NotImplementedException();
        }

        protected override ISet<string> SetCellContents(string name, string text)
        {
            throw new NotImplementedException();
        }

        protected override ISet<string> SetCellContents(string name, double number)
        {
            throw new NotImplementedException();
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
            if (content == null)
                throw new ArgumentNullException("The content of a cell cannot be null!");
            ValidateName(name);

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
