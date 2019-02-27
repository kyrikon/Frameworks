using Newtonsoft.Json;
using PLEXOS.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PLEXOS.SocketServer.Notifications
{
    public class BroadcastNotifications
    {

        #region Instance Variables / Events / Delegates

        #endregion
        #region Constructors
        public BroadcastNotifications(AsynchronousSocketListener l)
        {
            Listen = l;
            Cosmos = new CosmosConnector();
            Cosmos.ChangeRecieved += Cosmos_ChangeRecieved;
            Task.Run(()=>Cosmos.Connect());
            SQL = new SQLStuff.SQLNotify();
            SQL.ChangeRecieved += Cosmos_ChangeRecieved;
            Task.Run(()=>SQL.StartListening());
        }

       


        #endregion
        #region Commands   
        #endregion
        #region Properties
        public AsynchronousSocketListener Listen
        {
            get;set;
        }
        public CosmosConnector Cosmos
        {
            get;set;
        }
        public SQLStuff.SQLNotify SQL
        {
            get;set;
        }

        #endregion
        #region Methods    
        private void Cosmos_ChangeRecieved(object sender, ChangeRecievedEventArgs e)
        {
            foreach (Guid ID in Listen.ConnectedClients.Keys)
            {
                string SerialChange = JsonConvert.SerializeObject(e.Change);
                MessageBody mb = new MessageBody() { MessageType = MessageTypes.DBChange, MessageSerial = SerialChange };
                NetworkMessage Nm = new NetworkMessage(Listen.ID) { MessageBody = mb };
                Listen.Send(ID, Nm);
            }
        }
      
        #endregion
    }
}
