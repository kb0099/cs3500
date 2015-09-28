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
        /// The invalid name used is "&".
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
