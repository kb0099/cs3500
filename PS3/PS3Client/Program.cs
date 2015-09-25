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
            List<String> s = new List<string>();
            s.Add("k");
            s.Add("-");
            s.Add("b");
            Console.WriteLine(String.Join("", s));
           Console.WriteLine(Regex.IsMatch("1.", @"^(?:\d+\.\d*|\d*\.\d+|\d+)(?:[eE][\+-]?\d+)?$"));
           Formula f1 = new Formula(" 1 ");
            Console.WriteLine(f1.GetType());
            Console.WriteLine(typeof(Formula));
            Console.WriteLine("2.0 == 2: "  + (2.00 == 2));
        }
    }
}
