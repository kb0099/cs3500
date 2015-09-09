using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormulaEvaluator;
using System.Text.RegularExpressions;

namespace FormulaEvaluatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // Method to test behavior of Regex.Split
            TestRegexSplit();
            // Method to test FormulaEvaluator.Evaluate
            //TestEvaluate();
        }

        static void TestRegexSplit()
        {
            String testStart = "(15-12)* (57)\n+ s\tb3/f";
            Console.WriteLine("Original expression: " + testStart);
            // Remove whitespace
            testStart = Regex.Replace(testStart, "\\s", "");
            Console.WriteLine("Removed spaces: " + testStart);
            // Split string
            String[] testSplit = Regex.Split(testStart, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            for (int i = 0; i < testSplit.Length; i++)
            {
                Console.WriteLine("element " + i + ": [" + testSplit[i] + "]");
            }
            Console.ReadLine();
        }

        static void TestEvaluate()
        {

        }
    }
}
