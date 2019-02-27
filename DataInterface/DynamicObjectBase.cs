using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Core.Helpers;
using System.Linq;
using Core.Extensions;
using System.Windows.Data;
using System.Collections.Generic;
using System.Dynamic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace DataInterface
{
    [Serializable]
    public abstract class DynamicObjectBase : INotifyPropertyChanged, IDataObject, ICustomTypeDescriptor
    {
        // Considerations for base class
        // 1. Dynamic nature
        // 2. (De) Serialization - parameterless constructor
        // 3. Edit Auditing
        // 4. Syncronization with backing data store (multi user)
        #region Events / Delegates

        #endregion
        #region Fields 
        private string[] _PropNames;
        #endregion

        #region Constructors
        protected DynamicObjectBase(bool _Transactional = false)
        {
            GetPropNames();
            ObjectData = new DataObjectDictionary();
            ObjectType = new DataTypeDictionary();
            ModifiedData = new ObservableConcurrentDictionary<string, ConcurrentStack<ModifiedDataItem>>();
            EditStack = new ConcurrentStack<string>();
            IsTransactional = _Transactional;
           
        }
        protected DynamicObjectBase(KeyValuePair<string, dynamic>[] InitArray, bool _Transactional = false)
        {
            GetPropNames();
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
                ObjectData.TryGetValue(key.ToLower(), out tmpItem);
                return tmpItem;
            }
            set
            {

                double TimeStamp = DateTime.Now.ToOADate();
                SetModified(key.ToLower(), TimeStamp);
                ObjectData.AddOrUpdate(key.ToLower(), value);
                ObjectType.AddOrUpdate(key.ToLower(), value.GetType().AssemblyQualifiedName);
                OnPropertyChanged(Binding.IndexerName);
                OnPropertyChanged(key.ToLower());
            }
        }
        [JsonProperty]
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
        [JsonProperty]
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
        [JsonProperty]
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
                return string.Join(" ", ObjectData.Keys.OrderBy(x => x).ToArray());
            }
        }
        [JsonProperty]
        protected DataObjectDictionary ObjectData { get; }
        [JsonProperty]
        protected DataTypeDictionary ObjectType { get; }
        private ObservableConcurrentDictionary<string, ConcurrentStack<ModifiedDataItem>> ModifiedData { get; }
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
                if (GetPropertyValue<bool>() != value)
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
                if (GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
        [JsonProperty]
        public bool IsTransactional
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            private set
            {
                if (GetPropertyValue<bool>() != value)
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
                if (GetPropertyValue<bool>() != value)
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
                if (CvrtType == null)
                {
                    return tmpItem;
                }
                if (CvrtType.IsEnum)
                {

                    return Enum.ToObject(CvrtType, tmpItem);
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
            if (ModifiedData.ContainsKey(key))
            {
                ModifiedData[key].TryPeek(out tmpItem);
            }
            if (tmpItem == null)
            {
                return null;
            }
            return tmpItem;
        }
        public ModifiedDataItem GetLastUndo()
        {
            if (string.IsNullOrEmpty(LastFieldEdited))
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
            if (ObjectData.ContainsKey(key))
            {
                object tmpObj = null;
                string tmpStr = null;
                ConcurrentStack<ModifiedDataItem> tmpMod = null;
                ObjectData.TryRemove(key, out tmpObj);
                ObjectType.TryRemove(key, out tmpStr);
                ModifiedData.TryRemove(key, out tmpMod);
                string[] tmpstack = EditStack.Reverse().ToArray();
                EditStack.Clear();
                foreach (string k in tmpstack)
                {
                    if (k != key)
                    {
                        EditStack.Push(k);
                    }
                }
            }
        }
        private void SetModified(string key, double TimeStamp, string UserID = "")
        {
            if (!ModifiedData.ContainsKey(key))
            {
                ModifiedData.TryAdd(key, new ConcurrentStack<ModifiedDataItem>());
            }
            if (ObjectData.ContainsKey(key))
            {
                if (!IsTransactional)
                {
                    ModifiedData[key].Clear();
                    string[] tmpstack = EditStack.Reverse().ToArray();
                    EditStack.Clear();
                    foreach (string k in tmpstack)
                    {
                        if (k != key)
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
            if (ModifiedData.ContainsKey(key))
            {
                ModifiedData[key].Clear();
            }
        }
        public void CommitChanges()
        {
            foreach (ConcurrentStack<ModifiedDataItem> ChangeSet in ModifiedData.Values)
            {
                ChangeSet.Clear();
                EditStack.Clear();
            }
        }
        public void UndoChange(string key)
        {
            if (ModifiedData.ContainsKey(key))
            {
                ModifiedDataItem GetLast = null;
                if (ModifiedData[key].TryPop(out GetLast))
                {
                    ObjectData[key] = GetLast.Value;
                    OnPropertyChanged(Binding.IndexerName);
                }
            }
        }
        public void Undo()
        {
            string LastEditKey = string.Empty;
            if (EditStack.TryPop(out LastEditKey))
            {
                UndoChange(LastEditKey);
            }
        }

        public void RevertChanges(string key)
        {
            foreach (ConcurrentStack<ModifiedDataItem> ChangeSets in ModifiedData.Values)
            {
                if (!ChangeSets.IsEmpty)
                {
                    ObjectData[key] = ChangeSets.FirstOrDefault().Value;
                    OnPropertyChanged(Binding.IndexerName);
                    ChangeSets.Clear();
                }
            }
        }
        public void Reset()
        {
            foreach (string key in ObjectData.Keys)
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
            if (ModifiedData.ContainsKey(key))
            {
                return ModifiedData[key].OrderBy(x => x.TimeStamp).ToArray();
            }
            return new ModifiedDataItem[] { };
        }

        public KeyValuePair<string, Tuple<dynamic, string>>[] ToArray()
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
            foreach (KeyValuePair<string, dynamic> Item in ObjValues)
            {
                ObjectData.AddOrUpdate(Item);
                ObjectType.AddOrUpdate(Item.Key, Item.Value.GetType());
                OnPropertyChanged(Binding.IndexerName);
            }
        }
        public void CastProps()
        {
            foreach (KeyValuePair<string, dynamic> Itm in ObjectData)
            {
                Type ValType = Type.GetType(ObjectType[Itm.Key]);
                if (ValType != null && !Itm.Value.GetType().Equals(ValType))
                {
                    ObjectData[Itm.Key] = GetValue(Itm.Key);
                }
            }
        }
        #endregion
        #region ICustomTypeDescriptor
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            var attributes = new Attribute[0];
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            var attributes = new Attribute[0];
            List<DynamicPropertyDescriptor> properties = new List<DynamicPropertyDescriptor>();
            
            properties.AddRange(ObjectData.Select(pair => new DynamicPropertyDescriptor(this, pair.Key, pair.Value.GetType(), attributes)));
            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<DynamicPropertyDescriptor> properties = new List<DynamicPropertyDescriptor>();           
            properties.AddRange(ObjectData.Select(pair => new DynamicPropertyDescriptor(this, pair.Key, pair.Value.GetType(), attributes)));
            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
        #region Dynamic Methods
        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    string name = binder.Name;

        //    //exclude Properties
        //    if (!_PropNames.Contains(binder.Name) && ObjectData.ContainsKey(name))
        //    {
        //        result = this[name];
        //        return true;
        //    }
        //    result = null;
        //    return false;

        //}
        //public override bool TrySetMember(SetMemberBinder binder, object value)
        //{
        //    //exclude Properties
        //    if (!_PropNames.Contains(binder.Name))
        //    {
        //        this[binder.Name] = value;
        //        return true;
        //    }
        //    return false;
            
        //}
        private void GetPropNames()
        {
            _PropNames = new string[this.GetType().GetProperties().Length];
            int currCnt = 0;
            foreach (PropertyInfo pinfo in this.GetType().GetProperties())
            {
                _PropNames[currCnt] = pinfo.Name;
                currCnt++;
            }
        }
        #endregion
        #region NotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            object value;
            if (_propertyBackingDictionary.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        protected bool SetPropertyValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            if (EqualityComparer<T>.Default.Equals(newValue, GetPropertyValue<T>(propertyName))) return false;

            _propertyBackingDictionary[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        #endregion
    }

    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        private readonly DynamicObjectBase DynamicObjectBaseInst;
        private readonly Type propertyType;

        public DynamicPropertyDescriptor(DynamicObjectBase _DynamicObjectBase,
            string propertyName, Type propertyType, Attribute[] propertyAttributes)
            : base(propertyName, propertyAttributes)
        {
            this.DynamicObjectBaseInst = _DynamicObjectBase;
            this.propertyType = propertyType;
        }

        public override bool CanResetValue(object component)
        {

            return true;
        }

        public override object GetValue(object component)
        {
            return DynamicObjectBaseInst[Name];
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            DynamicObjectBaseInst[Name] = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(DynamicObjectBase); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return propertyType; }

        }
    }
}
