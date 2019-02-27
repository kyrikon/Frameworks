using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging
{
    public interface IAppLog
    {

        #region Properties

        ObservableCollection<AppLogItem> AppLogItems { get; set; }
        #endregion

    }

    public interface IAppLogItem
    {
        #region Properties
        int ID { get; set; }

        LogTypes LogType { get; set; }       
        string LogDescription { get; set; }

        string LogDetails { get; set; }

         DateTime LogTimeStamp { get; set; }
        #endregion

    }
    #region Enums

    public enum LogTypes
    {
        Warning
      , Error
      , Status
      ,Notifiction

    }
    #endregion
}
