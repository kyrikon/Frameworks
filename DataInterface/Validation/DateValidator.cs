using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public class DateValidator : AbstractValidator<DateValidationRules>,IValidator
    {

        public DateValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Default Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() == typeof(DateTime)).Unless(x => x.Value == null).WithMessage(x => $"Default Value must be Type {typeof(DateTime).Name}");
            RuleFor(x => x).Must(MinMaxCheck).WithMessage(x => $"{x.Min} must be less than or equal to {x.Max}");
            RuleFor(x => x).Must(RangeCheck).WithMessage(x => $"Default Value must be in range {x.Min} : {x.Max}");
            Rules = new DateValidationRules();
        }

        public IValidationRules Rules { get; set; }
        public DateValidationRules DateValidationRules
        {
            get
            {
                return (DateValidationRules)Rules;
            }
        }
        private bool RangeCheck(DateValidationRules CurrItem)
        {
            if (!CurrItem.Min.HasValue && !CurrItem.Max.HasValue || CurrItem.Value == null)
            {
                return true;
            }
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return ((DateTime)CurrItem.Value) >= CurrItem.Min && ((DateTime)CurrItem.Value) <= CurrItem.Max;
            }
            if (!CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return ((DateTime)CurrItem.Value) <= CurrItem.Max;
            }
            if (CurrItem.Min.HasValue && !CurrItem.Max.HasValue)
            {
                return ((DateTime)CurrItem.Value) >= CurrItem.Min;
            }
            return true;


        }
        private bool MinMaxCheck(DateValidationRules CurrItem)
        {
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return CurrItem.Min.Value <= CurrItem.Max.Value;
            }
            return true;
        }
        public ValidationResult Validate(object Val)
        {
            DateValidationRules.Value = Val;
            return base.Validate(DateValidationRules);
        }
    }
    public class DateValidationRules : BaseValidationRules
    {
        public DateTime? Min {
            get
            {
                return GetPropertyValue<DateTime?>();
            }
            set
            {
                SetPropertyValue<DateTime?>(value);
            }
        }
        public DateTime? Max {
            get
            {
                return GetPropertyValue<DateTime?>();
            }
            set
            {
                SetPropertyValue<DateTime?>(value);
            }
        }
    }
}
