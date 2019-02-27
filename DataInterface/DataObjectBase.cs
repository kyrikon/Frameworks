using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Core.Helpers;
using System.Linq;
using Core.Extensions;
using System.Windows.Data;
using System.Collections.Generic;

namespace DataInterface
{
    [Serializable]
    public abstract class DataObjectBase : NotifyPropertyChanged, IDataObject
    {
        // Considerations for base class
        // 1. Dynamic nature
        // 2. (De) Serialization - parameterless constructor
        // 3. Edit Auditing
        // 4. Syncronization with backing data store (multi user)
        #region Events / Delegates

        #endregion
        #region Fields 
        #endregion

        #region Constructors
        protected DataObjectBase(bool _Transactional = false)
        {
            ObjectData = new DataObjectDictionary();
            ObjectType = new DataTypeDictionary();
            ModifiedData = new ObservableConcurrentDictionary<string, ConcurrentStack<ModifiedDataItem>>();
            EditStack = new ConcurrentStack<string>();
            IsTransactional = _Transactional;          
        }
        protected DataObjectBase(KeyValuePair<string,dynamic>[] InitArray,bool _Transactional = false)
        {
            ObjectData = new DataObjectDictionary();
            ObjectType = new DataTypeDictionary();
            ModifiedData = new ObservableConcurrentDictionary<string, ConcurrentStack<ModifiedDataItem>>();
            EditStack = new ConcurrentStack<string>();
            IsTransactional = _Transactional;
            this.FromArray(InitArray);
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties

        public dynamic this[string key]
        {
            get
            {
                dynamic tmpItem;
                ObjectData.TryGetValue(key, out tmpItem);
                return tmpItem;
            }
            set
            {
               
                double TimeStamp = DateTime.Now.ToOADate();
                SetModified(key, TimeStamp);
                ObjectData.AddOrUpdate(key, value);
                ObjectType.AddOrUpdate(key, value.GetType().AssemblyQualifiedName);                
                OnPropertyChanged(Binding.IndexerName);               
            }
        }
    
        public string Name
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if (GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }
        public string Description
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if (GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }
        public bool IsContainer
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if (GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
        [JsonIgnore]
        public string[] Fields
        {
            get
            {
                return ObjectData.Keys.OrderBy(x => x).ToArray();
            }
        }

        [JsonIgnore]
        public string FieldString
        {
            get
            {
                return string.Join(" ",ObjectData.Keys.OrderBy(x => x).ToArray());
            }
        }
        [JsonProperty]        
        protected DataObjectDictionary ObjectData { get; }
        [JsonProperty]
        protected DataTypeDictionary ObjectType { get; }
        private ObservableConcurrentDictionary<string,ConcurrentStack<ModifiedDataItem>> ModifiedData { get; }       
        [JsonIgnore]
        private ConcurrentStack<string> EditStack { get; }
        [JsonIgnore]
        public string LastFieldEdited
        {
            get
            {
                string LastEditKey = string.Empty;
                EditStack.TryPeek(out LastEditKey);
                return LastEditKey;
            }            
        }
        [JsonIgnore]
        public double VersionTimestamp
        {
            get
            {
                return GetPropertyValue<double>();
            }
           private set
            {
                if (GetPropertyValue<double>() != value)
                {
                    SetPropertyValue<double>(value);
                }
            }
        }
        [JsonIgnore]
        public bool IsModified
        {
            get
            {
                return EditStack.Any();
            }            
        }

        [JsonIgnore]
        public bool DeleteDenied
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            private set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }

        [JsonIgnore]
        public bool IsEditing
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

        public bool IsTransactional
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            private set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }

        [JsonIgnore]
        public bool IsReadOnly
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
        #endregion
        #region Methods     
        public T GetValue<T>(string key)
        {
            object tmpItem;
            ObjectData.TryGetValue(key, out tmpItem);
            try
            {
                return (T)tmpItem;
            }
            catch (InvalidCastException)
            {            
                return (T)Convert.ChangeType(tmpItem, typeof(T));
            }
        }
        public dynamic GetValue(string key)
        {
            object tmpItem;
            ObjectData.TryGetValue(key, out tmpItem);
            string tmpItemType;
            ObjectType.TryGetValue(key, out tmpItemType);
            try
            {
                Type CvrtType = Type.GetType(tmpItemType);
                if(CvrtType == null)
                {
                    return tmpItem;
                }
                if (CvrtType.IsEnum)
                {
                    
                    return Enum.ToObject(CvrtType,tmpItem);
                    //return Convert.ChangeType(Convert.ToInt32(tmpItem), CvrtType);
                }
                return Convert.ChangeType(tmpItem, CvrtType);
            }
            catch (InvalidCastException)
            {
                return tmpItem;
            }          
        }
        public ModifiedDataItem GetUndo(string key)
        {
            ModifiedDataItem tmpItem = null;
            if(ModifiedData.ContainsKey(key))
            {
                ModifiedData[key].TryPeek(out tmpItem);
            }
            if(tmpItem == null)
            {
                return null;
            }
            return tmpItem;
        }
        public ModifiedDataItem GetLastUndo()
        {
            if(string.IsNullOrEmpty(LastFieldEdited))
            {
                return null;
            }
            return GetUndo(LastFieldEdited);
        }
        public bool HasKey(string key)
        {
            return ObjectData.ContainsKey(key);
        }
        public void RemoveKey(string key)
        {
            if(ObjectData.ContainsKey(key))
            {
                object tmpObj = null;
                string tmpStr = null;
                ConcurrentStack<ModifiedDataItem> tmpMod = null;
                ObjectData.TryRemove(key,out tmpObj);
                ObjectType.TryRemove(key, out tmpStr);
                ModifiedData.TryRemove(key, out tmpMod);
                string[] tmpstack = EditStack.Reverse().ToArray();
                EditStack.Clear();
                foreach(string k in tmpstack)
                {
                    if(k != key)
                    {
                        EditStack.Push(k);
                    }
                }
            }           
        }              
        private void SetModified(string key,double TimeStamp,string UserID = "")
        {
            if(!ModifiedData.ContainsKey(key))
            {
                ModifiedData.TryAdd(key, new  ConcurrentStack<ModifiedDataItem>());              
            }
            if(ObjectData.ContainsKey(key))
            {
                if(!IsTransactional)
                {
                    ModifiedData[key].Clear();
                    string[] tmpstack = EditStack.Reverse().ToArray();
                    EditStack.Clear();
                    foreach(string k in tmpstack)
                    {
                        if(k != key)
                        {
                            EditStack.Push(k);
                        }
                    }
                }
                ModifiedData[key].Push(new ModifiedDataItem { Value = ObjectData[key], TimeStamp = TimeStamp, UserID = UserID });
                EditStack.Push(key);
                VersionTimestamp = TimeStamp;
                OnPropertyChanged("IsModified");
            }
        }        
        private void ClearModified(string key)
        {
            if(ModifiedData.ContainsKey(key))
            {
            ModifiedData[key].Clear();
            }           
        }
        public void CommitChanges()
        {
            foreach(ConcurrentStack<ModifiedDataItem> ChangeSet in ModifiedData.Values)
            {
                ChangeSet.Clear();
                EditStack.Clear();               
            }
        }
        public void UndoChange(string key)
        {
            if(ModifiedData.ContainsKey(key))
            {
                ModifiedDataItem GetLast = null;
                if(ModifiedData[key].TryPop(out GetLast))
                {
                    ObjectData[key] = GetLast.Value;
                    OnPropertyChanged(Binding.IndexerName);
                }               
            }
        }
        public void Undo()
        {
            string LastEditKey = string.Empty;
            if(EditStack.TryPop(out LastEditKey))
            {
                UndoChange(LastEditKey);
            }
        }        
        public void RevertChanges(string key)
        {
            foreach(ConcurrentStack<ModifiedDataItem> ChangeSets in ModifiedData.Values)
            {
                if(!ChangeSets.IsEmpty)
                {
                    ObjectData[key] = ChangeSets.FirstOrDefault().Value;
                    OnPropertyChanged(Binding.IndexerName);
                    ChangeSets.Clear();
                }
            }
        }
        public void Reset()
        {
            foreach(string key in ObjectData.Keys)
            {
                RevertChanges(key);               
                EditStack.Clear();                
            }
        }
        public string[] GetModifiedKeys()
        {           
            return ModifiedData.Where(x => !x.Value.IsEmpty).Select(x => x.Key).ToArray();
        }
        public ModifiedDataItem[] GetChangeSet(string key)
        {
            if(ModifiedData.ContainsKey(key))
            {
               return ModifiedData[key].OrderBy(x => x.TimeStamp).ToArray();
            }
            return new ModifiedDataItem[] { };
        }

        public KeyValuePair<string,Tuple<dynamic,string>>[] ToArray()
        {
            KeyValuePair<string, Tuple<dynamic, string>>[] Tmp = new KeyValuePair<string, Tuple<dynamic, string>>[ObjectData.Count];
            int lineCnt = 0;
            foreach (var line in ObjectData)
            {
                Tmp[lineCnt] = new KeyValuePair<string, Tuple<dynamic, string>>(line.Key, new Tuple<dynamic, string>(line.Value, line.GetType().AssemblyQualifiedName));
                lineCnt++;
            }
            return Tmp;
        }
        public void FromArray(KeyValuePair<string, dynamic>[] ObjValues)
        {
            foreach(KeyValuePair<string, dynamic> Item in ObjValues)
            {
                ObjectData.AddOrUpdate(Item);
                ObjectType.AddOrUpdate(Item.Key, Item.Value.GetType());
                OnPropertyChanged(Binding.IndexerName);
            }
        }
        
        public void CastProps()
        {
            foreach(KeyValuePair<string,dynamic> Itm in ObjectData)
            {
                Type ValType = Type.GetType(ObjectType[Itm.Key]);               
                if (ValType != null && !Itm.Value.GetType().Equals(ValType))
                {
                    ObjectData[Itm.Key] = GetValue(Itm.Key);
                }
            }
        }
        #endregion
      
    }
    [Serializable]
    public class ModifiedDataItem
    {
        public object Value
        {
            get; set;
        }
        public double TimeStamp
        {
            get; set;
        }
        public string UserID
        {
            get; set;
        }
        public T GetValue<T>()
        {
            try
            {
                return (T)Value;
            }
            catch(InvalidCastException)
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
        }
    }

}
