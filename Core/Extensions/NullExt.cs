using Core.Models;
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
    public static class NullExt
    {

        public static bool CheckNullValRef(this object Obj)
        {
            return Obj.GetType() == typeof(NullValRef);
        }

    }
   
}
