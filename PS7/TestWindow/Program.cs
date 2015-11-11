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
            // TODO TestCube() is broken due to bad references to Json.NET
            // TestCube();
            TestWorld();

            Console.Read();
        }

        /// <summary>
        /// Code to test the behavior of the Cube object.
        /// </summary>
        private static void TestCube()
        {
            Console.WriteLine("Starting TestCube()");
            // create a cube
            Cube c = new Cube(20, 30, -2987746, 5318, 5318, false, "bill", 1026.3458);
            Console.Write("Original cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            // serialize cube
            string message = JsonConvert.SerializeObject(c);
            Console.WriteLine("Cube serialized to: " + message);
            // deserialize cube
            Cube r = JsonConvert.DeserializeObject<Cube>(message);
            Console.Write("Deserialized cube: ");
            PrintCubeData(r);
            Console.WriteLine();
            Console.WriteLine("TestCube() done");
        }

        /// <summary>
        /// Code to test the behavior of the World object.
        /// </summary>
        private static void TestWorld()
        {
            Console.WriteLine("Starting TestWorld()");
            // create world, player id 1234
            World w = new World(100, 100, 1234);
            // add cube
            Cube c = new Cube(50, 48, -300000, 1234, 1234, false, "Player", 200);
            Console.Write("Adding cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            w.AddCube(c);
            PrintWorldCubes(w);
            // add same cube with different data
            c = new Cube(55, 42, -300000, 1234, 1234, false, "Player", 203);
            Console.Write("Adding cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            w.AddCube(c);
            PrintWorldCubes(w);
            // add different cube
            c = new Cube(3, 15, -400000, 100, 100, true, "", 1);
            Console.Write("Adding cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            w.AddCube(c);
            PrintWorldCubes(w);
            // add cube of similar mass to another
            c = new Cube(75, 18, -200000, 2240, 2240, false, "Enemy", 203);
            Console.Write("Adding cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            w.AddCube(c);
            PrintWorldCubes(w);
            // kill a cube
            c = new Cube(75, 18, -200000, 2240, 2240, false, "Enemy", 0);
            Console.Write("Adding cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            w.AddCube(c);
            PrintWorldCubes(w);
            // get player cubes
            Console.WriteLine("Checking player cubes in current state");
            IEnumerable<Cube> pc = w.GetPlayerCubes();
            PrintPlayerCubes(w);
            // add cube for player's team
            c = new Cube(75, 18, -200000, 1250, 1234, false, "Player", 200);
            Console.Write("Adding cube: ");
            PrintCubeData(c);
            Console.WriteLine();
            w.AddCube(c);
            PrintWorldCubes(w);
            // get player cubes again
            Console.WriteLine("Checking player cubes in current state");
            pc = w.GetPlayerCubes();
            PrintPlayerCubes(w);
            Console.WriteLine("TestWorld() done");
        }


        /// <summary>
        /// A helper method to print cube data to a line on console. Prints a line, not a newline.
        /// </summary>
        /// <param name="c"></param>
        private static void PrintCubeData(Cube c)
        {
            Console.Write("{X:"+c.X+",Y:"+c.Y+",argb_color:"+c.argb_color+",uid:"+c.uId+",team_id:"+c.teamId+",food:"+c.food+",Name:"+c.Name+",Mass:"+c.Mass+"}");
        }

        /// <summary>
        /// A helper method to print world data (collection of cubes) to lines on console.
        /// </summary>
        /// <param name="w"></param>
        private static void PrintWorldCubes(World w)
        {
            Console.WriteLine("World Cubes {");
            foreach (Cube c in w.GetCubes())
            {
                Console.Write("\t");
                PrintCubeData(c);
                Console.WriteLine();
            }
            Console.WriteLine("}");
        }

        /// <summary>
        /// A helper method to print world data (collection of cubes for the player) to lines on console.
        /// </summary>
        /// <param name="w"></param>
        private static void PrintPlayerCubes(World w)
        {
            Console.WriteLine("Player cubes {");
            foreach (Cube c in w.GetPlayerCubes())
            {
                Console.Write("\t");
                PrintCubeData(c);
                Console.WriteLine();
            }
            Console.WriteLine("}");
        }
    }
}
