using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

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
                " 73  ",
                "  0.36 ",
                " 5.4  ",
                "  0 ",
                " 0.00  ",
                "9.02+3.7",
                "12.7 +  4.33",
                "7.3-3.0",
                "15.8 -  2.9",
                "7.2*1.3",
                "9.3 *  12.4",
                "60.5/1.25",
                "7.6 /  22.3",
                "(3.2)",
                "  ( 7.5  ) ",
                "((42.3))",
                " ((  84.9 )  )  ",
                "(7.1+6)",
                "( 94.45  + 16  )",
                "(12.2-8)",
                "(  22.3  - 17 )",
                "(27*0.3)",
                "( 15  *  4)",
                "(9/3.0)",
                "(12.7  / 6  )",
                "9+(7.3)",
                "(12.9)+3",
                "17.3 +  ( 7.2  ) + 3",
                "8.2-(12)",
                "(0.6)-5.3",
                "82 - (  4.3)  -12",
                "7*(8.2)",
                "(5.3)*4",
                "7 *(0.2  ) *  3",
                "9/(3.0)",
                "(42.3)/7",
                "18/  ( 9.7 )/ 4.3",
                "15.84-(9+3.7)",
                "(83-4)+12.3",
                "6*(42.3/7.9)",
                "(9*10.2)/4",
                "(42+(19/4))",
                "((4-1.3)*7.4)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f);
                }
                catch (FormulaFormatException e)
                {
                    Assert.Fail("Threw exception for {" + f + "}: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Formula(string) should not throw exceptions for syntatically correct formulas.
        /// Using formulas with valid variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructor2()
        {
            // Create string array of formulas
            string[] formulas = new string[]{
                "_",
                " _  ",
                "m",
                "  V ",
                "_sam",
                " _phil  ",
                "_34",
                "  _1943 ",
                "_____",
                " ______  ",
                "_corey98",
                "_12anson",
                "___43",
                "_184___",
                "_mitt_",
                "____tom",
                "_brad_is_94",
                "_try28__6catch",
                "michael",
                "p04",
                "f__",
                "in_b4",
                "i45_trip",
                "p4 - _fiona",
                "sam+matt",
                "(a0*t)+__l33t__",
                "mass_4/(vol_7+vol_3)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f);
                }
                catch (FormulaFormatException e)
                {
                    Assert.Fail("Threw exception for {" + f + "}: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Formula(string, N(), V()) should not thow exceptions for syntatically correct formulas.
        /// N() will capitalize all letters, V() will return false when given an unspecified variable.
        /// Using formulas with valid variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructor3()
        {
            // Create normalizer
            Func<string, string> N = s =>
            {
                string t = "";
                foreach (char c in s)
                {
                    t = t + char.ToUpper(c);
                }
                return t;
            };
            // Create validator
            HashSet<string> valid = new HashSet<string>();
            valid.Add("EMILY");
            valid.Add("BALL_SPEED");
            valid.Add("_PUMP3");
            Func<string, bool> V = s =>
            {
                return valid.Contains(s);
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "emily",
                "Emily",
                "eMiLy",
                "ball_speed",
                "Ball_Speed",
                "ball_SPEED",
                "BALL_speed",
                "_pump3",
                "_Pump3",
                "_PUmP3",
                "Emily - Ball_speed",
                "BALL_Speed*_pump3",
                "(emilY+ball_speed)/_PUMP3"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f, N, V);
                }
                catch (FormulaFormatException e)
                {
                    Assert.Fail("Threw exception for {" + f + "}: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Formula(string, N(), V()) should not throw exceptions if N() returns invalid variables when called but it doesn't get called
        /// because the formulas don't have variables contained.
        /// N() returns the input with a number added to the front, and V() returns true.
        /// Using formulas with parentheses, operators, and numbers, but not variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructor4()
        {
            // Create normalizer
            Func<string, string> N = s =>
            {
                return "7" + s;
            };
            // Create validator
            Func<string, bool> V = s =>
            {
                return true;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "30",
                "0.9400",
                "12.03",
                "0",
                "0.0",
                " 73  ",
                "  0.36 ",
                " 5.4  ",
                "  0 ",
                " 0.00  ",
                "9.02+3.7",
                "12.7 +  4.33",
                "7.3-3.0",
                "15.8 -  2.9",
                "7.2*1.3",
                "9.3 *  12.4",
                "60.5/1.25",
                "7.6 /  22.3",
                "(3.2)",
                "  ( 7.5  ) ",
                "((42.3))",
                " ((  84.9 )  )  ",
                "(7.1+6)",
                "( 94.45  + 16  )",
                "(12.2-8)",
                "(  22.3  - 17 )",
                "(27*0.3)",
                "( 15  *  4)",
                "(9/3.0)",
                "(12.7  / 6  )",
                "9+(7.3)",
                "(12.9)+3",
                "17.3 +  ( 7.2  ) + 3",
                "8.2-(12)",
                "(0.6)-5.3",
                "82 - (  4.3)  -12",
                "7*(8.2)",
                "(5.3)*4",
                "7 *(0.2  ) *  3",
                "9/(3.0)",
                "(42.3)/7",
                "18/  ( 9.7 )/ 4.3",
                "15.84-(9+3.7)",
                "(83-4)+12.3",
                "6*(42.3/7.9)",
                "(9*10.2)/4",
                "(42+(19/4))",
                "((4-1.3)*7.4)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f, N, V);
                }
                catch (FormulaFormatException e)
                {
                    Assert.Fail("Threw exception for {" + f + "}: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Formula(string, N(), V()) should not throw exceptions if V() returns false when called but it doesn't get called because
        /// the formulas don't have variables contained.
        /// N() returns the input, V() returns false when called.
        /// Using formulas with parentheses, operators, and numbers, but not variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructor5()
        {
            // Create normalizer
            Func<string, string> N = s =>
            {
                return s;
            };
            // Create validator
            Func<string, bool> V = s =>
            {
                return false;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "30",
                "0.9400",
                "12.03",
                "0",
                "0.0",
                " 73  ",
                "  0.36 ",
                " 5.4  ",
                "  0 ",
                " 0.00  ",
                "9.02+3.7",
                "12.7 +  4.33",
                "7.3-3.0",
                "15.8 -  2.9",
                "7.2*1.3",
                "9.3 *  12.4",
                "60.5/1.25",
                "7.6 /  22.3",
                "(3.2)",
                "  ( 7.5  ) ",
                "((42.3))",
                " ((  84.9 )  )  ",
                "(7.1+6)",
                "( 94.45  + 16  )",
                "(12.2-8)",
                "(  22.3  - 17 )",
                "(27*0.3)",
                "( 15  *  4)",
                "(9/3.0)",
                "(12.7  / 6  )",
                "9+(7.3)",
                "(12.9)+3",
                "17.3 +  ( 7.2  ) + 3",
                "8.2-(12)",
                "(0.6)-5.3",
                "82 - (  4.3)  -12",
                "7*(8.2)",
                "(5.3)*4",
                "7 *(0.2  ) *  3",
                "9/(3.0)",
                "(42.3)/7",
                "18/  ( 9.7 )/ 4.3",
                "15.84-(9+3.7)",
                "(83-4)+12.3",
                "6*(42.3/7.9)",
                "(9*10.2)/4",
                "(42+(19/4))",
                "((4-1.3)*7.4)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f, N, V);
                }
                catch (FormulaFormatException e)
                {
                    Assert.Fail("Threw exception for {" + f + "}: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Formula(string) should throw exceptions for syntatically incorrect formulas.
        /// Using formulas with parentheses, operators, and numbers, but not variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructorException1()
        {
            // Create string array of formulas
            string[] formulas = new string[]{
                "",
                "+",
                "-",
                "*",
                "/",
                "(",
                ")",
                "9 4.3",
                "9+",
                "+4",
                "12-",
                "-6",
                "*4",
                "0.7*",
                "/2.5",
                "12/",
                "(8.3",
                "15(",
                ")6",
                "42)",
                "12(44)",
                "(18.3)9",
                "(42+)",
                "(-6.7)",
                "+-",
                "*/",
                "((6.2)",
                "(47))"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f);
                    Assert.Fail("Didn't throw exception for {" + f + "}");
                }
                catch (FormulaFormatException)
                {
                }
            }
        }

        /// <summary>
        /// Formula(string) should thow exceptions for formulas with invalid variables, or variables with invalid characters.
        /// Using formulas containing invalid characters.
        /// </summary>
        [TestMethod]
        public void publicTestConstructorException2()
        {
            // Create string array of formulas
            string[] formulas = new string[]{
                "^x",
                "m&m's",
                "#DontFail",
                "xX||U83RH4X0R||Xx",
                "...maybe",
                "<Type>",
                "show=3",
                "whatNow?",
                "!!UWOTM8!!"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f);
                    Assert.Fail("Didn't throw exception for {" + f + "}");
                }
                catch (FormulaFormatException)
                {
                }
            }
        }

        /// <summary>
        /// Formula(string, N(), V()) should throw exceptions if N() returns an invalid variable.
        /// N() returns the input with a number added to the front, and V() returns true.
        /// Using formulas with valid varaibles.
        /// </summary>
        [TestMethod]
        public void publicTestConstructorException3()
        {
            // Create normalizer
            Func<string, string> N = s =>
            {
                return "7" + s;
            };
            // Create validator
            Func<string, bool> V = s =>
            {
                return true;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "_",
                " _  ",
                "m",
                "  V ",
                "_sam",
                " _phil  ",
                "_34",
                "  _1943 ",
                "_____",
                " ______  ",
                "_corey98",
                "_12anson",
                "___43",
                "_184___",
                "_mitt_",
                "____tom",
                "_brad_is_94",
                "_try28__6catch",
                "michael",
                "p04",
                "f__",
                "in_b4",
                "i45_trip",
                "p4 - _fiona",
                "sam+matt",
                "(a0*t)+__l33t__",
                "mass_4/(vol_7+vol_3)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f, N, V);
                    Assert.Fail("Didn't throw exception for {" + f + "}");
                }
                catch (FormulaFormatException)
                {
                }
            }
        }

        /// <summary>
        /// Formula(string, N(), V()) should throw exceptions if V() returns false when called.
        /// V() will be called when a variable is found.
        /// N() returns the input, and V() returns false when called.
        /// Using formulas with valid variables.
        /// </summary>
        [TestMethod]
        public void publicTestConstructorException4()
        {
            // Create normalizer
            Func<string, string> N = s =>
            {
                return s;
            };
            // Create validator
            Func<string, bool> V = s =>
            {
                return false;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "_",
                " _  ",
                "m",
                "  V ",
                "_sam",
                " _phil  ",
                "_34",
                "  _1943 ",
                "_____",
                " ______  ",
                "_corey98",
                "_12anson",
                "___43",
                "_184___",
                "_mitt_",
                "____tom",
                "_brad_is_94",
                "_try28__6catch",
                "michael",
                "p04",
                "f__",
                "in_b4",
                "i45_trip",
                "p4 - _fiona",
                "sam+matt",
                "(a0*t)+__l33t__",
                "mass_4/(vol_7+vol_3)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                try
                {
                    new Formula(f, N, V);
                    Assert.Fail("Didn't throw exception for {" + f + "}");
                }
                catch (FormulaFormatException)
                {
                }
            }
        }

        /// <summary>
        /// Evaluate(L()) should use L(N()) to find values. Use Formula(string) and Formula(string, N(), V()) to compare the
        /// difference that occurs.
        /// N() capitalizes the input, and V() returns true.
        /// Using formulas with variables.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate1()
        {
            // Create normalizer
            Func<string, string> N = s =>
            {
                string t = "";
                foreach (char c in s)
                {
                    t = t + char.ToUpper(c);
                }
                return t;
            };
            // Create validator
            Func<string, bool> V = s =>
            {
                return true;
            };
            // Create lookup
            Dictionary<string, double> set = new Dictionary<string, double>();
            set.Add("x12", 1);
            set.Add("X12", 2);
            set.Add("fun17", 1);
            set.Add("FUN17", 7);
            set.Add("Woah42", 4);
            set.Add("WOAH42", 2);
            Func<string, double> L = s =>
            {
                double value;
                if (set.TryGetValue(s, out value)) return value;
                return 0;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "x12",
                "fun17",
                "Woah42"
            };
            // Create string arrays of results
            double[] resultsA = new double[]{
                1,
                1,
                4
            };
            double[] resultsB = new double[]{
                2,
                7,
                2
            };
            // Loop through formulas; construct Formula objects from f
            for (int i = 0; i < formulas.Length; i++)
            {
                string f = formulas[i];
                object oa = new Formula(f).Evaluate(L);
                object ob = new Formula(f, N, V).Evaluate(L);
                if (oa is double)
                {
                    double va = (double)oa;
                    Assert.AreEqual(resultsA[i], va);
                }
                else
                {
                    Assert.Fail("Could not evaluate {" + f + "}, A gave: " + ((FormulaError)oa).Reason);
                }
                if (ob is double)
                {
                    double vb = (double)ob;
                    Assert.AreEqual(resultsB[i], vb);
                }
                else
                {
                    Assert.Fail("Could not evaluate {" + f + "}, B gave: " + ((FormulaError)ob).Reason);
                }
            }
        }

        /// <summary>
        /// Evaluate(L()) should calculate solutions proplerly.
        /// L() returns -1.
        /// Using Formula(string) and parentheses, operators, and numbers, but not variables.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate2()
        {
            // Create lookup
            Func<string, double> L = s =>
            {
                return -1;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "30", // 1
                "0.9400",
                "12.03",
                "0",
                "0.0", // 5
                " 73  ",
                "  0.36 ",
                " 5.4  ",
                "  0 ",
                " 0.00  ", // 10
                "9.02+3.7",
                "12.7 +  4.33",
                "7.3-3.0",
                "15.8 -  2.9",
                "7.2*1.3", // 15
                "9.3 *  12.4",
                "60.5/1.25",
                "7.6 /  22.3",
                "(3.2)",
                "  ( 7.5  ) ", // 20
                "((42.3))",
                " ((  84.9 )  )  ",
                "(7.1+6)",
                "( 94.45  + 16  )",
                "(12.2-8)", // 25
                "(  22.3  - 17 )",
                "(27*0.3)",
                "( 15  *  4)",
                "(9/3.0)",
                "(12.7  / 6  )", // 30
                "9+(7.3)",
                "(12.9)+3",
                "17.3 +  ( 7.2  ) + 3",
                "8.2-(12)",
                "(0.6)-5.3", // 35
                "82 - (  4.3)  -12",
                "7*(8.2)",
                "(5.3)*4",
                "7 *(0.2  ) *  3",
                "9/(3.0)", // 40
                "(42.3)/7",
                "18/  ( 9.7 )/ 4.3",
                "15.84-(9+3.7)",
                "(83-4)+12.3",
                "6*(42.3/7.9)", // 45
                "(9*10.2)/4",
                "(42+(19/4))",
                "((4-1.3)*7.4)"
            };
            // Create string array of results
            double[] results = new double[]{
                30, // 1
                0.94,
                12.03,
                0,
                0, // 5
                73,
                0.36,
                5.4,
                0,
                0, // 10
                12.72,
                17.03,
                4.3,
                12.9,
                9.36, // 15
                115.32,
                48.4,
                0.34080717488789237668161434977578,
                3.2,
                7.5, // 20
                42.3,
                84.9,
                13.1,
                110.45,
                4.2, // 25
                5.3,
                8.1,
                60,
                3,
                2.1166666666666666666666666666667, // 30
                16.3,
                15.9,
                27.5,
                -3.8,
                -4.7, // 35
                65.7,
                57.4,
                21.2,
                4.2,
                3, // 40
                6.0428571428571428571428571428571,
                0.43155118676576360584991608726924,
                3.14,
                91.3,
                32.126582278481012658227848101266, // 45
                22.95,
                46.75,
                19.98
            };
            // Loop through formulas; construct Formula objects from f
            for (int i = 0; i < formulas.Length; i++)
            {
                string f = formulas[i];
                object o = new Formula(f).Evaluate(L);
                if (o is double)
                {
                    double v = (double)o;
                    Assert.AreEqual(results[i], v, 0.001);
                }
                else
                {
                    Assert.Fail("Could not evaluate {" + f + "}, gave: " + ((FormulaError)o).Reason);
                }
            }
        }

        /// <summary>
        /// Evaluate(L()) should not return a FormulaError if L() always throws ArgumentExceptions but the formulas have no variables.
        /// L() throws ArgumentException.
        /// Using Formula(string) and parentheses, operators, and numbers, but not variables.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluate3()
        {
            // Create lookup
            Func<string, double> L = s =>
            {
                throw new ArgumentException();
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "30", // 1
                "0.9400",
                "12.03",
                "0",
                "0.0", // 5
                " 73  ",
                "  0.36 ",
                " 5.4  ",
                "  0 ",
                " 0.00  ", // 10
                "9.02+3.7",
                "12.7 +  4.33",
                "7.3-3.0",
                "15.8 -  2.9",
                "7.2*1.3", // 15
                "9.3 *  12.4",
                "60.5/1.25",
                "7.6 /  22.3",
                "(3.2)",
                "  ( 7.5  ) ", // 20
                "((42.3))",
                " ((  84.9 )  )  ",
                "(7.1+6)",
                "( 94.45  + 16  )",
                "(12.2-8)", // 25
                "(  22.3  - 17 )",
                "(27*0.3)",
                "( 15  *  4)",
                "(9/3.0)",
                "(12.7  / 6  )", // 30
                "9+(7.3)",
                "(12.9)+3",
                "17.3 +  ( 7.2  ) + 3",
                "8.2-(12)",
                "(0.6)-5.3", // 35
                "82 - (  4.3)  -12",
                "7*(8.2)",
                "(5.3)*4",
                "7 *(0.2  ) *  3",
                "9/(3.0)", // 40
                "(42.3)/7",
                "18/  ( 9.7 )/ 4.3",
                "15.84-(9+3.7)",
                "(83-4)+12.3",
                "6*(42.3/7.9)", // 45
                "(9*10.2)/4",
                "(42+(19/4))",
                "((4-1.3)*7.4)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                if (new Formula(f).Evaluate(L) is FormulaError) Assert.Fail("Got FormulaError from {" + f + "}");
            }
        }

        /// <summary>
        /// Evaluate(L()) should return a FormulaError when division by 0 occurs.
        /// L() returns -1.
        /// Using Formula(string) and formulas that have division by 0.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluateException1()
        {
            // Create lookup
            Func<string, double> L = s =>
            {
                return -1;
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "0/0",
                "12/0",
                "5/0 +3",
                "9/(0)",
                "66/(0+0)",
                "42/(0-0*6)",
                "5/(7.3-7.3)",
                "18/(4.6*0)",
                "4/(18/6 - 3)"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                if (new Formula(f).Evaluate(L) is double) Assert.Fail("Didn't get FormulaError from {" + f + "}");
            }
        }

        /// <summary>
        /// Evaluate(L()) should return a FormulaError when an undefined variable is found.
        /// L() throws ArgumentException, like no variable was found.
        /// Using Formula(string) and formulas with variables.
        /// </summary>
        [TestMethod]
        public void publicTestEvaluateException2()
        {
            // Create lookup
            Func<string, double> L = s =>
            {
                throw new ArgumentException();
            };
            // Create string array of formulas
            string[] formulas = new string[]{
                "7 + bill",
                "oxford/dictionary",
                "42/(7-dog)",
                "this + that"
            };
            // Loop through formulas; construct Formula objects from f
            foreach (string f in formulas)
            {
                if (new Formula(f).Evaluate(L) is double) Assert.Fail("Didn't get FormulaError from {" + f + "}");
            }
        }

        //****************Private Tests********************//
        [TestMethod]
        public void privateTestMethod1()
        {

        }
    }
}
