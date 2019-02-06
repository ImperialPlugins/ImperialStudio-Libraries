using System.Collections.Generic;

namespace ImperialStudio.Core.Util
{
    public static class DictionaryExtensions
    {
        public static void AddOrSet<K, E>(this IDictionary<K, E> @this, K key, E value)
        {
            if (@this.ContainsKey(key))
            {
                @this[key] = value;
            }
            else
            {
                @this.Add(key, value);
            }
        }
    }
}