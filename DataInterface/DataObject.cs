using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Core.Helpers;
using System.Linq;
using Core.Extensions;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace DataInterface
{
    [Serializable]
    public class DataObject : DataObjectBase
    {
        // Considerations for base class
        // 1. Object nature
        // 2. (De) Serialization
        // 3. Edit Auditing
        // 4. Syncronization with backing data store (multi user)
        #region Events / Delegates                   
        #endregion
        #region Fields 
        #endregion

        #region Constructors
        public DataObject() :base (true)
        {               
        }
      
        public DataObject(KeyValuePair<string, Object>[] InitArray) : base(InitArray)
        {                     
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        #endregion
        #region Methods     
        #endregion
        #region Callbacks       
        #endregion


    }   
}
