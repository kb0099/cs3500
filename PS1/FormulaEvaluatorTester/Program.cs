﻿using System;
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
        private static Dictionary<String, int> dict;

        static void Main(string[] args)
        {
            // Method to test behavior of Regex.Split
            // TestRegexSplit();
            // Method to test FormulaEvaluator.Evaluate
            TestEvaluate();
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
            // Create array of test strings and expected solutions
            String[] testStrings = new String[]{
                "2",
                "2 +  3",
                "7 + 3+ 16",
                "9-13",
                "45- 12+2",
                "3*4",
                "7 + 3*4",
                "9* 2 - 4",
                "12/6",
                "5/3",
                "1/7",
                "9-4/2",
                "3/1+  6",
                "(75)",
                "(8)+3",
                "(2+3)",
                "8-(12-2)",
                "2*(6-3)",
                "4*(6/2)",
                "8/(2*2)",
                "x7",
                "x7+3",
                "14/x7",
                "3+ms*2",
                "25-6*ms",
                "F55/ms +x7"
            };
            String[] solutions = new String[]{
                "2",
                "5",
                "26",
                "-4",
                "35",
                "12",
                "19",
                "14",
                "2",
                "2",
                "0",
                "7",
                "9",
                "75",
                "11",
                "5",
                "-2",
                "6",
                "12",
                "2",
                "7",
                "10",
                "2",
                "11",
                "1",
                "10"
            };
            // Set a method for the lookup
            Evaluator.Lookup lookupFunc = lookup;
            // Construct the dictionary
            dict = new Dictionary<string, int>();
            dict.Add("x7", 7);
            dict.Add("ms", 4);
            dict.Add("F55", 12);

            // Loop through test strings; print the string, the expected solution, and the calculated solution
            for (int i = 0; i < testStrings.Length; i++)
            {
                Console.WriteLine("Eq: " + testStrings[i] + " Ans: " + solutions[i] + " Sol: " + Evaluator.Evaluate(testStrings[i], lookupFunc));
            }
            Console.ReadLine();
        }

        static int lookup(String variable)
        {
            // Throw ArgumentException if given uncontained variable
            if (!dict.ContainsKey(variable)) throw new ArgumentException();
            // Return the value of the variable
            int value;
            dict.TryGetValue(variable, out value);
            return value;
        }
    }
}
