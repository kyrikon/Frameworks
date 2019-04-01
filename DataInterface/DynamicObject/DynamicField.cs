using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

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
        //TODO Implement
        public const string Double = "Double";
        public const string Long = "Long";
        public const string Float = "Float";


        public ValueTypes()
        {
            this[Text] = new ValueType() { AssemblyTypeName = typeof(string).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Text | ValueFlags.Primative), Validator = new StrValidator() };
            this[Int] = new ValueType() { AssemblyTypeName = typeof(int).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Number | ValueFlags.Primative), Validator = new IntValidator() };
            this[Date] = new ValueType() { AssemblyTypeName = typeof(DateTime).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.DateTime), Validator = new DateValidator() };
            this[Decimal] = new ValueType() { AssemblyTypeName = typeof(decimal).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Number | ValueFlags.Primative), Validator = new DecimalValidator() };
            this[List] = new ValueType() { AssemblyTypeName = typeof(DataInterface.CustomList).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.List) };
            this[Boolean] = new ValueType() { AssemblyTypeName = typeof(bool).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Primative) };
            this[TimePeriod] = new ValueType() { AssemblyTypeName = typeof(DataInterface.TimePeriod).AssemblyQualifiedName, Nullable = true, Flags = (ValueFlags.Enum) };

        }
    }
    public class ValueType
    {

        public bool Nullable { get; set; }
        public ValueFlags Flags { get; set; }
        public string AssemblyTypeName { get; set; }
        public IValidator Validator { get; set; }
    }

    [Flags]
    public enum ValueFlags
    {
        Primative = 0x0,
        Text = 0x1,
        Number = 0x2,
        DateTime = 0x4,
        List = 0x8,
        Enum = 0x16

    } 
    #endregion

    #region Field Template
    public class FieldTemplate : Core.Helpers.NotifyPropertyChanged
    {
        public FieldTemplate(ValueType _ValType, bool _IsNullable = true, bool _AllowValidation = true)
        {
            IsNullable = _IsNullable;
            AllowValidation = _AllowValidation;
            ValueType = _ValType;
        }
        
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
                    VResult = Validator.Validate(value);
                    Console.WriteLine($"Value {value}");
                    Console.WriteLine(ValidationErrors);
                    OnPropertyChanged("ValidationErrors");
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
        public string ValidationErrors
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Valid {VResult.IsValid}");
                if (!VResult.IsValid)
                {
                    sb.Append($"- {VResult.ToString(":")}");
                }
                return sb.ToString();
            }

        }
        private ValidationResult VResult { get; set; }
        public bool IsNullable
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
        public IValidator Validator
        {
            get
            {
                return ValueType.Validator;
            }
        }


    } 
    #endregion
}
