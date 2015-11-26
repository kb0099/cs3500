using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AgCubio
{
    class Server
    {
        private static World world;
        private static string configFilePath = "world_parameters.xml";
        private static List<Socket> clientSockets = new List<Socket>();

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
            InitWorld();
            return;
            
            Console.WriteLine("========== Server ============");
            Console.WriteLine("Type quit and press return/enter to stop the server.");
            Start();
            while (Console.ReadLine() != "quit") ;
        }

        public static void InitWorld()
        {
            // if file is not present in current director get from user
            //if(System.IO.File.Exists(configFilePath))
            GetFileFromUser();        
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
            timer.Interval = 500; // 1/heartbeat*1000
            timer.Elapsed += new ElapsedEventHandler(Update);
            timer.Start();
            // grow food

            Network.ServerAwaitingClientLoop(NewClientConnects);
        }

        /// <summary>
        /// This method should be a callback function for the networking code. It should setup a callback to recieve a
        /// player name, then request more data from the connection.
        /// </summary>
        private static void NewClientConnects(PreservedState ps)
        {
            //Console.WriteLine("Handling a new Client in thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            clientSockets.Add(ps.socket);
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

            // after getting name should send the player cube
            Network.Send(ps.socket, "{}");

            // should clear the received data
            ps.receivedData.Clear();

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
                            Console.WriteLine("ToDo: Moving!");
                            break;

                        case "split":
                            Console.WriteLine("ToDo: Splits!");
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException occurred while processing client message.\n" + ex.Message + "\n");
            }
            ps.receivedData.Clear();
        }

        /// <summary>
        /// This method should get the world to update. This involves adding new food, handle players eating cubes,
        /// handle player cube attrition, and handle sending the current state of the world to every client.
        /// Also, if a client disconnected, this method should clean it up.
        /// </summary>
        private static void Update(object o, ElapsedEventArgs e)
        {
            (o as System.Timers.Timer).Stop();

            // handle eat food, eat players
            
            // remove dead connections
            // lock on world and clients

            lock(world)
            {
                lock (clientSockets)
                {
                    for (int i = clientSockets.Count - 1; i >= 0; i--)
                    {
                        if (!Network.Send(clientSockets[i], "todo: update from server!"))
                        {
                            clientSockets.RemoveAt(i);
                        }
                    }
                }
            }
            (o as System.Timers.Timer).Start();
        }
    }
}
