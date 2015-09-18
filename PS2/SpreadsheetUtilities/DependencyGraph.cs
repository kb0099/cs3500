// Coded by Mitchell Terry
// Holds class for creating objects to store dependency graphs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// s1 depends on t1 --> t1 must be evaluated before s1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates. If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///     (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///     
    ///     (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///     
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        ///<summary>
        /// The implementation uses two lists to store the dependents and dependees (dents,dees), or the
        /// s and t of (s,t) respectively. Their pairs are defined by the index in both lists, so a pair
        /// would be (dents[i], dees[i]).
        ///</summary>
        private List<String> dents, dees;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dents = new List<String>();
            dees = new List<String>();
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return dents.Count; }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer. If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a").
        /// </summary>
        public int this[String s]
        {
            get { return this.GetDependees(s).Count<String>(); }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(String s)
        {
            return dents.Contains(s);
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(String s)
        {
            return dees.Contains(s);
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<String> GetDependents(String s)
        {
            HashSet<String> output = new HashSet<String>();
            // Loop through the dents list; when dents[i] matches s, store the dependent (in dees) into output
            for (int i = 0; i < dents.Count; i++)
            {
                if (dents[i] == s)
                {
                    output.Add(dees[i]);
                }
            }
            return output;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<String> GetDependees(String s)
        {
            HashSet<String> output = new HashSet<String>();
            // Loop through the dees list; when dees[i] matches s, store the dependee (in dents) into output
            for (int i = 0; i < dees.Count; i++)
            {
                if (dees[i] == s)
                {
                    output.Add(dents[i]);
                }
            }
            return output;
        }

        /// <summary>
        /// <para>
        /// Adds the ordered pair (s,t), if it doesn't exist.
        /// </para>
        /// <para>
        /// This should be thought of as:
        /// </para>
        ///     s depends on t
        /// </summary>
        /// <param name="s"> s cannot be evaluated until t is</param>
        /// <param name="t"> t must be evaluated first. s depends on t</param>
        public void AddDependency(String s, String t)
        {
            // Get the dependees of t
            IEnumerable<String> tDees = this.GetDependees(t);
            // If t doesn't have s as a dependee yet, add the pair
            if (!tDees.Contains<String>(s))
            {
                dents.Add(s);
                dees.Add(t);
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(String s, String t)
        {
            // Loop through the dents list; if the pair is found, remove it and end the function
            for (int i = 0; i < dents.Count; i++)
            {
                if (dents[i] == s && dees[i] == t)
                {
                    dents.RemoveAt(i);
                    dees.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(String s, IEnumerable<String> newDependents)
        {
            // Remove the pairs (s,r)
            IEnumerable<String> remove = this.GetDependents(s);
            foreach (String r in remove)
            {
                this.RemoveDependency(s, r);
            }
            // Add the pairs (s,t)
            foreach (String t in newDependents)
            {
                this.AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(String s, IEnumerable<String> newDependees)
        {
            // Remove the pairs (r,s)
            IEnumerable<String> remove = this.GetDependees(s);
            foreach (String r in remove)
            {
                this.RemoveDependency(r, s);
            }
            // Add the pairs (s,t)
            foreach (String t in newDependees)
            {
                this.AddDependency(t, s);
            }
        }
    }
}
