using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging
{
    public class AppLog : IAppLog
    {
        #region Constructor
        public AppLog()
        {
            AppLogItems = new ObservableCollection<AppLogItem>();
        }

        #endregion

        #region Properties
        public ObservableCollection<AppLogItem> AppLogItems { get; set; } 
        #endregion

    }

    public class AppLogItem : IAppLogItem
    {
        #region Properties
        public int ID 
        {
            get; set;
        }
        public LogTypes LogType
        {
            get; set;
        }

        public string LogDescription
        {
            get; set;
        }
        public string LogDetails
        {
            get; set;
        }

        public DateTime LogTimeStamp
        {
            get;set;
        }
        #endregion

    }
}
