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
            
            Assert.Fail();
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        [TestMethod()]
        public void GetCellContentsTest()
        {
            Assert.Fail();
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
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsTest1()
        {
            Spreadsheet s1 = new Spreadsheet();
            s1.SetCellContents("abc", 10);
            Assert.AreEqual((double)s1.GetCellContents("abc"), 10);
            new Spreadsheet().SetCellContents("a-1", 99);       // invalid cell name must throw
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
        [TestMethod()]
        public void SetCellContentsTest2()
        {
            Assert.Fail();
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
        [TestMethod()]
        public void SetCellContentsTest3()
        {
            Assert.Fail();
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
        [TestMethod()]
        public void GetDirectDependentsTest()
        {

        }
    }
}