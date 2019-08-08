using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashSimModels.Scheduling
{
    public class Schedule
    {
        #region Events | Delegates            
        #endregion
        #region Fields           
        #endregion
        #region Constructors
        public Schedule()
        {          
            FrequencyUnits = 1;
            IncludeDays = Days.All;
            NumOccurences = -1;
        }
        #endregion
        #region Properties  
        public FrequencyType Frequency
        {
            get;
            set;
        }
        public int FrequencyUnits
        {
            get;
            set;
        }
        public DateTime FirstDate
        {
            get;set;
        }
        public DateTime? LastDate
        {
            get; set;
        }
        public Days IncludeDays
        {
            get;set;
        }
        public int NumOccurences
        {
            get;set;
        }

        #endregion
        #region Methods     
        public  IEnumerable<DateTime> GetDates(Timeline tl)
        {
            DateTime CurrDate = new[] { FirstDate, tl.StartDate }.Max();
            DateTime EndDate = tl.EndDate;
            if (LastDate.HasValue)
            {
                 EndDate = new[] { LastDate.Value, tl.EndDate }.Min();
            }
            int DayCnt = 0;

            if (DayOfWeekEnabled(CurrDate.DayOfWeek))
            {
                DayCnt++;
                yield return CurrDate;
            }
            while (CurrDate < EndDate)
            {
                if(DayCnt  >= NumOccurences && NumOccurences != -1)
                {
                    yield break;
                }
                switch (Frequency)
                {
                    case (FrequencyType.Daily):
                        CurrDate = CurrDate.AddDays(FrequencyUnits);
                        break;
                    case (FrequencyType.Weekly):
                        CurrDate = CurrDate.AddDays(FrequencyUnits * 7);
                        break;
                    case (FrequencyType.Monthly):
                        CurrDate = CurrDate.AddMonths(FrequencyUnits);
                        break;
                    case (FrequencyType.Yearly):
                        CurrDate = CurrDate.AddYears(FrequencyUnits);
                        break;
                }
                //DayOfWeek logic
                if (DayOfWeekEnabled(CurrDate.DayOfWeek))
                {
                    DayCnt++;
                    yield return CurrDate;
                }               
               
            }            
        }
        public bool DayOfWeekEnabled(DayOfWeek dow)
        {
            return IncludeDays.HasFlag(GetDayOfWeekMap[dow]);
        }
        #endregion

        #region CallBacks           
        #endregion

        #region Static Methods     
        public static Dictionary<Days,DayOfWeek> GetFlagMap
        {
            get
            {
                return DaysFlagMap.MapDayFlag;
            }
           
        }
        public static Dictionary<DayOfWeek, Days> GetDayOfWeekMap
        {
            get
            {
                return DaysFlagMap.MapDayOfWeek;
            }

        }
        #endregion
    }
    public enum FrequencyType
    {
        Daily,
        DayOfWeek,
        Weekly,
        Monthly,
        Yearly
    }
    [Flags]
    public enum Days
    {
        None =      0b_0000_0000, // 0
        Sunday =    0b_0000_0001, // 1
        Monday =    0b_0000_0010, // 2
        Tuesday =   0b_0000_0100, // 4
        Wednesday = 0b_0000_1000, // 8
        Thursday =  0b_0001_0000, // 16
        Friday =    0b_0010_0000, // 32
        Saturday =  0b_0100_0000,  // 64 
        All =       0b_0111_1111, 
        WeekDays =  0b_0011_1110,
        WeekEnd =   0b_0100_0001  
    }
    static class DaysFlagMap
    {
        /// <summary>
        /// Static string Dictionary example
        /// </summary>
        public static Dictionary<Days, DayOfWeek> MapDayFlag = new Dictionary<Days, DayOfWeek>
        {
            {Days.Monday, DayOfWeek.Monday},
            {Days.Tuesday, DayOfWeek.Tuesday},
            {Days.Wednesday, DayOfWeek.Wednesday},
            {Days.Thursday, DayOfWeek.Thursday},
            {Days.Friday, DayOfWeek.Friday},
            {Days.Saturday, DayOfWeek.Saturday},
            {Days.Sunday, DayOfWeek.Sunday},
        };
        public static Dictionary<DayOfWeek, Days> MapDayOfWeek = new Dictionary<DayOfWeek, Days>
        {
            {DayOfWeek.Monday, Days.Monday},
            {DayOfWeek.Tuesday, Days.Tuesday},
            {DayOfWeek.Wednesday, Days.Wednesday},
            {DayOfWeek.Thursday, Days.Thursday},
            {DayOfWeek.Friday, Days.Friday},
            {DayOfWeek.Saturday, Days.Saturday},
            {DayOfWeek.Sunday, Days.Sunday},
        };

    }


}
