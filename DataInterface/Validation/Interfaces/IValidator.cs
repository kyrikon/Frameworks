using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation;

namespace DataInterface
{
    public interface IValidator
    {

        #region Properties       
        IValidationRules Rules { get; set; }
        #endregion
        #region Methods
        ValidationResult Validate(object Val);
        ValidationResult Validate();
        #endregion
    }
    public interface IValidationRules
    {
        #region Properties       
        object Value { get; set; }
        bool Nullable { get; set; }
        #endregion

        object ResetDefault();
    }

}
