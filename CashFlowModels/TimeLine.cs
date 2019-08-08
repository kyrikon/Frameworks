using Core.Helpers;
using System;


namespace CashFlowModels
{
    public class Timeline : NotifyPropertyChanged
    {
       
        public DateTime StartDate
        {
            get
            {
                return GetPropertyValue<DateTime>();
            }set
            {
                SetPropertyValue(value);
            }
        }
        public DateTime EndDate
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
