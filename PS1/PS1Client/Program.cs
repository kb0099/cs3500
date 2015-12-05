using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static FormulaEvaluator.Evaluator;
using static System.Console;

namespace PS1Client
{
    class Program
    {
         delegate int f1(int a, int b);
        static void Main(string[] args)
        {
            String d = "+1.07e-3";
            double r;
            Console.WriteLine(Double.TryParse(d, out r));
            Console.WriteLine(r);
            /*
            String exp = " ( 12    + 13)";
            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Console.WriteLine(String.Join(", ", tokens));
            */

            WriteLine(Evaluate("a1*2*10 - (20 + 20 + 50) - 100", e => 2));

            /*
            Stack<int> intStack = new Stack<int>();
            intStack.Push(1);
            intStack.Push(2);

            WriteLine(intStack.HasOnTop(new int[]{2, 1,10,11}));

            Stack<String> strStack = new Stack<string>();
            strStack.Push("hello");
            strStack.Push("jello");
            strStack.Push("friend");

            WriteLine(strStack.HasOnTop(new string[] {"friend", "hi", "bye", "my" }));
            */

            ReadLine();
        }
    }
}
