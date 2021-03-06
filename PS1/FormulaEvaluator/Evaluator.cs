﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// Represents a static class that acts as a container for static members.
    /// <para>
    /// Created as Part of:
    /// University of Utah
    /// CS 3500 Fall 2015, Assignment 1
    /// by Kedar Bastakoti
    /// Completed: 09/10/2015
    /// </para>
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Represents a valid pattern for variable name or identifier.
        /// <para>PS1 Requirement: Variable name consists of one or more letters followed by one or more digits.</para>
        /// </summary>
        private const String VALID_VAR_PATTERN = @"^[a-zA-Z]+[0-9]+$";

        /// <summary>
        /// A delegate that can be used to look up the value of a variable. 
        /// <para>Given a variable as its parameter, the delegate will either return 
        /// an int (the value of the variable) or throw an ArgumentException 
        /// (if the variable has no value).</para>
        /// </summary>
        /// <param name="v">Variable of <see cref="System.String"/> type.</param>
        /// <returns>The value represented by the <paramref name="v"/>.</returns>
        public delegate int Lookup(String v);

        /// <summary>
        /// The method evaluates the expression, using the algorithm provided. 
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if one of the possible errors from the algorithm occurs.</exception>
        /// <param name="exp">The expression to be evaluated. </param>
        /// <param name="variableEvaluator">A delegate that can be used to look up the value of a variable.</param>
        /// <returns>It returns the value of the expression (if it has a value). </returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Stack<double> valueStack = new Stack<double>();
            Stack<char> opStack = new Stack<char>();
            char[] plusMinus = new char[] { '+', '-' };
            char[] mulDiv = new char[] { '*', '/' };

            int t = 0;  //temporary variable
            double d = 0; // temporary variable to hold parsed value

            if (String.IsNullOrEmpty(exp))
                throw ArgEx();

            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //process each token
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Trim();
                switch (tokens[i])
                {
                    case "":
                        break;
                    case "+":
                    case "-":
                        /*
                        If + or - is at the top of the operator stack, pop the value stack twice and the operator stack once. 
                        Apply the popped operator to the popped numbers. Push the result onto the value stack. 
                        Next, push t onto the operator stack
                        */
                        if (opStack.HasOnTop(plusMinus))
                        {
                            if (valueStack.Count < 2)
                                throw ArgEx();
                            t = (int)Calc(valueStack.Pop(), valueStack.Pop(), opStack.Pop());
                            valueStack.Push(t);
                        }
                        opStack.Push(tokens[i][0]);
                        break;
                    case "*":
                    case "/":
                        /*
                        Push t onto the operator stack
                        */
                        opStack.Push(tokens[i][0]);              
                        break;
                    case "(":
                        opStack.Push('(');
                        break;
                    case ")":
                        /*
                        If + or - is at the top of the operator stack, pop the value stack twice and the operator stack once. 
                        Apply the popped operator to the popped numbers. Push the result onto the value stack. 
                        Next, the top of the operator stack should be a (. Pop it. Finally, if * or / is at the top of the 
                        operator stack, pop the value stack twice and the operator stack once. Apply the popped operator to the popped numbers. 
                        Push the result onto the value stack.
                        */
                        if (opStack.HasOnTop(plusMinus))
                        {
                            if (valueStack.Count < 2)
                                throw ArgEx();
                            valueStack.Push((int)Calc(valueStack.Pop(), valueStack.Pop(), opStack.Pop()));
                        }
                        // opStack should at least contain '('
                        if (opStack.Count < 1 || !opStack.Pop().Equals('('))
                            throw ArgEx();
                        // there still might be '*' or '/' after popping '(' for example: 1 + 2 * (3 + 4)
                        if (opStack.HasOnTop(mulDiv))
                        {
                            if (valueStack.Count < 2)
                                throw ArgEx();
                            valueStack.Push((int)Calc(valueStack.Pop(), valueStack.Pop(), opStack.Pop()));

                        }
                        break;
                    default:
                        if (Double.TryParse(tokens[i], out d))
                        {
                            // token is integer(or double)
                            /*If * or / is at the top of the operator stack, pop the value stack, pop the operator stack, 
                            and apply the popped operator to t and the popped number. Push the result onto the value stack.
                            Otherwise, push t onto the value stack.
                            */
                            t = (int)d;
                        }
                        else if (IsValidVar(tokens[i]))
                        {
                            // token is a variable
                            /*
                            Proceed as above, using the looked-up value of t instead of t
                            */
                            t = variableEvaluator(tokens[i]);
                        }
                        else
                        {
                            throw ArgEx();
                        }
                        // at this point t has a value assigned to it
                        if (opStack.HasOnTop(mulDiv))
                        {
                            if (valueStack.Count == 0 || t == 0)
                                throw ArgEx();
                            t = (int)Calc(t, valueStack.Pop(), opStack.Pop());
                        }
                        valueStack.Push(t);

                        break;
                }
            }
            // after all the tokens has been processed
            /*
            If Operator stack is empty	
            Value stack should contain a single number
            Pop it and report as the value of the expression

            Else, There should be exactly one operator on 
            the operator stack, and it should be either + or -. There should be 
            exactly two values on the value stack. Apply the operator to the two 
            values and report the result as the value of the expression.
            */
            if (opStack.Count == 0)
            {
                if (valueStack.Count == 1)
                    return (int)valueStack.Pop();
                else
                    throw ArgEx();
            }
            else
            {
                if (!(valueStack.Count == 2 && opStack.Count == 1))
                    throw ArgEx();
                else
                    return (int)Calc(valueStack.Pop(), valueStack.Pop(), opStack.Pop());
            }
        }
        /// <summary>
        /// Checks if top of the stack has any of the array values.
        /// </summary>
        /// <typeparam name="T">The underlying type of values.</typeparam>
        /// <param name="stack">Represents stack.</param>
        /// <param name="values">An array of T.</param>
        /// <returns></returns>
        private static bool HasOnTop<T>(this Stack<T> stack, T[] values)
        {
            if(stack.Count > 0)
                foreach(T v in values)
                {
                    if (stack.Peek().Equals(v))
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Represents a helper method for arithmetic operations based on stack pop order.
        /// The order of operation is reversed: v2 op v1
        /// Example: Calc(1, 2, '-') would return the value of 2-1 which is 1.
        /// </summary>
        /// <param name="v1">First popped item</param>
        /// <param name="v2">Last popped item</param>
        /// <param name="op">Operator: +, -, *, or /</param>
        /// <returns>The result of applying the operator to the operands in reverse order</returns>
        private static double Calc(double v1, double v2, char op)
        {
            switch (op)
            {
                case '+':
                    return v2 + v1;
                case '-':
                    return v2 - v1;
                case '*':
                    return v2 * v1;
                case '/':
                    return v2 / v1;
                default:
                    throw ArgEx();
            }
        }

        /// <summary>
        /// Throws the prevalent ArgumentException.
        /// </summary>
        private static ArgumentException ArgEx()
        {            
            return new ArgumentException("Expression contains invalid token or operation.");
        }

        /// <summary>
        /// Checks if the passed parameter is a valid variable name.
        /// </summary>
        /// <param name="varName">Variable name to check</param>
        /// <returns>True if valid else False</returns>
        private static bool IsValidVar(String varName)
        {
            return Regex.IsMatch(varName, VALID_VAR_PATTERN);
        }
    }
}
