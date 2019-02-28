using Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DataInterface
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum YesNoEnum
    {
        [Description("Yes")]
        Yes,
        [Description("No")]
        No
    }
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
    public class KeyValueEnum<T> : Dictionary<string,object>
    {
         
    }
}
