using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace DataInterface
{
    public class DynamicObjectTemplate : Core.Helpers.NotifyPropertyChanged
    {
        public DynamicObjectTemplate()
        {
            IsNullable = true;
            SetValidator(ValueType.Text);
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
        public ValueType ValueType
        {
            get
            {
                return GetPropertyValue<ValueType>();
            }
            set
            {
                SetPropertyValue(value);
                SetValidator(value);
            }
        }
        public Type ValueGetType
        {
            get
            {
                switch (ValueType)
                {
                    case (ValueType.Date):
                        return typeof(DateTime);
                    case (ValueType.DateTime):
                        return typeof(DateTime);
                    case (ValueType.Decimal):
                        return typeof(decimal);
                    case (ValueType.Enum):
                        return typeof(Enum);
                    case (ValueType.Integer):
                        return typeof(int);
                    case (ValueType.Text):
                        return typeof(string);
                    default:
                        return typeof(string);
                }
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
                //new ValidationRuleCheck() { Value = value, ValueType = ValueGetType, Nullable = IsNullable, HasRange = ValueType == ValueType.Integer, Range = Range }
                VResult = Validator.Validate(value);               
                Console.WriteLine(ValidationErrors);
                OnPropertyChanged("ValidationErrors");
                if (VResult.IsValid)
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
                sb.Append(VResult.IsValid);
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
                if (ValueGetType.IsValueType)
                {
                    return false;
                }
                return GetPropertyValue<bool>();

            }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string Range
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
        public IValidator Validator
        {
            get; private set;
        }

        private void SetValidator(ValueType VT)
        {
            switch (VT)
            {
                case ValueType.Integer:
                    Validator = new IntValidator();
                    break;
                default:
                    Validator = new StrValidator();
                    break;
            }
        }
    }
   
}
