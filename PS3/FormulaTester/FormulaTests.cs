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
            Formula f1 = new Formula("x");
            f1 = new Formula(" ");
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

        /// <summary>
        /// When reading tokens from left to right, at no point 
        /// should the number of closing parentheses seen so far 
        /// be greater than the number of opening parentheses 
        /// seen so far.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestRightParenRule()
        {
            Assert.Fail();
        }

        /// <summary>
        /// The total number of opening parentheses 
        /// must equal the total number of closing parentheses.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestBalancedParenRule()
        {
            Assert.Fail();
        }

        /// <summary>
        /// The first token of an expression must be a number, 
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestStartingTokenRule()
        {
            Assert.Fail();
        }

        /// <summary>
        /// The last token of an expression must be a number, 
        /// a variable, or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestEndingTokenRule()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestParenthesisFollowingRule()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing 
        /// parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestExtraFollowingRule()
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