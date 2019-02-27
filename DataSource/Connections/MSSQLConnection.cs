using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using DataInterface;

namespace DataSource
{
    public class MSSQLConnection : IConnection
    {

        #region Events / Delegates
        public event ConnectionChangedEventHandler ConnectionChangedEvent;
        #endregion
        #region Fields 

        private ConnectionState _ConnectionState;
        #endregion
        #region Constructors
        public MSSQLConnection()
        {
            ConnectionState = ConnectionState.Closed;
            SqlConnection = new SqlConnection();
            ConnectionBuilder = new SqlConnectionStringBuilder();
            SQLMessages = new List<string>();
            CT = new CancellationTokenSource();
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public string ConnectionName { get; set; }
        public string ConnectionString
        {
            get
            {
                return ConnectionBuilder.ConnectionString;
            }
            set
            {
                ConnectionBuilder.ConnectionString = value;
            }
        }
        public SqlConnection SqlConnection
        {
            get;private set;
        }
        public ConnectionState ConnectionState
        {
            get
            {
                return _ConnectionState;
            }
            private set
            {
                if(_ConnectionState != value)
                {
                    _ConnectionState = value;
                    OnConnectionChanged(new ConnectionChangedEventArgs(_ConnectionState));
                }
            }
        }
        public SqlConnectionStringBuilder ConnectionBuilder
        {
            get;private set;
        }
        public CancellationTokenSource CT
        {
            get;private set;
        }
        public List<string> SQLMessages
        {
            get;private set;
        }
       public DataSourceType DSType { get; set; }
        public SaveFormat SaveFormat { get; set; }
      
        #endregion
        #region Methods     
        public async void Connect()
        {
            SqlConnection.ConnectionString = ConnectionString;
            SqlConnection.InfoMessage -= SqlConnection_InfoMessage;
            SqlConnection.InfoMessage += SqlConnection_InfoMessage;
            SqlConnection.StateChange -= SqlConnection_StateChange;
            SqlConnection.StateChange += SqlConnection_StateChange;

            await SqlConnection.OpenAsync(CT.Token);
        }

       

        public void Disconnect()
        {
            SqlConnection.Close();
        }
        #endregion

        #region Event Callbacks
        protected void OnConnectionChanged(ConnectionChangedEventArgs Args)
        {
            ConnectionChangedEvent?.Invoke(this, Args);
        }

        private void SqlConnection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            SQLMessages.Add($"Source - {e.Source} : Error - {e.Message}");
        }
        private void SqlConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            ConnectionState = e.CurrentState;
        }
        #endregion




    }
}
