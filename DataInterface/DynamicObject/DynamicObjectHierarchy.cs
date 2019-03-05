using Core.Helpers;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataInterface
{
    public class DynamicObjectHierarchy : ConcurrentBag<DynamicObjectHierarchyItem>
    {
        #region Methods 
        public void ClearItems()
        {
            while (!this.IsEmpty)
            {
                DynamicObjectHierarchyItem rmv = new DynamicObjectHierarchyItem();
                this.TryTake(out rmv);
            }
        }
        #endregion       
    }

    public class DynamicObjectHierarchyItem : NotifyPropertyChanged
    {       
        #region Properties
        public HKey ID
        {
            get
            {
                return GetPropertyValue<HKey>();
            }
            set
            {
                if(GetPropertyValue<HKey>() != value)
                {
                    SetPropertyValue<HKey>(value);
                }
            }
        }
        public string Name
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if(GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }
        public int Rank
        {
            get
            {
                return ID.Rank;
            }            
        }        
        #endregion
    }
}
