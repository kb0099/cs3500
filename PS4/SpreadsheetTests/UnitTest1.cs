using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Test that constructor creates an empty spreadsheet.
        /// Uses GetNamesOfAllNonemptyCells() to prove it.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual(0, new HashSet<string>(ss.GetNamesOfAllNonemptyCells()).Count);
        }

        /// <summary>
        /// Test that GetNamesOfAllNonemptyCells() enumerates names of all nonempty cells in Spreadsheet.
        /// Uses SetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // Add cells, assert that they get added
            ss.SetCellContents("A1", 2);
            Assert.AreEqual(new HashSet<string>() { "A1" }, ss.GetNamesOfAllNonemptyCells());
            ss.SetCellContents("B1", 7);
            Assert.AreEqual(new HashSet<string>() { "A1", "B1" }, ss.GetNamesOfAllNonemptyCells());
            ss.SetCellContents("C3", new Formula("92-2"));
            Assert.AreEqual(new HashSet<string>() { "A1", "B1", "C3" }, ss.GetNamesOfAllNonemptyCells());
            ss.SetCellContents("D5", new Formula("A1*B1"));
            Assert.AreEqual(new HashSet<string>() { "A1", "B1", "C3", "D5" }, ss.GetNamesOfAllNonemptyCells());
            ss.SetCellContents("E2", "Bill");
            Assert.AreEqual(new HashSet<string>() { "A1", "B1", "C3", "D5", "E2" }, ss.GetNamesOfAllNonemptyCells());
            // Reset a cell, assert that nothing changed
            ss.SetCellContents("D5", "Katie");
            Assert.AreEqual(new HashSet<string>() { "A1", "B1", "C3", "D5", "E2" }, ss.GetNamesOfAllNonemptyCells());
            // Add an empty cell, assert that nothing changed
            ss.SetCellContents("F22", "");
            Assert.AreEqual(new HashSet<string>() { "A1", "B1", "C3", "D5", "E2" }, ss.GetNamesOfAllNonemptyCells());
            // Reset a cell to empty, assert that the cell is not included
            ss.SetCellContents("C3", "");
            Assert.AreEqual(new HashSet<string>() { "A1", "B3", "D5", "E2" }, ss.GetNamesOfAllNonemptyCells());
        }

        /// <summary>
        /// Test that GetCellContents() returns a string for a cell that contains a string.
        /// Uses SetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // Add cells
            ss.SetCellContents("A1", 2);
            ss.SetCellContents("B1", 7);
            ss.SetCellContents("C3", new Formula("92-2"));
            ss.SetCellContents("D5", new Formula("A1*B1"));
            ss.SetCellContents("E2", "Bill");
            ss.SetCellContents("F5", "Francis");
            // assert the string cells return strings
            Assert.IsTrue(ss.GetCellContents("E2") is string);
            Assert.IsTrue(ss.GetCellContents("F5") is string);
            // assert the non-string cells don't return strings
            Assert.IsFalse(ss.GetCellContents("A1") is string);
            Assert.IsFalse(ss.GetCellContents("B1") is string);
            Assert.IsFalse(ss.GetCellContents("C3") is string);
            Assert.IsFalse(ss.GetCellContents("D5") is string);
        }

        /// <summary>
        /// Test that GetCellContents() returns a double for a cell that contains a double.
        /// Uses SetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // Add cells
            ss.SetCellContents("A1", 2);
            ss.SetCellContents("B1", 7);
            ss.SetCellContents("C3", new Formula("92-2"));
            ss.SetCellContents("D5", new Formula("A1*B1"));
            ss.SetCellContents("E2", "Bill");
            ss.SetCellContents("F5", "Francis");
            // assert the double cells return doubles
            Assert.IsTrue(ss.GetCellContents("A1") is double);
            Assert.IsTrue(ss.GetCellContents("B1") is double);
            // assert the non-double cells don't return doubles
            Assert.IsFalse(ss.GetCellContents("C3") is double);
            Assert.IsFalse(ss.GetCellContents("D5") is double);
            Assert.IsFalse(ss.GetCellContents("E2") is double);
            Assert.IsFalse(ss.GetCellContents("F5") is double);
        }

        /// <summary>
        /// Test that GetCellContents() returns a Formula for a cell that contains a Formula.
        /// Uses SetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents3()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // Add cells
            ss.SetCellContents("A1", 2);
            ss.SetCellContents("B1", 7);
            ss.SetCellContents("C3", new Formula("92-2"));
            ss.SetCellContents("D5", new Formula("A1*B1"));
            ss.SetCellContents("E2", "Bill");
            ss.SetCellContents("F5", "Francis");
            // assert the Formula cells return Formulas
            Assert.IsTrue(ss.GetCellContents("C3") is Formula);
            Assert.IsTrue(ss.GetCellContents("D5") is Formula);
            // assert the non-Formula cells don't return Formuals
            Assert.IsFalse(ss.GetCellContents("A1") is Formula);
            Assert.IsFalse(ss.GetCellContents("B1") is Formula);
            Assert.IsFalse(ss.GetCellContents("E2") is Formula);
            Assert.IsFalse(ss.GetCellContents("F5") is Formula);
        }

        /// <summary>
        /// Test that GetCellContents() returns the contents of the cell.
        /// Uses SetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // Add cells
            ss.SetCellContents("A1", 2);
            ss.SetCellContents("B1", 7);
            ss.SetCellContents("C3", new Formula("92-2"));
            ss.SetCellContents("D5", new Formula("A1*B1"));
            ss.SetCellContents("E2", "Bill");
            ss.SetCellContents("F5", "Francis");
            // assert the string cells return the string contained
            Assert.AreEqual("Bill", ss.GetCellContents("E2"));
            Assert.AreEqual("Francis", ss.GetCellContents("F5"));
            // assert the double cells return the contained double
            Assert.AreEqual(2, ss.GetCellContents("A1"));
            Assert.AreEqual(7, ss.GetCellContents("B1"));
            // assert the Formula cells return the contained Formula
            Assert.AreEqual(new Formula("92-2"), ss.GetCellContents("C3"));
            Assert.AreEqual(new Formula("A1*B1"), ss.GetCellContents("D5"));
        }

        /// <summary>
        /// Test that GetCellContents() returns an empty string when calling for an empty cell.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // assert that an empty cell returns a blank string
            Assert.AreEqual("", ss.GetCellContents("B52"));
        }

        /// <summary>
        /// Test that GetCellContents() throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsException1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetCellContents(null);
        }

        /// <summary>
        /// Test that GetCellContents() throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "25".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsException20()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetCellContents("25");
        }

        /// <summary>
        /// Test that GetCellContents() throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "2x".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsException21()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetCellContents("2x");
        }

        /// <summary>
        /// Test that GetCellContents() throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "&amp;".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsException22()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.GetCellContents("&");
        }

        /// <summary>
        /// Test that SetCellContents(name,number) sets the cells' contents to a number.
        /// Uses GetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsNumber1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // assert with several number cells
            ss.SetCellContents("A1", 5);
            Assert.AreEqual(5, ss.GetCellContents("A1"));
            ss.SetCellContents("B1", 12);
            Assert.AreEqual(12, ss.GetCellContents("B1"));
            ss.SetCellContents("C3", -56);
            Assert.AreEqual(-56, ss.GetCellContents("C3"));
            ss.SetCellContents("D2", 0);
            Assert.AreEqual(0, ss.GetCellContents("D2"));
            ss.SetCellContents("E5", double.PositiveInfinity);
            Assert.AreEqual(double.PositiveInfinity, ss.GetCellContents("E5"));
        }

        /// <summary>
        /// Test that SetCellContents(name,number) returns a set of cell names that are the given cell and its dependents (direct and indirect).
        /// Uses SetCellContents(name,formula) to prove it.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsNumber2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // create formulas with dependencies
            ss.SetCellContents("A1", new Formula("F6"));
            ss.SetCellContents("B1", new Formula("A1 - 4"));
            ss.SetCellContents("C3", new Formula("B1*A1"));
            ss.SetCellContents("D5", new Formula("C3/2"));
            ss.SetCellContents("E2", new Formula("B52-2*5"));
            // assert the set contains expected dependents
            Assert.AreEqual(new HashSet<string>() { "F6", "A1", "B1", "C3", "D5" }, ss.SetCellContents("F6", 20));
        }

        /// <summary>
        /// Test that SetCellContents(name,number) throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNumberException1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents(null, 20);
        }

        /// <summary>
        /// Test that SetCellContents(name,number) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "25".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNumberException20()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("25", 20);
        }

        /// <summary>
        /// Test that SetCellContents(name,number) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "2x".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNumberException21()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("2x", 20);
        }

        /// <summary>
        /// Test that SetCellContents(name,number) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "&amp;".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNumberException22()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("&", 20);
        }

        /// <summary>
        /// Test that SetCellContents(name,text) sets the cells' contents to a string.
        /// Uses GetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsText1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // assert with several text cells
            ss.SetCellContents("A1", "Bill");
            Assert.AreEqual("Bill", ss.GetCellContents("A1"));
            ss.SetCellContents("B1", "Tom");
            Assert.AreEqual("Tom", ss.GetCellContents("B1"));
            ss.SetCellContents("C3", "L33T_H4X0R");
            Assert.AreEqual("L33T_H4X0R", ss.GetCellContents("C3"));
            ss.SetCellContents("D5", "");
            Assert.AreEqual("", ss.GetCellContents("D5"));
        }

        /// <summary>
        /// Test that SetCellContents(name,text) returns a set of cell names that are the given cell and its dependents (direct and indirect).
        /// Uses SetCellContents(name,formula) to prove it.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsText2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // create formulas with dependencies
            ss.SetCellContents("A1", new Formula("F6"));
            ss.SetCellContents("B1", new Formula("A1 - 4"));
            ss.SetCellContents("C3", new Formula("B1*A1"));
            ss.SetCellContents("D5", new Formula("C3/2"));
            ss.SetCellContents("E2", new Formula("B52-2*5"));
            // assert the set contains expected dependents
            Assert.AreEqual(new HashSet<string>() { "F6", "A1", "B1", "C3", "D5" }, ss.SetCellContents("F6", "Brady"));
        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an ArgumentNullException when given a null text.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsTextException1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            string text = null;
            ss.SetCellContents("A1", text);
        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsTextException2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents(null, "Bill");
        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "25".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsTextException30()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("25", "Bill");
        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "2x".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsTextException31()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("2x", "Bill");
        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "&amp;".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsTextException32()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("&", "Bill");
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) sets the cells' contents to a formula.
        /// Uses GetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // assert with several Formula cells
            ss.SetCellContents("A1", new Formula("20 - 3*5"));
            Assert.AreEqual(new Formula("20-3*5"), ss.GetCellContents("A1"));
            ss.SetCellContents("B1", new Formula("A1*3"));
            Assert.AreEqual(new Formula("A1*3"), ss.GetCellContents("B1"));
            ss.SetCellContents("C3", new Formula("5"));
            Assert.AreEqual(new Formula("5"), ss.GetCellContents("C3"));
            ss.SetCellContents("D5", new Formula("B1 + C3"));
            Assert.AreEqual(new Formula("B1+C3"), ss.GetCellContents("D5"));
            ss.SetCellContents("E2", new Formula("F7*9"));
            Assert.AreEqual(new Formula("F7*9"), ss.GetCellContents("E2"));
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) returns a set of cell names that are the given cell and its dependents (direct and indirect).
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            // create formulas with dependencies
            ss.SetCellContents("A1", new Formula("F6"));
            ss.SetCellContents("B1", new Formula("A1 - 4"));
            ss.SetCellContents("C3", new Formula("B1*A1"));
            ss.SetCellContents("D5", new Formula("C3/2"));
            ss.SetCellContents("E2", new Formula("B52-2*5"));
            // assert the set contains expected dependents
            Assert.AreEqual(new HashSet<string>() { "F6", "A1", "B1", "C3", "D5" }, ss.SetCellContents("F6", new Formula("7")));
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws an ArugmentNullException when given a null formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsFormulaException1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula formula = null;
            ss.SetCellContents("A1", formula);
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaException2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents(null, new Formula("5"));
        }

        /// <summary>
        /// Test that SetCellContets(name,formula) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "25".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaException30()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("25", new Formula("5"));
        }

        /// <summary>
        /// Test that SetCellContets(name,formula) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "2x".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaException31()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("2x", new Formula("5"));
        }

        /// <summary>
        /// Test that SetCellContets(name,formula) throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "&amp;".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaException32()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("&", new Formula("5"));
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws a CircularException when formula would cause a circular dependency.
        /// The circular dependency will be direct
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsFormulaException40()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", new Formula("A1"));
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws a CircularException when formula would cause a circular dependency.
        /// The circular dependency will be indirect.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsFormulaException41()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", new Formula("B1"));
            ss.SetCellContents("B1", new Formula("A1"));
        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throwing a CircularException does not change the spreadsheet.
        /// Uses GetCellContents() to prove it.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormulaException42()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            try
            {
                ss.SetCellContents("A1", new Formula("A1"));
                Assert.Fail("Didn't throw CircularException, so can't check changes to spreadsheet");
            }
            catch (CircularException)
            {
                Assert.AreEqual("", ss.GetCellContents("A1"));
            }
        }

        /// <summary>
        /// Test that GetDirectDependents() returns an enumeration without duplicates.
        /// Uses SpreadsheetWrapper in place of Spreadsheet.
        /// Uses SetCellContents(name,formula) to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependents1()
        {
            SpreadsheetWrapper ss = new SpreadsheetWrapper();
            // create several formula cells
            ss.SetCellContents("B1", new Formula("A1*A1"));
            ss.SetCellContents("C3", new Formula("B1 + A1"));
            ss.SetCellContents("D5", new Formula("B1 - C1"));
            IEnumerable<string> dents = ss.GetDirectDependentsWrapper("A1");
            HashSet<string> set = new HashSet<string>();
            foreach (string d in dents)
            {
                Assert.IsTrue(set.Add(d), "Had duplicates for {" + d + "}");
            }
        }

        /// <summary>
        /// Test that GetDirectDependents() returns an enumeration of all direct dependents of name.
        /// Uses SpreadsheetWrapper in place of Spreadsheet.
        /// Uses SetCellContents(name,formula) to prove it.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependents2()
        {
            SpreadsheetWrapper ss = new SpreadsheetWrapper();
            ss.SetCellContents("B1", new Formula("A1*A1"));
            ss.SetCellContents("C3", new Formula("B1 + A1"));
            ss.SetCellContents("D5", new Formula("B1 - C1"));
            Assert.IsTrue(new HashSet<string>() { "B1", "C1" }.SetEquals(ss.GetDirectDependentsWrapper("A1")));
        }

        /// <summary>
        /// Test that GetDirectDependents() throws an ArgumentNullException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDirectDependentsException1()
        {
            SpreadsheetWrapper ss = new SpreadsheetWrapper();
            ss.GetDirectDependentsWrapper(null);
        }

        /// <summary>
        /// Test that GetDirectDependents() throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "25".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetDirectDependentsException20()
        {
            SpreadsheetWrapper ss = new SpreadsheetWrapper();
            ss.GetDirectDependentsWrapper("25");
        }

        /// <summary>
        /// Test that GetDirectDependents() throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "2x".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetDirectDependentsException21()
        {
            SpreadsheetWrapper ss = new SpreadsheetWrapper();
            ss.GetDirectDependentsWrapper("2x");
        }

        /// <summary>
        /// Test that GetDirectDependents() throws an InvalidNameException when given an invalid name.
        /// The invalid name used is "&amp;".
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetDirectDependentsException22()
        {
            SpreadsheetWrapper ss = new SpreadsheetWrapper();
            ss.GetDirectDependentsWrapper("&");
        }
    }

    /// <summary>
    /// The purpose of this class is to test the protected method GetDirectDependents() by making an operable wrapper.
    /// </summary>
    public class SpreadsheetWrapper : Spreadsheet
    {
        public SpreadsheetWrapper()
            : base()
        {

        }

        internal IEnumerable<string> GetDirectDependentsWrapper(string p)
        {
            return base.GetDirectDependents(p);
        }
    }
}
