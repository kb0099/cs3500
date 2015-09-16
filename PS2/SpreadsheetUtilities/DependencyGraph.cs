﻿using System;
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
        /// The implementation uses two lists to store the dependents and dependees, or the s and t of (s,t)
        /// respectively. Their pairs are defined by the index in both lists, so a pair would be
        /// (dents.Get(i), dees.Get(i))
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
            get { return 0; }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer. If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[String s]
        {
            get { return 0; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(String s)
        {
            return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(String s)
        {
            return false;
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<String> GetDependents(String s)
        {
            return null;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<String> GetDependees(String s)
        {
            return null;
        }

        /// <summary>
        /// <para>
        /// Adds the ordered pair (s,t), if it doesn't exist
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

        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(String s, String t)
        {

        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(String s, IEnumerable<String> newDependents)
        {

        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(String s, IEnumerable<String> newDependees)
        {

        }
    }
}
