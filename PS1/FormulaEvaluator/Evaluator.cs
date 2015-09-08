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
            return 0;
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
