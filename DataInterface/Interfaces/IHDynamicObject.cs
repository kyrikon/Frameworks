using System;
using System.Collections.ObjectModel;

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
        ObservableCollection<HDynamicObject> Children { get; }
        HDynamicObject Parent { get; set; }
        HDynamicObject Root { get; set; }
        #endregion
    }
}