using DevExpress.Xpf.Editors;
using Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.WPF.Singletons
{
    public sealed class GlobalLogging
    {
        private static readonly Lazy<GlobalLogging> lazy = new Lazy<GlobalLogging>(() => new GlobalLogging());

        private double _ProgressVal;
        private double _MaxProgress;
        private BaseProgressBarStyleSettings _BarStyle;
        private bool _ShowProgress;

        public delegate void ProgressValChangedEventHandler(object sender, EventArgs args);
        public event ProgressValChangedEventHandler ProgressValChanged;

        public delegate void MaxProgressValChangedEventHandler(object sender, EventArgs args);
        public event MaxProgressValChangedEventHandler MaxProgressValChanged;

        public delegate void BarStyleChangedEventHandler(object sender, EventArgs args);
        public event BarStyleChangedEventHandler BarStyleChanged;

        public delegate void ShowProgressEventHandler(object sender, EventArgs args);
        public event ShowProgressEventHandler ShowProgressChanged;

        private GlobalLogging()
        {
            AppLog = new AppLog();
        }

        public static GlobalLogging Instance
        {
            get
            {
                return lazy.Value;
            }
            
        }
        public AppLog AppLog { get; set; }

        public static void AddLog(LogTypes LType, string Description,string Details = "")
        {
            AppLogItem Log = new AppLogItem() { ID = Instance.AppLog.AppLogItems.Count + 1, LogDescription = Description, LogDetails = Details, LogTimeStamp = DateTime.Now, LogType = LType };
            Instance.AppLog.AppLogItems.Add(Log);
        }

        public double ProgressVal
        {
            get
            {
                return _ProgressVal;
            }
            set
            {
                _ProgressVal = value;
                OnProgressChanged(new EventArgs());
            }

        }
        public double MaxProgress
        {
            get
            {
                return _MaxProgress;
            }
            set
            {
                _MaxProgress = value;
                OnMaxProgressChanged(new EventArgs());

            }
        }
        public bool ShowProgress
        {
            get
            {
                return _ShowProgress;
            }
            set
            {
                _ShowProgress = value;
                OnShowProgressChanged(new EventArgs());
            }
        }
        
        public BaseProgressBarStyleSettings BarStyleSetting
        {
            get
            {
                return _BarStyle;
            }
            set
            {
                _BarStyle = value;
                OnBarStylesChanged(new EventArgs());
            }
        }
        protected void OnMaxProgressChanged(EventArgs Args)
        {
            MaxProgressValChanged?.Invoke(this, Args);
        }
        protected void OnProgressChanged(EventArgs Args)
        {
            ProgressValChanged?.Invoke(this, Args);
        }
        protected void OnBarStylesChanged(EventArgs Args)
        {
            BarStyleChanged?.Invoke(this, Args);
        }
        protected void OnShowProgressChanged(EventArgs Args)
        {
            ShowProgressChanged?.Invoke(this, Args);
        }
        
    }
}
