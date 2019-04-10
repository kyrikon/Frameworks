using Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Core.Extensions;

namespace DataInterface
{
    [Serializable]
    public abstract class DynamicObjectBase : INotifyPropertyChanged, IDynamicObject, ICustomTypeDescriptor
    {
        // Considerations for base class
        // 1. Object nature
        // 2. (De) Serialization - parameterless constructor      
        // 3. Edit Auditing
        // 4. Syncronization with backing data store (multi user)
        #region Fields 
        private string[] _PropNames;
        private bool _FirePropChange = true;
        object _lockObj = new object();
        public const string IndexerName = "Item[]";
        #endregion

        #region Constructors
        protected DynamicObjectBase(bool _Transactional = true)
        {
            ObjectData = new DynamicValuesDictionary();
            ObjectType = new DynamicTypeDictionary();
            ModifiedData = new ObservableConcurrentDictionary<string, ConcurrentStack<ModifiedDataItem>>();
            EditStack = new ConcurrentStack<string>();
            IsTransactional = _Transactional;
        }
        protected DynamicObjectBase(KeyValuePair<string, Object>[] InitArray, bool _Transactional = true)
        {
            ObjectData = new DynamicValuesDictionary();
            ObjectType = new DynamicTypeDictionary();
            ModifiedData = new ObservableConcurrentDictionary<string, ConcurrentStack<ModifiedDataItem>>();
            EditStack = new ConcurrentStack<string>();
            IsTransactional = _Transactional;
            this.FromArray(InitArray);
        }
        #endregion
        #region Properties
        public Object this[string key]
        {
            get
            {
                Object tmpItem;
                ObjectData.TryGetValue(key, out tmpItem);
                return tmpItem;
            }
            set
            {

                double TimeStamp = DateTime.Now.ToOADate();
                SetModified(key, TimeStamp);
                ObjectData[key] =  value;
                ObjectType[key] = value.GetType().AssemblyQualifiedName;
                OnPropertyChanged(IndexerName);
                OnPropertyChanged(key);
                OnPropertyChanged("Properties");
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
        protected DynamicValuesDictionary ObjectData { get; }
        [JsonProperty]
        protected DynamicTypeDictionary ObjectType { get; }
        [JsonIgnore]
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

        [JsonProperty]
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

        [JsonIgnore]
        public ObservableCollection<PropertyItem> Properties
        {
            get
            {
                ObservableCollection<PropertyItem> PList = new ObservableCollection<PropertyItem>();
                foreach (KeyValuePair<string, object> items in ObjectData.ToArray().OrderBy(x => x.Key))
                {
                    PropertyItem PI = new PropertyItem() { Name = items.Key, Value = items.Value };
                    PI.PropertyChanged += PI_PropertyChanged;
                    PList.Add(PI);
                }
                return PList;
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
        public Object GetValue(string key)
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
                    OnPropertyChanged(IndexerName);
                    OnPropertyChanged(key);
                    OnPropertyChanged("Properties");
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
                    OnPropertyChanged(IndexerName);
                    OnPropertyChanged(key);
                    OnPropertyChanged("Properties");
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

        public KeyValuePair<string, Tuple<object, string>>[] ToArray()
        {
            KeyValuePair<string, Tuple<Object, string>>[] Tmp = new KeyValuePair<string, Tuple<Object, string>>[ObjectData.Count];
            int lineCnt = 0;
            foreach (var line in ObjectData)
            {
                Tmp[lineCnt] = new KeyValuePair<string, Tuple<Object, string>>(line.Key, new Tuple<Object, string>(line.Value, line.GetType().AssemblyQualifiedName));
                lineCnt++;
            }
            return Tmp;
        }
        public void FromArray(KeyValuePair<string, object>[] ObjValues)
        {
            foreach (KeyValuePair<string, object> Item in ObjValues)
            {
                ObjectData[Item.Key] = Item.Value;
                ObjectType[Item.Key] = Item.Value.GetType().AssemblyQualifiedName;
                OnPropertyChanged(IndexerName);
                OnPropertyChanged(Item.Key);
                OnPropertyChanged("Properties");
            }
        }
        public void CastProps()
        {
            foreach (KeyValuePair<string, object> Itm in ObjectData)
            {
                Type ValType = Type.GetType(ObjectType[Itm.Key]);
                if (ValType != null && !Itm.Value.GetType().Equals(ValType))
                {
                    ObjectData[Itm.Key] = GetValue(Itm.Key);
                }
            }
        }
        #endregion

        #region CallBacks
        private void PI_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            lock (_lockObj)
            {
                if (e.PropertyName.Equals("Value", StringComparison.InvariantCultureIgnoreCase) && _FirePropChange)
                {
                    Stopwatch _SW1 = Stopwatch.StartNew();
                    PropertyItem itm = (PropertyItem)sender;

                    string tmpItemType;
                    ObjectType.TryGetValue(itm.Name, out tmpItemType);

                    try
                    {
                        Type CvrtType = Type.GetType(tmpItemType);
                        if (CvrtType == null || CvrtType == itm.Value.GetType())
                        {
                            this[itm.Name] = itm.Value;
                        }

                        if (CvrtType.IsEnum)
                        {
                            this[itm.Name] = Enum.ToObject(CvrtType, itm.Value);
                        }
                        this[itm.Name] = Convert.ChangeType(itm.Value, CvrtType);
                        _FirePropChange = true;
                    }
                    catch (InvalidCastException)
                    {
                        itm.Value = this[itm.Name];
                        _FirePropChange = false;
                    }
                    catch (FormatException)
                    {
                        itm.Value = this[itm.Name];
                        _FirePropChange = false;
                    }
                    _SW1.Stop();
                    Console.WriteLine($"Conversion Took {_SW1.Elapsed.TotalSeconds} seconds");
                }
                else
                {
                    _FirePropChange = true;
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
    public class PropertyItem : NotifyPropertyChanged
    {
        
        public PropertyItem()
        {
           
        }
        public string Name
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
        public object Value
        {
            get
            {
                return GetPropertyValue<object>();
            }
            set
            {
                SetPropertyValue<object>(value);
            }
        }
        public string AssemblyType
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
        //TODO change this to FieldTemplate
        public ValueType ValType
        {
            get
            {
                return GetPropertyValue<ValueType>();
            }
             set
            {
                SetPropertyValue<ValueType>(value);
            }

        }
        public string EditorMask
        {
            get
            {
                return GetPropertyValue<string>();
            }
            private set
            {
                SetPropertyValue<string>(value);
            }
        }
        public object DefaultValue
        {
            get
            {
                return GetPropertyValue<object>();
            }
            private set
            {
                SetPropertyValue<object>(value);
            }
        }
    }
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
            catch (InvalidCastException)
            {
                return (T)Convert.ChangeType(Value, typeof(T));
            }
        }
    }
    public class DynamicObject: DynamicObjectBase
    {

    }
}
