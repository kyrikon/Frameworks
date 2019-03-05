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
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ValueType 
    {
        [Description("Date")]
        Date,
        [Description("DateTime")]
        DateTime,
        [Description("Integer")]
        Integer,
        [Description("Decimal")]
        Decimal,
        [Description("Text")]
        Text,
        [Description("Enum")]
        Enum,
        [Description("List")]
        KeyValue

    }
    public class KeyValueEnum<T> : Dictionary<string,object>
    {
         
    }

    public static class TypeExtensions
    {
        public static ValueType GetEditor(this Type MyType)
        {
            if(MyType == typeof(string))
            {
                return ValueType.Text;
            }
            if (MyType == typeof(int) || MyType == typeof(long))
            {
                return ValueType.Integer;
            }
            if (MyType == typeof(decimal) || MyType == typeof(double) || MyType == typeof(float))
            {
                return ValueType.Decimal;
            }
            if (MyType.IsEnum)
            {
                return ValueType.Enum;
            }
            if (MyType == typeof(DateTime))
            {
                return ValueType.DateTime;
            }
            if (MyType.IsSubclassOf(typeof(IDictionary<string,object>)))
            {
                return ValueType.KeyValue;
            }
            return ValueType.Text;
        }
    }

}
