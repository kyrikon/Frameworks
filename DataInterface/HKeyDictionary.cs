﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;

namespace DataInterface
{
    public class HKeyDictionary : ObservableConcurrentDictionary<HKey, HDynamicObject>
    {

        public byte[] ToBinary()
        {
            KeyValuePair<HKey, KeyValuePair<string, Tuple<dynamic, string>>[]>[] Serial = new KeyValuePair<HKey, KeyValuePair<string, Tuple<dynamic,string>>[]>[this.Count];
            int currIdx = 0;
            foreach (KeyValuePair<HKey, HDynamicObject> Row in this)
            {
                Serial[currIdx] = new KeyValuePair<HKey, KeyValuePair<string, Tuple<dynamic, string>>[]>(Row.Key, Row.Value.ToArray());
                currIdx++;
            }
            return Serial.ToBinary();
        }
        public static HKeyDictionary FromBinary(byte[] Serial)
        {
            KeyValuePair<HKey, KeyValuePair<string, dynamic>[]>[] DeSerial = Serialization.FromBinary<KeyValuePair<HKey, KeyValuePair<string, dynamic>[]>[]>(Serial);
            HKeyDictionary DoD = new HKeyDictionary();
            foreach (KeyValuePair<HKey, KeyValuePair<string, object>[]> Row in DeSerial.OrderBy(x => x.Key))
            {
                DoD.TryAdd(Row.Key, new HDynamicObject(Row.Value));
            }

            return DoD;
        }
    }
}
