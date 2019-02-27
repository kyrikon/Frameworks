using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
   public class APIDetails : NotifyPropertyChanged
    {
        public string Description
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if (GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }
        public string EndPoint
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if (GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
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
                if (GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }
        public int ID
        {
            get
            {
                return GetPropertyValue<int>();
            }
            set
            {
                if (GetPropertyValue<int>() != value)
                {
                    SetPropertyValue<int>(value);
                }
            }
        }

    }
}
