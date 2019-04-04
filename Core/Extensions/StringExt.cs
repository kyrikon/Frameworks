using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class StringExt
    {
        public const string AplhaNumericUScore = @"^[a-zA-Z](?:[a-zA-Z0-9_]*[a-zA-Z0-9])?$";
        public const string AlphaOnly = @"^[a-zA-Z]+$";
        public const string NumbersOnly = @"^[0-9]+$";
        #region Json
        public static bool IsFieldRules(this string InStr)
        {
            return Regex.IsMatch(InStr, AplhaNumericUScore);
        }
        public static bool IsAlphaOnly(this string InStr)
        {
            return Regex.IsMatch(InStr, AlphaOnly);
        }
        public static bool IsNumberOnly(this string InStr)
        {
            return Regex.IsMatch(InStr, NumbersOnly);
        }

        #endregion

    }
   
}
