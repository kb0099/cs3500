﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace AgCubio
{
    public static class Network
    {
        // Represents a default port (const are static by default!)
        public const int DEFAULT_PORT = 11000;

        /// <summary>
        /// This function should attempt to connect to the server via a provided hostname. 
        /// It should save the callback function (in a state object) for use when data arrives.
        /// It will need to open a socket and then use the BeginConnect method. 
        /// Note this method take the "state" object and "regurgitates" it back
        /// when a connection is made, thus allowing "communication" between this function 
        /// and the Connected_to_Server function.
        /// </summary>
        /// <param name="callback">a function inside the View to be called when a 
        /// connection is made</param>
        /// <param name="hostname">the name of the server to connect to</param>
        /// <returns></returns>
        static Socket ConnectToServer(Action<PreservedState> callback, string hostname, int port = DEFAULT_PORT)
        {
            try
            {
                // Instantiate the remote endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectedToServer), new PreservedState() { clientSocket = clientSocket, callback = callback });

                return clientSocket;

                //connectDone.WaitOne();

                //// Send test data to the remote device.
                //Send(client, "This is a test<EOF>");
                //sendDone.WaitOne();

                //// Receive the response from the remote device.
                //Receive(client);
                //receiveDone.WaitOne();

                //// Write the response to the console.
                //Console.WriteLine("Response received : {0}", response);

                //// Release the socket.
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

            }
            catch (Exception e)
            {
                callback(new PreservedState() { errorMsg = e.Message });
                Console.WriteLine(e.ToString());
                return null;
            }
        }


        /// <summary>
        /// This function is reference by the BeginConnect method above and is "called" by the OS
        /// when the socket connects to the server. The "ar" object contains a field "AsyncState" 
        /// which contains the "state" object saved away in the above function.
        /// Once a connection is established the "saved away" callback function needs to called. 
        /// Additionally, the network connection should "BeginReceive" expecting more data to 
        /// arrive (and provide the ReceiveCallback function for this purpose)
        /// </summary>
        /// <param name="ar"></param>
        static void ConnectedToServer(IAsyncResult ar)
        {
            // Retrieve the PreservedState object
            PreservedState stateObj = (PreservedState)ar.AsyncState;
            try
            {
                // Complete the connection.
                stateObj.clientSocket.EndConnect(ar);
                stateObj.callback(stateObj);
                WantMoreData(stateObj);
            }
            catch (Exception ex)
            {
                stateObj.errorMsg = ex.Message;
                stateObj.callback(stateObj);
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// The ReceiveCallback method is called by the OS when new data arrives. 
        /// This method should check to see how much data has arrived. If 0, the connection 
        /// has been closed (presumably by the server). On greater than zero data, this method 
        /// should call the callback function provided above.
        /// For our purposes, this function should not request more data. 
        /// It is up to the code in the callback function above to request more data.
        /// </summary>
        /// <param name="ar"></param>
        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                PreservedState state = (PreservedState)ar.AsyncState;
                int count = state.clientSocket.EndReceive(ar);
                if (count <= 0)
                    return;
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, count));
                state.callback(state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// This is a small helper function that the client View code will call 
        /// whenever it wants more data. 
        /// Note: the client will probably want more data every time it gets data.
        /// </summary>
        /// <param name="state"></param>
        static void WantMoreData(PreservedState state)
        {
            state.clientSocket.BeginReceive(state.buffer, 0, 1024, SocketFlags.None, new AsyncCallback(ReceiveCallback), (object)state);
        }


        /// <summary>
        /// This function (along with it's helper 'SendCallback') will allow a 
        /// program to send data over a socket. This function needs to convert 
        /// the data into bytes and then send them using socket.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        static void Send(Socket socket, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            try
            {
                // Begin sending the data to the remote device.
                socket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
            }
            catch
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }


        /// <summary>
        /// This function "assists" the Send function. If all the data has been sent, 
        /// then life is good and nothing needs to be done If there is more data to send, 
        /// the SendCallBack needs to arrange to send this data 
        /// </summary>
        static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
