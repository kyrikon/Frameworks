using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;

namespace DataInterface
{
    public class KeyObjectDictionary<T> : Core.Helpers.NotifyPropertyChanged
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
            Items = new ObservableConcurrentDictionary<string, T>();
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

        public ObservableConcurrentDictionary<string,T> Items
        {
            get
            {
                return GetPropertyValue<ObservableConcurrentDictionary<string, T>>();
            }
            private set
            {
                SetPropertyValue(value);
            }
        }
        public string SelectedKey
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                string OldValue = SelectedKey;
                SetPropertyValue(value);
                SelectionChangedEvent?.Invoke(this, new ListSelectionChangedEventArgs(OldValue,value));
            }
        }

        public T SelectedValue
        {
            get
            {
                return Items[SelectedKey];
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
        public Type ValueType
        {
            get
            {
                return typeof(T);
            }            
        }

        #endregion

        #region Methods
        public void AddItem(string Key,T Value)
        {
            Items.AddOrUpdate(Key, Value);
        }
        public void RemoveItem(string Key, T Value)
        {
            T tmpVal;
            Items.TryRemove(Key, out tmpVal);
        }
        #endregion
    }
    public class ListSelectionChangedEventArgs
    {
        public ListSelectionChangedEventArgs(string _oldKey, string _newKey)
        {
            OldKey = _oldKey;
            NewKey = _newKey;
        }
        public string OldKey { get; internal set; }
        public string NewKey { get; internal set; }


    }
}
