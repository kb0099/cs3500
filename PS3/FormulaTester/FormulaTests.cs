using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents a test class for Formula class
    /// </summary>
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
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestRightParenRule()
        {
            Formula f1 = new Formula("(((1+2)-3)*2)))");
        }

        /// <summary>
        /// The total number of opening parentheses 
        /// must equal the total number of closing parentheses.
        /// </summary>
        [TestMethod()]
        public void PublicFormulaTestBalancedParenRule()
        {
            // should pass
            Formula f1 = new Formula("(1+3)*(4+5)-(((6+10)*2)*2)");
        }

        /// <summary>
        /// The first token of an expression must be a number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestStartingTokenRule1()
        {
            Formula f1 = new Formula(" + 9 - 3");
        }

        /// <summary>
        /// The first token of an expression must be a number, a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestStartingTokenRule2()
        {
            Formula f2 = new Formula(" * 23 / 44 + x1");
        }

        /// <summary>
        /// The last token of an expression must be a number, 
        /// a variable, or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestEndingTokenRule1()
        {
            Formula f1 = new Formula("(x1 + x2 + (");
        }

        /// <summary>
        /// The last token of an expression must be a number, 
        /// a variable, or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestEndingTokenRule2()
        {
            Formula f1 = new Formula("99 - 23 + 1.602e-19/");
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule1()
        {
            Formula f1 = new Formula("()");

        }/// <summary>
         /// Any token that immediately follows an opening parenthesis or
         /// an operator must be either a number,
         /// a variable, or an opening parenthesis.
         /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule2()
        {
            Formula f2 = new Formula("a + 99.00e+2 + (-");
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule3()
        {
            Formula f3 = new Formula("2 + (*");
            Formula f4 = new Formula("x1 + y1 + (/ + z3*y10");
            Formula f5 = new Formula("((h1 + w2) + (+ 3 + 99))");
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule4()
        {
            Formula f4 = new Formula("x1 + y1 + (/ + z3*y10");
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule5()
        {
            Formula f5 = new Formula("((h1 + w2) + (+ 3 + 99))");
        }


        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule6()
        {
            Formula f6 = new Formula("9e-9 + + 2 * x1 + y1");
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or
        /// an operator must be either a number,
        /// a variable, or an opening parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestParenthesisFollowingRule7()
        {
            Formula f7 = new Formula("x1 / ) + 3.0");
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing 
        /// parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestExtraFollowingRule1()
        {
            Formula f1 = new Formula("9.0 (2 + b2) * 9");
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing 
        /// parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestExtraFollowingRule2()
        {
            Formula f2 = new Formula("(x1+y1)(x1-y1)");
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing 
        /// parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestExtraFollowingRule3()
        {
            Formula f3 = new Formula("1.11e-34 3.14 - x1");
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing 
        /// parenthesis must be either an operator or a closing parenthesis.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicFormulaTestExtraFollowingRule4()
        {
            Formula f4 = new Formula("3.14 * (r1 r1)");
        }

        /// <summary>
        /// Tests the Evaluate public method
        /// </summary>
        [TestMethod()]
        public void PublicEvaluateTest()
        {
            Assert.AreEqual(new Formula("1.2 + 3.4").Evaluate(s => 0), 4.6);
        }

        /// <summary>
        /// Tests the GetVariables method.
        /// </summary>
        [TestMethod()]
        public void PublicGetVariablesTest()
        {
            Formula f1 = new Formula("(x1+y1)/2.0");
            Assert.IsTrue(new HashSet<string>(f1.GetVariables()).SetEquals(new HashSet<string>() { "x1", "y1" }));

            Formula f2 = new Formula("a1 * b1 - c1 * A1 /C1 + 99.99e-99", s => s.ToUpper(), s => true);
            Assert.IsTrue(new HashSet<string>(f2.GetVariables()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));

            // without normalization
            Formula f3 = new Formula("a1 * b1 - c1 * A1 /C1 + 99.99e-99");
            Assert.IsTrue(new HashSet<string>(f3.GetVariables()).SetEquals(new HashSet<string>() { "a1", "b1", "c1", "A1", "C1" }));
        }

        /// <summary>
        /// Tests the ToString method.
        /// </summary>
        [TestMethod()]
        public void PublicToStringTest()
        {
            Formula f1 = new Formula("(x1+y1)/2.0");
            Assert.AreEqual("(x1+y1)/2.0", f1.ToString());

            Formula f2 = new Formula("a1 * b1 - c1 * A1 /C1 + 99.99e-99", s => s.ToUpper(), s => true);
            Assert.AreEqual("A1*B1-C1*A1/C1+99.99e-99", f2.ToString());

            Formula f3 = new Formula("a1 * b1 - c1 * A1 /C1 + 99.99e-99");
            Assert.AreEqual("a1*b1-c1*A1/C1+99.99e-99", f3.ToString());

        }

        /// <summary>
        /// Tests two instance of formula for equality
        /// </summary>
        [TestMethod()]
        public void PublicEqualsTest()
        {
            Formula f1 = new Formula("3.14*pi*r*r");
            Formula f2 = new Formula(" 3.14 * pi * r * r ");

            Assert.IsTrue(f1.Equals(f2));
        }

        /// <summary>
        /// Tests the GetHashCode method
        /// </summary>
        [TestMethod()]
        public void PublicGetHashCodeTest()
        {
            Formula f1 = new Formula("3.14*pi*r*r");
            Formula f2 = new Formula(" 3.14 * pi * r * r ");

            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
            Assert.IsTrue(f1.GetHashCode() == new Formula("0.314e+1*pi*r*r").GetHashCode());
        }

        // Below are comprehensive tests which will try to cover the parts that could have been missed in individual tests.

        /// <summary>
        /// Comprehensive Test2
        /// Maximal coverage test to expect FormulaFError
        /// </summary>
        [TestMethod()]
        public void PublicComprehensiveTest0()
        {
            Formula f = new Formula("a1+c1+2");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Lookup failed"); }), typeof(FormulaError));
        }

        /// <summary>
        /// Comprehensive Test2: Tries to cover maximum possible different kinds of tests
        /// Maximal coverage test Assert
        /// </summary>
        [TestMethod()]
        public void PublicComprehensiveTest1()
        {
            Formula f = new Formula("(x1+y1)/(x1-x2)");
            Assert.IsInstanceOfType(f.Evaluate(s => 99), typeof(FormulaError));
            Assert.IsInstanceOfType(new Formula("99/00").Evaluate(s=>0), typeof(FormulaError));
            f = new Formula("9.99 * x3/10.0 -11e-27");
            Assert.IsTrue(f.Equals(new Formula(" 999e-2 *    x3/10 - 11.00e-27")));
            Assert.IsFalse(f.Equals(new Formula(" 999e-2 *    x3/10 - 11.00e-27 + 4 * z3")));
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals("99"));

            Formula f2 = new Formula("a1+b2");

            Assert.IsFalse(f2 == null);
            Assert.IsFalse(f == f2);
            Assert.IsTrue(f2 != null);
            Assert.IsFalse(f == null);

            //Should not fail
            Formula stress = new Formula("(((0.0000001e-1+(2-x1)+3.0e+33)*45-z)/10.00-a1)+67*z1 - 90+z2/y1-m3+0.00000000334");
            Assert.IsFalse(new Formula("99").GetVariables().GetEnumerator().MoveNext());

            Formula f3 = new Formula("100+x1+y2+z3");
            Formula f4 = new Formula("500-300");
            Assert.IsFalse(f3 == f4);
            Assert.IsTrue(f3.ToString().IndexOf("100") >= 0);
            Assert.IsTrue(f3.ToString().IndexOf("x1") > 0);
            Assert.IsTrue(f3.ToString().IndexOf("y2") > 0);
            Assert.IsTrue(f3.ToString().IndexOf("z3") > 0);
        }

        /// <summary>
        /// Comprehensive Test3
        /// Maximal coverage test expected formula format exception
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicComprehensiveTest2()
        {
            Formula f = new Formula("(((((x1+y1)/(x1-x2)");
            f = new Formula("**/sldfj99023982034-=02023503850");
            f = new Formula("x1+-222282-234+334-(((--2234-234343eee-982*e2-z3*39847293+xx2234@");
        }
    }
}