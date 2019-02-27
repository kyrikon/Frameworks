using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class NotificationSettings : NotifyPropertyChanged
    {
        public string NotificationEndpoint {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
        public string DeviceID
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
        public string DeviceKey
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }

        public string Status
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }
    }
}
