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
        /// If, valid formulas are given to constructor, and it should not throw exception.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestParsingValidTokens1()
        {
            // All of these must not throw exceptions, hence, this test should pass.
            Formula f1 = new Formula("x_ + _123 + _ * 3.49");
            f1 = new Formula("99");
            f1 = new Formula("123.4e+47 + a2 + b3 + a99 + c6 + .99 + 0.99 + 99.99e99 * 99.99e-99/9999+e99");
            f1 = new Formula("(___ + myVar99 / yourVar24) + (2*x) + 3*x*y*z*(hello /world)");
        }

        /// <summary>
        /// Verify that the only tokens are (, ), +, -, *, /, 
        /// variables, and floating-point numbers.
        /// Invalid tokens must throw exception.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParsingValidTokens2()
        {
            Formula f2 = new Formula("#"); //invalid symbol
        }


        /// <summary>
        /// Verify that the only tokens are (, ), +, -, *, /, 
        /// variables, and floating-point numbers.
        /// Invalid tokens must throw exception.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParsingValidTokens3()
        {
            Formula f3 = new Formula("#(((x1+y1) + y2 + z3)"); //invalid symbol #
        }


        /// <summary>
        /// Verify that the only tokens are (, ), +, -, *, /, 
        /// variables, and floating-point numbers.
        /// Invalid tokens must throw exception.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParsingValidTokens4()
        {
            Formula f4 = new Formula("x1 - b!+ 3"); //invalid symbol !
        }


        /// <summary>
        /// Verify that the only tokens are (, ), +, -, *, /, 
        /// variables, and floating-point numbers.
        /// Invalid tokens must throw exception.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParsingValidTokens5()
        {
            Formula f5 = new Formula("a#%$~!@#^^$$"); //invalid symbols
        }

        /// <summary>
        /// Verify that the only tokens are (, ), +, -, *, /, 
        /// variables, and floating-point numbers.
        /// Invalid tokens must throw exception.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParsingValidTokens6()
        {
            Formula f6 = new Formula("x$ - y$"); //invalid symbols
        }


        /// <summary>
        /// Verifies One Token Rule
        /// There must be at least one token.
        /// Throws exception if does not contain at least one token.
        /// </summary> 
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestAtLeastOneToken1()
        {
            Formula f1 = new Formula("");
        }

        /// <summary>
        /// Verifies One Token Rule
        /// There must be at least one token.
        /// Throws exception if does not contain at least one token.
        /// </summary> 
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestAtLeastOneToken2()
        {
            Formula f7 = new Formula("          ");
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