using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            float x = 33554431;
            Console.WriteLine("{0, 20:G}", x);
            char[] arr = "aAaabbbbbBBcDdd\0".ToCharArray();
            ToggleChar(arr);
            ToggleStr(arr, 1, 0);
            Console.ReadLine();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="i">Current char</param>
        /// <param name="j">Last unique/nonrepeating char position</param>
        public static void ToggleStr(char[] arr, int i, int j)
        {

            if (arr[j] == arr[i])      // if current char is same as last nonrepeating char
            {
                ToggleStr(arr, i+1, j);
            }
            else
            {
                arr[j + 1] = arr[i];
                if(arr[i] != '\0')
                    ToggleStr(arr, i+1, j + 1);
                Console.Write(arr[j]);
            }
        }

        public static void ToggleChar(char[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                char c = arr[i];
                if (Char.IsUpper(c))
                    arr[i] = Char.ToLower(c);
                else
                    arr[i] = Char.ToUpper(c);
            }
        }
    }
}
