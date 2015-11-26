﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AgCubio
{
    class Server
    {
        private static World world;
        private static string configFilePath;

        private static List<Socket> clientSockets = new List<Socket>();

        /// <summary>
        /// When the program starts, this function is run first.
        /// It should build a new world and start the server.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //world = new World(1000, 1000, 0);  
            Start();                         
        }

        /// <summary>
        /// This method should populate the initial world with food, set the heartbeat of the program, and await
        /// network client connections.
        /// </summary>
        private static void Start()
        {
            Console.WriteLine("========== Server ============");
            Console.WriteLine("Type quit and press return/enter to stop the server.");
            Start();
            while (Console.ReadLine() != "quit") ;
        }

        /// <summary>
        /// This method should be a callback function for the networking code. It should setup a callback to recieve a
        /// player name, then request more data from the connection.
        /// </summary>
        private void NewClientConnects()
        {

        }

        /// <summary>
        /// This method should be a callback function for the networking code. It should create a player cube (and
        /// update the world about it) and store necessary data for the connection to be used for further communication.
        /// It should also set up a callback for handling move/split requests and request new data from the socket.
        /// Finally, it should send the current stat of the world to the player.
        /// </summary>
        private void ReceivePlayerName()
        {

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
        private void Update(object o, ElapsedEventArgs e)
        {
            (o as Timer).Stop();

            // handle eat food, eat players
            // remove dead connections
            // lock on world and clients

            for (int i = clientSockets.Count - 1; i >= 0; i--)
            {
                if (!Network.Send(clientSockets[i], "todo: update from server!"))
                {
                    clientSockets.RemoveAt(i);
                }
            }
            (o as Timer).Start();
        }
    }
}
