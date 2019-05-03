using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public class StrValidator : AbstractValidator<StrValidationRules>,IValidator
    {
        private IValidationRules _Rules;
        public StrValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
           base.
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() ==typeof(string)).Unless(x => x.Value == null).WithMessage(x => $"Value must be Type {typeof(string).Name}");
            RuleFor(x => x).Must(MinMaxLengthCheck).WithMessage(x => $"Min length must be less than or equal to max length");
            RuleFor(x => x).Must(LengthCheck).WithMessage(x => $"Value length between  {x.MinLength} to {x.MaxLength} characters");           
            RuleFor(x => (string)x.Value).Matches(x => x.RegExpPattern).When(x => !string.IsNullOrEmpty(x.RegExpPattern)).WithMessage(x => $"Value {x.Value} Doesnt match Pattern [{x.RegExpPattern}]");
            Rules = new StrValidationRules();
        }

        public IValidationRules Rules
        {
            get
            {
                return _Rules;
            }
            set
            {
                if (value.GetType() == typeof(StrValidationRules))
                {
                    _Rules = value;
                }
                else
                {
                    throw new InvalidOperationException("Can Only Allocate StrValidationRules");
                }
            }
        }
        public StrValidationRules StrRules
        {
            get
            {
                return (StrValidationRules)Rules;
            }
        }       

        private bool LengthCheck(StrValidationRules CurrItem)
        {
            try
            {
                if (!CurrItem.MinLength.HasValue && !CurrItem.MaxLength.HasValue || CurrItem.Value == null)
                {
                    return true;
                }
                if (CurrItem.MinLength.HasValue && CurrItem.MaxLength.HasValue)
                {
                    return ((string)CurrItem.Value).Length >= CurrItem.MinLength && ((string)CurrItem.Value).Length <= CurrItem.MaxLength;
                }
                if (!CurrItem.MinLength.HasValue && CurrItem.MaxLength.HasValue)
                {
                    return ((string)CurrItem.Value).Length <= CurrItem.MaxLength;
                }
                if (CurrItem.MinLength.HasValue && !CurrItem.MaxLength.HasValue)
                {
                    return ((string)CurrItem.Value).Length >= CurrItem.MinLength;
                }
            }
            catch (InvalidCastException)
            {

            }
            return true;
            

        }
        private bool MinMaxLengthCheck(StrValidationRules CurrItem)
        {
            try
            {
               
                if (CurrItem.MinLength.HasValue && CurrItem.MaxLength.HasValue)
                {
                    return CurrItem.MinLength.Value <= CurrItem.MaxLength.Value;
                }
                return true;
            }
            catch (InvalidCastException)
            {

            }
            return true;


        }
        public  ValidationResult Validate(object Val)
        {
            StrRules.Value = Val;
            return base.Validate(StrRules);
        }
        public ValidationResult Validate()
        {
            return base.Validate(StrRules);
        }
    }
    public class StrValidationRules : BaseValidationRules
    {
        public StrValidationRules()
        {
            Nullable = true;
        }
        public int? MinLength
        {
            get
            {
                return GetPropertyValue<int?>();
            }
            set
            {
                SetPropertyValue<int?>(value);
            }
        }
        public int? MaxLength
        {
            get
            {
                return GetPropertyValue<int?>();
            }
            set
            {
                SetPropertyValue<int?>(value);
            }
        }
        public string RegExpPattern
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
        public override object ResetDefault()
        {

            return string.Empty;
        }
    }
}
