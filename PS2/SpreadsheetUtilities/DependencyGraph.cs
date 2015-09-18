// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// s1 depends on t1 --> t1 must be evaluated before s1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// Holds dependees as required by specification using key-value pair.
        /// </summary>
        private Dictionary<string, HashSet<string>> dependees;
        /// <summary>
        /// Holds key-value pair of dependents. dependents and dependees will have reverse entries.
        /// </summary>
        private Dictionary<string, HashSet<string>> dependents;

        /// <summary>
        /// represents the size
        /// </summary>
        private int size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (dependees.ContainsKey(s))       // if dependees has the key s
                    return dependees[s].Count;      // return the Count belonging to that key
                return 0;                           // else return 0
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
                return dependents[s].Count > 0;
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s))
                return dependees[s].Count > 0;
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!dependents.ContainsKey(s))
                return new HashSet<string>();
            return dependents[s];
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!dependees.ContainsKey(s))
                return new HashSet<string>();
            return dependees[s];
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   s depends on t
        ///
        /// </summary>
        /// <param name="s"> s cannot be evaluated until t is</param>
        /// <param name="t"> t must be evaluated first.  S depends on T</param>
        public void AddDependency(string s, string t)
        {
            if (AddDependency(dependents, s, t))
                ++size;
            AddDependency(dependees, t, s);
        }

        /// <summary>
        /// Represents a helper for AddDependency method.
        /// </summary>
        /// <param name="d">The dictionary object.</param>
        /// <param name="s">The key</param>
        /// <param name="t">The value in HashSet.</param>
        private bool AddDependency(Dictionary<string, HashSet<String>> d, string s, string t)
        {
            if (!d.ContainsKey(s))
                d.Add(s, new HashSet<string>());
            if (!d[s].Contains(t))
            {
                d[s].Add(t);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (RemoveDependency(dependents, s, t))
                --size;
            RemoveDependency(dependees, t, s);
        }
        /// <summary>
        /// Represents a helper for RemoveDependency
        /// </summary>
        /// <param name="d">The dictionary that contains the key.</param>
        /// <param name="k">The key</param>
        /// <param name="v">The value</param>
        /// <returns></returns>
        private bool RemoveDependency(Dictionary<string, HashSet<string>> d, string k, string v)
        {
            if (d.ContainsKey(k))
            {
                d[k].Remove(v);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (!dependents.ContainsKey(s))
                dependents.Add(s, new HashSet<string>());
            size = size - dependents[s].Count + newDependents.Count<string>();
            foreach(string t in dependents[s])
            {
                RemoveDependency(dependees, t, s);
            }
            dependents[s] = new HashSet<string>(newDependents); // effectively remove the old set of dependents
            foreach (string t in newDependents)
            {
                AddDependency(dependees, t, s);
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (!dependees.ContainsKey(t))
                dependees.Add(t, new HashSet<string>());
            size = size - dependees[t].Count + newDependees.Count<string>();
            foreach(string s in dependees[t])
            {
                RemoveDependency(dependents, s, t);
            }
            dependees[t] = new HashSet<string>(newDependees);
            foreach (string s in newDependees)
            {
                AddDependency(dependents, s, t);
            }
        }

    }




}


