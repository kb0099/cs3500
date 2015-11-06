using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Model;

namespace TestWindow
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCube();
        }

        /// <summary>
        /// Code to test the behavior of the Cube object.
        /// </summary>
        private static void TestCube()
        {
            // create a cube
            Cube c = new Cube(20, 30, -2987746, 5318, false, "bill", 1026.3458);
            // serialize cube
            string message = JsonConvert.SerializeObject(c);
            Console.WriteLine("Cube serialized to: " + message);
            // deserialize cube
            Cube r = JsonConvert.DeserializeObject<Cube>(message);
            Console.Read();
        }
    }
}
