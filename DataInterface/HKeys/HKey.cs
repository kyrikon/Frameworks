using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataInterface
{
    [JsonArray]
    [Serializable]
    public class HKey : IComparable<HKey>, IEnumerable
    {

        #region Fields 
        private int[] _Key;
        #endregion
        #region Constructors
        public HKey(int[] _inpt)
        {
            if(_inpt.Length == 0)
            {
                throw new HKeyException("Key must have length > 0");
            }
            _Key = _inpt;
           
        }
        public HKey(string _inpt)
        {
            if (_inpt.Length == 0)
            {
                throw new HKeyException("Key must have length > 0");
            }
            string[] Split = _inpt.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                _Key = Array.ConvertAll<string, int>(Split, int.Parse);
            }
            catch (FormatException)
            {
                throw new HKeyException("String Key in incorrect format. Must be N1.N2....NN where N is an integer");
            }

        }

        #endregion
        #region Commands   
        #endregion
        #region Properties

        public int this[int index]
        {
            get
            {
                return _Key[index];
            }
        }
       
        [JsonIgnore]
        public HKey ParentKey
        {             
            get
            {
                return new HKey(((Span<int>)_Key).Slice(0, _Key.Length > 1 ? _Key.Length - 1: 1).ToArray());
            }
        }
        [JsonIgnore]
        public bool IsRoot
        {
            get
            {
               return _Key.Length == 1;                
            }
        }
        [JsonIgnore]
        public HKey RootKey
        {
            get
            {
                return new HKey(((Span<int>)_Key).Slice(0, 1).ToArray());
            }
        }
        [JsonIgnore]
        public string StrKey
        {
            get
            {
                return $"({ string.Join(",", (int[])this)})";
            }
        }
        [JsonIgnore]
        public int Rank
        {
            get
            {
                return _Key[_Key.Length-1];
            }
        }
        [JsonIgnore]
        public int Count
        {
            get
            {
                return _Key.Length;
            }
        }
        #endregion
        #region Methods     
        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(HKey))
            {
                HKey comp = (HKey)obj;
                return this._Key.SequenceEqual(comp._Key);
            }
            return false;
        }

        public bool Contains(object obj)
        {
            if(obj.GetType() == typeof(HKey))
            {
                HKey comp = (HKey)obj;
                if(this._Key.Length < comp._Key.Length)
                {
                    return false;
                }
                return ((Span<int>)this._Key).Slice(0, comp._Key.Length).SequenceEqual(comp._Key);
            }           
                return false;            
        }

        public override int GetHashCode()
        {

            int hash = 0;
            foreach(int KeyFrag in _Key)
            {
                hash ^= KeyFrag;
            }

            return hash;
        }
        public IEnumerator GetEnumerator()
        {
            return _Key.GetEnumerator();
        }
        public int CompareTo(HKey comp)
        {
            if(this._Key.Length > comp._Key.Length)
            {
                return 1;
            }
            if(this._Key.Length < comp._Key.Length)
            {
                return -1;
            }
            if(this._Key.SequenceEqual(comp._Key))
            {
                return 0;
            }
            for(int i = 0; i < this._Key.Length; i++)
            {
                if(this[i] > comp[i])
                {
                    return 1;
                }
                if(this[i] < comp[i])
                {
                    return -1;
                }
            }
            return 0;
          
        }
       public HKey CreateChildKey(int LastKey)
       {
            int[] newKey = new int[_Key.Length + 1];
            Array.Copy(_Key, newKey, _Key.Length);
            newKey[newKey.Length -1] = LastKey;
            return new HKey(newKey);
        }
        public override string ToString()
        {
            return String.Join(".", _Key.Select(p => p.ToString()).ToArray());
        }

        #endregion
        #region Static
        static public implicit operator HKey(int[] value)
        {
            return new HKey(value);
        }
        static public implicit operator int[] (HKey value)
        {
            return value._Key;
        }

        static public implicit operator HKey(string value)
        {
            return new HKey(value);
        }
        static public implicit operator string (HKey value)
        {
            return value.ToString();
        }
        public static HKey RootKeyVal
        {
            get
            { 
                return (HKey)new int[] { 1 };
            }
        }
        #endregion

    }
    public class HKeyException : Exception
    {
        public HKeyException(string message) : base(message)
        {
            
        }
    }
}
