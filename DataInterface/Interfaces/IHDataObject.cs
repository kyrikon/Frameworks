using System;

namespace DataInterface
{
    public interface IHDataObject : IDataObject
    {

        #region Events / Delegates
        #endregion
        #region Properties       
        int[] ID { get;  }
        HKey HID
        {
            get;
        }
        HKeyDictionary Children { get; }
        HDynamicObject Parent { get; set; }
        HDynamicObject Root { get; set; }
        #endregion
        #region Methods             

        #endregion
    }
}