using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Core.Network
{
    public class PLEXOSClient
    {        
        #region Instance Variables
        private bool _SendQueueRun = false;
        private bool _ReceiveQueueRun = false;
        private  ManualResetEvent connectDone = new ManualResetEvent(false);
        private  ManualResetEvent sendDone = new ManualResetEvent(false);
        private  ManualResetEvent receiveDone =  new ManualResetEvent(false);
        private const int QueueTimeout = 100; //Timeout in miliseconds
        private string TmpBuff = string.Empty;
        private const int BufferSize = 1024 * 32;

        #endregion
        #region Constructors
        public PLEXOSClient(string EndPointStr, int port,string CliName)
        {
            ClientName = CliName;
            EndPointName = EndPointStr;
            //try parsing IP
            IPAddress SetConnection;
           
            if (!IPAddress.TryParse(EndPointStr, out SetConnection))
            {
                //get DNS
                IPHostEntry ipHostInfo = Dns.GetHostEntry(EndPointStr);
                SetConnection = ipHostInfo.AddressList[0]; // have to iterate through the interfaces to get the correct IP
            }
            EndPointIP = SetConnection;
            try
            { 
                EndPoint = new IPEndPoint(EndPointIP, port);
                Client = new Socket(EndPointIP.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
                SendQueue = new Queue<NetworkMessage>();               
                ReceiveQueue = new Queue<NetworkMessage>();
                SendHistory = new List<NetworkMessage>();
                ReceiveHistory = new List<NetworkMessage>();
                
            
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Invalid Connection for server - {EndPointStr} port - {port}\n{ex.Message}");
            }
        }
        #endregion       
        #region Properties

        #region Client Info
        public Guid ID
        {
            get; internal set;
        }
        public Guid ServerID
        {
            get; internal set;
        }
        public String ClientName
        {
            get; private set;
        }
        public string EndPointStr
        {
            get
            {
                return $"{EndPointName} : {EndPoint.Port}";
            }
           
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

        public Socket Client
        {
            get;
            internal set;
        }
        #endregion

        #region Queues
        public Queue<NetworkMessage> SendQueue
        {
            get;internal set;
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

        #region Connect
        public void Connect()
        {
            //make socket connection
            Client.BeginConnect(EndPoint, new AsyncCallback(ConnectCallback), Client);
            connectDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Complete the connection.  
                Client.EndConnect(ar);
                //Once Connection is established start the Send and receive message pumps 
                //This manages the requests

                Console.WriteLine("Socket connected to {0}",
                    Client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
                Task.Run(() => StartSendQueuePump());
                Task.Run(() => StartReceiveQueuePump());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }       
        #endregion
        #region Send
        private void Send(NetworkMessage data)
        {
            // Convert the Netowrk message + End of message token to byte data using ASCII encoding.  
            string SerialMsg = JsonConvert.SerializeObject(data);
            byte[] byteData = Encoding.ASCII.GetBytes($"{SerialMsg}{NetworkMessage.EndOfMessage}");

            // Begin sending the data to the remote device.  
            Client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), new object());
        }
        private  void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Complete sending the data to the remote device.  
                int bytesSent = Client.EndSend(ar);
               
                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion
        #region Receive
        private void Receive()
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
             
                // Begin receiving the data from the remote device.  
                Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private  void ReceiveCallback(IAsyncResult ar)
        {
            
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
              

                // Read data from the remote device.  
                int bytesRead = Client.EndReceive(ar);

                // the socket connection will always receive byte chunks until
                // the connection is closed
                // When a NetworkMessage.EndOfMessage is detected in the current message
                //(which may take several receive buffers), the message has been fully received
                // It can then be deserialised and handles. The receive will continue to run

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    //Process the buffer string

                    if (Client.Available > 0)
                    {
                        // Get the rest of the data. if available 
                        Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }                    
                }
                //if (!state.sb.ToString().Contains(NetworkMessage.EndOfMessage))
                //{
                //    TmpBuff += state.sb.ToString();
                //}
                //if (!string.IsNullOrEmpty(TmpBuff))
                //{
                //    state.sb.Insert(0, TmpBuff);
                //    TmpBuff = string.Empty;
                //}


                HandleReadBuff(state);
                receiveDone.Set();
                Receive();
                //StartReceiveQueuePump();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
        #region CloseConnection
        public void CloseConnection()
        {
            Client.Shutdown(SocketShutdown.Both);
            Client.Close();
        }
        #endregion

        #region Queues
        public void StartSendQueuePump()
        {
            _SendQueueRun = true;
            while (_SendQueueRun)
            {
                if (SendQueue.Count > 0)
                {
                    NetworkMessage SendMsg = SendQueue.Dequeue();                    
                    Send(SendMsg);
                    sendDone.WaitOne();
                }
                Thread.Sleep(QueueTimeout);
            }
        }
        public void StopSendQueuePump()
        {
            _SendQueueRun = false;
        }
        public void StartReceiveQueuePump()
        {
          //  _ReceiveQueueRun = true;
            //while (_ReceiveQueueRun)
            //{
            //   Task.Run(() => Receive());
            //    receiveDone.WaitOne();
            //    Thread.Sleep(QueueTimeout);
            //}
             Receive();           
          //  receiveDone.WaitOne();
        //    Thread.Sleep(QueueTimeout);

        }
        public void StopReceiveQueuePump()
        {
            _ReceiveQueueRun = false;
        } 
        #endregion
        public void HandleMsg(NetworkMessage mn)
        {
            if(mn.MessageBody.MessageType == MessageTypes.ConnectMsg)
            {
                ConnectionMessage Cm = JsonConvert.DeserializeObject<ConnectionMessage>(mn.MessageBody.MessageSerial);
                ID = Cm.ClientID;
                ServerID = Cm.ServerID;

                NetworkMessage SendClientName = new NetworkMessage(ID);
                SendClientName.MessageBody = new MessageBody() { MessageType = MessageTypes.ConnectMsg, MessageSerial = ClientName };
                SendQueue.Enqueue(SendClientName);
            }
        }
        #endregion
    }

}
