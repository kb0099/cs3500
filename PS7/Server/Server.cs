using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Drawing;

namespace AgCubio
{
    class Server
    {
        // this is the server world!
        private static World world;
        private static string configFilePath = "world_parameters.xml";      // default location
        private static Dictionary<Socket, int> clientSockets = new Dictionary<Socket, int>();   // socket to player uid mapping

        // current working directory
        private static string cwd = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// When the program starts, this function is run first.
        /// It should build a new world and start the server.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            if (!InitWorld()) return;

            Console.WriteLine("========== Server ============");
            Console.WriteLine("Type quit and press return/enter to stop the server.");
            Start();
            while (Console.ReadLine() != "quit") ;
        }

        /// <summary>
        /// Tries to initialize the server world from a config file.
        /// First checks for file named "world_parameters.xml" in executable's directory.
        /// If it cannot loate it will ask the user to locate the file.
        /// </summary>
        /// <returns>True if successful false otherwise.</returns>
        public static bool InitWorld()
        {
            // if file is not present in current director get from user
            string path = System.IO.Path.Combine(cwd, configFilePath);
            if (!System.IO.File.Exists(path))
                path = GetFileFromUser();
            try
            {
                world = new World(path);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create world from file: " + path + "\n" + ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// This method will open a Open File dialog and retrieve the file path the user provides.
        /// If the dialog does not successfully retrieve a file path, the method returns null.
        /// </summary>
        private static string GetFileFromUser()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = cwd;
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1; // uses .xml filter when opened
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
                return null;
        }

        /// <summary>
        /// This method should populate the initial world with food, set the heartbeat of the program, and await
        /// network client connections.
        /// </summary>
        private static void Start()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1.0/world.HeartbeatsPerSecond * 1000;
            timer.Elapsed += new ElapsedEventHandler(Update);
            timer.Start();
            // grow/populate some food to the max_food
            Cube tempFood;  // will be ignored.
            while (world.AddFood(out tempFood)) { /* just grow food till it reaches max */ }
            // set up connections
            Network.ServerAwaitingClientLoop(NewClientConnects);
        }

        /// <summary>
        /// This method should be a callback function for the networking code. It should setup a callback to recieve a
        /// player name, then request more data from the connection.
        /// </summary>
        private static void NewClientConnects(PreservedState ps)
        {
            //Console.WriteLine("Handling a new Client in thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            ps.callback = ReceivePlayerName;
            Network.WantMoreData(ps);
        }

        /// <summary>
        /// This method should be a callback function for the networking code. It should create a player cube (and
        /// update the world about it) and store necessary data for the connection to be used for further communication.
        /// It should also set up a callback for handling move/split requests and request new data from the socket.
        /// Finally, it should send the current stat of the world to the player.
        /// </summary>
        static private void ReceivePlayerName(PreservedState ps)
        {
            Console.WriteLine("Receiving a new Client data.");
            Console.WriteLine("Client sent name: " + ps.receivedData);

            // after getting name world should generate a new cube
            Cube player = world.NextPlayer(ps.receivedData.ToString().TrimEnd(new char[] { ' ', '\n' }));

            // send the player cube
            Network.Send(ps.socket, JsonConvert.SerializeObject(player) + "\n");

            // add to update queue after receiving name
            lock (clientSockets)
                clientSockets[ps.socket] = player.uId;

            // should clear the received data
            ps.receivedData.Clear();

            // send the initial world only to this socket
            List<Cube> sendFood = new List<Cube>(world.foodCubes.Values); // TODO: SendCubes() is able to have exception if food cubes are removed while it is trying to send them; this is temp way to try separating the collections while food is sent over, would be better to set up locks for foodCubes
            SendCubes(sendFood, ps.socket);

            // Ready to receive commands
            ps.callback = ProcessClientData;
            Network.WantMoreData(ps);
        }

        /// <summary>
        /// This method should be a callback function for the networking code. It should interpret that a client
        /// wants to move/split and respond accordingly.
        /// </summary>
        private static void ProcessClientData(PreservedState ps)
        {
            int x = 0, y = 0;
            try
            {
                string[] cmds = ps.receivedData.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var cmdStr in cmds)
                {
                    string[] tokens = cmdStr.Split(new char[] { ' ', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 3) continue;

                    // get params
                    int.TryParse(tokens[1], out x);
                    int.TryParse(tokens[2], out y);

                    switch (tokens[0])
                    {
                        case "move":
                            world.MoveCube(clientSockets[ps.socket], x, y);
                            break;

                        case "split":
                            //Console.WriteLine("ToDo: Splits!");
                            world.SplitCube(clientSockets[ps.socket], x, y);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException occurred while processing client message.\n" + ex.Message + "\n");
            }
            ps.receivedData.Clear();
            Network.WantMoreData(ps);
        }

        /// <summary>
        /// This method should get the world to update. This involves adding new food, handle players eating cubes,
        /// handle player cube attrition, and handle sending the current state of the world to every client.
        /// Also, if a client disconnected, this method should clean it up.
        /// </summary>
        private static void Update(object o, ElapsedEventArgs e)
        {
            // Stop raising further events until this call is done!
            (o as System.Timers.Timer).Stop();

            // grow one food in each update according to the spec
            Cube fd;
            world.AddFood(out fd);
            // try sending that food to all clients
            if(fd != null)
            {
                SendCubes(new Cube[] { fd });
            }

            // handle eat food, then, send to clients
            IEnumerable<Cube> eatenFood = world.EatFoods();
            SendCubes(eatenFood);

            world.ApplyAttrition();
            // handle eat players, then, send dead cubes
            IEnumerable<Cube> eatenPlayers = world.EatPlayers();
            SendCubes(eatenPlayers);

            // Finally, send remaining player cubes.
            SendCubes(world.playerCubes.Values);

            // okay to raise another event
            (o as System.Timers.Timer).Start();
        }
        /// <summary>
        /// Helper for Update function to send cubes to all clients.
        /// </summary>
        /// <param name="cubes">The updated cubes to send to all clients.</param>
        private static void SendCubes(IEnumerable<Cube> cubes)
        {
            lock (world)
            {
                lock (clientSockets)
                {
                    List<Socket> removal = new List<Socket>();
                    foreach (Socket s in clientSockets.Keys)
                    {
                        foreach (Cube c in cubes)
                        {
                            // the connection is dead, safe to remove the socket, but the cube remains in the world
                            if (!Network.Send(s, JsonConvert.SerializeObject(c) + "\n"))
                            {
                                //clientSockets.Remove(s);
                                removal.Add(s);
                            }
                        }
                    }
                    // TODO: removing sockets in foreach causes exceptions, so removal done outside of loop
                    foreach (Socket removeS in removal)
                    {
                        clientSockets.Remove(removeS);
                    }
                }
            }

        }

        /// <summary>
        /// Sends the <paramref name="cubes"/> through a specific socket <paramref name="s"/>
        /// </summary>
        /// <param name="cubes">Cube to send to.</param>
        /// <param name="s">Socket to send through.</param>
        private static void SendCubes(IEnumerable<Cube> cubes, Socket s)
        {
            lock (world)
            {
                lock (clientSockets)
                {
                        foreach (Cube c in cubes)
                        {
                            if (!Network.Send(s, JsonConvert.SerializeObject(c) +"\n"))
                            {
                                clientSockets.Remove(s);
                            }
                        }
                }

            }
        }
    }
}
