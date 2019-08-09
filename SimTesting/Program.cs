using System;

using CashSimModels.Scheduling;

namespace SimTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            CashSimModels.Scheduling.Schedule sh = new Schedule()
            {   FirstDate = DateTime.Now.AddDays(-3),              
                FrequencyUnits = 1,
                NumOccurences = -1,
                IncludeDays = Days.All,
                Frequency = FrequencyType.Monthly
            };
            Timeline tl = new Timeline() { StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4) };
            Console.WriteLine($"Start {tl.StartDate.ToShortDateString()} day {tl.StartDate.DayOfWeek.ToString()} : End {tl.EndDate.ToShortDateString()} day {tl.EndDate.DayOfWeek.ToString()}");
            foreach (DateTime dt in sh.GetDates(tl))
            {
                Console.WriteLine($"{dt.ToShortDateString()} day {dt.DayOfWeek.ToString()}");
            }           
        }
    }
}
