using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Helpers;

namespace UI.UWP
{
    public class mainVM : NotifyPropertyChanged
    {
        public mainVM()
        {
            AString = "hello";
            Cmd = new DelegateCommand(ACmd);
        }

        private void ACmd(object obj)
        {
            throw null;
        }



        public string AString
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
        public DelegateCommand Cmd
        {
            get; private set;

        }
    }
}
