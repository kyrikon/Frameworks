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
        }
        public DataModel(IDataSource DS, DynamicObjectHierarchy _Hierarchy)
        {
            Hierarchy = _Hierarchy;
            DataSource = DS;
            Objects = new HKeyDynamicObjectDictionary();
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
        public HKeyDynamicObjectDictionary Objects
        {
            get
            {
                return GetPropertyValue<HKeyDynamicObjectDictionary>();
            }
            private set
            {
                if (GetPropertyValue<HKeyDynamicObjectDictionary>() != value)
                {
                    SetPropertyValue<HKeyDynamicObjectDictionary>(value);
                    OnPropertyChanged("Root");
                }
            }

        }
        
        public ReadOnlyObservableCollection<HDynamicObject> Root
        {
            get
            {
                return Objects.Root;
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

        #endregion
        #region Methods    

        public void Initialize()
        {

            Objects = new HKeyDynamicObjectDictionary();
            //DataSource.Connection.ConnectionChangedEvent -= Connection_ConnectionChangedEvent;
            //DataSource.Connection.ConnectionChangedEvent += Connection_ConnectionChangedEvent;
            DataSource.DataInitializedEvent -= DataSource_DataInitializedEvent;
            DataSource.DataInitializedEvent += DataSource_DataInitializedEvent;
            DataSource.Connection.Disconnect();
            DataSource.Connection.Connect();
        }
       
        public void Clear()
        {
            if (Objects != null && Root != null)
            {
                Objects.Clear();
            }
        }

        public void CreateNewProject(DynamicObjectHierarchy DOH)
        {
            Hierarchy = DOH;
            Hierarchy.FirstOrDefault(x => x.ID.Equals(HKey.RootKeyVal)).Name = DataSource.Connection.ConnectionName;

            Objects = new HKeyDynamicObjectDictionary();

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

            Objects = new HKeyDynamicObjectDictionary();

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
        // to be implemented
        // Get from storage
        // Write back to storage
        // start transactions
        // manage Memberships
        // manage properties

        // manage changes in storage (compare server timestamp for update and fix conflicts)
        // queue of storage key changes feed to update objects

        #endregion
        #region Callbacks
        protected void OnModelInitialized(EventArgs Args)
        {
            ModelInitialized?.Invoke(this, Args);
        }
    
        private async void Connection_ConnectionChangedEvent(object sender, ConnectionChangedEventArgs args)
        {
            if (args.ConnectionState == System.Data.ConnectionState.Open)
            {
               await Task.Delay(1);
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
            OnModelInitialized(new EventArgs());
        }
        #endregion
    }
}
