using Core.Helpers;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataInterface
{
    public class DataObjectHierarchy : ConcurrentBag<DataObjectHierarchyItem>
    {

        #region Events / Delegates
        #endregion
        #region Fields 
        #endregion
        #region Constructors
        public DataObjectHierarchy()
        {            
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties

        #endregion
        #region Methods 
        public HKey CreateKey(int[] Vals)
        {
            return new HKey(Vals);           
        }
        public void ClearItems()
        {
            while (!this.IsEmpty)
            {
                DataObjectHierarchyItem rmv = new DataObjectHierarchyItem();
                this.TryTake(out rmv);
            }
        }
        #endregion
       
    }

    public class DataObjectHierarchyItem : NotifyPropertyChanged
    {

        #region Events / Delegates
        #endregion
        #region Fields 
        #endregion
        #region Constructors
        #endregion
        #region Commands   
        #endregion
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
        #region Methods     
        #endregion


    }
}
