using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public class DecimalValidator : AbstractValidator<DecimalValidationRules>,IValidator
    {

        public DecimalValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Default Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() == typeof(decimal)).Unless(x => x.Value == null).WithMessage(x => $"Default Value must be Type {typeof(decimal).Name}");
            RuleFor(x => x).Must(MinMaxCheck).WithMessage(x => $"{x.Min} must be less than or equal to {x.Max}");
            RuleFor(x => x).Must(RangeCheck).WithMessage(x => $"Default Value must be in range {x.Min} : {x.Max}");
            Rules = new DecimalValidationRules();
        }

       

        public IValidationRules Rules { get; set; }
        public DecimalValidationRules DecimalValidationRules
        {
            get
            {
                return (DecimalValidationRules)Rules;
            }
        }
        private bool RangeCheck(DecimalValidationRules CurrItem)
        {
            if (!CurrItem.Min.HasValue && !CurrItem.Max.HasValue || CurrItem.Value == null)
            {
                return true;
            }
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return ((decimal)CurrItem.Value) >= CurrItem.Min && ((decimal)CurrItem.Value) <= CurrItem.Max;
            }
            if (!CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return ((decimal)CurrItem.Value) <= CurrItem.Max;
            }
            if (CurrItem.Min.HasValue && !CurrItem.Max.HasValue)
            {
                return ((decimal)CurrItem.Value) >= CurrItem.Min;
            }
            return true;


        }
        private bool MinMaxCheck(DecimalValidationRules CurrItem)
        {
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return CurrItem.Min.Value <= CurrItem.Max.Value;
            }
            return true;
        }
        public ValidationResult Validate(object Val)
        {
            DecimalValidationRules.Value = Val;
            return base.Validate(DecimalValidationRules);
        }
    }
    public class DecimalValidationRules : BaseValidationRules
    {
        public decimal? Min {
            get
            {
                return GetPropertyValue<decimal?>();
            }
            set
            {
                SetPropertyValue<decimal?>(value);
            }
        }
        public decimal? Max {
            get
            {
                return GetPropertyValue<decimal?>();
            }
            set
            {
                SetPropertyValue<decimal?>(value);
            }
        }
    }
}
