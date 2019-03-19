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
            Validator = new DynamicObjectValidator();
            IsNullable = true;
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
                ValidationResult VR = Validator.Validate(new ValidationRuleCheck() { Value = value,ValueType = ValueGetType,Nullable = IsNullable });
                Console.WriteLine($"Validation result {VR.IsValid} - {VR.ToString(":")}");
                if (VR.IsValid)
                {
                    SetPropertyValue(value);
                }
            }
        }
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
        public DynamicObjectValidator Validator
        {
            get; private set;
        }
    }
    public class DynamicObjectValidator : AbstractValidator<ValidationRuleCheck>
    {
        public DynamicObjectValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Default Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() == x.ValueType).Unless(x => x.Value == null).WithMessage(x => $"Default Value must be Type {x.ValueType.Name}");
           
        }
    }
    public struct ValidationRuleCheck
    {
        public object Value { get;  set; }
        public Type ValueType { get;  set; }
        public bool Nullable { get;  set; }

    }
}
