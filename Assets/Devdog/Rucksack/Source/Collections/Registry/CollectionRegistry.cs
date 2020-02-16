using System.Collections.Generic;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// The CollectionRegistry is used to register collection instances by their ID / Name. This helps find collections back dynamically.
    /// </summary>
    public static class CollectionRegistry
    {
        public class Helper<TKey, TValue>
        {
            private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

            public IEnumerable<TKey> keys
            {
                get { return _dict.Keys; }
            }

            public IEnumerable<TValue> values
            {
                get { return _dict.Values; }
            }

            public delegate void OnAction(TKey key, TValue value);

            /// <summary>
            /// Fires when new value successfully registered
            /// </summary>
            public event OnAction OnAddedItem;

            /// <summary>
            /// Fires when existing value removed
            /// </summary>
            public event OnAction OnRemovedItem;
            
            /// <summary>
            /// Get value of the registry by Key
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns></returns>
            public TValue Get(TKey identifier)
            {
                TValue o;
                TryGet(identifier, out o);
                return o;
            }

            /// <summary>
            /// Get multiple values of the registry by Keys
            /// </summary>
            /// <param name="identifiers"></param>
            /// <returns></returns>
            public IEnumerable<TValue> Get(IEnumerable<TKey> identifiers)
            {
                foreach (var identifier in identifiers)
                {
                    TValue o;
                    if (_dict.TryGetValue(identifier, out o))
                    {
                        yield return o;
                    }
                }
            }

            /// <summary>
            /// Try get value by key
            /// </summary>
            /// <param name="identifier"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool TryGet(TKey identifier, out TValue value)
            {
                return _dict.TryGetValue(identifier, out value);
            }
            
            /// <summary>
            /// Add new value to registry
            /// </summary>
            /// <param name="identifier"></param>
            /// <param name="val"></param>
            public void Register(TKey identifier, TValue val)
            {
                _dict[identifier] = val;
                OnAddedItem?.Invoke(identifier, val);
            }
        
            /// <summary>
            /// Remove existing value from registry
            /// </summary>
            /// <param name="identifier"></param>
            public void UnRegister(TKey identifier)
            {
                TValue currentValue;
                if (TryGet(identifier, out currentValue))
                {
                    _dict.Remove(identifier);
                    OnRemovedItem?.Invoke(identifier, currentValue);
                }
            }

            /// <summary>
            /// Clear registry
            /// </summary>
            public void Clear()
            {
                foreach (var kvp in _dict)
                {
                    OnRemovedItem?.Invoke(kvp.Key, kvp.Value);
                }
                
                _dict.Clear();
            }
        
            /// <summary>
            /// Check if registry contains value with given key
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns></returns>
            public bool Contains(TKey identifier)
            {
                return _dict.ContainsKey(identifier);
            }

            /// <summary>
            /// Check if registry contains value with given value
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool Contains(TValue value)
            {
                return _dict.ContainsValue(value);
            }
        }
        
        private static Helper<System.Guid, ICollection> _idCols = new Helper<System.Guid, ICollection>();
        private static Helper<string, ICollection> _nameCols = new Helper<string, ICollection>();

        /// <summary>
        /// Get list of values by ID
        /// </summary>
        public static Helper<System.Guid, ICollection> byID
        {
            get { return _idCols; }
        }

        /// <summary>
        /// Get list of values by name
        /// </summary>
        public static Helper<string, ICollection> byName
        {
            get { return _nameCols; }
        }
    }
}