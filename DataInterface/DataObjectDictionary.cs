using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using System.Dynamic;

namespace DataInterface
{
    public class DataObjectDictionary : ObservableConcurrentDictionary<string,dynamic>
    {

        public byte[] ToBinary()
        {
            KeyValuePair<string, dynamic>[] Serial = new  KeyValuePair<string, dynamic>[this.Count];
            int currIdx = 0;
            foreach (KeyValuePair<string, dynamic> Row in this)
            {
                Serial[currIdx] = new KeyValuePair<string, dynamic>(Row.Key, Row.Value.ToArray());
                currIdx++;
            }
            return Serial.ToBinary();
        }
        public static DataObjectDictionary FromBinary(byte[] Serial)
        {
            KeyValuePair<string, dynamic>[] DeSerial = Serialization.FromBinary<KeyValuePair<string, dynamic>[]>(Serial);
            DataObjectDictionary DoD = new DataObjectDictionary();
            foreach (KeyValuePair<string, dynamic> Row in DeSerial.OrderBy(x => x.Key))
            {
                DoD.TryAdd(Row);
            }

            return DoD;
        }        
    }
    public class DataTypeDictionary : ObservableConcurrentDictionary<string, string>
    {
        
    }
}
