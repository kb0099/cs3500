// Kedar Bastakoti
// CS3500, Assignment 03
// Fall 2015, University of Utah
// Created using provided Skeleton/API

// Skeleton written by Joe Zachary for CS 3500, September 2013
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
        // Represents the formula/expression as a list of normalized validated tokens
        private List<Token> tokens;

        private Func<string, string> normalize; // Represents normalizer 
        private Func<string, bool> isValid;     // Represents validator
        private string formula;                 // Represents the raw formula expression

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
            // Everything done in another constructor call
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
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            tokens = new List<Token>();
            this.formula = formula;
            this.normalize = normalize;
            this.isValid = isValid;
            ProcessTokens();
        }

        /// <summary>
        /// Represents a helper for constructor(s) to process tokens.
        /// </summary>
        private void ProcessTokens()
        {
            Token currToken;        // represents the current token
            // track # of tokens processed
            Dictionary<TokenType, int> tokenCounts = new Dictionary<TokenType, int>();
            //initialize token counts for each token type
            foreach (TokenType tt in Enum.GetValues(typeof(TokenType)))
            {
                tokenCounts[tt] = 0;
            }

            foreach (string token in GetTokens(formula))
            {
                currToken = new Token(token, normalize, isValid);  // normalized and validity checked
                if (currToken.Type == TokenType.UNDEFINED)
                    throw new FormulaFormatException("Error parsing the formula. Check your formula near the token: " + token);

                tokenCounts[currToken.Type] += 1;
                // # fo right paren can't be greater than # of left paren
                if (tokenCounts[TokenType.LEFT_PAREN] < tokenCounts[TokenType.RIGHT_PAREN])
                    throw new FormulaFormatException("Error parsing the formula. When reading tokens from left to right, at no point should the number of closing parentheses seen so far be greater than the number of opening parentheses seen so far.");

                // Add the token
                tokens.Add(currToken);

                // validate parenthesis following rule
                ValidateParenthesisFollowingRuleAt(tokens.Count - 1);

                // validate extra following rule
                ValidateExtraFollowingRuleAt(tokens.Count - 1);
            }

            // cannot be empty
            if (tokens.Count < 1)
                throw new FormulaFormatException("The formula must contain at least at least one token. Spaces don't count as token. Check your formula.");


            // The first token of an expression must be a number, a variable, or an opening parenthesis.
            TokenType[] firstValidTokens = new TokenType[] { TokenType.NUMBER, TokenType.VARIABLE, TokenType.LEFT_PAREN };
            if (!firstValidTokens.Contains(tokens[0].Type))
                throw new FormulaFormatException("The first token of an expression must be a number, a variable, or an opening parenthesis.");

            // The last token of an expression must be a number, a variable, or a closing parenthesis.
            TokenType[] lastValidTokens = new TokenType[] { TokenType.NUMBER, TokenType.VARIABLE, TokenType.RIGHT_PAREN };
            if (!lastValidTokens.Contains(tokens[tokens.Count - 1].Type))
                throw new FormulaFormatException("The last token of an expression must be a number, a variable, or a closing parenthesis.");
        }

        /// <summary>
        /// Validates the Parenthesis Following Rule
        /// Any token that immediately follows an opening parenthesis or 
        /// an operator must be either a number, a variable, or an opening parenthesis.
        /// </summary>
        /// <param name="i">The index of token to validate at. </param>
        private void ValidateParenthesisFollowingRuleAt(int i)
        {
            TokenType[] preTokens = new TokenType[] { TokenType.LEFT_PAREN, TokenType.OP_DIV, TokenType.OP_MINUS, TokenType.OP_MULT, TokenType.OP_PLUS };
            TokenType[] validPostTokens = new TokenType[] { TokenType.NUMBER, TokenType.VARIABLE, TokenType.LEFT_PAREN };

            if (i > 0)
            {
                if (preTokens.Contains(tokens[i - 1].Type))  // if previous token was left paren or an op 
                {
                    if (!validPostTokens.Contains(tokens[i].Type))
                        throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                }
            }
        }


        /// <summary> 
        /// Extra Following Rule
        /// Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis
        /// </summary>
        /// <param name="i">The index of token to validate at.</param>
        private void ValidateExtraFollowingRuleAt(int i)
        {
            TokenType[] preTokens = new TokenType[] { TokenType.NUMBER, TokenType.VARIABLE, TokenType.RIGHT_PAREN };
            TokenType[] validPostTokens = new TokenType[] { TokenType.OP_DIV, TokenType.OP_MINUS, TokenType.OP_MULT, TokenType.OP_PLUS, TokenType.RIGHT_PAREN };
            if (i > 0)
            {
                if (preTokens.Contains(tokens[i - 1].Type))  // if previous token was  number, variable, or closing paren
                {
                    if (!validPostTokens.Contains(tokens[i].Type))
                        throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis");

                }
            }
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
            // Get only those tokens which are variables
            HashSet<Token> variables = new HashSet<Token>(tokens.Where(t => t.Type == TokenType.VARIABLE));

            foreach (Token token in variables)
            {
                yield return token.Value;
            }
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
            return String.Join("", tokens);
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
            if (!(obj == null && typeof(Formula) != obj.GetType()))
            {
                Formula fObj = (Formula)obj;
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (!tokens.ElementAt(i).Equals(fObj.tokens.ElementAt(i)))
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 != null)                 // if f1 is not null we can use Equals
                return f1.Equals(f2);
            else
                return f2 == null;          // else if f2 is null then they are both equal to null.
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            // returns hashcode on the normalized string representation of the formula
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?:[a-zA-Z_]|\d)*";
            String doublePattern = @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0})|({1})|({2})|({3})|({4})|({5})",
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
        /// Represents an immutable Token object.
        /// </summary>
        private class Token
        {
            private string value;                           // string value of token
            private TokenType type;                         // type of token
            /// <summary>
            /// Returns the current value of the token as object.
            /// Note: it simply returns the token as it is. 
            /// For example: if token = "x1", it doesn't evaluate the value of "x1".
            /// It is simply returns the token itself which is "x1".
            /// </summary>            
            public string Value { get { return value; } }

            /// <summary>
            /// Returns the type of the token.
            /// </summary>
            public TokenType Type { get { return type; } }

            /// <summary>
            /// Creates a token object representing the given argument.
            /// </summary>
            /// <param name="token">String representing a token.</param>
            /// <param name="normalizer">Delegate normalizer used to normalize the token.</param>
            /// <param name="validator">Delegate that validates a variable token</param>
            public Token(string token, Func<string, string> normalizer, Func<string, bool> validator)
            {
                value = token;
                switch (token)
                {
                    case "(":
                        type = TokenType.LEFT_PAREN;
                        break;
                    case ")":
                        type = TokenType.RIGHT_PAREN;
                        break;
                    case "+":
                        type = TokenType.OP_PLUS;
                        break;
                    case "-":
                        type = TokenType.OP_MINUS;
                        break;
                    case "*":
                        type = TokenType.OP_MULT;
                        break;
                    case "/":
                        type = TokenType.OP_DIV;
                        break;
                    default:
                        if (Regex.IsMatch(token, DOUBLE_PATTERN))
                        {                // double                                                       
                            type = TokenType.NUMBER;
                        }
                        else if (Regex.IsMatch(token, VAR_PATTERN) && validator(token))
                        {               // variables
                            value = normalizer(token);
                            type = TokenType.VARIABLE;
                        }
                        else
                        {                                                           // undefined/illegal symbol
                            type = TokenType.UNDEFINED;
                        }
                        break;
                }
            }

            /// <summary>
            /// Tests equality based upon the string representaiton of token.
            /// </summary>
            /// <param name="obj">The obj to test equaltity with.</param>
            /// <returns>True if represent the same string token.</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj.GetType() == typeof(Token) && value.Equals(((Token)obj).value))
                    return true;
                return false;
            }

            /// <summary>
            /// It basically calls the GetHashCode() on the string representation of the hashcode.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return value.GetHashCode();
            }

            /// <summary>
            /// Overrides ToString() to return the value of the token as string.
            /// </summary>
            /// <returns>String representation</returns>
            public override string ToString()
            {
                return value;
            }
        }


        /// <summary>
        /// Represents a type of the token object.
        /// </summary>
        private enum TokenType
        {
            NUMBER,         // non-negative numbers written using double-precision floating point syntax
            VARIABLE,       // variables that consist of a letter or underscore followed by 
                            // zero or more letters, underscores, or digits
            LEFT_PAREN,
            RIGHT_PAREN,
            OP_PLUS,        // +
            OP_MINUS,       // -
            OP_MULT,        // * (Multiplication)
            OP_DIV,         // / (Division)  
            UNDEFINED       // Represents an undefined/invalid token given the current specification          
        }

        // private members

        // pattern for double as per assignment specs
        private static string DOUBLE_PATTERN = @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$";

        // pattern for a variable as per assignment specs
        private static string VAR_PATTERN = @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$";
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

