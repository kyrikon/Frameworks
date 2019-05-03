using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public abstract class BaseValidationRules : Core.Helpers.NotifyPropertyChanged, IValidationRules
    {

        [JsonIgnore]
        public  object Value
        {
            get
            {
                return GetPropertyValue<object>();
            }
            set
            {
                SetPropertyValue<object>(value);
            }
        }        
        public  bool Nullable
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                SetPropertyValue<bool>(value);
            }
        }

        public virtual object ResetDefault()
        {
            return new NullValRef();
        }
    }
}
