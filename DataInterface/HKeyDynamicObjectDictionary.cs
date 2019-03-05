using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;

namespace DataInterface
{
    public class HKeyDynamicObjectDictionary : ObservableConcurrentDictionary<HKey, HDynamicObject>
    {

        public byte[] ToBinary()
        {
            KeyValuePair<HKey, KeyValuePair<string, Tuple<Object, string>>[]>[] Serial = new KeyValuePair<HKey, KeyValuePair<string, Tuple<Object,string>>[]>[this.Count];
            int currIdx = 0;
            foreach (KeyValuePair<HKey, HDynamicObject> Row in this)
            {
                Serial[currIdx] = new KeyValuePair<HKey, KeyValuePair<string, Tuple<Object, string>>[]>(Row.Key, Row.Value.ToArray());
                currIdx++;
            }
            return Serial.ToBinary();
        }
        public static HKeyDynamicObjectDictionary FromBinary(byte[] Serial)
        {
            KeyValuePair<HKey, KeyValuePair<string, Object>[]>[] DeSerial = Serialization.FromBinary<KeyValuePair<HKey, KeyValuePair<string, Object>[]>[]>(Serial);
            HKeyDynamicObjectDictionary DoD = new HKeyDynamicObjectDictionary();
            foreach (KeyValuePair<HKey, KeyValuePair<string, object>[]> Row in DeSerial.OrderBy(x => x.Key))
            {
                DoD.TryAdd(Row.Key, new HDynamicObject(Row.Value));
            }

            return DoD;
        }
    }
}
