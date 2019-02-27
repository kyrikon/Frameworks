using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Core.Network
{
    public class StateObject
    {
        // Client socket.  
      
        // Size of receive buffer.  
        public const int BufferSize = 1024 * 32;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
        public Guid ClientID = Guid.Empty;
    }
}
