using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FederatedAuthNAuthZ.Extensions
{
	public static class EnumExtensions
	{
		public static IEnumerable<T> GetValues<T>()
			where T : struct, IComparable
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
		public static IDictionary<T, string> ToDictionary<T>(this IEnumerable<object> data)
			where T : struct, IComparable
		{
			IDictionary<T, string> collection = new Dictionary<T, string>();
			foreach (T enumEntry in GetValues<T>())
			{
				int ordinal = (int)(object)enumEntry;
				string value = ordinal < data.Count() ? data.ElementAt(ordinal).ToString() : null;
				collection.Add(enumEntry, value);
			}
			return collection;
		}
	}
}
