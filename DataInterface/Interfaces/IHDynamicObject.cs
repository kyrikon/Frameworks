using System;

namespace DataInterface
{
    public interface IHDynamicObject : IDynamicObject
    {
        #region Properties       
        int[] ID { get;  }
        HKey HID
        {
            get;
        }
        HKeyDynamicObjectDictionary Children { get; }
        HDynamicObject Parent { get; set; }
        HDynamicObject Root { get; set; }
        #endregion
    }
}