﻿using System;
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
        /// Creates an empty Spreadsheet
        /// In a new spreadsheet, the contents of every cell is the empty string.
        /// </summary>
        public Spreadsheet()
        {
            cells = new Dictionary<string, Cell>();
            dGraph = new DependencyGraph();
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells()
        {
            // The logic here is that if the value is not equal to empty string, 
            // we will return the key corrseponding to that.
            foreach(var kv in cells)
            {
                if (!Object.Equals("", kv.Value.Content))
                    yield return kv.Key ;
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
        public override ISet<String> SetCellContents(String name, double number)
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
        public override ISet<String> SetCellContents(String name, String text)
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
        public override ISet<String> SetCellContents(String name, Formula formula)
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
            if(content.GetType() == typeof(Formula))
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
                if(oldContent != null)
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
