using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace PS2Client
{
    class Program
    {
        static void Main(string[] args)
        {
            test8();
            /*
            DependencyGraph d = new DependencyGraph();
            Console.WriteLine(d.GetDependees("a"));
            Dictionary<string, HashSet<string>> dependents = new Dictionary<string, HashSet<string>>();
            Console.WriteLine(dependents.ContainsKey("x"));
            */
        }

        public static  void test8()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            t.ReplaceDependees("c", new HashSet<string>() { "x", "y", "z" });
            HashSet<String> cDees = new HashSet<string>(t.GetDependees("c"));
            Console.WriteLine(cDees.SetEquals(new HashSet<string>() { "x", "y", "z" }));
        }

        public static void test6()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            t.RemoveDependency("a", "b");
            Console.WriteLine(t.Size);

        }

        static public void NonEmptyTest4()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("d", "c");
            HashSet<String> aDents = new HashSet<String>(t.GetDependents("a"));
            HashSet<String> bDents = new HashSet<String>(t.GetDependents("b"));
            HashSet<String> cDents = new HashSet<String>(t.GetDependents("c"));
            HashSet<String> dDents = new HashSet<String>(t.GetDependents("d"));
            HashSet<String> eDents = new HashSet<String>(t.GetDependents("e"));
            HashSet<String> aDees = new HashSet<String>(t.GetDependees("a"));
            HashSet<String> bDees = new HashSet<String>(t.GetDependees("b"));
            HashSet<String> cDees = new HashSet<String>(t.GetDependees("c"));
            HashSet<String> dDees = new HashSet<String>(t.GetDependees("d"));
            HashSet<String> eDees = new HashSet<String>(t.GetDependees("e"));
            Console.WriteLine(aDents.Count == 2 && aDents.Contains("b") && aDents.Contains("c"));
            Console.WriteLine(bDents.Count == 0);
            Console.WriteLine(cDents.Count == 0);
            Console.WriteLine(dDents.Count == 1 && dDents.Contains("c"));
            Console.WriteLine(eDents.Count == 0);
            Console.WriteLine(aDees.Count == 0);
            Console.WriteLine(bDees.Count == 1 && bDees.Contains("a"));
            Console.WriteLine(cDees.Count == 2 && cDees.Contains("a") && cDees.Contains("d"));
            Console.WriteLine(dDees.Count == 0);
            Console.WriteLine(dDees.Count == 0);
        }
    }
}
