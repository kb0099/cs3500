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
        /// Use GetNamesOfAllNonemptyCells() to prove it.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual(0, new HashSet<string>(ss.GetNamesOfAllNonemptyCells()).Count);
        }

        /// <summary>
        /// Test that GetNamesOfAllNonemptyCells() enumerates names of all nonempty cells in Spreadsheet.
        /// Use SetCellContents() to prove it.
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
        /// </summary>
        [TestMethod]
        public void TestGetCellContents1()
        {

        }

        /// <summary>
        /// Test that GetCellContents() returns a double for a cell that contains a double.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents2()
        {

        }

        /// <summary>
        /// Test that GetCellContents() returns a Formula for a cell that contains a Formula.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents3()
        {

        }

        /// <summary>
        /// Test that GetCellContents() returns an empty string when calling for a cell that has not been set content yet.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents4()
        {

        }

        /// <summary>
        /// Test that GetCellContents() throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsException1()
        {

        }

        /// <summary>
        /// Test that GetCellContents() throws an InvalidNameException when given an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsException2()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,number) sets the cells' contents to a number.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsNumber1()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,number) returns a set of cell names that are the given cell and its dependents (direct and indirect).
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsNumber2()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,number) throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNumberException1()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,number) throws an InvalidNameException when given an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNumberException2()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,text) sets the cells' contents to a string.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsText1()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,text) returns a set of cell names that are the given cell and its dependents (direct and indirect).
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsText2()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an ArgumentNullException when given a null text.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsTextException1()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsTextException2()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,text) throws an InvalidNameException when given an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsTextException3()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,formula) sets the cells' contents to a formula.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormula1()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,formula) returns a set of cell names that are the given cell and its dependents (direct and indirect).
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsFormula2()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws an ArugmentNullException when given a null formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsFormulaException1()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws an InvalidNameException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaException2()
        {

        }

        /// <summary>
        /// Test that SetCellContets(name,formula) throws an InvalidNameException when given an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaException3()
        {

        }

        /// <summary>
        /// Test that SetCellContents(name,formula) throws a CircularException when formula would cause a circular dependency.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsFormulaException4()
        {

        }

        /// <summary>
        /// Test that GetDirectDependents() returns an enumeration without duplicates.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependents1()
        {

        }

        /// <summary>
        /// Test that GetDirectDependents() returns an enumeration of all direct dependents of name.
        /// </summary>
        [TestMethod]
        public void TestGetDirectDependents2()
        {

        }

        /// <summary>
        /// Test that GetDirectDependents() throws an ArgumentNullException when given a null name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDirectDependentsException1()
        {

        }

        /// <summary>
        /// Test that GetDirectDependents() throws an InvalidNameException when given an invalid name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetDirectDependentsException2()
        {

        }
    }
}
