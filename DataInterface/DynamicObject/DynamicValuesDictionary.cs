using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using System.Dynamic;

namespace DataInterface
{
    public class DynamicValuesDictionary : ObservableConcurrentDictionary<string,object>
    {
       
        public byte[] ToBinary()
        {
            KeyValuePair<string, Object>[] Serial = new  KeyValuePair<string, Object>[this.Count];
            int currIdx = 0;
            foreach (KeyValuePair<string, Object> Row in this)
            {
                Serial[currIdx] = new KeyValuePair<string, Object>(Row.Key, Row.Value);
                currIdx++;
            }
            return Serial.ToBinary();
        }
        public static DynamicValuesDictionary FromBinary(byte[] Serial)
        {
            KeyValuePair<string, Object>[] DeSerial = Serialization.FromBinary<KeyValuePair<string, Object>[]>(Serial);
            DynamicValuesDictionary DoD = new DynamicValuesDictionary();
            foreach (KeyValuePair<string, Object> Row in DeSerial.OrderBy(x => x.Key))
            {
                DoD.TryAdd(Row);
            }

            return DoD;
        }        
    }
    public class DynamicTypeDictionary : ObservableConcurrentDictionary<string, string>
    {
        
    }
}
