using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace PS3Client
{
    class Program
    {
        //static Formula f1 = new Formula("");
        
        public static void Main()
        {
            Formula f2 = new Formula("a1 * b1 - c1 * A1 /C1 + 99.99e-99", s => s.ToUpper(), s => true);
            var x = new HashSet<string>(f2.GetVariables());
            Console.WriteLine(string.Join(", ", x));
        }

        public static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?:[a-zA-Z_]|\d)*";
            String doublePattern = @"(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0})|({1})|({2})|({3})|({4})|({5})",
                                            lpPattern, rpPattern, opPattern, doublePattern, varPattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }
}
