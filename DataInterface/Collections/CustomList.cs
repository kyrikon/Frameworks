using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using Newtonsoft.Json;
using Core.Helpers;
using System.Collections.ObjectModel;

namespace DataInterface
{
    public class CustomList : Core.Helpers.NotifyPropertyChanged 
    {

        #region Fields

        #endregion

        #region Events
        public delegate void ListSelectionChangedEventHandler(object sender, ListSelectionChangedEventArgs args);
        public event ListSelectionChangedEventHandler SelectionChangedEvent; 
        #endregion

        #region Constructor
        public CustomList()
        {
            SortByValue = false;
            Items = new ObservableConcurrentDictionary<string, object>();
            AddListItemCmd = new DelegateCommand((x) => AddItem(x));
            Items.DictionaryChanged += Items_DictionaryChanged;
            Items.PropertyChanged += Items_PropertyChanged;
        }
       
        #endregion
        #region Commands        
        [JsonIgnore]
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

        [JsonIgnore]
        public ReadOnlyObservableCollection<KeyValuePair<string, object>> SortedItems
        {           
            get
            {
                if (SortByValue)
                {
                    return new ReadOnlyObservableCollection<KeyValuePair<string, object>>(new ObservableCollection<KeyValuePair<string, object>>(Items.ItemList.OrderBy(x => x.Value)));
                }

                return new ReadOnlyObservableCollection<KeyValuePair<string, object>>(new ObservableCollection<KeyValuePair<string, object>>(Items.ItemList.OrderBy(x => x.Key)));
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
        public KeyValuePair<string, object>? DefaultItem
        {
            get
            {
                return GetPropertyValue<KeyValuePair<string, object>?>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }

     
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

        [JsonIgnore]
        public string Validation
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
        #endregion

        #region Methods
        public void AddItem(object x)
        {
            object _TmpNewVal = new object();
            if (!NewKey.IsFieldRules())
            {
                Validation = "Invalid Key Name";
                return;
            }
            if (Items.ContainsKey(NewKey))
            {
                Validation = "Key Name Exists";
                return;
            }
            if(NewValue == null)
            {
                if (Items.Values.Any(x =>  x.CheckNullValRef()))
                {
                    Validation = "Null Value Exists";
                    return;
                }
            }
            else
            {
                if (Items.Values.Where(x => x!= null).Any(x => x.Equals(NewValue)))
                {
                    Validation = "Value Exists";
                    return;
                }
            }
           
            if (!ValueType.Nullable && NewValue == null)
            {
                Validation = "Value Can't be null";
                return;
            }
            if(NewValue != null && NewValue.GetType().AssemblyQualifiedName != ValueType.AssemblyTypeName)
            {               
                NewValue = Convert.ChangeType(NewValue, Type.GetType(ValueType.AssemblyTypeName));
            }
            if(NewValue == null)
            {
                _TmpNewVal = Core.Models.NullValueRef.NullRefVal();
            }
            else
            {
                _TmpNewVal = NewValue;

            }
            if(Items.TryAdd(NewKey, _TmpNewVal))
            {
                Validation = string.Empty;
            }
        }
        public void RemoveItem(string Key, object Value)
        {
            object tmpVal;
            Items.TryRemove(Key, out tmpVal);
        }


        #endregion
        #region Callbacks
        private void Items_DictionaryChanged(object sender, DictionaryChangedEventArgs<string, object> args)
        {
            OnPropertyChanged("SortedItems");
        }
        private void Items_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SortedItems");
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
