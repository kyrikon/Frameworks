using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DataInterface;

namespace DataSource
{
    public class LocalFileConnection : IConnection
    {

        #region Events / Delegates
        public event ConnectionChangedEventHandler ConnectionChangedEvent;
        #endregion
        #region Fields 

        private ConnectionState _ConnectionState;
        #endregion
        #region Constructors
        public LocalFileConnection()
        {
            ConnectionState = ConnectionState.Closed;
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
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
        public string ConnectionName { get; set; }
        public DataSourceType DSType { get; set; }
        public SaveFormat SaveFormat { get; set; }
        #endregion
        #region Methods     
        public void Connect()
        {
            ConnectionState = ConnectionState.Open;
        }

        public void Disconnect()
        {
            ConnectionState = ConnectionState.Closed;
        }
        #endregion
        protected void OnConnectionChanged(ConnectionChangedEventArgs Args)
        {
            ConnectionChangedEvent?.Invoke(this, Args);
        }
    }
}
