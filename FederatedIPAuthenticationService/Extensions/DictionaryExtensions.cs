using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FederatedAuthNAuthZ.Extensions
{
    public static class DictionaryExtensions
    {
        public static T First<T>(this IDictionary<string, IEnumerable<T>> collection, string key) => collection.ContainsKey(key) && collection[key].First() is T value ? value: default;

        public static void AddUpdate<T>(this IDictionary<string, IEnumerable<T>> collection, string key, IEnumerable<T> value)
        {
            if (collection.ContainsKey(key))
            {
                collection.Remove(key);
            }
            collection.Add(key, value);
        }
        public static void AddUpdate<T>(this IDictionary<string, IEnumerable<T>> collection, string key, T value) => collection.AddUpdate(key, new T[1] { value });
        private static T ConvertTo<T>(this object value)
        {
            T returnValue;

            if (value is T variable)
                returnValue = variable;
            else
                try
                {
                    //Handling Nullable types i.e, int?, double?, bool? .. etc
                    if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
                        returnValue = (T)conv.ConvertFrom(value);
                    }
                    else
                    {
                        returnValue = (T)Convert.ChangeType(value, typeof(T));
                    }
                }
                catch (Exception)
                {
                    returnValue = default(T);
                }

            return returnValue;
        }
        public static IEnumerable<T> Get<T>(this IDictionary<string, IEnumerable<string>> collection, string key)
            => collection.ContainsKey(key) ? collection[key].Select(i=> ConvertTo<T>(i)): null;
        public static T ParseFirst<T>(this IDictionary<string, IEnumerable<string>> collection, string key) => ConvertTo<T>(collection.First(key));
    }
}