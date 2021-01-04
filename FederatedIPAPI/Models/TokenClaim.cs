using System.Collections.Generic;
using System.Linq;

namespace FederatedIPAPI.Models
{
    public class TokenClaim
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IEnumerable<string> Values { get; set; }
        public TokenClaim() { }
        internal TokenClaim(string name, IEnumerable<string> values)
        {
            Name = name;
            Values = values.Count() == 1 ? values.First().Split(",") : values;
            Value = string.Join(',', Values);
        }
        internal IEnumerable<string> GetValues() => Values != null ? Values : !string.IsNullOrWhiteSpace(Value) ? Value.Split(",") : new string[0];
        internal string GetValue() => string.Join(',', GetValues());
    }
}
