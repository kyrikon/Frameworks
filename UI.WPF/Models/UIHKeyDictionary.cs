using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using DataInterface;

namespace UI.WPF.Models
{
    public class UIHKeyDictionary : UIObservableConcurrentDictionary<HKey, HDynamicObject>
    {

        public byte[] ToBinary()
        {
            KeyValuePair<HKey, KeyValuePair<string, Tuple<object, string>>[]>[] Serial = new KeyValuePair<HKey, KeyValuePair<string, Tuple<object, string>>[]>[this.Count];
            int currIdx = 0;
            foreach (KeyValuePair<HKey, HDynamicObject> Row in this)
            {
                Serial[currIdx] = new KeyValuePair<HKey, KeyValuePair<string, Tuple<object, string>>[]>(Row.Key, Row.Value.ToArray());
                currIdx++;
            }
            return Serial.ToBinary();
        }
        public static UIHKeyDictionary FromBinary(byte[] Serial)
        {
            KeyValuePair<HKey, KeyValuePair<string, object>[]>[] DeSerial = Serialization.FromBinary<KeyValuePair<HKey, KeyValuePair<string, object>[]>[]>(Serial);
            UIHKeyDictionary DoD = new UIHKeyDictionary();
            foreach (KeyValuePair<HKey, KeyValuePair<string, object>[]> Row in DeSerial.OrderBy(x => x.Key))
            {
                DoD.TryAdd(Row.Key, new HDynamicObject(Row.Value));
            }

            return DoD;
        }
    }
}
