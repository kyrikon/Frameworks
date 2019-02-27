using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Core.Helpers;
using System.Linq;
using Core.Extensions;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace DataInterface
{
    [Serializable]
    public class HDynamicObject : DynamicObjectBase,IHDataObject
    {
        // Considerations for base class
        // 1. Dynamic nature
        // 2. (De) Serialization
        // 3. Edit Auditing
        // 4. Syncronization with backing data store (multi user)
        #region Events / Delegates
            
        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args);
        [field: NonSerialized]
        public event SelectionChangedEventHandler SelectionChangedEvent;

        public delegate void CheckedChangedEventHandler(object sender, CheckedChangedEventArgs args);
        [field: NonSerialized]
        public event CheckedChangedEventHandler CheckedChangedEvent;

        #endregion
        #region Fields 
        private bool _IsInit;
        #endregion

        #region Constructors
        public HDynamicObject() :base (false)
        {
            _IsInit = true;
            ObjectData.CollectionChanged += ObjectData_CollectionChanged;           
            Children = new HKeyDictionary();
            PropertyExtensions = new ObservableConcurrentDictionary<string, PropertyExtensions>();
            IsExpanded = true;
            _IsInit = false;
        }
        public HDynamicObject(HKey Key) : base(false)
        {
            _IsInit = true;
            ObjectData.CollectionChanged += ObjectData_CollectionChanged;
            Children = new HKeyDictionary();
            PropertyExtensions = new ObservableConcurrentDictionary<string, PropertyExtensions>();
            IsExpanded = true;
            ID = Key;
            _IsInit = false;
        }
        public HDynamicObject(bool _Transactional = false) : base(_Transactional)
        {
            _IsInit = true;
            ObjectData.CollectionChanged += ObjectData_CollectionChanged;
            Children = new HKeyDictionary();
            PropertyExtensions = new ObservableConcurrentDictionary<string, PropertyExtensions>();
            IsExpanded = true;
            _IsInit = false;
        }
        public HDynamicObject(HKey Key,bool _Transactional = false) : base(_Transactional)
        {
            _IsInit = true;
            ObjectData.CollectionChanged += ObjectData_CollectionChanged;
            Children = new HKeyDictionary();
            PropertyExtensions = new ObservableConcurrentDictionary<string, PropertyExtensions>();
            IsExpanded = true;
            ID = Key;
            _IsInit = false;
        }
        public HDynamicObject(KeyValuePair<string, dynamic>[] InitArray, bool _Transactional = false) : base(InitArray,_Transactional)
        {
            _IsInit = true;
            ObjectData.CollectionChanged += ObjectData_CollectionChanged;
            Children = new HKeyDictionary();
            PropertyExtensions = new ObservableConcurrentDictionary<string, PropertyExtensions>();
            IsExpanded = true;
            _IsInit = false;
        }
        public HDynamicObject(HKey Key, KeyValuePair<string, dynamic>[] InitArray, bool _Transactional = false) : base(InitArray, _Transactional)
        {
            _IsInit = true;
            ObjectData.CollectionChanged += ObjectData_CollectionChanged;
            Children = new HKeyDictionary();
            PropertyExtensions = new ObservableConcurrentDictionary<string, PropertyExtensions>();
            IsExpanded = true;
            ID = Key;
            _IsInit = false;
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties

        [JsonProperty]
        public int[] ID { get;  private set; }

        [JsonIgnore]
        public HKey HID
        {
            get
            {
                return (HKey)ID;
            }
        }

        [JsonIgnore]
        [IgnoreDataMember]
        public HKeyDictionary Children { get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ObservableCollection<HDynamicObject> ChildrenCol
        {
            get
            {
                return new ObservableCollection<HDynamicObject>(Children.OrderBy(x => x.Key).Select(x => x.Value));
            }

        }

        [JsonIgnore]
        [IgnoreDataMember]
        public HDynamicObject Parent { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public HDynamicObject Root { get; set; }

        [JsonProperty]
        private ObservableConcurrentDictionary<string, PropertyExtensions> PropertyExtensions { get; }
        
        public bool IsSelected
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                    SelectionChangedEvent?.Invoke(this, new SelectionChangedEventArgs(value));
                }
            }
        }
        public bool IsChecked
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                    CheckedChangedEvent(this, new CheckedChangedEventArgs(value));
                }
            }
        }
        public bool Highlight
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
        public bool IsFiltered
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
        public bool IsExpanded
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                    if (!_IsInit)
                    {
                        if (IsExpanded && !HID.IsRoot)
                        {
                            ExpandParents(Parent);
                        }
                    }
                }
            }
        }

        #endregion
        #region Methods     
        private void ExpandParents(HDynamicObject Obj)
        {
            Obj.IsExpanded = true;
            if (!Obj.HID.IsRoot)
            {
                ExpandParents(Obj.Parent);
            }
        }
        #endregion
        #region Callbacks
        private void ObjectData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    PropertyExtensions.TryAdd(((KeyValuePair<string, object>)e.NewItems[0]).Key, new PropertyExtensions());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    PropertyExtensions DelExt = null;
                    PropertyExtensions.TryRemove(((KeyValuePair<string, object>)e.OldItems[0]).Key,out DelExt);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PropertyExtensions.Clear();
                    break;

            }
        }
        #endregion
    }   
}
