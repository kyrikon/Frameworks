using System;
using System.Collections.Generic;

namespace DataInterface
{
    public interface IDynamicObject 
    {
        #region Properties       
        object this[string key] { get; set; }
        bool IsTransactional { get; }
        bool IsEditing { get; set; }
        bool IsModified { get; }
        bool DeleteDenied { get; }
        string LastFieldEdited { get; }
        double VersionTimestamp { get; }
        bool IsContainer { get; set; }
        #endregion
        #region Methods             
        ModifiedDataItem[] GetChangeSet(string key);
        ModifiedDataItem GetLastUndo();
        string[] GetModifiedKeys();
        ModifiedDataItem GetUndo(string key);
        T GetValue<T>(string key);
        bool HasKey(string key);
        void RemoveKey(string key);
        void CommitChanges();
        void Reset();
        void RevertChanges(string key);
        void Undo();
        void UndoChange(string key);
        KeyValuePair<string, Tuple<object, string>>[] ToArray();
        void FromArray(KeyValuePair<string, object>[] ObjValues);
        void CastProps();
        #endregion
    }
}