using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTester
{
    [TestClass]
    public class FormulaTester1
    {
        //*****************Public Tests*********************//
        /// <summary>
        /// Formula(string) should not throw exceptions for syntatically correct formulas.
        /// Using formulas with parentheses, operators, and numbers, but not variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructor1()
        {
            // Create string array of formulas
            string[] formulas = new string[]{
                "30",
                "0.9400",
                "12.03",
                "0",
                "0.0",
                "9.02+3.7",
                "12.7 +  4.33",
                "7.3-3.0",
                "15.8 -  2.9",
                "7.2*1.3",
                "9.3 *  12.4",
                "60.5/1.25",
                "7.6/ 22.3",
            };
            // Loop through formulas; construct Formula objects from them
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f);
                }
                catch (FormulaFormatException)
                {
                    Assert.Fail("Threw exception for {" + f + "}");
                }
            }
        }

        //****************Private Tests********************//
        [TestMethod]
        public void privateTestMethod1()
        {

        }
    }
}
