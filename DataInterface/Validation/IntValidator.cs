using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public class IntValidator : AbstractValidator<IntValidationRules>,IValidator
    {
        private IValidationRules _Rules;
        public IntValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().When(x => !x.Nullable).WithMessage(x => $"Value must not be null");
            RuleFor(x => x).Must(x => x.Value?.GetType() == typeof(int)).Unless(x => x.Value == null).WithMessage(x => $"Value must be Type {typeof(int).Name}");
            RuleFor(x => x).Must(MinMaxCheck).WithMessage(x => $"Min({x.Min}) must be less than or equal to Max({x.Max})");
            RuleFor(x => x).Must(RangeCheck).WithMessage(x => $"Value must be in range {x.Min} : {x.Max}");
            Rules = new IntValidationRules();
        }

        public IValidationRules Rules
        {
            get
            {
                return _Rules;
            }
            set
            {
                if(value.GetType() == typeof(IntValidationRules))
                {
                    _Rules = value;
                    if (_Rules.Value != null)
                    {
                        _Rules.Value = Convert.ToInt32(_Rules.Value);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Can Only Allocate IntValidationRules");
                }
            }
        }
        public IntValidationRules IntRules
        {
            get
            {
                return (IntValidationRules)Rules;
            }
        }
        private bool RangeCheck(IntValidationRules CurrItem)
        {
            int CurrItemCheck = Convert.ToInt32(CurrItem.Value);
            if (!CurrItem.Min.HasValue && !CurrItem.Max.HasValue || CurrItem.Value == null)
            {
                return true;
            }
            if (CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return CurrItemCheck >= CurrItem.Min && CurrItemCheck <= CurrItem.Max;
            }
            if (!CurrItem.Min.HasValue && CurrItem.Max.HasValue)
            {
                return CurrItemCheck <= CurrItem.Max;
            }
            if (CurrItem.Min.HasValue && !CurrItem.Max.HasValue)
            {
                return CurrItemCheck >= CurrItem.Min;
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
        public ValidationResult Validate()
        {
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

        public override object ResetDefault()
        {
            if (this.Nullable)
            {
                return null;
            }
            if (Min.HasValue)
            {
                return Min.Value;
            }
            if (Max.HasValue)
            {
                return Max.Value;
            }
            return 0;
        }
    }
}
