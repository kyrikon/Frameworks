using Core.Helpers;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Walkthrough
{
    sealed internal class MainWindowVM 
    {
        public IDemoModel DemoModel { get; private set; }
        public DelegateCommand ClickCmd
        {
            get;
            private set;
        }
        public MainWindowVM(IDemoModel Model)
        {
            DemoModel = Model;
            ClickCmd = new DelegateCommand(delegate (object o)
            {
                DemoModel.IntValue++;
                DemoModel.BoolValue = !DemoModel.BoolValue;

            });
        }
       
    }
}
