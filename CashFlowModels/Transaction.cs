using Core.Helpers;
using System;

namespace CashFlowModels
{
    public class Transaction : NotifyPropertyChanged
    {    
        public DateTime StartDate
        { 
            get
            {
                return GetPropertyValue<DateTime>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        
    }
}
