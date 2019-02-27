using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Network
{
    public class NetworkMessage
    {
        #region Instance Variables / Events / Delegates
        public static string EndOfMessage = "<E^O$M>";
        #endregion
        #region Constructors
        public NetworkMessage(Guid Sender)
        {
            ID = Guid.NewGuid();
            SenderID = Sender;
        }

        #endregion

        #region Properties
        public Guid ID { get; private set; }
        public Guid? ResponseTo { get; set; }


        public Guid SenderID { get; set; }
        public List<Guid> RecipientIDs { get; set; }

        public MessageBody MessageBody
        {
            get; set;
        }

        #endregion
        #region Methods     
        #endregion
        #region StaticMethods
        public static Tuple<List<NetworkMessage>, string> DecodeMessageBuffer(string StrBuffer)
        {
            string MsgRes = StrBuffer;
            List<NetworkMessage> MgsList = new List<NetworkMessage>();

            while (MsgRes.Contains(NetworkMessage.EndOfMessage))
            {
                string SerialMsg = MsgRes.Substring(0, MsgRes.IndexOf(NetworkMessage.EndOfMessage));
                NetworkMessage mn = JsonConvert.DeserializeObject<NetworkMessage>(SerialMsg);
                MgsList.Add(mn);
                MsgRes = MsgRes.Substring(SerialMsg.Length + NetworkMessage.EndOfMessage.Length);
            }
            Tuple<List<NetworkMessage>, string> result = new Tuple<List<NetworkMessage>, string>(MgsList, MsgRes);
            return result;
        }
        #endregion
    }
    public class MessageBody
    {
        public MessageTypes MessageType { get; set; }
        public string MessageSerial { get; set; }
    }
    public enum MessageTypes
    {
        ConnectMsg,
        StringMsg,
        IntMsg,
        GetClients,
        GetDate,
        DBChange
    }
    public class ConnectionMessage
    {
        public Guid ServerID
        {
            get; set;
        }
        public Guid ClientID
        {
            get; set;
        }
    }
    public class ConnectedClientsInfo : Helpers.NotifyPropertyChanged
    {
        public string ClientName
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }

        public bool IsChecked
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue<bool>(value);
            }
        }
    }
    public class DBChangeData
    {
        public string DataSource
        {
            get;
            set;
        }
        public string DataType
        {
            get;
            set;
        }
        public string DataKey
        {
            get;
            set;
        }


    }
}
