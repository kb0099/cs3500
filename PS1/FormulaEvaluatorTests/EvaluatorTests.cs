using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulaEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEvaluator.Tests
{
    [TestClass()]
    public class EvaluatorTests
    {
        [TestMethod()]
        public void IsValidVarTest_ValidTokens()
        {
            String[] valids = { "xx11", "a99", "a0", "abc1123", "nepal9900", "x6", "z2" };
           
            //Array.ForEach<String>(valids, s => Assert.IsTrue(Evaluator.IsValidVar(s)));
        }

        [TestMethod]
        public void IsValidVarTest_InValidTokens()
        {
            String[] inValids = { "1xx11", "a99b", "ahel099o", "abc1123hi", "n1epal9900", "xx", "yyzz11aa", " yy11 " };

            //Array.ForEach<String>(inValids, s => Assert.IsFalse(Evaluator.IsValidVar(s)));
        }
    }
}