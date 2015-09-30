// Implemented by Mitchell Terry

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// An Spreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&amp;" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
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
        /// A DependencyGraph to track all dependency pairs made by Formula cells.
        /// Dependents(s) would have s be a cell name and return the cell variables that cell contains.
        /// Dependees(s) would have s be a cell variable and return cells that contain that cell variable.
        /// </summary>
        private DependencyGraph depGraph;
        /// <summary>
        /// A Dictionary of cell names and Cells to track all nonempty cells.
        /// The dictionary shouldn't store empty cells.
        /// </summary>
        private Dictionary<string, Cell> cells;

        /// <summary>
        /// Create an empty Spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            // Initialize depGraph and cells
            depGraph = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        /// <exception cref="InvalidNameException"></exception>
        public override object GetCellContents(string name)
        {
            // Exception checks
            if (name == null) throw new InvalidNameException();
            if (!isValidName(name)) throw new InvalidNameException();

            // The cell may be an empty cell, so look out for a KeyNotFoundException when accessing the cell
            try
            {
                Cell ce = cells[name];
                return ce.Contents;
            }
            catch (KeyNotFoundException)
            {
                // The cell was empty, so return an empty string
                return "";
            }
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
        /// <exception cref="InvalidNameException"></exception>
        public override ISet<string> SetCellContents(string name, double number)
        {
            // Exception checks
            if (name == null) throw new InvalidNameException();
            if (!isValidName(name)) throw new InvalidNameException();

            // If the cell is already holding a formula, its dependents need to be removed; do this by replacing the dependents with an empty HashSet
            if (cells.ContainsKey(name) && cells[name].isFormula()) depGraph.ReplaceDependents(name, new HashSet<string>());

            // Set the cell contents
            cells[name] = new Cell(number);

            // GetCellsToRecalculate() is able to return the name and all cells that directly and indirectly depend on name, so return its output
            return new HashSet<string>(GetCellsToRecalculate(name));
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        public override ISet<string> SetCellContents(string name, string text)
        {
            // Exception checks
            if (text == null) throw new ArgumentNullException();
            if (name == null) throw new InvalidNameException();
            if (!isValidName(name)) throw new InvalidNameException();

            // If the cell is already holding a formula, its dependents need to be removed; do this by replacing the dependents with an empty HashSet
            if (cells.ContainsKey(name) && cells[name].isFormula()) depGraph.ReplaceDependents(name, new HashSet<string>());

            // If text is an empty string, then an empty cell must be made; this means clearing out the cell from cells
            // Otherwise, set the cell contents
            if (text == "") cells.Remove(name);
            else cells[name] = new Cell(text);

            // GetCellsToRecalculate() is able to return the name and all cells that directly and indirectly depend on name, so return its output
            return new HashSet<string>(GetCellsToRecalculate(name));
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        /// <exception cref="CircularException"></exception>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            // Exception checks
            if (formula == null) throw new ArgumentNullException();
            if (name == null) throw new InvalidNameException();
            if (!isValidName(name)) throw new InvalidNameException();

            // Store the old content temporarily
            Cell temp = null;
            if (cells.ContainsKey(name)) temp = cells[name];
            // Set the cell contents
            cells[name] = new Cell(formula);
            // Add the new dependencies, which can overwrite old dependencies
            depGraph.ReplaceDependents(name, formula.GetVariables());

            // Use GetCellsToRecalculate(name) to find circular dependency since it throws a CircularException when it finds one
            IEnumerable<string> output;
            try
            {
                output = GetCellsToRecalculate(name);
            }
            catch (CircularException e)
            {
                // A circular dependency was found; reset the cell contents by using the temp, then re-throw the exception
                if (temp == null) cells.Remove(name); // The cell was an empty cell before
                else cells[name] = temp;
                throw e;
            }

            // GetCellsToRecalculate() is able to return the name and all cells that directly and indirectly depend on name, so return its output
            return new HashSet<string>(output);
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
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // Exception checks
            if (name == null) throw new ArgumentNullException();
            if (!isValidName(name)) throw new InvalidNameException();

            // Return dependees(name), which will be cells that contain formulas using the named cell
            return depGraph.GetDependees(name);
        }

        /// <summary>
        /// A check that the given name is valid.
        /// A name is a valid cell name if and only if:
        ///   (1) its first character is an underscore or a letter
        ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
        /// </summary>
        private bool isValidName(string name)
        {
            // Check in case of empty string given
            if (name == string.Empty) return false;
            // Check that the first character is a letter or underscore
            if (!char.IsLetter(name[0]) && name[0] != '_') return false;
            // Get substring of rest of variable
            string sub = name.Substring(1);
            // Check if sub consists of letters, numbers, and/or digits
            foreach (char c in sub)
            {
                if (!char.IsLetterOrDigit(c) && c != '_') return false;
            }
            // The string did not fail checks, so return true
            return true;
        }

        /// <summary>
        /// A Cell has contents and value, but PS4 does not need value management to be implemented.
        /// Cells can store a string, a double, or a Formula.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// Construct the cell with the given contents.
            /// </summary>
            public Cell(object contents)
            {
                this.Contents = contents;
            }

            /// <summary>
            /// The content contained. Can be a string, a double, or a Formula.
            /// Can only get this value, not set it.
            /// </summary>
            public object Contents
            {
                get;
                private set;
            }

            /// <summary>
            /// A check that the cell contains a string.
            /// </summary>
            public bool isString()
            {
                return Contents is string;
            }

            /// <summary>
            /// A check that the cell contains a double.
            /// </summary>
            public bool isDouble()
            {
                return Contents is double;
            }

            /// <summary>
            /// A check that the cell contains a Formula.
            /// </summary>
            public bool isFormula()
            {
                return Contents is Formula;
            }

            /// <summary>
            /// A check that the cell is an empty cell
            /// </summary>
            /// <returns></returns>
            public bool isEmptyCell()
            {
                return Contents == "";
            }
        }
    }
}
