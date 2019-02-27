using PLEXOS.Core.Helpers;
using PLEXOS.DataInterface;
using PLEXOS.DataSource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PLEXOS.DataModel
{
    public class DataModel : NotifyPropertyChanged
    {
        #region Events / Delegates
        public delegate void ModelInitializedEventHandler(object sender, EventArgs args);
        public event ModelInitializedEventHandler ModelInitialized;
        #endregion
        #region Fields 
        private List<KeyValuePair<HKey, DataObject>> _Rslt;
        #endregion
        #region Constructors
        public DataModel()
        {
            Hierarchy = new DataObjectHierarchy();
            Root = new ObservableConcurrentDictionary<HKey, DataObject>();
            //DataSource = new MSSQLDataSource();
            Objects = new ObservableConcurrentDictionary<HKey, DataObject>();          
            Objects.TreeChanged += Objects_TreeChanged;
           
        }
        public DataModel(DataObjectHierarchy _Hierarchy)
        {
            Hierarchy = _Hierarchy;
            Objects = new ObservableConcurrentDictionary<HKey, DataObject>();        
        
         //   Objects.TreeChanged += Objects_TreeChanged;
        }

        #endregion
        #region Commands   
        #endregion
        #region Properties
        public DataObjectHierarchy Hierarchy
        {
            get
            {
                return GetPropertyValue<DataObjectHierarchy>();
            }
            private set
            {
                if(GetPropertyValue<DataObjectHierarchy>() != value)
                {
                    SetPropertyValue<DataObjectHierarchy>(value);
                }
            }

        }
        public ObservableConcurrentDictionary<HKey, DataObject> Objects
        {
            get
            {
                return GetPropertyValue<ObservableConcurrentDictionary<HKey, DataObject>>();
            }
            private set
            {
                if(GetPropertyValue<ObservableConcurrentDictionary<HKey, DataObject>>() != value)
                {
                    SetPropertyValue<ObservableConcurrentDictionary<HKey, DataObject>>(value);
                }
            }
           
        }

        public ObservableConcurrentDictionary<HKey, DataObject> Root
        {
            get; private set;
            
        }
        public KeyValuePair<HKey, DataObject> CurrentItem
        {
            get; private set;
        }

        public IDataSource DataSource
        {
            get
            {
                return GetPropertyValue<IDataSource>();
            }
             set
            {
                SetPropertyValue<IDataSource>(value);
            }

        }
        #endregion
        #region Methods    

        public string SetCurrenItem(HKey key)
        {
            if(!Objects.ContainsKey(key))
            {
                if(!Objects.ContainsKey(key.ParentKey))
                {
                    return "Navigation Failed, Parent doesnt exist";
                }
                Objects.TryAdd(key, new DataObject());
            }
            CurrentItem = new KeyValuePair<HKey, DataObject>(key,Objects[key]);
            return "Navigation Successful";
        }
       public void Initialize()
        {
            Objects.TreeChanged -= Objects_TreeChanged;
            Root = new ObservableConcurrentDictionary<HKey, DataObject>();
            Objects = new ObservableConcurrentDictionary<HKey, DataObject>();
            Objects.TreeChanged += Objects_TreeChanged;
            DataSource.Connection.ConnectionChangedEvent += Connection_ConnectionChangedEvent;
            DataSource.DataInitializedEvent += DataSource_DataInitializedEvent;
            DataSource.Connection.Disconnect();
            DataSource.Connection.Connect();           
            Hierarchy = DataSource.GetDataObjectHierarchy();   
            
        }

        public string GetHierarchyNames(HKey key)
        {
            if(key.IsRoot)
            {
                return $"{Objects[key].GetValue<string>("Name")}({string.Join(",", (int[])key)})";
            }
            else
            {
                List<string> HNames = new List<string>();
                HKey CurrKey = key;
                while(!CurrKey.Equals(key.RootKey))
                {                    
                    HNames.Add(Objects[CurrKey].GetValue<string>("Name"));
                    CurrKey = CurrKey.ParentKey;
                }
                HNames.Add(Objects[CurrKey].GetValue<string>("Name"));
                HNames.Reverse();
                return $"{string.Join(",", HNames)}({string.Join(",", (int[])key)})";
            }
           
        }

        // to be implemented
        // Get from storage
        // Write back to storage
        // start transactions
        // manage Memberships
        // manage properties

        // build graph
        // manage changes in storage (compare server timestamp for update and fix conflicts)
        // queue of storage key changes feed to update objects

        #endregion
        #region Callbacks
        protected void OnModelInitialized(EventArgs Args)
        {
            ModelInitialized?.Invoke(this, Args);          
        }
        private void Objects_TreeChanged(object sender, TreeChangedEventArgs<HKey, DataObject> e)
        {
            switch(e.Action)
            {               
                case  CollectionAction.Add:
                    HKey NewKey = e.NewVal.Key;
                if(!NewKey.IsRoot)
                {

                        if(string.IsNullOrEmpty(e.NewVal.Value.GetValue<string>("Name")))
                        {

                        }
                    if(Objects.ContainsKey(NewKey.ParentKey))
                    {
                        DataObject Parent = Objects[NewKey.ParentKey];
                        DataObject NewItem = e.NewVal.Value;
                        NewItem.Parent = Parent;
                        NewItem.Root = Objects[NewKey.RootKey];
                            //Logic to add in all children in case not added in order
                            //foreach(KeyValuePair<HKey, DataObject> Chldrn in Objects.Where(x => x.Key.Contains(NewKey) && !x.Key.Equals(NewKey)))
                            //{
                                
                            //    Chldrn.Value.Parent = NewItem;
                            //    NewItem.Children.TryAdd(Chldrn.Key, Chldrn.Value);

                            //}
                            bool isok = Objects[NewKey.ParentKey].Children.TryAdd(NewKey, e.NewVal.Value);
                    }
                }
                else
                {
                        Root.TryAdd(e.NewVal.Key,e.NewVal.Value);
                }
                    break;
                case CollectionAction.Remove:                   
                    HKey DelKey = e.RemVal.Key;
                    if(!DelKey.IsRoot)
                    {
                        HKey ParKey = DelKey.ParentKey;
                        if(Objects.ContainsKey(ParKey))
                        {
                            DataObject OldItem;
                            Objects[ParKey].Children.TryRemove(DelKey, out OldItem);
                        }
                    }
                    break;
            }
        }
        private async void Connection_ConnectionChangedEvent(object sender, ConnectionChangedEventArgs args)
        {
            if(args.ConnectionState == System.Data.ConnectionState.Open)
            {
         
                IObservable<KeyValuePair<HKey, DataObject>> getEnum = DataSource.GetObjects();
              _Rslt = new List<KeyValuePair<HKey, DataObject>>() ;


                await getEnum.ForEachAsync((itm) => _Rslt.Add(itm));
                //getEnum.Subscribe(
                //    (Row) => { Rslt.Add(Row); },
                //    (Row) => { Console.WriteLine("Err"); },
                //   async (Row) => await Task.Run(() =>
                //    {
                //        Objects.AddList(Rslt);
                //        Objects.EndEdit(Rslt);
                //    }));
            }
           

        }
        private async void DataSource_DataInitializedEvent(object sender, DataInitializedEventEventArgs args)
        {
            await Task.Run(() =>
                {
                    Objects.AddList(_Rslt.OrderBy(x => x.Key));
                    Objects.EndEdit(_Rslt);
                });

            Console.WriteLine("Complete");
            SetCurrenItem(new HKey(new int[] { 1 }));
            OnModelInitialized(new EventArgs());
        }
        #endregion
    }
}
