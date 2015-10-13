using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpreadsheetUtilities;

namespace SSTests
{
    /// <summary>
    /// Represents a test class for SS.Spreadsheet class.
    /// </summary>
    [TestClass()]
    public class SpreadsheetTests
    {
        /// <summary>
        /// An empty spreadsheet should have infinite number of cells
        /// Each cell contains empty string initially.
        /// </summary>
        [TestMethod()]
        public void SpreadsheetTest()
        {
            Spreadsheet s1 = new Spreadsheet();
            Assert.AreEqual("", s1.GetCellContents("a1"));
            Assert.AreEqual("", s1.GetCellContents("x_"));
            Assert.AreEqual("", s1.GetCellContents("_"));

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
            p1.Invoke("GetDirectDependents", new string[]{null});
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
            HashSet<string> directPendents = new HashSet<string>((IEnumerable<string>)p1.Invoke("GetDirectDependents", new string[] {"A1"}));
            Assert.IsTrue(directPendents.SetEquals(new HashSet<string>() { "B1", "C1" }));
        }
    }
}