﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using Newtonsoft.Json;
using Core.Helpers;


namespace DataInterface
{
    public class KeyObjectDictionary : Core.Helpers.NotifyPropertyChanged 
    {

        #region Fields

        #endregion

        #region Events
        public delegate void ListSelectionChangedEventHandler(object sender, ListSelectionChangedEventArgs args);
        public event ListSelectionChangedEventHandler SelectionChangedEvent; 
        #endregion

        #region Constructor
        public KeyObjectDictionary()
        {
            SortByValue = false;
            Items = new ObservableConcurrentDictionary<string, object>();
            AddListItemCmd = new DelegateCommand((x) => AddItem(x));
        }
        #endregion
        #region Commands           
        public DelegateCommand AddListItemCmd
        {
            get; private set;
        }
        #endregion
        #region Properties
        public string Name
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }

        public ObservableConcurrentDictionary<string,object> Items
        {
            get
            {
                return GetPropertyValue<ObservableConcurrentDictionary<string, object>>();
            }
            private set
            {
                SetPropertyValue(value);
                OnPropertyChanged("SortedItems");
            }
        }
        public Dictionary<string, object> SortedItems
        {
            get
            {
                if (SortByValue)
                {
                    return Items.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                }
                return Items.OrderBy(x => x.Key).ToDictionary(x => x.Key,x => x.Value);
            }
        }
        
        [JsonIgnore]
        public object SelectedValue
        {
            get
            {
                if(SelectedItem.HasValue)
                {
                    return SelectedItem.Value;
                }
                return null;
            }
            
        }
        [JsonIgnore]
        public KeyValuePair<string,object>? SelectedItem
        {
            get
            {
                return GetPropertyValue<KeyValuePair<string, object>>();
            }
            set
            {
                KeyValuePair<string, object>? OldValue = SelectedItem;
                SetPropertyValue(value);
               // OnPropertyChanged("SelectedValue");
                SelectionChangedEvent?.Invoke(this, new ListSelectionChangedEventArgs(OldValue, value));
            }
        }
        public string DefaultKey
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        
        [JsonIgnore]
        public DataInterface.ValueType ValueType
        {
            get
            {
                return GetPropertyValue<DataInterface.ValueType>();
            }
            set
            {
                SetPropertyValue(value);
            }

        }
        public bool SortByValue
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue(value);
                OnPropertyChanged("SortedItems");                    
            }

        }

        [JsonIgnore]
        public string NewKey
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        [JsonIgnore]
        public object NewValue
        {
            get
            {
                return GetPropertyValue<object>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        #endregion

        #region Methods
        public void AddItem(object x)
        {
            Items.AddOrUpdate(NewKey, NewValue);
        }
        public void RemoveItem(string Key, object Value)
        {
            object tmpVal;
            Items.TryRemove(Key, out tmpVal);
        }

       
        #endregion

    }
    public class ListSelectionChangedEventArgs
    {
        public ListSelectionChangedEventArgs(KeyValuePair<string, object>? _oldKey, KeyValuePair<string, object>? _newKey)
        {
            OldKey = _oldKey;
            NewKey = _newKey;
        }
        public KeyValuePair<string, object>? OldKey { get; internal set; }
        public KeyValuePair<string, object>? NewKey { get; internal set; }
    }
    
}
