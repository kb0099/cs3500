using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpreadsheetUtilities;
using System.Xml.Linq;

namespace SSTests
{
    /// <summary>
    /// Represents a test class for SS.Spreadsheet class.
    /// </summary>
    [TestClass()]
    public class SpreadsheetTests
    {
        // All the previous tests from PS4 are at the bottom and new tests are being added from top to down:

        /// <summary>      
        /// If any of the names contained in the saved spreadsheet are invalid, an exception should be thrown.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void ConstructorTest01()
        {
            Spreadsheet ss = new Spreadsheet(s => s.Length < 5, b => b.ToUpper(), "test");
            ss.SetContentsOfCell("x1", "99");
            Assert.AreEqual(99.0, (double)ss.GetCellContents("X1"));

            ss.SetContentsOfCell("invalidlengtth99", "=(3.14*r1*r1)");     // invalidated by constraint
        }

        /// <summary>
        /// Tests for all read-write exceptions.
        /// kb_err*.xml files have malformed xml, and problematic formulas.
        /// </summary>
        [TestMethod()]
        public void ConstructorTest02()
        {
            RWHelper("../../testfiles/kb_err01.xml");
            RWHelper("../../testfiles/kb_err02.xml");
            RWHelper("../../testfiles/kb_err03.xml");
            RWHelper("../../testfiles/kb_err04.xml");
            RWHelper("../../testfiles/kb_err05.xml");
            RWHelper("../../testfiles/kb_err06.xml");
            RWHelper("../../testfiles/kb_err07.xml");
            RWHelper("../../testfiles/kb_err08.xml");
            RWHelper("../../testfiles/kb_err09.xml");

            //Valid spreadsheet, but zero cells.
            //Should not throw exceptions or anything
            Spreadsheet ss = new Spreadsheet("../../testfiles/kb_nocells.xml", s=> true, s=>s, "test");
            Assert.IsTrue(ss.GetNamesOfAllNonemptyCells().Count() == 0);
        }
        /// <summary>
        /// Helper for ConstructorTest02, to indicate it failed to throw exceptions.
        /// </summary>
        public void RWHelper(string file)
        {
            Spreadsheet s1;
            try
            {
                s1 = new Spreadsheet(file, s => true, s => s, "test");
                Assert.Fail($"For file: {file}, Did not throw expected: SpreadsheetReadWriteException");
            }
            catch (SpreadsheetReadWriteException) { }
        }

        // Multiple tests for save() with invalid paths

        [TestMethod()]
        public void SaveTest01()
        {
            Spreadsheet ss = new Spreadsheet(s => s.Length < 10, s => s, "test");

            ss.SetContentsOfCell("a1", "9");
            ss.SetContentsOfCell("b1", "10");
            ss.SetContentsOfCell("c1", "=a1+b1");
            ss.Save("../../testfiles/kb01_1.xml");

            // "../../testfiles/kb01.xml" is pre-written and saved in the /bin/degug directory for the SpreadsheetTests project
            XDocument expected = XDocument.Load("../../testfiles/kb01.xml");       
            XDocument actual = XDocument.Load("../../testfiles/kb01_1.xml");

            Assert.IsTrue(XDocument.DeepEquals(expected, actual));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest02()
        {
            Spreadsheet ss = new Spreadsheet(s => s.Length < 10, s => s, "test");

            ss.SetContentsOfCell("a1", "9");
            ss.SetContentsOfCell("b1", "10");
            ss.SetContentsOfCell("c1", "=a1+b1");
            ss.Save("../../testfiles/ZZ:\\kb01_2.xml");     // invalid path
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest03()
        {
            Spreadsheet ss = new Spreadsheet(s => s.Length < 10, s => s, "test");

            ss.SetContentsOfCell("a1", "9");
            ss.SetContentsOfCell("b1", "10");
            ss.SetContentsOfCell("c1", "=a1+b1");
            ss.Save("c:\\");     // incomplete path
        }


        // formula error, division by zero etc.
        [TestMethod()]
        public void DviByZeroTest()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "2.0");
            ss.SetContentsOfCell("b1", "0.0");
            ss.SetContentsOfCell("c1", "=a1/b1");

            Assert.IsInstanceOfType(ss.GetCellValue("c1"), typeof(FormulaError));
        }
        // un-initialized/empty cells for formula
        [TestMethod()]
        public void UninitializedTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("x3", "=9 + b4 - c99");
            ss.SetContentsOfCell("x4", "=x1 - x2");

            Assert.IsInstanceOfType(ss.GetCellValue("x3"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("x4"), typeof(FormulaError));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "2.0");
            ss.SetContentsOfCell("b1", "0.0");
            ss.SetContentsOfCell("a0", "Grocery Bill");
            ss.SetContentsOfCell("x3", "=9 + b4 - c99");
            ss.SetContentsOfCell("x4", "=x1 - x2");
            ss.SetContentsOfCell("x9", "=a1");

            Assert.AreEqual(ss.GetCellValue("x9"), ss.GetCellValue("a1"));
            Assert.IsInstanceOfType(ss.GetCellValue("x3"), typeof(FormulaError));
            Assert.AreEqual("Grocery Bill", ss.GetCellContents("a0"));
            Assert.AreEqual(2.0, ss.GetCellContents("a1"));
            Assert.AreEqual(new Formula("x1-x2", s=>s, s=> true), ss.GetCellContents("x4"));

            // Catches the first lets to throw the next
            try
            {
                ss.GetCellContents("this-is-invalid99#Cell");
            }catch(InvalidNameException)
            {
                ss.GetCellContents(null);
            }

        }

        /// <summary>
        /// Timing Test: ensures that cell operations/reading/writing should be done within reasonable time.
        /// Mimics a fibonacci sequence! 
        /// In doing so, it tests for the rigitidy of the save method.
        /// Makes sure save method saves correctly and constructor can read back accurately!
        /// </summary>
        [TestMethod()]
        public void TimingTest()
        {
            Spreadsheet ss = new Spreadsheet();
            var s1 = System.Diagnostics.Stopwatch.StartNew();
            for(int i = 2; i < 901; i++)
            {
                ss.SetContentsOfCell("a" + i, "=a" + (i - 2) + " + a" + (i - 1));
            }
            ss.SetContentsOfCell("a0", "" + 0);
            ss.SetContentsOfCell("a1", "" + 1);
            s1.Stop();

            WriteValues(ss);
            Spreadsheet resurrected = new Spreadsheet("../../testfiles/fibo.xml", s => true, s => s, "fibo");
            resurrected.Save("../../testfiles/fibo_.xml");

            XDocument.DeepEquals(XDocument.Load("../../testfiles/fibo.xml"), XDocument.Load("fibo_.xml"));
            Assert.IsTrue(s1.ElapsedMilliseconds < 15*1000);    // should not take more than 15 seconds!
        }
        // helper for TimingTest()
        public void WriteValues(Spreadsheet ss)
        {
            XElement spreadsheet = new XElement("spreadsheet", new XAttribute("version", "fibo"),          /* root element */
                    from name in ss.GetNamesOfAllNonemptyCells()                 /* We are saving only the non-empty cells! */
                    select new XElement("cell",
                      new XElement("name", name),                             /* <name> element */
                      new XElement("contents", ss.GetCellValue(name)))       /* saving VALUES HERE! */
                    );
            spreadsheet.Save("../../testfiles/fibo.xml");
        }
        //


        /// <summary>
        /// Two spreadsheets should not interfere with one another's data
        /// </summary>
        [TestMethod()]
        public void IndependencyTest()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            s1.SetContentsOfCell("a1", "100");
            s2.SetContentsOfCell("a1", "Invoice");

            Assert.AreEqual(s1.GetCellValue("a1"), 100.0);
            Assert.AreEqual(s2.GetCellValue("a1"), "Invoice");
        }

        /// <summary>
        /// The Normalized delegate should be used for all calls
        /// </summary>
        [TestMethod()]
        public void NormalizationTest()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "test");
            ss.SetContentsOfCell("a1", "9");
            ss.SetContentsOfCell("A1", "99E2");
            ss.SetContentsOfCell("aBc01", "BookStore");

            Assert.AreEqual(99E2, (double)ss.GetCellValue("A1"));
            Assert.AreEqual("BookStore", ss.GetCellValue("AbC01"));
        }

        /// <summary>
        /// If setting to a new content throws circular exception, 
        /// the previous state should not be affected.
        /// </summary>
        [TestMethod()]
        public void CircularTest()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("B1", "9");
            try
            {
                s1.SetContentsOfCell("A1", "=B1 + C1");
                s1.SetContentsOfCell("B1", "=D1-E1*F1/G1-99*A1");
                Assert.Fail("Did not throw Circular Exception!");
            }
            catch (CircularException)
            {
                Assert.AreEqual(s1.GetCellValue("B1"), 9.0);
                Assert.AreEqual(s1.GetCellContents("A1"), new Formula("B1+C1"));
            }
        }















        /// <summary>
        /// An empty spreadsheet should have infinite number of cells
        /// Each cell contains empty string initially.
        /// </summary>
        [TestMethod()]
        public void SpreadsheetTest()
        {
            Spreadsheet s1 = new Spreadsheet();
            Assert.AreEqual("", s1.GetCellContents("a1"));
            Assert.AreEqual("", s1.GetCellContents("x3"));
            Assert.AreEqual("", s1.GetCellContents("b4"));

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        [TestMethod()]
        public void GetNamesOfAllNonemptyCellsTest()
        {
            Spreadsheet s1 = new Spreadsheet();
            Assert.IsTrue(new HashSet<string>(s1.GetNamesOfAllNonemptyCells()).Count == 0);     // should be empty!
            s1.SetContentsOfCell("a1", "10");
            s1.SetContentsOfCell("b1", "100");
            s1.SetContentsOfCell("c1", "Rate");
            s1.SetContentsOfCell("a2", "=a1-b1/100");
            Assert.IsTrue(new HashSet<string>(s1.GetNamesOfAllNonemptyCells()).
                SetEquals(new HashSet<string>() { "a1", "b1", "c1", "a2" }));                     // should contain the names/cell just added

            s1.SetContentsOfCell("a1", "");
            s1.SetContentsOfCell("b1", "");
            s1.SetContentsOfCell("c1", "");
            Assert.IsTrue(new HashSet<string>(s1.GetNamesOfAllNonemptyCells()).
                SetEquals(new HashSet<string>() { "a2" }));

        }

        /// <summary>
        /// If name is null throws an InvalidNameException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest1()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.GetCellContents(null);
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest2()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.GetCellContents("$xyz");
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest3()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.GetCellContents("-1+2");
        }



        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest4()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.GetCellContents("x-y");
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsTest5()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.GetCellContents("a1b1c#");
        }


        /// <summary>
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        [TestMethod()]
        public void GetCellContentsTest6()
        {
            Spreadsheet s1 = new Spreadsheet();
            // Simple bill
            s1.SetContentsOfCell("a1", "Unit Cost");
            s1.SetContentsOfCell("a2", "Quantity");
            s1.SetContentsOfCell("a3", "Net Cost");

            s1.SetContentsOfCell("b1", "10");
            s1.SetContentsOfCell("b2", "9");
            s1.SetContentsOfCell("b3", "=b1*b2");

            s1.SetContentsOfCell("c1", "100");
            s1.SetContentsOfCell("c2", "2");
            s1.SetContentsOfCell("c3", "=c1*c2");

            s1.SetContentsOfCell("d1", "Total Cost");
            s1.SetContentsOfCell("d3", "=a3 + b3 + c3");

            Assert.AreEqual((string)s1.GetCellContents("a1"), "Unit Cost");
            Assert.AreEqual((double)s1.GetCellContents("b1"), 10);
            Assert.AreEqual((double)s1.GetCellContents("c2"), 2);
            Assert.AreEqual((Formula)s1.GetCellContents("d3"), new Formula("a3+b3+c3"));
        }



        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// Test for: SetContentsOfCell(String name, double number)
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCellTest1()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("abc", "10");
            Assert.AreEqual((double)s1.GetCellContents("abc"), 10);
            new Spreadsheet().SetContentsOfCell("", "99");       // empty cell name is invalid, must throw
        }


        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// Test for: SetContentsOfCell(String name, String text)
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTest2()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("abc", (string)null);
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
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCellTest3()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("abc", null);
        }



        /// <summary>
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
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCellTest4_CircularDependency()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("b1", "10");
            s1.SetContentsOfCell("a1", "=b1 + c1");
            s1.SetContentsOfCell("c1", "=a1 * b1");
        }


        // <summary>
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        [TestMethod()]
        public void SetContentsOfCellTest5_CircularDependency()
        {
            Spreadsheet s1 = new Spreadsheet();
            // Simple bill
            s1.SetContentsOfCell("a1", "Unit Cost");
            s1.SetContentsOfCell("a2", "Quantity");
            s1.SetContentsOfCell("a3", "Net Cost");

            s1.SetContentsOfCell("b1", "10");
            s1.SetContentsOfCell("b2", "9");
            s1.SetContentsOfCell("b3", "=b1*b2");

            s1.SetContentsOfCell("c1", "100");
            s1.SetContentsOfCell("c2", "2");
            s1.SetContentsOfCell("c3", "=c1*c2");

            s1.SetContentsOfCell("d1", "Subtotal");
            s1.SetContentsOfCell("e1", "Discount");
            s1.SetContentsOfCell("e3", "=d3*0.15");       // 15 % off the subtotal
            HashSet<string> dependees = new HashSet<string>(s1.SetContentsOfCell("d3", "=a3 + b3 + c3"));

            Assert.IsTrue(dependees.SetEquals(new HashSet<string>() { "d3", "e3" }));      // only "e3" depends on "d3"
        }



        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectDependentsTest1_NameIsNull()
        {
            PrivateObject p1 = new PrivateObject(new Spreadsheet());
            p1.Invoke("GetDirectDependents", new string[] { null });
        }

        /// <summary>
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependentsTest2_InvalidName()
        {
            PrivateObject p1 = new PrivateObject(new Spreadsheet());
            p1.Invoke("GetDirectDependents", new string[] { "b1!" });
        }

        /// <summary>
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
        [TestMethod()]
        public void GetDirectDependentsTest3()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.SetContentsOfCell("A1", "3");
            s1.SetContentsOfCell("B1", "=A1 * A1");
            s1.SetContentsOfCell("C1", "=B1 + A1");
            s1.SetContentsOfCell("D1", "=B1 - C1");

            PrivateObject p1 = new PrivateObject(s1);
            HashSet<string> directPendents = new HashSet<string>((IEnumerable<string>)p1.Invoke("GetDirectDependents", new string[] { "A1" }));
            Assert.IsTrue(directPendents.SetEquals(new HashSet<string>() { "B1", "C1" }));
        }
    }
}