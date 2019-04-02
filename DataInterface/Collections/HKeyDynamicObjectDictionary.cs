using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using System.Collections.ObjectModel;

namespace DataInterface
{
    public class HKeyDynamicObjectDictionary : ObservableConcurrentDictionary<HKey, HDynamicObject>
    {
        #region Fields
        private ObservableCollection<HDynamicObject> _Root;

        #endregion

        #region Constructors
        public HKeyDynamicObjectDictionary()
        {
            Root = new ObservableCollection<HDynamicObject>();
            base.DictionaryChanged += HKeyDynamicObjectDictionary_DictionaryChanged;

        }

        #endregion
        #region Properties
        public ObservableCollection<HDynamicObject> Root
        {
            get
            {
                return _Root;
            }
            private set
            {
                _Root = value;
            }
        }
        #endregion
        #region Methods

        #endregion
        #region Callbacks
        private void HKeyDynamicObjectDictionary_DictionaryChanged(object sender, DictionaryChangedEventArgs<HKey, HDynamicObject> args)
        {
            switch (args.Action)
            {
                case CollectionAction.Add:
                    HKey NewKey = args.NewVal.Key;

                    if (!NewKey.IsRoot)
                    {
                        if (this.ContainsKey(NewKey.ParentKey))
                        {
                            HDynamicObject Parent = (HDynamicObject)this[NewKey.ParentKey];
                            HDynamicObject NewItem = (HDynamicObject)args.NewVal.Value;
                            NewItem.Parent = Parent;
                            NewItem.Root = (HDynamicObject)this[NewKey.RootKey];
                            if (args.NewVal.Value.Rank == null || args.NewVal.Value.Rank == 0)
                            {
                                args.NewVal.Value.Rank = args.NewVal.Key.Rank;
                            }
                            //Logic to add in all children in case not added in order
                            //foreach(KeyValuePair<HKey, DataObject> Chldrn in Objects.Where(x => x.Key.Contains(NewKey) && !x.Key.Equals(NewKey)))
                            //{

                            //    Chldrn.Valuargs.Parent = NewItem;
                            //    NewItem.Children.TryAdd(Chldrn.Key, Chldrn.Value);

                            //}
                            this[NewKey.ParentKey].Children.Add(args.NewVal.Value);
                        }
                    }
                    else
                    {
                        Root.Add(args.NewVal.Value);
                        if (!Root.FirstOrDefault().HasKey("CustomLists"))
                        {
                            Root.FirstOrDefault()["CustomLists"] = new ObservableConcurrentDictionary<string, CustomList>();
                        }
                    }
                    break;
                case CollectionAction.Remove:
                    HKey DelKey = args.RemVal.Key;
                    //   TreeListExpandedNodesHelper.RegisterBaseObject(args.RemVal.Value);
                    if (!DelKey.IsRoot)
                    {
                        HKey ParKey = DelKey.ParentKey;
                        if (this.ContainsKey(ParKey))
                        {
                            var DelItem = this[ParKey].Children.FirstOrDefault(x => x.HID.Equals(DelKey));
                            if (DelItem != null)
                            {
                                this[ParKey].Children.Remove(DelItem);
                            }
                        }
                    }
                    break;
            }
        } 
        #endregion

        #region Static Members
        public byte[] ToBinary()
        {
            KeyValuePair<HKey, KeyValuePair<string, Tuple<Object, string>>[]>[] Serial = new KeyValuePair<HKey, KeyValuePair<string, Tuple<Object, string>>[]>[this.Count];
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
        #endregion
    }
}
