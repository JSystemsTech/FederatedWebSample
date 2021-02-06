using FederatedAuthNAuthZ.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedAuthNAuthZ.Models.Collection
{
	public class CollectionModel<T>
	where T : struct, IComparable
	{
		private IDictionary<T, string> Collection { get; set; }
		public IDictionary<T, string> ModelBindingErrors { get; set; }
		public bool HasBindingErrors() { return ModelBindingErrors.Count == 0; }
		public CollectionModel(IEnumerable<object> data)
		{
			Collection = data.ToDictionary<T>();
			ModelBindingErrors = new Dictionary<T, string>();
			Init();
		}
		protected string GetValue(T key, bool required = false)
		{
			if (required && (!Collection.ContainsKey(key) || string.IsNullOrWhiteSpace(Collection[key])))
			{
				if (!ModelBindingErrors.ContainsKey(key))
				{
					ModelBindingErrors.Add(key, "Collection expected a valid value for " + key.ToString());
				}
			}
			return Collection.ContainsKey(key) ? Collection[key] : null;
		}
		protected virtual void Init() { }
	}
}
