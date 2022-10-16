using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Walkthrough
{
    interface IDemoModel
    {
        string StringValue { get; set; }
        int IntValue { get; set; }
        bool BoolValue { get; set; }
    }
    internal sealed class DemoModel : NotifyPropertyChanged, IDemoModel
    {
        #region CTor
        public DemoModel()
        {
            StringValue = "A String";
            IntValue = 0;
            BoolValue = false;
        }
        #endregion
        #region Properties
        public string StringValue
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
        public int IntValue
        {
            get
            {
                return GetPropertyValue<int>();
            }
            set
            {
                SetPropertyValue<int>(value);
            }
        }
        public bool BoolValue
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
        #endregion
    }
}
