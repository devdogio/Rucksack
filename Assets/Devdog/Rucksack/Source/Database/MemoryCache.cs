using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Database
{
    /// <summary>
    /// A memory cache that can be used with a database.
    /// <remarks>Note that the cache is never cleared automatically and will always continue growing.</remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MemoryCache<T> : ICache<T>
    {
        private readonly Dictionary<Guid, T> _dict;
        
        public MemoryCache()
        {
            _dict = new Dictionary<Guid, T>();
        }
        
        public bool Contains(IIdentifiable identifier)
        {
            return _dict.ContainsKey(identifier.ID);
        }

        public T Get(IIdentifiable identifier, T defaultValue = default(T))
        {
            T val;
            if (_dict.TryGetValue(identifier.ID, out val))
            {
                return val;
            }

            return defaultValue;
        }

        public void Set(IIdentifiable identifier, T item)
        {
            _dict[identifier.ID] = item;
        }

        public void Remove(IIdentifiable identifier)
        {
            _dict.Remove(identifier.ID);
        }
    }
}