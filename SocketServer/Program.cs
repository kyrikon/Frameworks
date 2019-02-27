using Newtonsoft.Json;
using PLEXOS.Core.Network;
using PLEXOS.SocketServer.Notifications;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PLEXOS.SocketServer
{
    class Program
    {

       static string ConsoleStr = string.Empty;
        static void Main(string[] args)
        {
            AsynchronousSocketListener Listen = new AsynchronousSocketListener();
            Task.Run(() => Listen.StartListening(11000));
            //Task.Run(() => TimedEvent(Listen));
            while (true)
            {
                ConsoleStr = Console.ReadLine();
                ParseCommand(ConsoleStr, Listen);
            }
        }



        private static void ParseCommand(string Cmd, AsynchronousSocketListener Listen)
        {
            switch (Cmd)
            {
                case "q":
                    Environment.Exit(0);
                    //should cleanup resources
                    break;
                case "c":
                    PrintClients(Listen);
                    break;
                case "s":
                    PrintServer(Listen);
                    break;
                case "t":
                    SendTest(Listen);
                    break;
                case "rc":
                    SendClients(Listen);
                    break;
                case "h":
                    PrintHelp();
                    break;
                case "t1":
                    TestMsgParse();
                    break;
                    case "g":
                  BroadcastChanges(Listen);
                    break;
                case "rt":
                    TimedEvent(Listen);
                    break;
            }
        }


        private static void PrintHelp()
        {
            Console.WriteLine("h  - This menu");
            Console.WriteLine("c  - Show Clients");
            Console.WriteLine("rc - Send Connected Client");
            Console.WriteLine("s  - Show Server");
            Console.WriteLine("t  - Send test string");
            Console.WriteLine("rt - Restart Timer");
            Console.WriteLine("g  - GraphDB");
            Console.WriteLine("q  - Quit");
        }
        private static void PrintClients(AsynchronousSocketListener Listen)
        {
            foreach (Guid ID in Listen.ConnectedClients.Keys)
            {
                Console.WriteLine($"ID - {ID} : Name - {Listen.GetClientName(ID)} ");
            }
        }
        private static void SendClients(AsynchronousSocketListener Listen)
        {
            Listen.NotifyConnections();
        }

        private static void PrintServer(AsynchronousSocketListener Listen)
        {
            Console.WriteLine($"ID - {Listen.ID}");
            Console.WriteLine($"HostName - {Listen.EndPointName}");
            Console.WriteLine($"IP EndPoint - {Listen.EndPoint.Address} : {Listen.EndPoint.Port}");
        }
        private static void SendTest(AsynchronousSocketListener Listen)
        {
            foreach (Guid ID in Listen.ConnectedClients.Keys)
            {

                MessageBody mb = new MessageBody() { MessageType = MessageTypes.StringMsg, MessageSerial = "Test" };
                NetworkMessage Nm = new NetworkMessage(Listen.ID) { MessageBody = mb };
                Listen.Send(ID, Nm);
            }

        }
        private static void TestMsgParse()
        {

            NetworkMessage NM = new NetworkMessage(Guid.NewGuid());
            string testStr = JsonConvert.SerializeObject(NM);
            Tuple<List<NetworkMessage>, string> result = NetworkMessage.DecodeMessageBuffer(testStr.Substring(0, 5));
            result = NetworkMessage.DecodeMessageBuffer(result.Item2 + testStr.Substring(5, 5));
            result = NetworkMessage.DecodeMessageBuffer(result.Item2 + testStr.Substring(10) + NetworkMessage.EndOfMessage);
        }

        private static void TimedEvent(AsynchronousSocketListener Listen)
        {
            Timer ticker = new Timer(TimerMeth, Listen, 5000,10000);
        }

        private static void TimerMeth(object state)
        {
          ((AsynchronousSocketListener)state).SendDate();              
        }

        private static void BroadcastChanges(AsynchronousSocketListener Listen)
        {
            BroadcastNotifications bn = new BroadcastNotifications(Listen);
        }

      
    }
}
