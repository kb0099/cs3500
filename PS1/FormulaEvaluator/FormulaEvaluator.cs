using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// A library class that provides methods to parse an equation in a string. Read notation of methods for more information.
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate for an Evaluate() parameter. The method is expected to take a string that represents some variable used
        /// in the expression string given to Evaluate(), then return the integer that variable contains. The method will be
        /// storing a table of the variables and values so it can return the values for all variables in the expression.
        /// If the given string is not a specified variable, the method should throw an ArgumentException.
        /// </summary>
        /// <param name="v">
        /// The string representing a variable.
        /// </param>
        /// <returns>
        /// The value stored in the variable.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public delegate int Lookup(String v);

        /// <summary>
        /// Parses an infix expression and returns the solution. The legal elements for the expression are +, -, *, /, (, ),
        /// non-negative integers, whitespace, and variables of one or more letters followed by one or more digits. Variable
        /// values are found by using variableEvaluator, which should take in the string of the variable and return the value.
        /// If the solution could not be found, throws an ArgumentException.
        /// </summary>
        /// <param name="expression">
        /// The expression to parse and solve.
        /// </param>
        /// <param name="variableEvaluator">
        /// Method that can take in a String variable and return the variable's value.
        /// </param>
        /// <returns>
        /// The solution to the expression.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            // Split string into an array of readable elements
            String noWhitespace = Regex.Replace(expression, "\\s", "");
            String[] substrings = Regex.Split(noWhitespace, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Make stacks for the values and operators
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            // Loop through elements of substrings
            for (int i = 0; i < substrings.Length; i++)
            {
                // Check if the element is an empty string and skip the element, Regex.Split is doing this before ( and after )
                if ("" == substrings[i]) continue;

                // Check if the element is *, /, or (
                if ("*" == substrings[i] || "/" == substrings[i] || "(" == substrings[i])
                {
                    // Push to operators stack, then move on to next element
                    operators.Push(substrings[i]);
                    continue;
                }

                // Check if the element is an integer
                int result;
                if (int.TryParse(substrings[i], out result))
                {
                    // Take the result and operate on it, then move on to next element
                    ReadInteger(result, operators, values);
                    continue;
                }

                // Check if the element is + or -
                if ("+" == substrings[i] || "-" == substrings[i])
                {
                    // Apply the operation for this case, push the element to operators, then move on to next element
                    ReadAddOrSub(operators, values);
                    operators.Push(substrings[i]);
                    continue;
                }

                // Check if the element is )
                if (")" == substrings[i])
                {
                    // Apply the operation for finding + or -, pop ( from the operators stack and the value from values, operate on that value as an integer read, then move on to next element
                    ReadAddOrSub(operators, values);
                    // Error check that operators had (, throw ArgumentException otherwise
                    String op = operators.Pop();
                    if ("(" != op) throw new ArgumentException();
                    int val = values.Pop();
                    ReadInteger(val, operators, values);
                    continue;
                }

                // As the last possible task, try to check if the element is a variable. If so, take the result and operate on it like an integer
                if (!char.IsLetter(substrings[i][0]) || !char.IsDigit(substrings[i][substrings[i].Length - 1])) throw new ArgumentException();
                result = variableEvaluator(substrings[i]); // If it is an unspecified variable, the method should throw an ArgumentException
                ReadInteger(result, operators, values);
            }

            // Declare the variable to store the output
            int output;

            // Check if operators is not empty
            if (operators.Count > 0)
            {
                // There should be one more operation (+ or -) and two values, Finish the calculation and save to output
                // Error check that operators has 1 string and values has 2 values, throw ArgumentException otherwise
                if (operators.Count != 1 || values.Count != 2) throw new ArgumentException();
                String op = operators.Pop();
                int current = values.Pop();
                int last = values.Pop();
                if ("+" == op)
                {
                    output = last + current;
                }
                else
                {
                    output = last - current;
                }
            }
            else
            {
                // Values has the output value
                // Error check that values has one value, throw ArgumentException otherwise
                if (values.Count != 1) throw new ArgumentException();
                output = values.Pop();
            }

            // Return the final value
            return output;
        }

        /// <summary>
        /// Method that will run necessary operations when an integer is read from the expressions' elements. It will
        /// multiply or divide the previous value with the given value if it can be done, then push the final value to vals.
        /// If vals is empty when trying to multiply or divide, an ArgumentException is thrown.
        /// </summary>
        /// <param name="value">
        /// The integer read.
        /// </param>
        /// <param name="ops">
        /// The operator stack.
        /// </param>
        /// <param name="vals">
        /// The values stack.
        /// </param>
        /// <exception cref="ArgumentException"></exception>
        private static void ReadInteger(int value, Stack<String> ops, Stack<int> vals)
        {
            int current;
            // Check if * or / is currently in the operator stack, then apply the operation to the last and current values
            if (ops.Count > 0)
            {
                if ("*" == ops.Peek() || "/" == ops.Peek())
                {
                    // Error check that vals is not empty, throw ArgumentException otherwise
                    if (vals.Count < 1) throw new ArgumentException();

                    String op = ops.Pop();
                    int last = vals.Pop();
                    if ("*" == op)
                    {
                        current = last * value;
                    }
                    else
                    {
                        // Error check that division by zero does not happen, throw ArgumentException otherwise
                        if (value == 0) throw new ArgumentException();
                        // Use double to round the final value
                        double temp = (double)last / value;
                        current = (int)Math.Round(temp);
                    }
                }
                else
                {
                    // Put the value in current
                    current = value;
                }
            }
            else
            {
                // Put the value in current
                current = value;
            }
            // The final value will be in current, so push it to vals
            vals.Push(current);
        }

        /// <summary>
        /// Method that will run necessary operations when +, -, or ) is read from the expressions' elements. If the previous
        /// operation was a + or -, it will apply the operation on the last two values and put the result in vals.
        /// If vals has less than two values when trying to add or subtract, an ArgumentException is thrown.
        /// </summary>
        /// <param name="ops"></param>
        /// <param name="vals"></param>
        private static void ReadAddOrSub(Stack<String> ops, Stack<int> vals)
        {
            // Check if + or - are on the ops stack, then apply the operation to the last and current values
            if (ops.Count > 0)
                if ("+" == ops.Peek() || "-" == ops.Peek())
                {
                    // Error check that vals has enough values, throw ArgumentException otherwise
                    if (vals.Count < 2) throw new ArgumentException();

                    String sign = ops.Pop();
                    int current = vals.Pop();
                    int last = vals.Pop();
                    int result;
                    // Check which sign is present and apply operation
                    if ("+" == sign)
                    {
                        result = last + current;
                    }
                    else
                    {
                        result = last - current;
                    }
                    // Add result to vals
                    vals.Push(result);
                }
        }
    }
}
