using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataInterface
{    
    public class SelectionChangedEventArgs
    {
        #region Constructor
        public SelectionChangedEventArgs(bool _IsSelected)
        {
            IsSelected = _IsSelected;
        }
        #endregion

        #region Properties
        public bool IsSelected { get; }
        #endregion
    }
    public class CheckedChangedEventArgs
    {
        #region Constructor
        public CheckedChangedEventArgs(bool _IsChecked)
        {
            IsChecked = _IsChecked;
        }
        #endregion

        #region Properties
        public bool IsChecked { get; }
        #endregion
    }

    [Serializable]
    public class PropertyExtensions : NotifyPropertyChanged
    {
        public PropertyExtensions()
        {
            AllowEdit = true;
            IsVisible = true;
        }
        public bool AllowEdit
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            private set
            {
                if (GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
        public bool IsVisible
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            private set
            {
                if (GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
    }
    public enum EditorType
    {
        Text,
        Int,
        Dec,
        ListSingle,
        ListMulti,
        Bool,
        DateTime
    }
}
