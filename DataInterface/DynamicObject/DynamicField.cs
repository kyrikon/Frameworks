using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Core.Helpers;
using DataInterface.Extensions;
using System.ComponentModel;

namespace DataInterface
{
    public class DynamicField : Core.Helpers.NotifyPropertyChanged
    {
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
        public FieldTemplate FieldTemplate
        {
            get
            {
                return GetPropertyValue<FieldTemplate>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        [JsonIgnore]
        public string ValidationText
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
        //this is an alt name used for column headers or labels
        public string LabelText
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

        public int Rank
        {
            get
            {
                return GetPropertyValue<int>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }

        public bool Enabled
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
    }

    #region Field Value Types
    public class ValueTypes : ObservableConcurrentDictionary<string, ValueType>
    {
        public const string Int = "Int";
        public const string Text = "Text";
        public const string Date = "Date";
        public const string Decimal = "Decimal";
        public const string List = "List";
        public const string Boolean = "Bool";
        public const string TimePeriod = "TimePeriod";
        //TODO Implement below 
        public const string Double = "Double";
        public const string Long = "Long";
        public const string Float = "Float";


        public ValueTypes()
        {
            this[Text] = new ValueType() { Name = Text, AssemblyTypeName = typeof(string).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Text | ValueFlags.Primative), Validator = new StrValidator() };
            this[Int] = new ValueType() { Name = Int, AssemblyTypeName = typeof(int).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Number | ValueFlags.Primative), Validator = new IntValidator() };
            this[Date] = new ValueType() { Name = Date, AssemblyTypeName = typeof(DateTime).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.DateTime), Validator = new DateValidator() };
            this[Decimal] = new ValueType() { Name = Decimal, AssemblyTypeName = typeof(decimal).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Number | ValueFlags.Primative), Validator = new DecimalValidator() };
            this[List] = new ValueType() { Name = List, AssemblyTypeName = typeof(DataInterface.CustomList).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.List) };
            this[Boolean] = new ValueType() { Name = Boolean, AssemblyTypeName = typeof(bool).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Primative) };
            this[TimePeriod] = new ValueType() { Name = TimePeriod, AssemblyTypeName = typeof(DataInterface.TimePeriod).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Enum) };

        }

        public string GetValueType(object Lookup)
        {
            return this.FirstOrDefault(x => x.Value.AssemblyTypeName.Equals(Lookup.GetType().AssemblyQualifiedName)).Key;
        }
   

        public List<ValueType> Primatives
        {
            get
            {
                return this.ItemList.Select(x => x.Value).Where(x => x.Flags.HasFlag(ValueFlags.Primative)).ToList();
            }
        }
        public List<ValueType> All
        {
            get
            {
                return this.ItemList.Select(x => x.Value).ToList();
            }
        }

    }
    public class ValueType
    {
        public string Name { get; set; }
        public bool Nullable { get; set; }
        public ValueFlags Flags { get; set; }
        public string AssemblyTypeName { get; set; }

        [JsonIgnore]
        public IValidator Validator { get; set; }
    }

    [Flags]
    public enum ValueFlags
    {
        Primative = 0x1,
        Text = 0x2,
        Number = 0x4,
        DateTime = 0x8,
        List = 0x16,
        Enum = 0x32

    } 
    #endregion

    #region Field Template
    public class FieldTemplate : Core.Helpers.NotifyPropertyChanged
    {
        #region Constructors
        public FieldTemplate()
        {
            ValidateCmd = new DelegateCommand((x) => Validate());
        }
        public FieldTemplate(ValueType _ValType, bool _AllowValidation = true)
        {
            AllowValidation = _AllowValidation;
            ValueType = _ValType;
            if (ValidatonRules == null)
            {
                ValidatonRules = ValueType.Validator.Rules;
            }
            ValidateCmd = new DelegateCommand((x) => Validate());
        }
        #endregion

        [JsonIgnore]
        public DelegateCommand ValidateCmd
        {
            get;private set;
        }
        #region Properties
        [JsonProperty]
        public ValueType ValueType
        {
            get
            {

                return GetPropertyValue<ValueType>();
            }
            private set
            {
                SetPropertyValue(value);
            }
        }
        public string FormatString
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
        [JsonProperty]
        public object DefaultValue
        {
            get
            {
                return GetPropertyValue<object>();
            }
            set
            {
                if (AllowValidation)
                {
                    if (ValueType.Validator == null)
                    {
                        ValueTypes VT = new ValueTypes();
                        ValueType.Validator = VT[ValueType.Name].Validator;
                    }
                    if (ValidatonRules == null)
                    {
                        ValidatonRules = ValueType.Validator.Rules;
                    }
                    ValidatonRules.Value = value;
                    Validate();
                    Console.WriteLine($"Value {value}");
                    Console.WriteLine(ValidationErrors);
                    if (VResult.IsValid)
                    {
                        SetPropertyValue(value);
                    }
                }
                else
                {
                    SetPropertyValue(value);
                }
            }
        }
        [JsonIgnore]
        public string ValidationErrors
        {
            get
            {
                if(VResult != null)
                {
                 return   VResult.ValidationErrorText();
                }
                return string.Empty;
            }

        }
        [JsonIgnore]
        private ValidationResult VResult { get; set; }
        public bool AllowValidation
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        [JsonProperty]
        public IValidationRules ValidatonRules
        {
            get
            {
                return GetPropertyValue<IValidationRules>();
            }
            private set
            {
                if(ValidatonRules != null)
                {
                    ((BaseValidationRules)ValidatonRules).PropertyChanged -= ValidationPropertyChanged;
                }
                SetPropertyValue(value);
                if (ValidatonRules != null)
                {
                    ((BaseValidationRules)ValidatonRules).PropertyChanged +=   ValidationPropertyChanged;
                }
            }
        }

       
        #endregion
        #region Methods
        private void Validate()
        {
            if (ValueType.Validator == null)
            {
                ValueTypes VT = new ValueTypes();
                ValueType.Validator = VT[ValueType.Name].Validator;
            }
            if (ValidatonRules == null)
            {
                ValidatonRules = ValueType.Validator.Rules;
            }
            ValueType.Validator.Rules = ValidatonRules;
            VResult = ValueType.Validator.Validate();
            OnPropertyChanged("ValidationErrors");

        }

        #endregion

        #region Callbacks
        private void ValidationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != "Value") { 
            if(ValidatonRules.Value != DefaultValue)
            {
                ValidatonRules.Value = DefaultValue;
            }
            
            Validate();
                if (!VResult.IsValid)
                {
                    DefaultValue = ValidatonRules.ResetDefault();
                }
            }
        } 
        #endregion
    } 
    #endregion
}
