using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// 
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Represents a valid pattern for variable name or identifier.
        /// </summary>
        const String VALID_VAR_PATTERN = @"^[a-zA-Z]+[0-9]+$";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
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
