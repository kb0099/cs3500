using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Net.Sockets;


namespace AgCubio
{
    /// <summary>
    /// Represents a state object for receiving data from server.
    /// </summary>
    class PreservedState
    {
        // Represents client socket.
        public Socket clientSocket = null;

        // Represents a callback delegate
        public Action<PreservedState> callback;

        // Size of receive buffer.
        public const int BUFFER_SIZE = 1024;

        // Receive buffer.
        public byte[] buffer = new byte[BUFFER_SIZE];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Represents error message in case of error
        public string errorMsg = null;
    }

}
