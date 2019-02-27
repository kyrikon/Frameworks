using Newtonsoft.Json;
using Core.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

// State object for reading client data asynchronously  
namespace Core.Network
{

    public class AsynchronousSocketListener
    {

        #region Instance Variables / Events / Delegates
        public ManualResetEvent _allDone = new ManualResetEvent(false);
        private const int QueueTimeout = 100;
        private const int RecieveBufferSize = 1024;
        private Dictionary<Guid,bool> _ReceiveQueueRun = new Dictionary<Guid, bool>();
        private Dictionary<Guid,ManualResetEvent> receiveDone = new Dictionary<Guid,ManualResetEvent>();
        private Dictionary<Guid, string> _ConnectedClientNames = new Dictionary<Guid, string>();

        #endregion
        #region Constructors
        public AsynchronousSocketListener()
        {
            ConnectedClients = new Dictionary<Guid, Socket>();
            ID = Guid.NewGuid();
            ReceiveQueue = new Queue<NetworkMessage>();
        }
        #endregion

        #region Properties
        #region Server Info
        public Dictionary<Guid, Socket> ConnectedClients
        {
            get;
            set;
        }
        public Guid ID
        {
            get; private set;
        }
       
        #endregion
        #region Socket
        public IPEndPoint EndPoint
        {
            get;
            internal set;
        }
        public IPAddress EndPointIP
        {
            get;
            internal set;
        }
        public string EndPointName
        {
            get;
            internal set;
        }

        public Socket Listener
        {
            get;
            internal set;
        }

     
        #endregion

        #region Queues
        public Queue<NetworkMessage> SendQueue
        {
            get; internal set;
        }
        public Queue<NetworkMessage> ReceiveQueue
        {
            get; internal set;
        }
        public List<NetworkMessage> SendHistory
        {
            get; internal set;
        }
        public List<NetworkMessage> ReceiveHistory
        {
            get; internal set;
        }
        #endregion

        #endregion
        #region Methods     
        public void StartListening(int port)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[RecieveBufferSize];

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            EndPointName = ipHostInfo.HostName;
         
            foreach (var item in ipHostInfo.AddressList)
            {
                Console.WriteLine($"{item.ToString()}");
            }
            EndPointIP = ipHostInfo.AddressList[0];
            EndPoint = new IPEndPoint(EndPointIP, port);
            //  Console.WriteLine($"{localEndPoint.Address} port {localEndPoint.Port}");

            // Create a TCP/IP socket.  
            Listener = new Socket(EndPointIP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                Listener.Bind(EndPoint);
                Listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    _allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine($"Server ID {ID}\nWaiting for a connection...");
                    Listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        new object());

                    // Wait until a connection is made before continuing.  
                    _allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            _allDone.Set();

            // Get the socket that handles the client request.  
           
            Socket handler = Listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            Guid ClientID = Guid.NewGuid();
            ConnectedClients.Add(ClientID, handler);
            receiveDone.Add(ClientID, new ManualResetEvent(false));
             //Once connected send ID
             NetworkMessage NW = new NetworkMessage(ID);
            ConnectionMessage CM = new ConnectionMessage() { ServerID = ID,ClientID = ClientID };
            NW.MessageBody = new MessageBody() { MessageType = MessageTypes.ConnectMsg,MessageSerial = JsonConvert.SerializeObject(CM) };

            Send(ClientID, NW);

            //finally start receive Pump
            Task.Run(() => StartReceiveQueuePump(ClientID));
        }

        public string GetClientName(Guid id)
        {
            if (_ConnectedClientNames.ContainsKey(id))
            {
                return _ConnectedClientNames[id];
            }
            return "Client name not set";
        }

        #region Read
        private void Receive(Guid ClientID)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();               
                state.ClientID = ClientID;
                // Begin receiving the data from the remote device.  
                ConnectedClients[ClientID].BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = ConnectedClients[state.ClientID];

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                if (handler.Available > 0)
                {
                    // Get the rest of the data. if available 
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
            }
            HandleReadBuff(state);
            receiveDone[state.ClientID].Set();
          
        }
        private void HandleReadBuff(StateObject StateBuffer)
        {
            //If NetworkMessage.EndOfMessage is in the string, extract it , deserialise and add it to
            //the receive Queue

            if (StateBuffer.sb.Length > 1)
            {
                Tuple<List<NetworkMessage>, string> DecodeMsg = NetworkMessage.DecodeMessageBuffer(StateBuffer.sb.ToString());
                if (DecodeMsg.Item1 != null && DecodeMsg.Item1.Any())
                {
                    foreach (NetworkMessage NM in DecodeMsg.Item1)
                    {
                        HandleMsg(NM);
                        ReceiveQueue.Enqueue(NM);
                    }
                    StateBuffer.sb.Clear();
                    StateBuffer.sb.Append(DecodeMsg.Item2);
                }
            }
        }
        #endregion

        #region Send
        public void Send(Guid handler, NetworkMessage data)
        {

            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes($"{ JsonConvert.SerializeObject(data)}{NetworkMessage.EndOfMessage}");

            // Begin sending the data to the remote device.  
            ConnectedClients[handler].BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Guid handlerID = (Guid)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = ConnectedClients[handlerID].EndSend(ar);
                Console.WriteLine($"Sent {bytesSent} bytes to client ID {handlerID}");

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region Queues
        public void StartReceiveQueuePump(Guid clientID)
        {
            if (_ReceiveQueueRun.ContainsKey(clientID))
            {
                _ReceiveQueueRun[clientID] = true;
            }
            else
            {
                _ReceiveQueueRun.Add(clientID, true);
            }
           
            while (_ReceiveQueueRun[clientID])
            {
                Receive(clientID);
                receiveDone[clientID].WaitOne();
                Thread.Sleep(QueueTimeout);
            }
        }
        public void StopReceiveQueuePump(Guid clientID)
        {
            _ReceiveQueueRun[clientID] = false;
        }
        public void HandleMsg(NetworkMessage mn)
        {
            if (mn.MessageBody.MessageType == MessageTypes.StringMsg)
            {
                Console.WriteLine($"Message received from client {mn.SenderID}. {mn.MessageBody.MessageSerial}");
                if (mn.RecipientIDs != null &&  mn.RecipientIDs.Any())
                {
                    RelayMessage(mn);
                }
                return;
            }
            if(mn.MessageBody.MessageType == MessageTypes.ConnectMsg)
            {
                if (_ConnectedClientNames.ContainsKey(mn.SenderID))
                {
                    _ConnectedClientNames[mn.SenderID] = mn.MessageBody.MessageSerial;
                }
                else
                {
                    _ConnectedClientNames.Add(mn.SenderID, mn.MessageBody.MessageSerial);
                }
                Console.WriteLine($"Message received from client {mn.SenderID}. Client Name set to  {mn.MessageBody.MessageSerial}");
                NotifyConnections();
                return;
            }
            if (mn.MessageBody.MessageType == MessageTypes.GetDate)
            {
                NetworkMessage RNW = new NetworkMessage(ID);
                RNW.ResponseTo = mn.ID;
                DateTime SendNow = DateTime.Now;
                RNW.MessageBody = new MessageBody() {  MessageType = MessageTypes.GetDate,MessageSerial = JsonConvert.SerializeObject(SendNow) };
                Send(mn.SenderID,RNW);
                Console.WriteLine($"Date Request message received from client {mn.SenderID}. Sending Date {SendNow.ToString("dd/MM/yyyy hh:mm:ss tt")}");             
                return;
            }
        }
       public void RelayMessage(NetworkMessage NM)
        {
            foreach(Guid ClientID in NM.RecipientIDs)
            {
                NetworkMessage RelayMsg = new NetworkMessage(NM.SenderID);
                RelayMsg.MessageBody = NM.MessageBody;                
                Send(ClientID, NM);
            }
        }
        public void NotifyConnections()
        {
            NetworkMessage nm = new NetworkMessage(ID);                     
            nm.MessageBody = new MessageBody() { MessageType = MessageTypes.GetClients, MessageSerial = JsonConvert.SerializeObject(_ConnectedClientNames) };
            foreach (Guid ClientID in ConnectedClients.Keys)
            {
                Send(ClientID, nm);
            }
            Console.WriteLine("Sent Connections to clients");
        }
        public void SendDate()
        {
            NetworkMessage nm = new NetworkMessage(ID);
            nm.MessageBody = new MessageBody() { MessageType = MessageTypes.GetDate, MessageSerial = JsonConvert.SerializeObject(DateTime.Now) };
            foreach (Guid ClientID in ConnectedClients.Keys)
            {
                Send(ClientID, nm);
            }
            Console.WriteLine("Sent Date to clients");
        }
        #endregion
        #endregion
    }
}