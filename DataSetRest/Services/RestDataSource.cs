using DataInterface;
using DataSource;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataSetRest.Services
{
    public class RestDataSource : IDataSource
    {
        #region Events
        public event DataInitializedEventHandler DataInitializedEvent;
        #endregion
        #region Fields
        private IDataSource _DataSource;
        #endregion
        #region Constructor
        public RestDataSource()
        {
            _DataSource = DataConnectionFactory.CreateNewDataSource(DataConnectionFactory.CreateNewConnection(DataInterface.DataSourceType.XMLDataset));
            _DataSource.Connection.Connect();
        }

        #endregion
        #region Properties
        public IConnection Connection
        {
            get
            {
                return _DataSource.Connection;
            }
        }
        public DataObjectHierarchy Hierarchy
        {
            get; private set;
        }
        #endregion
        #region Methods
        public IObservable<KeyValuePair<HKey, HDataObject>> GetObjects()
        {
            return _DataSource.GetObjects();
        } 
        #endregion
    }
}
