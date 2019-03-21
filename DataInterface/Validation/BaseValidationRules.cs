using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{
    public abstract class BaseValidationRules : IValidationRules
    {
        public  object Value { get; set; }        
        public  bool Nullable { get; set; }
    }
}
