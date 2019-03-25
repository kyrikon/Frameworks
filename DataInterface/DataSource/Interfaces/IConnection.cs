using Core.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace DataInterface
{
    public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs args);
    public interface IConnection
    {
        #region Events / Delegates

        event ConnectionChangedEventHandler ConnectionChangedEvent;
        #endregion
        #region Fields 
        #endregion
        #region Properties       
        string ConnectionString { get; set; }
        string ConnectionName { get; set; }
        ConnectionState ConnectionState { get;  }
        SaveFormat SaveFormat { get; set; }
        DataSourceType DSType { get; set; }
        #endregion
        #region Methods             
        void Connect();
        void Disconnect();


        #endregion

    }

    public class ConnectionChangedEventArgs : EventArgs
    {
        public ConnectionState ConnectionState;
        public ConnectionChangedEventArgs(ConnectionState CS)
        {
            ConnectionState = CS;
        }
    }
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SaveFormat
    {
        [Description("JSON Format")]
        Json,
        [Description("BSON Format")]
        Bson,
        [Description("Binary Format")]
        Bin
    }
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataSourceType
    {
        [Description("Local File")]
        LocalFile,
        [Description("MS SQL Server")]
        MSSQL,
        [Description("XML Dataset")]
        XMLDataset,
        [Description("REST Service")]
        RestService
    }
}

