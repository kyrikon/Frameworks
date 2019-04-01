using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using Newtonsoft.Json;

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
            Items = new ObservableConcurrentDictionary<string, object>();
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
            }
        }
        //[JsonIgnore]
        //public string SelectedKey
        //{
        //    get
        //    {
        //        return GetPropertyValue<string>();
        //    }
        //    set
        //    {
        //        string OldValue = SelectedKey;
        //        SetPropertyValue(value);
        //        OnPropertyChanged("SelectedValue");
        //        SelectionChangedEvent?.Invoke(this, new ListSelectionChangedEventArgs(OldValue, value));
        //    }
        //}
        //[JsonIgnore]
        //public object SelectedValue
        //{
        //    get
        //    {
        //        return GetPropertyValue<object>();
        //    }
        //    set
        //    {
        //        object OldValue = SelectedValue;
        //        SetPropertyValue(value);
        //        OnPropertyChanged("SelectedKey");
        //        SelectionChangedEvent?.Invoke(this, new ListSelectionChangedEventArgs(OldValue, value));
        //    }
        //}
        [JsonIgnore]
        public KeyValuePair<string,object> SelectedItem
        {
            get
            {
                return GetPropertyValue<KeyValuePair<string, object>>();
            }
            set
            {
                KeyValuePair<string, object> OldValue = SelectedItem;
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
        public Type ValueType
        {
            get
            {

                return SelectedItem.Value.GetType();
            }            
        }

        #endregion

        #region Methods
        public void AddItem(string Key,object Value)
        {
            Items.AddOrUpdate(Key, Value);
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
        public ListSelectionChangedEventArgs(KeyValuePair<string, object> _oldKey, KeyValuePair<string, object> _newKey)
        {
            OldKey = _oldKey;
            NewKey = _newKey;
        }
        public KeyValuePair<string, object> OldKey { get; internal set; }
        public KeyValuePair<string, object> NewKey { get; internal set; }
    }
    
}
