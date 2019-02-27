using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataInterface;
using System.Collections.Concurrent;
using System.Reactive.Linq;

namespace DataSource
{
    public class MSSQLDataSource : IDataSource
    {


        #region Events / Delegates    
        public event DataInitializedEventHandler DataInitializedEvent;
        #endregion
        #region Fields 

        #endregion
        #region Constructors
        public MSSQLDataSource(IConnection _Connection)
        {
            Connection = _Connection;
            Hierarchy = new DataObjectHierarchy();
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public IConnection Connection { get; private set; }
        public MSSQLConnection MSSQLConnection
        {
            get
            {
                return (MSSQLConnection)Connection;
            }
        }

        public bool IsSelected
        {
            get;set;
        }
        #endregion
        #region Methods   


        public DataObjectHierarchy Hierarchy
        {
           get;private set;
        }
        public IObservable<KeyValuePair<HKey, HDataObject>> GetObjects()
        {
            return Observable.Create<KeyValuePair<HKey, HDataObject>>(
         async obs =>
         {

             obs.OnNext(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1 }), new HDataObject() { IsReadOnly = true, ["Name"] = "SQL" }));
             foreach(var GetItem in await GetObjectsAsync())
             {
                 obs.OnNext(GetItem);
             }
             obs.OnCompleted();
             OnDataInitialized(new DataInitializedEventEventArgs());
         });

        }
        private async Task<ConcurrentBag<KeyValuePair<HKey, HDataObject>>> GetObjectsAsync()
        {
            ConcurrentBag<KeyValuePair<HKey, HDataObject>> RetList = new ConcurrentBag<KeyValuePair<HKey, HDataObject>>();
            if(MSSQLConnection.ConnectionState == ConnectionState.Open)
            {
                SqlCommand cmd = MSSQLConnection.SqlConnection.CreateCommand();
                cmd.CommandText = "Select * from [t_class_group] where lang_id > 0";
                SqlDataReader DR = await cmd.ExecuteReaderAsync();
                while(await DR.ReadAsync())
                {
                    RetList.Add(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1, (int)DR["class_group_id"] }), new HDataObject() { IsReadOnly = false, ["Name"] = (string)DR["name"] }));
                }
                DR.Close();
                cmd.CommandText = "Select * from [t_class] where lang_id > 1";
                DR = await cmd.ExecuteReaderAsync();
                while(await DR.ReadAsync())
                {
                    RetList.Add(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1, (int)DR["class_group_id"], (int)DR["class_id"] }), new HDataObject() { IsReadOnly = false, ["Name"] = (string)DR["name"], ["Description"] = (string)DR["description"] }));
                }
                DR.Close();
                cmd.CommandText = @"SELECT 
                                        [object_id] as objId
                                        ,t_object.[class_id]
                                        ,t_object.[name]
                                        ,isnull(t_object.[description],'') as description
	                                    ,t_class.class_group_id
                                  FROM [t_object]
                                  inner join [t_class]
                                    on t_class.class_id = t_object.class_id
                                  where t_class.lang_id > 1";

                DR = await cmd.ExecuteReaderAsync();
                while(await DR.ReadAsync())
                {
                    RetList.Add(new KeyValuePair<HKey, HDataObject>(new HKey(new int[] { 1, (int)DR["class_group_id"], (int)DR["class_id"], (int)DR["objId"] }), new HDataObject() { IsReadOnly = false, ["Name"] = (string)DR["name"], ["Description"] = (string)DR["description"] }));
                }
            }
            return RetList;
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
