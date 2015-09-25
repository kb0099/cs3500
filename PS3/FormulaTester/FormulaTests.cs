using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    [TestClass()]
    public class FormulaTests
    {
        /// <summary>
        /// Verify that the only tokens are (, ), +, -, *, /, 
        /// variables, and floating-point numbers.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestParsing()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Verifies One Token Rule
        /// There must be at least one token.
        /// </summary> 
        [TestMethod()]
        public void PublicFormulaTestAtLeastOneToken()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PublicEvaluateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PublicGetVariablesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PublicToStringTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PublicEqualsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PublicGetHashCodeTest()
        {
            Assert.Fail();
        }
        
    }
}