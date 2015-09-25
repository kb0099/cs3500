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
            Formula f1 = new Formula(" a + b + 1 / 2");
            Console.WriteLine(f1.ToString());            
        }
    }
}
