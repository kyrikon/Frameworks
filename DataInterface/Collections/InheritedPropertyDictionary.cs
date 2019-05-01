using System;
using System.Collections.Generic;
using System.Text;
using Core.Extensions;
using System.Linq;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace DataInterface
{
    
    public class InheritedPropertyDictionary : ObservableConcurrentDictionary<string, DynamicField>
    {
        #region Fields
        #endregion

        #region Constructors
        public InheritedPropertyDictionary()
        {
        }

        #endregion
        #region Properties
        [JsonIgnore]
        public override ReadOnlyObservableCollection<DynamicField> ItemValList
        {
            get
            {                
                return new ReadOnlyObservableCollection<DynamicField>(new ObservableCollection<DynamicField>(base.ItemValList.OrderBy(x =>x.Enabled).ThenBy(x => x.Rank).ToList()));
            }

        }
        #endregion
        #region Methods
        public void PropertyRankChange()
        {
            OnPropertyChanged("ItemValList");
        }
        #endregion
        #region Callbacks
     
        #endregion

        #region Static Members
       
        #endregion
    }
}
