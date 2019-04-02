using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace DataInterface
{
    /// <summary>
    /// This should be changed as to not INotifyCollectionChanged.
    /// Binding this directly to the UI causes issues because the INotifyCollectionChanged needs Array indexes when updating
    /// And this being a dictionary doesnt store the data as a list
    /// The future pattern should be this as an internal store with a list Binding to the UI
    /// Adding and deleting Key/Values should still be done on the backing Dictionary and the UI refresh should be handled by the Tree changed event
    /// This also serves as the main Class for storing / retrieving the model
    /// /// </summary>
    /// <typeparam name="T1"> Key For Dictionary</typeparam>
    /// <typeparam name="T2"> Valye Type for dictionary</typeparam>
    [Serializable]
    public class ObservableConcurrentDictionary<T1, T2> : ConcurrentDictionary<T1, T2>
    {

        #region Events / Delegates

        //This notifies node changes without triggering a UI update. Useful for automating tree generation on the collection
        public delegate void DictionaryChangedEventHandler(object sender, DictionaryChangedEventArgs<T1,T2> args);
        public event DictionaryChangedEventHandler DictionaryChanged;
        #endregion
        #region Fields 

        protected bool _Notify = true;
        private ObservableCollection<KeyValuePair<T1, T2>> _ItemList;

        #endregion
        #region Constructors
        public ObservableConcurrentDictionary()
        {
            ItemList = new ObservableCollection<KeyValuePair<T1, T2>>();
        }
        #endregion
        #region Properties
        public new T2 this[T1 Key]
        {
            get
            {
                try
                {
                    return base[Key];
                }
                catch (KeyNotFoundException)
                {
                    return default(T2);
                }
            }
            set
            {
                DictionaryChangedEventArgs<T1,T2> TArgs;
                if(this.ContainsKey(Key))
                {
                    KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, value);
                    KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(Key, this[Key]);
                    TArgs = new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace,NewVal = NewVal,OldVal = OldVal };
                }
                else
                {
                    KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, value);
                    TArgs = new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = NewVal };
                }
                OnDictionaryChanged(TArgs);
                base[Key] = value;

            }
        }
        public ObservableCollection<KeyValuePair<T1,T2>> ItemList
        {
            get
            {
                return _ItemList;
            }
            private set
            {
                _ItemList = value;
            }
        }
        #endregion
        #region Methods     
        public new bool TryAdd(T1 Key, T2 Value)
        {
            bool IsSuccess = base.TryAdd(Key, Value);
            if(IsSuccess)
            {               
                OnDictionaryChanged(new DictionaryChangedEventArgs<T1, T2>() { Action   = CollectionAction.Add,NewVal = new KeyValuePair<T1, T2>(Key, Value) });
            }
            return IsSuccess;
        }
        public bool TryAdd(KeyValuePair<T1,T2> KVP)
        {           
            bool IsSuccess = base.TryAdd(KVP.Key, KVP.Value);
            if(IsSuccess)
            {
                OnDictionaryChanged(new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = KVP });
            }
            return IsSuccess;
        }
        public new bool TryRemove(T1 Key,out T2 RemVal)
        {           
            bool IsSuccess = base.TryRemove(Key,out RemVal);
            if(IsSuccess)
            {
                KeyValuePair<T1, T2> RemoveValue = new KeyValuePair<T1, T2>(Key, RemVal);            
                OnDictionaryChanged(new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Remove, RemVal = RemoveValue });
            }
            return IsSuccess;
        }
        public new bool TryUpdate(T1 Key,T2 NewVal,T2 ComparisonVal)
        {
            bool IsSuccess = false;
            if(this.ContainsKey(Key))
            {
                KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(Key, this[Key]);

                IsSuccess = base.TryUpdate(Key, NewVal, ComparisonVal);
                if(IsSuccess)
                {
                    OnDictionaryChanged(new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, OldVal = OldVal,NewVal = new KeyValuePair<T1, T2>(Key, NewVal)});
                }
            }
            return IsSuccess;
        }
        public bool TryUpdate(KeyValuePair<T1, T2> KVP, T2 ComparisonVal)
        {
            bool IsSuccess = false;
            if(this.ContainsKey(KVP.Key))
            {
                KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(KVP.Key, this[KVP.Key]);

                IsSuccess = base.TryUpdate(KVP.Key, KVP.Value, ComparisonVal);
                if(IsSuccess)
                {                    
                    OnDictionaryChanged(new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, OldVal = OldVal, NewVal = KVP });
                }
            }
            return IsSuccess;
        }
        public T2 AddOrUpdate(T1 Key, T2 Value)
        {            
            DictionaryChangedEventArgs<T1, T2> TArgs;
            if(this.ContainsKey(Key))
            {
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, Value);
                KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(Key, this[Key]);
                TArgs = new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, NewVal = NewVal, OldVal = OldVal };
            }
            else
            {
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, Value);
                TArgs = new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = NewVal };
            }
            T2 RetVal = base.AddOrUpdate(Key, Value, (k, val) => Value);
            OnDictionaryChanged(TArgs);                    
            return RetVal;
        }
        public T2 AddOrUpdate(KeyValuePair<T1, T2> KVP)
        {
            NotifyCollectionChangedEventArgs Args;
            DictionaryChangedEventArgs<T1, T2> TArgs;

            if(this.ContainsKey(KVP.Key))
            {
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(KVP.Key, KVP.Value);
                KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(KVP.Key, this[KVP.Key]);
                TArgs = new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, NewVal = NewVal, OldVal = OldVal };
            }
            else
            {
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(KVP.Key, KVP.Value);
                TArgs = new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = NewVal };
            }
            T2 RetVal = base.AddOrUpdate(KVP.Key, KVP.Value, (k, val) => KVP.Value);
            OnDictionaryChanged(TArgs);
            return RetVal;
        }
        public new void Clear()
        {           
            base.Clear();
            DictionaryChanged?.Invoke(this, new DictionaryChangedEventArgs<T1, T2>() { Action = CollectionAction.Reset });
        }
        public void AddList(IEnumerable<KeyValuePair<T1,T2>> NewItems,int Parallelism = 16)
        {
            this.BeginEdit(System.Collections.Specialized.NotifyCollectionChangedAction.Add);           
            Parallel.ForEach<KeyValuePair<T1, T2>>(NewItems,(CurrItem) =>
            {               
                bool rslt = this.TryAdd(CurrItem.Key, CurrItem.Value);               
            });

          
        }
        public void BeginInit()
        {
            _Notify = false;
        }
        public void EndInit()
        {
            _Notify = true;
        }
        public void BeginEdit(NotifyCollectionChangedAction act)
        {
            //TODO Implement deferred changed Notofication with aggregate events 
            _Notify = false;
        }
        public void EndEdit(IList<KeyValuePair<T1, T2>> itms)
        {
            _Notify = true;           
        }
       
        protected void OnDictionaryChanged(DictionaryChangedEventArgs<T1,T2> Args)
        {
            //TODO Implement deferred changed Notofication with aggregate events 
            DictionaryChanged?.Invoke(this, Args);

            switch (Args.Action)
            {
                case CollectionAction.Add:
                    ItemList.Add(Args.NewVal);
                    break;
                case CollectionAction.Remove:
                    ItemList.Remove(Args.RemVal);
                    break;
                case CollectionAction.Reset:
                    ItemList.Clear();
                    break;

            }
            
        }      
        #endregion
        #region Callbacks     
        #endregion

    }

    public class DictionaryChangedEventArgs<T1,T2>
    {
        public DictionaryChangedEventArgs()
        {
        }
        public CollectionAction Action { get; internal set; }
        public KeyValuePair<T1, T2> NewVal { get; internal set; }
        public KeyValuePair<T1, T2> OldVal { get; internal set; }
        public KeyValuePair<T1, T2> RemVal { get; internal set; }
        
    }

    public enum CollectionAction
    {
        Add,
        Remove,
        Replace,
        Reset
    }
}
