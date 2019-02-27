using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataInterface;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Linq;
using RestSharp;

namespace DataSource
{
    public class RESTDataSource : IDataSource
    {


        #region Events / Delegates    
        public event DataInitializedEventHandler DataInitializedEvent;
        #endregion
        #region Fields 

        #endregion
        #region Constructors
        public RESTDataSource(IConnection _Connection)
        {
            Connection = _Connection;
            Hierarchy = new DataObjectHierarchy();
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public IConnection Connection { get; private set; }
        public RESTConnection RESTConnection
        {
            get
            {
                return (RESTConnection)Connection;
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
           // this.RESTDatasetConnection.RClient
            return Observable.Create<KeyValuePair<HKey, HDataObject>>(
         async obs =>
         {
             RestRequest request = new RestRequest("api/values",Method.GET);
             request.RequestFormat = DataFormat.Json;
    
          //   byte[] payload = this.RESTConnection.RClient.GetAsync(request);
             IRestResponse response = this.RESTConnection.RClient.Execute(request);
             string Resp = response.Content;
             KeyValuePair<int[], HDataObject>[] RsltSet = Core.Extensions.Serialization.FromJson<KeyValuePair<int[], HDataObject>[]>(Resp);
             foreach (KeyValuePair<int[], HDataObject> item in RsltSet)
             {
                 obs.OnNext(new KeyValuePair<HKey, HDataObject>((HKey)item.Key, item.Value));
             }
             obs.OnCompleted();
             OnDataInitialized(new DataInitializedEventEventArgs());
         });

        }
       
        protected void OnDataInitialized(DataInitializedEventEventArgs Args)
        {
            DataInitializedEvent?.Invoke(this, Args);
        }

        public Tuple<bool, string> ValidateNewConnection()
        {
            throw new NotImplementedException();
        }

        public void CreateNewProject()
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<bool, string>> SaveProject(ObservableConcurrentDictionary<HKey, HDataObject> Objects)
        {
            throw new NotImplementedException();
        }

        public Task<IObservable<HDataObject>> LoadProject()
        {
            throw new NotImplementedException();
        }



        #endregion







    }
}
