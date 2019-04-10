using Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataInterface
{
   
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum TimePeriod
    {
        [Description("Year")]
        Year,
        [Description("Month")]
        Month,
        [Description("Week")]
        Week,
        [Description("Day")]
        Day

    }

}
