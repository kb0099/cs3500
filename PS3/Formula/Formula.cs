// Skeleton written by Joe Zachary for CS 3500, September 2013
// Full Implementation by Mitchell Terry
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// The normalizer function given.
        /// </summary>
        private Func<string, string> normalizer;
        /// <summary>
        /// The tokens of the formula given.
        /// </summary>
        private List<string> tokens;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        /// <exception cref="FormulaFormatException"></exception>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        /// <exception cref="FormulaFormatException"></exception>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // Store the normalizer function
            normalizer = normalize;
            // Get the tokens of the formula
            tokens = new List<string>(GetTokens(formula));
            // Check that syntax is correct, and that no invalid tokens are given
            // If there are no tokens, throw FormulaFormatException
            if (tokens.Count == 0) throw new FormulaFormatException("There must be at least one token to the formula");
            // First token must be a number, variable, or opening parentheses
            if (!isNumber(tokens[0]) && !isPossibleVariable(tokens[0]) && tokens[0] != "(") throw new FormulaFormatException("First token must be a number, variable, or opening parentheses");
            // Last token must be a number, variable, or closing parentheses
            if (!isNumber(tokens[tokens.Count - 1]) && !isPossibleVariable(tokens[tokens.Count - 1]) && tokens[tokens.Count - 1] != ")") throw new FormulaFormatException("Last token must be a number, variable, or closing parentheses");
            int parenthesesCount = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                string t = tokens[i];
                // If t is able to be a variable, check that normalize(t) is valid and isValid(normalize(t)) is true
                if (isPossibleVariable(t))
                {
                    if (!isValidVariable(normalize(t))) throw new FormulaFormatException("Formula had an illegal vairable: " + t);
                    if (!isValid(normalize(t))) throw new FormulaFormatException("Formula had an illegal variable according to isValid: " + t);
                }
                // If t is an opening parentheses, it can only have:
                // - an operator, opening parentheses, or nothing before it
                // - a number, variable, or opening parentheses after it
                if (t == "(")
                {
                    // parentheses count
                    parenthesesCount++;
                    if (i > 0)
                        if (!isOperatorSymbol(tokens[i - 1]) && tokens[i - 1] != "(") throw new FormulaFormatException("An opening parentheses was used improperly at token: " + (i + 1));
                    // The last token would already be confirmed not to be an opening parentheses, so no risk of IndexOutOfBounds for tokens[i + 1]
                    if (!isNumber(tokens[i + 1]) && !isPossibleVariable(tokens[i + 1]) && tokens[i + 1] != "(") throw new FormulaFormatException("An opening parentheses was used improperly at token: " + (i + 1));
                    continue;
                }
                // If t is a closing parentheses, it can only have:
                // - a number, variable, or closing parentheses before it
                // - an operator, closing parentheses, or nothing after it
                if (t == ")")
                {
                    // parentheses count
                    parenthesesCount--;
                    // The first token would already be confirmed not to be a closing parentheses, so no risk of IndexOutOfBounds for tokens[i - 1]
                    if (!isNumber(tokens[i - 1]) && !isPossibleVariable(tokens[i - 1]) && tokens[i - 1] != ")") throw new FormulaFormatException("A closing parentheses was used improperly at token: " + (i + 1));
                    if (i < tokens.Count - 1)
                        if (!isOperatorSymbol(tokens[i + 1]) && tokens[i + 1] != ")") throw new FormulaFormatException("A closing parentheses was used improperly at token: " + (i + 1));
                    continue;
                }
                // If t is an operator, it can only have:
                // - a number, variable, or closing parentheses before it
                // - a number, variable, or opening parentheses after it
                if (isOperatorSymbol(t))
                {
                    // The first and last token would already be confirmed not to be an operator, so no risk of IndexOutOfBounds for tokens[i + 1] and tokens[i - 1]
                    if (!isNumber(tokens[i - 1]) && !isPossibleVariable(tokens[i - 1]) && tokens[i - 1] != ")") throw new FormulaFormatException("An operator was used improperly at token: " + (i + 1));
                    if (!isNumber(tokens[i + 1]) && !isPossibleVariable(tokens[i + 1]) && tokens[i + 1] != "(") throw new FormulaFormatException("An operator was used improperly at token: " + (i + 1));
                    continue;
                }
                // If t is a number or variable, it can only have:
                // - an operator, opening parentheses, or nothing before it
                // - an operator, closing parentheses, or nothing after it
                if (isNumber(t) || isPossibleVariable(t))
                {
                    if (i > 0)
                        if (!isOperatorSymbol(tokens[i - 1]) && tokens[i - 1] != "(") throw new FormulaFormatException("A number or variable was used improperly at token: " + (i + 1));
                    if (i < tokens.Count - 1)
                        if (!isOperatorSymbol(tokens[i + 1]) && tokens[i + 1] != ")") throw new FormulaFormatException("A number or variable was used improperly at token: " + (i + 1));
                    continue;
                }
            }
            // If  parenthesesCount isn't 0, there were too many or too few parentheses
            if (parenthesesCount != 0) throw new FormulaFormatException("The number of opening parentheses did not match the number of closing parentheses");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            return null;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return null;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return null;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left parentheses;
        /// right parentheses; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// Checks that the string is an operator symbol (+, -, *, or /).
        /// </summary>
        private bool isOperatorSymbol(string s)
        {
            return s == "+" || s == "-" || s == "*" || s == "/";
        }

        /// <summary>
        /// Checks that the string is a double number.
        /// </summary>
        private bool isNumber(string s)
        {
            double value;
            return double.TryParse(s, out value);
        }

        /// <summary>
        /// Checks that the string is a possible variable.
        /// It does not check that the string is a variable in format, but rather that it isn't any other acceptable token.
        /// </summary>
        private bool isPossibleVariable(string s)
        {
            return !isOperatorSymbol(s) && !isNumber(s) && s != "(" && s != ")";
        }

        /// <summary>
        /// Checks that the string s is a valid vairable; it is valid if it consists of of a letter or underscore
        /// followed by zero or more letters, underscores, and/or digits.
        /// </summary>
        private bool isValidVariable(string s)
        {
            // Check that the first character is a letter or underscore
            if (!char.IsLetter(s[0]) && s[0] != '_') return false;
            // Get substring of rest of variable
            string sub = s.Substring(1);
            // Check if sub is zero length
            if (sub.Length == 0) return true;
            // Check if sub consists of letters, numbers, and/or digits
            foreach (char c in sub)
            {
                if (!char.IsLetterOrDigit(c) && c != '_') return false;
            }
            // The string did not fail checks, so return true
            return true;
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
