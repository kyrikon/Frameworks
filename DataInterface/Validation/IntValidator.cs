using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public class IntValidator : AbstractValidator<IntValidationRules>,IValidator
    {

        public IntValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Default Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() == typeof(int)).Unless(x => x.Value == null).WithMessage(x => $"Default Value must be Type {typeof(int).Name}");
            RuleFor(x => x).Must(MinMaxCheck).WithMessage(x => $"{x.Min} must be less than or equal to {x.Max}");
            RuleFor(x => x).Must(RangeCheck).WithMessage(x => $"Default Value must be in range {x.Min} : {x.Max}");
            Rules = new IntValidationRules();
        }

        public IValidationRules Rules { get; set; }
        public IntValidationRules IntRules
        {
            get
            {
                return (IntValidationRules)Rules;
            }
        }
        private bool RangeCheck(IntValidationRules CurrItem)
        {
            if (!CurrItem.Min.HasValue && !CurrItem.Max.HasValue || CurrItem.Value == null)
            {
                return true;
            }
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return ((int)CurrItem.Value) >= CurrItem.Min && ((int)CurrItem.Value) <= CurrItem.Max;
            }
            if (!CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return ((int)CurrItem.Value) <= CurrItem.Max;
            }
            if (CurrItem.Min.HasValue && !CurrItem.Max.HasValue)
            {
                return ((int)CurrItem.Value) >= CurrItem.Min;
            }
            return true;


        }
        private bool MinMaxCheck(IntValidationRules CurrItem)
        {
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return CurrItem.Min.Value <= CurrItem.Max.Value;
            }
            return true;
        }
        public ValidationResult Validate(object Val)
        {
            IntRules.Value = Val;
            return base.Validate(IntRules);
        }
    }
    public class IntValidationRules : BaseValidationRules
    {
        public int? Min {
            get
            {
                return GetPropertyValue<int?>();
            }
            set
            {
                SetPropertyValue<int?>(value);
            }
        }
        public int? Max {
            get
            {
                return GetPropertyValue<int?>();
            }
            set
            {
                SetPropertyValue<int?>(value);
            }
        }
    }
}
