using Core.Models;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace DataInterface.Extensions
{
    public static class ValidationExt
    {

        public static string ValidationErrorText(this ValidationResult VResult)
        {
            if (VResult == null || VResult.IsValid)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
           
            if (!VResult.IsValid)
            {
                sb.Append($"{VResult.ToString(":")}");
            }
            return sb.ToString();
        }

    }
   
}
