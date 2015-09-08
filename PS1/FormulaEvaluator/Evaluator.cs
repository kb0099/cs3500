using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// Represents a static class that acts as a container for static members.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Represents a valid pattern for variable name or identifier.
        /// <para>PS1 Requirement: Variable name consists of one or more letters followed by one or more digits.</para>
        /// </summary>
        const String VALID_VAR_PATTERN = @"^[a-zA-Z]+[0-9]+$";

        /// <summary>
        /// A delegate that can be used to look up the value of a variable. 
        /// <para>Given a variable as its parameter, the delegate will either return 
        /// an int (the value of the variable) or throw an ArgumentException 
        /// (if the variable has no value).</para>
        /// </summary>
        /// <param name="v">Variable of <see cref="System.String"/> type.</param>
        /// <returns>The value represented by the <paramref name="v"/>.</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// The method evaluates the expression, using the algorithm provided. 
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if one of the possible errors from the algorithm occurs.</exception>
        /// <param name="exp">The expression to be evaluated. </param>
        /// <param name="variableEvaluator">A delegate that can be used to look up the value of a variable.</param>
        /// <returns>It returns the value of the expression (if it has a value). </returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Stack<double> valueStack = new Stack<double>();
            Stack<char> opStack = new Stack<char>();

            if (String.IsNullOrEmpty(exp))
                ArgEx();

            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Console.WriteLine(String.Join(", ", tokens));

            //process each token
            for(int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Trim();
                switch (tokens[i])
                {
                    case "":
                        break;
                    case "+":
                    case "-":
                        /*
                        If + or - is at the top of the operator stack, pop the value stack twice and the operator stack once. Apply the popped operator to the popped numbers. Push the result onto the value stack. Next, push t onto the operator stack
                        */
                        break;
                    case "*":
                    case "/":
                        /*
                        Push t onto the operator stack
                        */
                        break;
                    case "(":
                        break;
                    case ")":
                        /*
                        If + or - is at the top of the operator stack, pop the value stack twice and the operator stack once. Apply the popped operator to the popped numbers. Push the result onto the value stack.

Next, the top of the operator stack should be a (. Pop it.

Finally, if * or / is at the top of the operator stack, pop the value stack twice and the operator stack once. Apply the popped operator to the popped numbers. Push the result onto the value stack.
*/
                        break;
                    default:
                        if (Regex.IsMatch(tokens[i], @"^\d+$"))
                        {
                            // integer
                            /*If * or / is at the top of the operator stack, pop the value stack, pop the operator stack, and apply the popped operator to t and the popped number. Push the result onto the value stack.

Otherwise, push t onto the value stack.
                            */
                }
                        else if (IsValidVar(tokens[i]))
                        {
                            // variable
                            /*
                            Proceed as above, using the looked-up value of t instead of t
                            */
                        }
                        else
                            ArgEx();

                        break;
                }
            }
            // after all the tokens has been processed
            /*
            If Operator stack is empty	
            Value stack should contain a single number
            Pop it and report as the value of the expression

            Else, There should be exactly one operator on 
            the operator stack, and it should be either + or -. There should be 
            exactly two values on the value stack. Apply the operator to the two 
            values and report the result as the value of the expression.
            */
                            if (opStack.Count == 0)
            {
                return (int)valueStack.Pop();
            }
            else
            {
                return (int)Calc(valueStack.Pop(), valueStack.Pop(), opStack.Pop());
            }
        }
        /// <summary>
        /// Represents a helper method for arithmetic operations.
        /// </summary>
        /// <param name="v1">Left Operand</param>
        /// <param name="v2">Right Operand</param>
        /// <param name="v3">Operator: +, -, *, or /</param>
        /// <returns>The result of applying the operator to the operands.</returns>
        private static double Calc(double v1, double v2, char v3)
        {
            switch (v3)
            {
                case '+':
                    return v1+v2;
                case '-':
                    return v1-v2;
                case '*':
                    return v1*v2;
                case '/':
                    return v1/v2;
                default:
                    ArgEx();
                    return Double.NaN;
            }
        }

        /// <summary>
        /// Throws the prevalent ArgumentException.
        /// </summary>
        private static void ArgEx()
        {
            throw new ArgumentException("Expression contains invalid token or operation.");
        }

        /// <summary>
        /// Checks if the passed parameter is a valid variable name.
        /// </summary>
        /// <param name="varName">Variable name to check</param>
        /// <returns>True if valid else False</returns>
        public static bool IsValidVar(String varName)
        {
            return Regex.IsMatch(varName, VALID_VAR_PATTERN);
        }
    }
}
