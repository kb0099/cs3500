﻿using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.IO;
using System.Threading;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SpreadsheetTest
{


    /// <summary>
    ///This is a test class for SpreadsheetTest and is intended
    ///to contain all SpreadsheetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpreadsheetTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }

        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        /// <summary>
        ///A test for GetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1", "Testing text entry.");
            target.SetContentsOfCell("b2", "5");
            target.SetContentsOfCell("c3", "=6/3");

            Assert.AreEqual("Testing text entry.", target.GetCellContents("a1"));
            Assert.AreEqual(5.0, target.GetCellContents("b2"));
            Assert.AreEqual(new Formula("6/3", s => s, s => true), target.GetCellContents("c3"));

            Assert.AreEqual("", target.GetCellContents("d4"));

            target.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest01()
        {
            Spreadsheet target = new Spreadsheet(); // TODO: Initialize to an appropriate value
            target.SetContentsOfCell("a1", "Testing text entry.");
            target.SetContentsOfCell("b2", "5");
            target.SetContentsOfCell("c3", "=6/3");

            target.GetCellContents("a7b");
        }

      

        /// <summary>
        ///A test for GetNamesOfAllNonemptyCells
        ///</summary>
        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest()
        {
            Spreadsheet target = new Spreadsheet();
            HashSet<String> expected = new HashSet<string>();
            expected.Add("b2");
            expected.Add("c3");
            expected.Add("d4");
            target.SetContentsOfCell("b2", "=1+a1");
            target.SetContentsOfCell("c3", "=a1*a1");
            target.SetContentsOfCell("d4", "5");
            IEnumerable<string> actual;
            actual = target.GetNamesOfAllNonemptyCells();

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));
        }

        /// <summary>
        ///A test for GetNamesOfAllNonemptyCells
        ///</summary>
        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest01()
        {
            Spreadsheet target = new Spreadsheet();
            IEnumerable<string> actual;
            actual = target.GetNamesOfAllNonemptyCells();

            foreach (String s in actual)
                Assert.Fail();
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleTest()
        {
            Spreadsheet target = new Spreadsheet();
            HashSet<String> expected = new HashSet<String> { "a1" };
            ISet<string> actual = target.SetContentsOfCell("a1", "4");

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            target.SetContentsOfCell("b2", "=1+a1");
            actual = target.SetContentsOfCell("a1", "6");
            expected = new HashSet<String> { "a1", "b2" };

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            Assert.AreEqual(6.0, (double)target.GetCellContents("a1"), .0000000001);

            target.SetContentsOfCell(null, "5");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleTest01()
        {
            Spreadsheet target = new Spreadsheet();

            target.SetContentsOfCell("a1a", "5");
        }

       
        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsFormulaTest()
        {
            Spreadsheet target = new Spreadsheet();
            HashSet<String> expected = new HashSet<String> { "a1" };
            ISet<string> actual = target.SetContentsOfCell("a1", "=5+2");

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            target.SetContentsOfCell("b2", "=1+a1");
            actual = target.SetContentsOfCell("a1", "=4+2");
            expected = new HashSet<String> { "a1", "b2" };

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            Assert.AreEqual(new Formula("4 + 2", s => s, s => true), target.GetCellContents("a1"));

            target.SetContentsOfCell("c3", "=b2+c3");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaTest01()
        {
            Spreadsheet target = new Spreadsheet();
            HashSet<String> expected = new HashSet<String> { "a1" };
            target.SetContentsOfCell("a1", "test");
            ISet<string> actual = target.SetContentsOfCell("a1", "=5+2");

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            target = new Spreadsheet();
            target.SetContentsOfCell(null, "=6+2");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaTest02()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1a", "=6+2");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsFormulaTest03()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1", null);
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsFormulaTest04()
        {
            Spreadsheet target = new Spreadsheet();

            target.SetContentsOfCell("b2", "=1+a1");
            target.SetContentsOfCell("a1", "=4+2");
            target.SetContentsOfCell("c3", "=b2+1");
            target.SetContentsOfCell("a1", "=c3+1");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTextTest()
        {
            Spreadsheet target = new Spreadsheet();
            ISet<string> actual = target.SetContentsOfCell("a1", "Testing");
            HashSet<String> expected = new HashSet<String> { "a1" };

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            target.SetContentsOfCell("b2", "=a1+1");
            actual = target.SetContentsOfCell("a1", "replace");
            expected = new HashSet<String> { "a1", "b2" };

            foreach (String s in actual)
                Assert.IsTrue(expected.Contains(s));

            Assert.AreEqual("replace", target.GetCellContents("a1"));

            target.SetContentsOfCell(null, "hello");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTextTest01()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1", "5");
            target.SetContentsOfCell("a1", "");

            foreach (String s in target.GetNamesOfAllNonemptyCells())
                Assert.Fail();

            target.SetContentsOfCell("a1a", "hello");
        }

        /// <summary>
        ///A test for SetCellContents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsTextTest02()
        {
            Spreadsheet target = new Spreadsheet();
            target.SetContentsOfCell("a1", "5");
            target.SetContentsOfCell("b2", "=a1");
            target.SetContentsOfCell("a1", "");

            foreach (String s in target.GetNamesOfAllNonemptyCells())
                Assert.AreEqual("b2", s);

            String temp = null;
            target.SetContentsOfCell("a1", temp);
        }

        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("AA");
        }

        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1A", "1.5");
        }

        [TestMethod()]
        public void Test6()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test11()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (String)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test13()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "=2");
        }

        [TestMethod()]
        public void Test14()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("Z7", "=3");
            Formula f = (Formula)s1.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3", s => s, s => true), f);
            Assert.AreNotEqual(new Formula("2", s => s, s => true), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test17()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test18()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test21()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test22()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test23()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SetEquals(new HashSet<string>() { "A1" }));
        }

        [TestMethod()]
        public void Test25()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test26()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SetEquals(new HashSet<string>() { "C1" }));
        }

        [TestMethod()]
        public void Test27()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SetEquals(new HashSet<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test29()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test30()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23", z => z, b => true), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24", z => z, b => true), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }
        [TestMethod()]
        public void Test32()
        {
            Test31();
        }
        [TestMethod()]
        public void Test33()
        {
            Test31();
        }
        [TestMethod()]
        public void Test34()
        {
            Test31();
        }

        [TestMethod()]
        public void Test35()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i + 1))));
            }
        }
        [TestMethod()]
        public void Test36()
        {
            Test35();
        }
        [TestMethod()]
        public void Test37()
        {
            Test35();
        }
        [TestMethod()]
        public void Test38()
        {
            Test35();
        }
        [TestMethod()]
        public void Test39()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try
            {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        [TestMethod()]
        public void Test43()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }
            HashSet<string> firstCells = new HashSet<string>();
            HashSet<string> lastCells = new HashSet<string>();
            for (int i = 0; i < 250; i++)
            {
                firstCells.Add("A1" + i);
                lastCells.Add("A1" + (i + 250));
            }
            Assert.IsTrue(s.SetContentsOfCell("A1249", "25.0").SetEquals(firstCells));
            Assert.IsTrue(s.SetContentsOfCell("A1499", "0").SetEquals(lastCells));
        }
        [TestMethod()]
        public void Test44()
        {
            Test43();
        }
        [TestMethod()]
        public void Test45()
        {
            Test43();
        }
        [TestMethod()]
        public void Test46()
        {
            Test43();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Arg3ConstructorTest()
        {
            Spreadsheet target = new Spreadsheet(s => s.Length < 10, b => b.ToUpper(), "v1.0");

            target.SetContentsOfCell("a1", "5");
            Assert.AreEqual(5.0, (double)target.GetCellContents("a1"), .000000001);

            target.SetContentsOfCell("abcdefghijkl10343", "hello");
        }

        [TestMethod()]
        public void Arg3ConstructorTest01()
        {
            Spreadsheet target = new Spreadsheet(s => s.Length < 10, b => b.ToUpper(), "v1.0");
            String expectedPath = @"expected01.xml";
            String newPath = "TestSave01.xml";

            target.SetContentsOfCell("a1", "5");
            target.SetContentsOfCell("b2", "hello");
            target.SetContentsOfCell("c3", "=a1");
            target.Save(newPath);

            String expected = File.ReadAllText(expectedPath);
            String actual = Regex.Replace(File.ReadAllText(newPath), @">\s+<", "><");
            System.Diagnostics.Debug.WriteLine(actual);
            File.WriteAllText("actual_for_expected01.xml", actual);        
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg3ConstructorTest02()
        {
            Spreadsheet target = new Spreadsheet(s => s.Length < 10, b => b.ToUpper(), "v1.0");
            String newPath = @"M:\TestSave02.xml";

            target.SetContentsOfCell("a1", "5");
            target.Save(newPath);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg3ConstructorTest03()
        {
            Spreadsheet target = new Spreadsheet(s => s.Length < 10, b => b.ToUpper(), "v1.0");
            String newPath = @"M:\";

            target.SetContentsOfCell("a1", "5");
            target.Save(newPath);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg3ConstructorTest04()
        {
            Spreadsheet target = new Spreadsheet(s => s.Length < 10, b => b.ToUpper(), "v1.0");
            String newPath = @"C:\DNE\";

            target.SetContentsOfCell("a1", "5");
            target.Save(newPath);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Arg4ConstructorTest()
        {
            Spreadsheet target = new Spreadsheet(@"Test4ArgConst.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");

            target.SetContentsOfCell("d4", "=5");
            Assert.AreEqual(5.0, (double)target.GetCellValue("a1"), .000000001);
            Assert.AreEqual(5.0, (double)target.GetCellValue("d4"), .000000001);
            Assert.AreEqual(5.0, (double)target.GetCellValue("D4"), .000000001);

            target.SetContentsOfCell("abcdefghijkl10343", "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg4ConstructorTest01()
        {
            Spreadsheet target = new Spreadsheet(null, s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg4ConstructorTest02()
        {
            Spreadsheet target = new Spreadsheet(@"BadCellName.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg4ConstructorTest03()
        {
            Spreadsheet target = new Spreadsheet(@"BadFormula.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg4ConstructorTest04()
        {
            Spreadsheet target = new Spreadsheet(@"CircularDependency.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Arg4ConstructorTest05()
        {
            Spreadsheet target = new Spreadsheet(@"Test4ArgConst.xml", s => s.Length < 10, b => b.ToUpper(), "v2.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest01()
        {
            Spreadsheet target = new Spreadsheet(@"Empty.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest02()
        {
            Spreadsheet target = new Spreadsheet(@"OnlyHeader.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest03()
        {
            Spreadsheet target = new Spreadsheet(@"InvalidStartElement.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest04()
        {
            Spreadsheet target = new Spreadsheet(@"NotSpreadsheetElement.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest05()
        {
            Spreadsheet target = new Spreadsheet(@"", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest06()
        {
            Spreadsheet target = new Spreadsheet(@"M:\DNE.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionTest07()
        {
            Spreadsheet target = new Spreadsheet(@"C:\DNE.xml", s => s.Length < 10, b => b.ToUpper(), "v1.0");
        }

        [TestMethod()]
        public void RecalculateCellValues01()
        {
            Spreadsheet target = new Spreadsheet();

            target.SetContentsOfCell("a1", "5");
            target.SetContentsOfCell("b2", "=a1");

            Assert.AreEqual(5.0, (double)target.GetCellValue("b2"), 0.0000000001);

            target.SetContentsOfCell("a1", "7");

            Assert.AreEqual(7.0, (double)target.GetCellValue("b2"), 0.0000000001);
        }

        [TestMethod()]
        public void RecalculateCellValues02()
        {
            Spreadsheet target = new Spreadsheet();

            target.SetContentsOfCell("a1", "5");
            target.SetContentsOfCell("b2", "=a1+5");
            target.SetContentsOfCell("c3", "=b2+5");
            target.SetContentsOfCell("d4", "=c3+5");

            Assert.AreEqual(10.0, (double)target.GetCellValue("b2"), 0.0000000001);
            Assert.AreEqual(15.0, (double)target.GetCellValue("c3"), 0.0000000001);
            Assert.AreEqual(20.0, (double)target.GetCellValue("d4"), 0.0000000001);
        }

        // Tests IsValid
        [TestMethod()]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod()]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod()]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod()]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod()]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod()]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod()]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod()]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod()]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod()]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod()]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod()]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod()]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod()]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod()]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("q:\\missing\\save.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s=>s,"");
        }

        [TestMethod()]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s=>s,"default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));            
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s=>s,"");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s=>s,"version");
        }

        [TestMethod()]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s=>s,"hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod()]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s=>s,"");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod()]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod()]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod()]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod()]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod()]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod()]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s=>s,"default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
        [TestMethod()]
        public void LongFormulaTest()
        {
            object result = "";
            Thread t = new Thread(() => LongFormulaHelper(out result));
            t.Start();
            t.Join(60 * 1000);
            if (t.IsAlive)
            {
                t.Abort();
                Assert.Fail("Computation took longer than 60 seconds");
            }
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            Stopwatch s1, s2;
            try
            {
                s1 = Stopwatch.StartNew();
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a0 + a1");
                int i = 0;
                int depth = 200; // change back to 100
                for (i = 0; i < depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                s1.Stop();
                s2 = Stopwatch.StartNew();
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1e-1);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), s.GetCellValue("sum1"));
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 1e-1);
                s2.Stop();
                Debug.WriteLine($"s1 = {s1.ElapsedMilliseconds} \ns2 = {s2.ElapsedMilliseconds}");
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

    }
}

