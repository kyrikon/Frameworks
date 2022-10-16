using Core.Helpers;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Walkthrough
{
    sealed internal class MainWindowVM : NotifyPropertyChanged
    {
        public string SomeText
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
        public int CountClicks
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
        public bool IsTBEnabled
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
        public DelegateCommand ClickCmd
        {
            get;
            private set;
        }
        public MainWindowVM(IDemoModel Model)
        {
            SomeText = Model.StringValue;
            CountClicks = Model.IntValue;
            IsTBEnabled = Model.BoolValue;
            ClickCmd = new DelegateCommand(delegate (object o)
            {
                CountClicks++;
                IsTBEnabled = !IsTBEnabled;

            });
        }
       
    }
}
