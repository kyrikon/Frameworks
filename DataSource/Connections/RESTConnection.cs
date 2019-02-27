using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using DataInterface;
using RestSharp;

namespace DataSource
{
    public class RESTConnection : IConnection
    {

        #region Events / Delegates
        public event ConnectionChangedEventHandler ConnectionChangedEvent;
        #endregion
        #region Fields 

        private ConnectionState _ConnectionState;
        #endregion
        #region Constructors
        public RESTConnection()
        {
            ConnectionState = ConnectionState.Closed;           
            CT = new CancellationTokenSource();          
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public string ConnectionName { get; set; }
        public string ConnectionString { get; set; }

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
        public CancellationTokenSource CT
        {
            get;private set;
        }


        public DataSourceType DSType { get; set; }
        public SaveFormat SaveFormat { get; set; }
        public RestClient RClient
        {
            get;private set;
        }
        #endregion
        #region Methods     
        public void Connect()
        {
            RClient = new RestClient(ConnectionString);
            ConnectionState = ConnectionState.Open;
        }

        public void Disconnect()
        {
            ConnectionState = ConnectionState.Closed;
        }
        #endregion

        #region Event Callbacks
        protected void OnConnectionChanged(ConnectionChangedEventArgs Args)
        {
            ConnectionChangedEvent?.Invoke(this, Args);
        }

        #endregion




    }
}
