using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public static class NullValueRef
    {
        //TODO: Translation to Value type Value
        public static NullValRef NullRefVal()
        {
            return new NullValRef();
        }       
    }

    /// <summary>
    /// This is required to make the serialization work in the case where collection 
    /// constructors with expected types fail
    /// </summary>
    public class NullValRef
    {

        public override string ToString()
        {
            return "Null";
        }
    }
}
