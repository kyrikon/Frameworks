using Core.Helpers;
using System;


namespace CashFlowModels
{
    public class Simulation : NotifyPropertyChanged
    {
       
        public Timeline RunHorizon
        {
            get
            {
                return GetPropertyValue<Timeline>();
            }set
            {
                SetPropertyValue(value);
            }
        }
        

    }
}
