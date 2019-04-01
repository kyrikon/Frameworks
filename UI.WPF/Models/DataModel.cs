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
using UI.WPF.Singletons;

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
            Hierarchy = new DynamicObjectHierarchy();
            DataSource = DS;
            KeyValueEnums = new ObservableConcurrentDictionary<string, KeyValueEnum<object>>();
        }
        public DataModel(IDataSource DS, DynamicObjectHierarchy _Hierarchy)
        {
            Hierarchy = _Hierarchy;
            DataSource = DS;
            Objects = new UIHKeyDictionary();
            KeyValueEnums = new ObservableConcurrentDictionary<string, KeyValueEnum<object>>();
        }

        #endregion
        #region Commands   
        #endregion
        #region Properties
        public DynamicObjectHierarchy Hierarchy
        {
            get
            {
                return GetPropertyValue<DynamicObjectHierarchy>();
            }
            private set
            {
                if (GetPropertyValue<DynamicObjectHierarchy>() != value)
                {
                    SetPropertyValue<DynamicObjectHierarchy>(value);
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
                if (GetPropertyValue<UIHKeyDictionary>() != value)
                {
                    SetPropertyValue<UIHKeyDictionary>(value);
                }
            }

        }
        
        public ObservableCollection<HDynamicObject> Root
        {
            get
            {
                return GetPropertyValue<ObservableCollection<HDynamicObject>>();
            }
            private set
            {
                if (GetPropertyValue<ObservableCollection<HDynamicObject>>() != value)
                {
                    SetPropertyValue<ObservableCollection<HDynamicObject>>(value);
                }
            }
        }
        public HDynamicObject CurrentItem
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

        public ObservableConcurrentDictionary<string, KeyValueEnum<object>> KeyValueEnums
        {
            get
            {
                return GetPropertyValue<ObservableConcurrentDictionary<string, KeyValueEnum<object>>>();
            }
            private set
            {
                if (GetPropertyValue<ObservableConcurrentDictionary<string, KeyValueEnum<object>>>() != value)
                {
                    SetPropertyValue<ObservableConcurrentDictionary<string, KeyValueEnum<object>>>(value);
                }
            }
        }
        #endregion
        #region Methods    

        public string SetCurrenItem(HKey key)
        {
            if (!Objects.ContainsKey(key))
            {
                if (!Objects.ContainsKey(key.ParentKey))
                {
                    return "Navigation Failed, Parent doesnt exist";
                }
                Objects.TryAdd(key, new HDynamicObject());
            }
            CurrentItem = Objects[key];
            return "Navigation Successful";
        }
        public void Initialize()
        {
            Objects.TreeChanged -= Objects_TreeChanged;
            Root = new ObservableCollection<HDynamicObject>();

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
            if (key.IsRoot)
            {
                return $"{Objects[key].GetValue<string>("Name")}({string.Join(",", (int[])key)})";
            }
            else
            {
                List<string> HNames = new List<string>();
                HKey CurrKey = key;
                while (!CurrKey.Equals(key.RootKey))
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

        public void CreateNewProject(DynamicObjectHierarchy DOH)
        {
            Hierarchy = DOH;
            Hierarchy.FirstOrDefault(x => x.ID.Equals(HKey.RootKeyVal)).Name = DataSource.Connection.ConnectionName;

            if (Objects != null)
            {
                Objects.TreeChanged -= Objects_TreeChanged;
            }
            Root = new ObservableCollection<HDynamicObject>();

            Objects = new UIHKeyDictionary();
            Objects.TreeChanged += Objects_TreeChanged;

            _Rslt = new List<KeyValuePair<HKey, HDynamicObject>>();
            foreach (var Itm in Hierarchy.OrderBy(x => x.ID))
            {
                _Rslt.Add(new KeyValuePair<HKey, HDynamicObject>(Itm.ID, new HDynamicObject(Itm.ID, true) { Name = Itm.Name, IsContainer = true, Rank = Itm.Rank }));
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
            Root = new ObservableCollection<HDynamicObject>();

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
                await getObjects.ForEachAsync((itm) =>
                {
                    _Rslt.Add(new KeyValuePair<HKey, HDynamicObject>(itm.HID, itm));
                    TreeListExpandedNodesHelper.RegisterBaseObject(itm);
                });
            }
        }
        public void AddEnum()
        {
           
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
            switch (e.Action)
            {
                case CollectionAction.Add:
                    HKey NewKey = e.NewVal.Key;
                    
                    if (!NewKey.IsRoot)
                    {                        
                        if (Objects.ContainsKey(NewKey.ParentKey))
                        {
                            HDynamicObject Parent = (HDynamicObject)Objects[NewKey.ParentKey];
                            HDynamicObject NewItem = (HDynamicObject)e.NewVal.Value;
                            NewItem.Parent = Parent;
                            NewItem.Root = (HDynamicObject)Objects[NewKey.RootKey];
                            if(e.NewVal.Value.Rank == null || e.NewVal.Value.Rank == 0)
                            {
                                e.NewVal.Value.Rank = e.NewVal.Key.Rank;
                            }
                            //Logic to add in all children in case not added in order
                            //foreach(KeyValuePair<HKey, DataObject> Chldrn in Objects.Where(x => x.Key.Contains(NewKey) && !x.Key.Equals(NewKey)))
                            //{

                            //    Chldrn.Value.Parent = NewItem;
                            //    NewItem.Children.TryAdd(Chldrn.Key, Chldrn.Value);

                            //}
                            Objects[NewKey.ParentKey].Children.Add(e.NewVal.Value);
                        }
                    }
                    else
                    {                        
                        Root.Add(e.NewVal.Value);
                        if (!Root.FirstOrDefault().HasKey("CustomLists"))
                        {
                            Root.FirstOrDefault()["CustomLists"] = new ObservableConcurrentDictionary<string, KeyObjectDictionary>();
                        }
                           
                        GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Root Added");
                    }
                    break;
                case CollectionAction.Remove:
                    HKey DelKey = e.RemVal.Key;
                    TreeListExpandedNodesHelper.RegisterBaseObject(e.RemVal.Value);
                    if (!DelKey.IsRoot)
                    {
                        HKey ParKey = DelKey.ParentKey;
                        if (Objects.ContainsKey(ParKey))
                        {
                            var DelItem = Objects[ParKey].Children.FirstOrDefault(x => x.HID.Equals(DelKey));
                            if(DelItem != null)
                            {
                                Objects[ParKey].Children.Remove(DelItem);
                            }                                              
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
           
           // SetCurrenItem(HKey.RootKeyVal);
            OnModelInitialized(new EventArgs());
        }
        #endregion
    }
}
