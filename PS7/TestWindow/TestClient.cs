using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgCubio
{
    class Program
    {
        public static void Main()
        {
            //JsonTesting();
            // SocketTesting();
            //ListTest2();
            DbTests();

            Console.ReadLine();
        }

        public static void DbTests()
        {
            Console.WriteLine(Db.AddGame());
        }

        public static void ListTest2()
        {
            // invalid
            List<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
            foreach (var x in list)
            {
                Console.WriteLine(" count: " + list.Count);
                list.Remove(x);
            }

        }
        public static void ListTest()
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };

            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(" i = " + i + ", count: " + list.Count);
                list.RemoveAt(0);
            }
            Console.WriteLine(" i = " + n + ", count: " + list.Count);
        }

        public static void JsonTesting()
        {
            try
            {
                Cube c0 = JsonConvert.DeserializeObject<Cube>("{}");
                Console.WriteLine("c0 = " + c0);
                Cube c = JsonConvert.DeserializeObject<Cube>("{invalid}");
                Console.WriteLine($"Didn't throw exception, c = {c}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception = {ex.GetType()}");
            }
        }
        static void SocketTesting()
        {
            // TODO TestCube() is broken due to bad references to Json.NET
            // TestCube();
            //TestWorld();

            LinkedList<PreservedState> list = new LinkedList<PreservedState>();
            string s = null;
            while (true)
            {
                s = Console.ReadLine();
                if (s == "exit") break;
                switch (s)
                {
                    case "process":
                        Task.Run(() =>
                       {
                           Network.ConnectToServer((pso) =>
                           {

                               Network.Send(pso.socket, "MyNameIsGood");
                               StringBuilder toFile = new StringBuilder();
                               pso.callback = (p) => {
                                   var str = pso.receivedData.ToString();
                                   var arr = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                   toFile.Append("# of items: " + arr.Length);

                                   // reset the receive data
                                   pso.receivedData.Clear();

                                   // preferrably all element should be a valid  json, except last one which might miss the closing }
                                   int j = 0;
                                   try
                                   {
                                       for (; j < arr.Length; j++)
                                       {
                                           toFile.Append(JsonConvert.DeserializeObject<Cube>(arr[j]).uId);
                                       }
                                   }
                                   catch
                                   {
                                       if (j == arr.Length - 1)
                                           pso.receivedData.Append(arr[j]);
                                       else
                                       {
                                           toFile.AppendLine("\n" + "Unable to parse: " + arr[j]);
                                       }
                                   }
                               };

                               // receive data { } { } { } ....
                               for (int i = 0; i < 10; i++)
                                   Network.WantMoreData(pso);

                               System.IO.File.WriteAllText("TestServerData.txt", toFile.ToString());
                               Console.WriteLine("done!");

                           }, "localhost");

                       });                       
                        break;
                    case "add":
                        CreateSocket(list);
                        break;
                    case "sendName":
                        Console.Write("\tName: ");
                        list.Last.Value.socket.Send(Encoding.UTF8.GetBytes(Console.ReadLine() + "\n"));
                        break;
                    case "close":
                        list?.Last?.Value?.socket?.Close();
                        break;
                    case "send":
                        Console.Write("\tCommand: ");
                        Network.Send(list.Last.Value.socket, Console.ReadLine());
                        break;
                    case "receive":
                        list.Last.Value.receivedData.Clear();
                        Network.WantMoreData(list.Last.Value);
                        break;
                    case "m":
                        for(int i = 0; i < 500; i++)
                        {
                            Network.Send(list.Last.Value.socket, "(move, 0, 0)\n");
                        }
                        break;
                    case "repeat":
                        Console.Write("Command to repeat 500 times: ");
                        s = Console.ReadLine();
                        for(int i = 0; i < 500; i++)
                        {
                            Network.Send(list.Last.Value.socket, s + "\n");
                        }
                        break;
                    case "u":
                        Network.Send(list.Last.Value.socket, "(move, 0, 2000)\n");
                        break;
                    case "d":
                        Network.Send(list.Last.Value.socket, "(move, 0, -2000)\n");
                        break;
                    case "l":
                        Network.Send(list.Last.Value.socket, "(move, -2000, 0)\n");
                        break;
                    case "4":
                        Network.Send(list.Last.Value.socket, "(move, +2000, 0)\n");
                        break;
                }
                System.Threading.Thread.Sleep(1000);
                Console.Write("\nSwitch: ");
            }
        }

        static void CreateSocket(LinkedList<PreservedState> list)
        {
            Network.ConnectToServer((s) => {
                //Console.WriteLine("========Received Data==========");
                Console.WriteLine(s.receivedData);
               if(s.errorMsg != null)
                    Console.WriteLine("Error: " + s.errorMsg);
                list.AddLast(s);
            }, "localhost");

            Console.WriteLine("tried to connetc... ");
        }


        /// <summary>
        /// Code to test the behavior of the Cube object.
        /// </summary>
        private static void TestCube()
        {
            Console.WriteLine("Starting TestCube()");
            // create a cube
            Cube c = new Cube(20, 30, -2987746, 5318, 5318, false, "bill", 10001);
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

        ///// <summary>
        ///// Code to test the behavior of the World object.
        ///// </summary>
        //private static void TestWorld()
        //{
        //    Console.WriteLine("Starting TestWorld()");
        //    // create world, player id 1234
        //    World w = new World(100, 100, 1234);
        //    // add cube
        //    Cube c = new Cube(50, 48, -300000, 1234, 1234, false, "Player", 200);
        //    Console.Write("Adding cube: ");
        //    PrintCubeData(c);
        //    Console.WriteLine();
        //    w.AddCube(c);
        //    PrintWorldCubes(w);
        //    // add same cube with different data
        //    c = new Cube(55, 42, -300000, 1234, 1234, false, "Player", 203);
        //    Console.Write("Adding cube: ");
        //    PrintCubeData(c);
        //    Console.WriteLine();
        //    w.AddCube(c);
        //    PrintWorldCubes(w);
        //    // add different cube
        //    c = new Cube(3, 15, -400000, 100, 100, true, "", 1);
        //    Console.Write("Adding cube: ");
        //    PrintCubeData(c);
        //    Console.WriteLine();
        //    w.AddCube(c);
        //    PrintWorldCubes(w);
        //    // add cube of similar mass to another
        //    c = new Cube(75, 18, -200000, 2240, 2240, false, "Enemy", 203);
        //    Console.Write("Adding cube: ");
        //    PrintCubeData(c);
        //    Console.WriteLine();
        //    w.AddCube(c);
        //    PrintWorldCubes(w);
        //    // kill a cube
        //    c = new Cube(75, 18, -200000, 2240, 2240, false, "Enemy", 0);
        //    Console.Write("Adding cube: ");
        //    PrintCubeData(c);
        //    Console.WriteLine();
        //    w.AddCube(c);
        //    PrintWorldCubes(w);
        //    // get player cubes
        //    Console.WriteLine("Checking player cubes in current state");
        //    IEnumerable<Cube> pc = w.GetPlayerCubes();
        //    PrintPlayerCubes(w);
        //    // add cube for player's team
        //    c = new Cube(75, 18, -200000, 1250, 1234, false, "Player", 200);
        //    Console.Write("Adding cube: ");
        //    PrintCubeData(c);
        //    Console.WriteLine();
        //    w.AddCube(c);
        //    PrintWorldCubes(w);
        //    // get player cubes again
        //    Console.WriteLine("Checking player cubes in current state");
        //    pc = w.GetPlayerCubes();
        //    PrintPlayerCubes(w);
        //    Console.WriteLine("TestWorld() done");
        //}


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
            foreach (Cube c in w.GetCubes()) 
            {
                Console.Write("\t");
                PrintCubeData(c);
                Console.WriteLine();
            }
            Console.WriteLine("}");
        }
    }
}
