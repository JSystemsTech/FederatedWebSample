using Newtonsoft.Json;

namespace FederatedAuthNAuthZ.Extensions
{
    public static class StringExtensions
    {
        public static bool TryDeserializeObject<T>(this string str, out T value)
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(str);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }
    }
}
