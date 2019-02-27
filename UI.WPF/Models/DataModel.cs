using Core.Helpers;
using DataInterface;
using DataSource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UI.WPF.Helpers;

namespace UI.WPF.Models
{
    public class DataModel : NotifyPropertyChanged
    {
        #region Events / Delegates
        public delegate void ModelInitializedEventHandler(object sender, EventArgs args);
        public event ModelInitializedEventHandler ModelInitialized;
        #endregion
        #region Fields 
        private List<KeyValuePair<HKey, HDynamicObject>> _Rslt;
        #endregion
        #region Constructors
        public DataModel(IDataSource DS)
        {
            Hierarchy = new DataObjectHierarchy();
            DataSource = DS;           
        }
        public DataModel(IDataSource DS,DataObjectHierarchy _Hierarchy)
        {
            Hierarchy = _Hierarchy;
            DataSource = DS;
            Objects = new UIHKeyDictionary();                
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
        public UIHKeyDictionary Objects
        {
            get
            {
                return GetPropertyValue<UIHKeyDictionary>();
            }
            private set
            {
                if(GetPropertyValue<UIHKeyDictionary>() != value)
                {
                    SetPropertyValue<UIHKeyDictionary>(value);                  
                }
            }
           
        }
    
        public UIHKeyDictionary Root
        {
            get; private set;
            
        }
        public KeyValuePair<HKey, HDynamicObject> CurrentItem
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
                if (GetPropertyValue<IDataSource>() != value)
                {
                    SetPropertyValue<IDataSource>(value);
                }
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
                Objects.TryAdd(key, new HDynamicObject());
            }
            CurrentItem = new KeyValuePair<HKey, HDynamicObject>(key,Objects[key]);
            return "Navigation Successful";
        }
       public void Initialize()
        {
            Objects.TreeChanged -= Objects_TreeChanged;
            Root = new UIHKeyDictionary();
           
            Objects = new UIHKeyDictionary();
            Objects.TreeChanged += Objects_TreeChanged;
            DataSource.Connection.ConnectionChangedEvent -= Connection_ConnectionChangedEvent;
            DataSource.Connection.ConnectionChangedEvent += Connection_ConnectionChangedEvent;
            DataSource.DataInitializedEvent -= DataSource_DataInitializedEvent;
            DataSource.DataInitializedEvent += DataSource_DataInitializedEvent;
            DataSource.Connection.Disconnect();
            DataSource.Connection.Connect();                       
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

        public void Clear()
        {
            if (Objects != null && Root != null)
            {
                Objects.Clear();
                Root.Clear();
            }
        }

        public void CreateNewProject(DataObjectHierarchy DOH)
        {
            Hierarchy = DOH;
            HKey RootKey = new HKey(new int[] { 1 });
            Hierarchy.FirstOrDefault(x => x.ID.Equals(RootKey)).Name = DataSource.Connection.ConnectionName;


            if (Objects != null)
            {
                Objects.TreeChanged -= Objects_TreeChanged;
            }
            Root = new UIHKeyDictionary();

            Objects = new UIHKeyDictionary();
            Objects.TreeChanged += Objects_TreeChanged;

            _Rslt = new List<KeyValuePair<HKey, HDynamicObject>>();            
            foreach (var Itm in Hierarchy.OrderBy(x => x.ID))
            {
                _Rslt.Add(new KeyValuePair<HKey,HDynamicObject>(Itm.ID,new HDynamicObject(Itm.ID) { Name = Itm.Name , IsContainer = true }));
            }
           
            DataSource.Connection.ConnectionChangedEvent -= Connection_ConnectionChangedEvent;
            DataSource.Connection.ConnectionChangedEvent += Connection_ConnectionChangedEvent;
            DataSource.DataInitializedEvent -= DataSource_DataInitializedEvent;
            DataSource.DataInitializedEvent += DataSource_DataInitializedEvent;
            DataSource.Connection.Disconnect();
            DataSource.Connection.Connect();
            DataSource.CreateNewProject();
        }
        public async Task<Tuple<bool, string>> Save()
        {
           return await DataSource.SaveProject(Objects);
        }

        public async Task LoadProject()
        {
            if (Objects != null)
            {
                Objects.TreeChanged -= Objects_TreeChanged;
            }
            Root = new UIHKeyDictionary();

            Objects = new UIHKeyDictionary();
            Objects.TreeChanged += Objects_TreeChanged;
        

            DataSource.Connection.ConnectionChangedEvent -= Connection_ConnectionChangedEvent;
            DataSource.Connection.ConnectionChangedEvent += Connection_ConnectionChangedEvent;
            DataSource.DataInitializedEvent -= DataSource_DataInitializedEvent;
            DataSource.DataInitializedEvent += DataSource_DataInitializedEvent;
            DataSource.Connection.Disconnect();
            DataSource.Connection.Connect();
            if (DataSource.Connection.ConnectionState == System.Data.ConnectionState.Open)
            {
                IObservable<HDynamicObject> getObjects = (IObservable<HDynamicObject>)await DataSource.LoadProject();
                _Rslt = new List<KeyValuePair<HKey, HDynamicObject>>();
                await getObjects.ForEachAsync((itm) => _Rslt.Add(new KeyValuePair<HKey, HDynamicObject>( itm.HID,itm)));
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
        private void Objects_TreeChanged(object sender, TreeChangedEventArgs<HKey, HDynamicObject> e)
        {
            switch(e.Action)
            {               
                case  CollectionAction.Add:
                    HKey NewKey = e.NewVal.Key;
                    if (!NewKey.IsRoot)
                    {
                        TreeListExpandedNodesHelper.RegisterBaseObject(e.NewVal.Value);

                        if (Objects.ContainsKey(NewKey.ParentKey))
                        {
                            HDynamicObject Parent = (HDynamicObject)Objects[NewKey.ParentKey];
                            HDynamicObject NewItem = (HDynamicObject)e.NewVal.Value;
                            NewItem.Parent = Parent;
                            NewItem.Root = (HDynamicObject)Objects[NewKey.RootKey];
                            //Logic to add in all children in case not added in order
                            //foreach(KeyValuePair<HKey, DataObject> Chldrn in Objects.Where(x => x.Key.Contains(NewKey) && !x.Key.Equals(NewKey)))
                            //{

                            //    Chldrn.Value.Parent = NewItem;
                            //    NewItem.Children.TryAdd(Chldrn.Key, Chldrn.Value);

                            //}
                            bool isok = Objects[NewKey.ParentKey].Children.TryAdd(NewKey, (HDynamicObject)e.NewVal.Value);
                        }
                    }
                    else
                    {
                        Root.TryAdd(e.NewVal.Key, e.NewVal.Value);
                        TreeListExpandedNodesHelper.RegisterBaseObject(e.NewVal.Value);
                    }
                    break;
                case CollectionAction.Remove:                   
                    HKey DelKey = e.RemVal.Key;
                    TreeListExpandedNodesHelper.RegisterBaseObject(e.RemVal.Value);
                    if (!DelKey.IsRoot)
                    {
                        HKey ParKey = DelKey.ParentKey;
                        if(Objects.ContainsKey(ParKey))
                        {
                            HDynamicObject OldItem;
                            Objects[ParKey].Children.TryRemove(DelKey, out OldItem);
                        }
                    }
                    break;
            }
        }
        private async void Connection_ConnectionChangedEvent(object sender, ConnectionChangedEventArgs args)
        {
            if (args.ConnectionState == System.Data.ConnectionState.Open)
            {
                //IObservable<KeyValuePair<HKey, HDataObject>> getEnum = DataSource.GetObjects();
                //_Rslt = new List<KeyValuePair<HKey, HDataObject>>();
                //await getEnum.ForEachAsync((itm) => _Rslt.Add(itm));
            }          
        }
        private async void DataSource_DataInitializedEvent(object sender, DataInitializedEventEventArgs args)
        {
            await Task.Run(() =>
                {
                    Objects.AddList(_Rslt.OrderBy(x => x.Key));
                    Objects.EndEdit(_Rslt);
                });
            SetCurrenItem(new HKey(new int[] { 1 }));
            OnModelInitialized(new EventArgs());
        }
        #endregion
    }
}
