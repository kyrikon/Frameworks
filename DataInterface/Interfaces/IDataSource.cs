using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace DataInterface
{
    public delegate void DataInitializedEventHandler(object sender, DataInitializedEventEventArgs args);

    public interface IDataSource
    {

        #region Events / Delegates
        event DataInitializedEventHandler DataInitializedEvent;
        #endregion
        #region Fields 
        #endregion
        #region Properties       
        IConnection Connection { get;   }
        DataObjectHierarchy Hierarchy { get; }
        #endregion
        #region Methods             
        Tuple<bool, string> ValidateNewConnection();
       
        void CreateNewProject();
        Task<Tuple<bool, string>> SaveProject(object Objects);
        Task<IObservable<IHDataObject>> LoadProject();
        #endregion

    }
    public class DataInitializedEventEventArgs : EventArgs
    {
        public DataInitializedEventEventArgs()
        {
        }
    }


}
