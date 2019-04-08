using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Core.Extensions
{
    public static class Serialization
    {
        #region Json
        public static string ToJson<T>(this T myObject)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(myObject, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });
        }
        public static T FromJson<T>(this T myObject, string s)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
        }
        public static T FromJson<T>(string s)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s,

                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        TypeNameHandling = TypeNameHandling.All

                    });
            }
            catch (ArgumentException)
            {

            }
            catch (JsonSerializationException)
            {

            }
            var settings = new JsonSerializerSettings { Converters = new JsonConverter[] { new JsonGenericDictionaryOrArrayConverter() } };

            var d2 = JsonConvert.DeserializeObject<T>(s, settings);

            return default(T);
        }
        #endregion
        #region BSon
        public static string ToBSon<T>(this T myObject)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BsonDataWriter datawriter = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(datawriter, myObject);
                return Convert.ToBase64String(ms.ToArray());
            }

        }
        public static T FromBSon<T>(this T myObject, string s)
        {
            byte[] data = Convert.FromBase64String(s);

            using (MemoryStream ms = new MemoryStream(data))
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }
        public static T FromBSon<T>(string s, bool isCollection = false)
        {
            byte[] data = Convert.FromBase64String(s);

            using (MemoryStream ms = new MemoryStream(data))
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                reader.ReadRootValueAsArray = isCollection;
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        #endregion
        #region XML
        //public static string ToXML<T>(this T myObject)
        //{

        //    XSerializer.XmlSerializer<T> XMLSerializer = new XSerializer.XmlSerializer<T>();            
        //    return XMLSerializer.Serialize(myObject);      
        //}
        //public static T FromXML<T>(this T myObject, string s)
        //{
        //    XSerializer.XmlSerializer<T> XMLSerializer = new XSerializer.XmlSerializer<T>();
        //    return XMLSerializer.Deserialize(s);
        //}
        //public static T FromXML<T>(string s)
        //{
        //    XSerializer.XmlSerializer<T> XMLSerializer = new XSerializer.XmlSerializer<T>();
        //    return XMLSerializer.Deserialize(s);
        //}
        #endregion
        #region Binary
        public static byte[] ToBinary<T>(this T myObject)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, myObject);
                return ms.ToArray();
            }
        }
        public static T FromBinary<T>(byte[] myBuffer)
        {
            using (MemoryStream ms = new MemoryStream(myBuffer))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                return (T)deserializer.Deserialize(ms);
            }

        }
        #endregion
    }
    public class JsonGenericDictionaryOrArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetDictionaryKeyValueTypes().Count() == 1;
        }

        public override bool CanWrite { get { return false; } }

        object ReadJsonGeneric<TKey, TValue>(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var tokenType = reader.TokenType;

            var dict = existingValue as IDictionary<TKey, TValue>;
            if (dict == null)
            {
                var contract = serializer.ContractResolver.ResolveContract(objectType);
                dict = (IDictionary<TKey, TValue>)contract.DefaultCreator();
            }

            if (tokenType == JsonToken.StartArray)
            {
                var pairs = new JsonSerializer().Deserialize<KeyValuePair<TKey, TValue>[]>(reader);
                if (pairs == null)
                    return existingValue;
                foreach (var pair in pairs)
                    dict.Add(pair);
            }
            else if (tokenType == JsonToken.StartObject)
            {
                // Using "Populate()" avoids infinite recursion.
                // https://github.com/JamesNK/Newtonsoft.Json/blob/ee170dc5510bb3ffd35fc1b0d986f34e33c51ab9/Src/Newtonsoft.Json/Converters/CustomCreationConverter.cs
                serializer.Populate(reader, dict);
            }
            return dict;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var keyValueTypes = objectType.GetDictionaryKeyValueTypes().Single(); // Throws an exception if not exactly one.

            var method = GetType().GetMethod("ReadJsonGeneric", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var genericMethod = method.MakeGenericMethod(new[] { keyValueTypes.Key, keyValueTypes.Value });
            return genericMethod.Invoke(this, new object[] { reader, objectType, existingValue, serializer });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public static class TypeExtensions
    {
        /// <summary>
        /// Return all interfaces implemented by the incoming type as well as the type itself if it is an interface.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetInterfacesAndSelf(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException();
            if (type.IsInterface)
                return new[] { type }.Concat(type.GetInterfaces());
            else
                return type.GetInterfaces();
        }

        public static IEnumerable<KeyValuePair<Type, Type>> GetDictionaryKeyValueTypes(this Type type)
        {
            foreach (Type intType in type.GetInterfacesAndSelf())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    var args = intType.GetGenericArguments();
                    if (args.Length == 2)
                        yield return new KeyValuePair<Type, Type>(args[0], args[1]);
                }
            }
        }
    }

}
