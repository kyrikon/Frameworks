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
    public class HDynamicObject : DynamicObjectBase, IHDynamicObject
    {
        // Considerations for base class
        // 1. Object nature
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

        public delegate void RankChangedEventHandler(object sender, RankChangedEventArgs args);
        [field: NonSerialized]
        public event RankChangedEventHandler RankChangedEvent;

        #endregion
        #region Fields 
        private bool _IsInit;
        #endregion

        #region Constructors
        public HDynamicObject() : base(false)
        {
            _IsInit = true;
            Children = new ObservableCollection<HDynamicObject>();
            InheritedProperties = new ObservableConcurrentDictionary<string, DynamicField>();
            IsExpanded = true;
            _IsInit = false;

        }

        public HDynamicObject(HKey Key) : base(false)
        {
            _IsInit = true;
            Children = new ObservableCollection<HDynamicObject>();
            InheritedProperties = new ObservableConcurrentDictionary<string, DynamicField>();
            IsExpanded = true;
            ID = Key;
            _IsInit = false;
        }
        public HDynamicObject(bool _Transactional = false) : base(_Transactional)
        {
            _IsInit = true;
            Children = new ObservableCollection<HDynamicObject>();
            InheritedProperties = new ObservableConcurrentDictionary<string, DynamicField>();
            IsExpanded = true;
            _IsInit = false;
        }
        public HDynamicObject(HKey Key, bool _Transactional = false) : base(_Transactional)
        {
            _IsInit = true;
            Children = new ObservableCollection<HDynamicObject>();
            InheritedProperties = new ObservableConcurrentDictionary<string, DynamicField>();
            IsExpanded = true;
            ID = Key;
            _IsInit = false;
        }
        public HDynamicObject(KeyValuePair<string, Object>[] InitArray, bool _Transactional = false) : base(InitArray, _Transactional)
        {
            _IsInit = true;
            Children = new ObservableCollection<HDynamicObject>();
            InheritedProperties = new ObservableConcurrentDictionary<string, DynamicField>();
            IsExpanded = true;
            _IsInit = false;
        }
        public HDynamicObject(HKey Key, KeyValuePair<string, Object>[] InitArray, bool _Transactional = false) : base(InitArray, _Transactional)
        {
            _IsInit = true;
            Children = new ObservableCollection<HDynamicObject>();
            InheritedProperties = new ObservableConcurrentDictionary<string, DynamicField>();
            IsExpanded = true;
            ID = Key;
            _IsInit = false;
        }
        #endregion
        #region Properties

        [JsonProperty]
        public int[] ID { get; private set; }

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
        public ObservableCollection<HDynamicObject> Children
        {
            get;
        }


        [JsonIgnore]
        [IgnoreDataMember]
        public HDynamicObject Parent { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public HDynamicObject Root { get; set; }
        [JsonProperty]
        public int Rank
        {
            get
            {
                return GetPropertyValue<int>();
            }
            set
            {
                SetPropertyValue<int>(value);
            }
        }
        public ObservableConcurrentDictionary<string, DynamicField> InheritedProperties{ get; set; }
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
       public void NodeRankChange()
       {
            OnPropertyChanged("ChildrenCollection");
       }
        public HDynamicObject NewFolder()
        {
            HDynamicObject lastObj = this.Children.OrderBy(x => x.HID).LastOrDefault();
            HKey NewKey = new HKey(new int[]{ 0 });
            if (lastObj != null)
            {
                NewKey = this.HID.CreateChildKey(lastObj.HID.Rank + 1);
            }
            else
            {
                NewKey = this.HID.CreateChildKey(1);
            }
            
           
            HDynamicObject NewFldr = new HDynamicObject(NewKey, true) { Name = "New Folder", IsContainer = true,  Rank = this.Children.Count + 1 };
            return NewFldr;
        }

        #endregion
        #region Callbacks    

        #endregion
    }   
    
}
