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


namespace DataInterface
{
    [Serializable]
    public class ObservableConcurrentDictionary<T1, T2> : ConcurrentDictionary<T1, T2>, INotifyCollectionChanged
    {

        #region Events / Delegates
        //This notifies the UI. For Big operations disable then raise at the end
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        //This notifies node changes without triggering a UI update. Useful for automating tree generation on the collection
        public delegate void TreeChangedEventHandler(object sender, TreeChangedEventArgs<T1,T2> args);
        public event TreeChangedEventHandler TreeChanged;
        #endregion

        #region Fields 
        protected bool _Notify = true;
        private NotifyCollectionChangedAction _DefferAction;
        #endregion
        #region Constructors
        public ObservableConcurrentDictionary()
        {
        }
        public ObservableConcurrentDictionary(SynchronizationContext ctx = null)
        {
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public new T2 this[T1 Key]
        {
            get
            {
                try { 
                return base[Key];
                }
                catch(KeyNotFoundException)
                {
                    return default(T2);
                }
            }
            set
            {
                NotifyCollectionChangedAction Act = NotifyCollectionChangedAction.Add;
                NotifyCollectionChangedEventArgs Args;
                TreeChangedEventArgs<T1,T2> TArgs;
                if(this.ContainsKey(Key))
                {
                    Act = NotifyCollectionChangedAction.Replace;
                    KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, value);
                    KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(Key, this[Key]);
                    Args = new NotifyCollectionChangedEventArgs(Act, NewVal, OldVal);
                    TArgs = new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace,NewVal = NewVal,OldVal = OldVal };
                }
                else
                {
                    Act = NotifyCollectionChangedAction.Add;
                    KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, value);
                    Args = new NotifyCollectionChangedEventArgs(Act, NewVal);
                    TArgs = new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = NewVal };
                }
                OnTreeChanged(TArgs);
                OnCollectionChanged(Args);
                base[Key] = value;

            }
        }

        #endregion
        #region Methods     
        public new bool TryAdd(T1 Key, T2 Value)
        {
            bool IsSuccess = base.TryAdd(Key, Value);
            if(IsSuccess)
            {               
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,new KeyValuePair<T1, T2>(Key, Value)));
                OnTreeChanged(new TreeChangedEventArgs<T1, T2>() { Action   = CollectionAction.Add,NewVal = new KeyValuePair<T1, T2>(Key, Value) });
            }
            return IsSuccess;
        }
        public bool TryAdd(KeyValuePair<T1,T2> KVP)
        {           
            bool IsSuccess = base.TryAdd(KVP.Key, KVP.Value);
            if(IsSuccess)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, KVP));
                OnTreeChanged(new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = KVP });
            }
            return IsSuccess;
        }
        public new bool TryRemove(T1 Key,out T2 RemVal)
        {           
            bool IsSuccess = base.TryRemove(Key,out RemVal);
            if(IsSuccess)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<T1, T2>(Key, RemVal)));
                OnTreeChanged(new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Remove, OldVal = new KeyValuePair<T1, T2>(Key, RemVal) });
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
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<T1, T2>(Key, NewVal), OldVal));
                    OnTreeChanged(new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, OldVal = OldVal,NewVal = new KeyValuePair<T1, T2>(Key, NewVal)});
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
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, KVP, OldVal));
                    OnTreeChanged(new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, OldVal = OldVal, NewVal = KVP });
                }
            }
            return IsSuccess;
        }
        public T2 AddOrUpdate(T1 Key, T2 Value)
        {            
            NotifyCollectionChangedAction Act = NotifyCollectionChangedAction.Add;
             NotifyCollectionChangedEventArgs Args;
            TreeChangedEventArgs<T1, T2> TArgs;

            if(this.ContainsKey(Key))
            {
                Act = NotifyCollectionChangedAction.Replace;
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, Value);
                KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(Key, this[Key]);
                Args = new  NotifyCollectionChangedEventArgs(Act,NewVal,OldVal);
                TArgs = new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, NewVal = NewVal, OldVal = OldVal };
            }
            else
            {
                Act = NotifyCollectionChangedAction.Add;
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(Key, Value);
                Args =  new NotifyCollectionChangedEventArgs(Act, NewVal);
                TArgs = new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = NewVal };
            }
            T2 RetVal = base.AddOrUpdate(Key, Value, (k, val) => Value);
            OnTreeChanged(TArgs);
            OnCollectionChanged(Args);            
            return RetVal;
        }
        public T2 AddOrUpdate(KeyValuePair<T1, T2> KVP)
        {
            NotifyCollectionChangedAction Act = NotifyCollectionChangedAction.Add;
            NotifyCollectionChangedEventArgs Args;
            TreeChangedEventArgs<T1, T2> TArgs;

            if(this.ContainsKey(KVP.Key))
            {
                Act = NotifyCollectionChangedAction.Replace;
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(KVP.Key, KVP.Value);
                KeyValuePair<T1, T2> OldVal = new KeyValuePair<T1, T2>(KVP.Key, this[KVP.Key]);
                Args = new NotifyCollectionChangedEventArgs(Act, NewVal, OldVal);
                TArgs = new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Replace, NewVal = NewVal, OldVal = OldVal };
            }
            else
            {
                Act = NotifyCollectionChangedAction.Add;
                KeyValuePair<T1, T2> NewVal = new KeyValuePair<T1, T2>(KVP.Key, KVP.Value);
                Args = new NotifyCollectionChangedEventArgs(Act, NewVal);
                TArgs = new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Add, NewVal = NewVal };
            }
            T2 RetVal = base.AddOrUpdate(KVP.Key, KVP.Value, (k, val) => KVP.Value);
            OnTreeChanged(TArgs);
            OnCollectionChanged(Args);
            return RetVal;
        }
        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnTreeChanged(new TreeChangedEventArgs<T1, T2>() { Action = CollectionAction.Reset });
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }
        public void BeginEdit(NotifyCollectionChangedAction act)
        {
            _DefferAction = act;
            _Notify = false;
        }
        public void EndEdit(IList<KeyValuePair<T1, T2>> itms)
        {
            _Notify = true;           
            NotifyCollectionChangedEventArgs Args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itms.ToList());
            OnCollectionChanged(Args);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs Args)
        {
            if (_Notify)
            {              
                    CollectionChanged?.Invoke(this, Args);                
            }
        }
        protected void OnTreeChanged(TreeChangedEventArgs<T1,T2> Args)
        {
            TreeChanged?.Invoke(this, Args);            
        }

      
        #endregion
        #region Callbacks     
        #endregion

    }

    public class TreeChangedEventArgs<T1,T2>
    {
        public TreeChangedEventArgs()
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
