using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace AgCubio
{
    public static class Network
    {
        static PreservedState preservedStateObject = new PreservedState();
        
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
        Socket ConnectToServer(Action callback, string hostname)
        {
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                Socket client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.
                Send(client, "This is a test<EOF>");
                sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                Console.WriteLine("Response received : {0}", response);

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
        void ConnectedToServer(IAsyncResult ar)
        {
        }

        /// <summary>
        /// The ReceiveCallback method is called by the OS when new data arrives. 
        /// This method should check to see how much data has arrived. If 0, the connection 
        /// has been closed (presumably by the server). On greater than zero data, this method 
        /// should call the callback function provided above.
        /// For our purposes, this function should not request more data. 
        /// It is up to the code in the callback function above to request more data.
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        void ReceiveCallback(IAsyncResult ar)
        {

        }

        /// <summary>
        /// This is a small helper function that the client View code will call 
        /// whenever it wants more data. 
        /// Note: the client will probably want more data every time it gets data.
        /// </summary>
        /// <param name="psObject"></param>
        void WantMoreData(PreservedState psObject)
        {

        }

       
        /// <summary>
        /// This function (along with it's helper 'SendCallback') will allow a 
        /// program to send data over a socket. This function needs to convert 
        /// the data into bytes and then send them using socket.BeginSend.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        void Send(Socket socket, String data)
        {
        }


        /// <summary>
        /// This function "assists" the Send function. If all the data has been sent, 
        /// then life is good and nothing needs to be done If there is more data to send, 
        /// the SendCallBack needs to arrange to send this data 
        /// </summary>
        void SendCallBack()
        {

        }
    }
}
