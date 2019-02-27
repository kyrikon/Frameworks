using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataInterface;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Linq;

namespace DataSource
{
    public class XMLDatasetDataSource : IDataSource
    {


        #region Events / Delegates    
        public event DataInitializedEventHandler DataInitializedEvent;
        #endregion
        #region Fields 

        #endregion
        #region Constructors
        public XMLDatasetDataSource(IConnection _Connection)
        {
            Connection = _Connection;
            Hierarchy = new DataObjectHierarchy();
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public IConnection Connection { get; private set; }
        public XMLDatasetConnection XMLDatasetConnection
        {
            get
            {
                return (XMLDatasetConnection)Connection;
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
            return Observable.Create<KeyValuePair<HKey, HDataObject>>(
         async obs =>
         {

             obs.OnNext(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1 }), new HDataObject() { IsReadOnly = true, ["Name"] = "XML Dataset" }));
             foreach(DataRow dr in XMLDatasetConnection.DataSet.Tables["t_class_group"].AsEnumerable().Where(x => int.Parse(x["class_group_id"].ToString()) > 1).OrderBy(x => int.Parse(x["class_group_id"].ToString())))
             {
                 obs.OnNext(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1, int.Parse(dr["class_group_id"].ToString()) }), new HDataObject() { ["Name"] = (string)dr["name"] }));
             }

             foreach(DataRow dr in XMLDatasetConnection.DataSet.Tables["t_class"].AsEnumerable().Where(x => int.Parse(x["class_id"].ToString()) > 1).OrderBy(x => int.Parse(x["class_id"].ToString())))
             {
                 obs.OnNext(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1, int.Parse(dr["class_group_id"].ToString()), int.Parse(dr["class_id"].ToString()) })
                     , new HDataObject() { ["Name"] = (string)dr["name"], ["Description"] = (string)dr["description"] }));
             }
            
             var getObj =
                 from o in XMLDatasetConnection.DataSet.Tables["t_class"].AsEnumerable()
                 join p in XMLDatasetConnection.DataSet.Tables["t_object"].AsEnumerable()
                 on int.Parse(o["class_id"].ToString()) equals int.Parse(p["class_id"].ToString())
                 where int.Parse(o["class_id"].ToString()) > 1
                 orderby int.Parse(p["object_id"].ToString())
                 select new { ClassGroup = int.Parse(o["class_group_id"].ToString())
                            ,ClassId = int.Parse(o["class_id"].ToString())
                            , ObjId = int.Parse(p["object_id"].ToString())
                            ,ObjName = p["name"] == DBNull.Value ? string.Empty : (string)p["name"], ObjDescript = p["description"] == DBNull.Value ? string.Empty : (string)p["description"]
                 };

             foreach(var Itm in getObj)
             {
                 obs.OnNext(new KeyValuePair<HKey, HDataObject>(
                         new HKey(new int[] { 1, Itm.ClassGroup, Itm.ClassId, Itm.ObjId })
                     , new HDataObject() { ["Name"] = Itm.ObjName, ["Description"] = Itm.ObjDescript }));
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
