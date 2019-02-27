using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Core.Helpers;
using Core.Models;

namespace UI.WPF.Singletons
{

    public sealed class GlobalSettings : NotifyPropertyChanged
    {
        private static readonly Lazy<GlobalSettings> lazy = new Lazy<GlobalSettings>(() => new GlobalSettings());
        private GlobalSettings()
        {

        }

        public static GlobalSettings Instance
        {
            get
            {
                return lazy.Value;
            }

        }


        public User AuthUser
        {
            get
            {
                return GetPropertyValue<User>();
            }
            set
            {
                SetPropertyValue<User>(value);
            }
        }
        public MainWindowVM ShellContext
        {
            get
            {
                return GetPropertyValue<MainWindowVM>();
            }
            set
            {
                SetPropertyValue<MainWindowVM>(value);
            }

        }


    }
}
