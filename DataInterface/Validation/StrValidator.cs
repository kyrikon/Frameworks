using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public class StrValidator : AbstractValidator<StrValidationRules>,IValidator
    {

        public StrValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Default Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() ==typeof(string)).Unless(x => x.Value == null).WithMessage(x => $"Default Value must be Type {typeof(string).Name}");
            RuleFor(x => x).Must(LengthCheck).WithMessage(x => $"Default Value length be  {x.MinLength} to {x.MaxLength} characters");
            RuleFor(x => (string)x.Value).Matches(x => x.RegExpPattern).When(x => !string.IsNullOrEmpty(x.RegExpPattern)).WithMessage(x => $"Default Value {x.Value} Doesnt match Pattern [{x.RegExpPattern}]");
            Rules = new StrValidationRules();
        }

        public IValidationRules Rules { get; set; }
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
        public  ValidationResult Validate(object Val)
        {
            StrRules.Value = Val;
            return base.Validate(StrRules);
        }
    }
    public class StrValidationRules : BaseValidationRules
    {
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
    }
}
