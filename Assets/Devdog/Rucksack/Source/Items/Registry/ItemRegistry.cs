using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Items
{
    public static class ItemRegistry
    {
        private static readonly Dictionary<System.Guid, IItemInstance> _dict = new Dictionary<System.Guid, IItemInstance>();

        /// <summary>
        /// Fires when a new item successfully registered
        /// </summary>
        public static event Action<Guid, IItemInstance> OnAddedItem;
        /// <summary>
        /// Fires when an existing item successfully unregistered, but will not fire when all items cleared.
        /// </summary>
        public static event Action<Guid> OnRemovedItem;
        /// <summary>
        /// Fires when all items cleared from registry
        /// </summary>
        public static event Action OnAllItemsCleared;

        /// <summary>
        /// Get item from registry by its ID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static IItemInstance Get(System.Guid guid)
        {
            return _dict[guid];
        }

        /// <summary>
        /// Try get item from registry by its id
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool TryGet(System.Guid guid, out IItemInstance item)
        {
            return _dict.TryGetValue(guid, out item);
        }

        /// <summary>
        /// Get all registered items
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IItemInstance> GetAll()
        {
            foreach (var kvp in _dict)
            {
                yield return kvp.Value;
            }
        }
            
        /// <summary>
        /// Register a new item
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="val"></param>
        public static void Register(System.Guid guid, IItemInstance val)
        {
            if (val != null)
            {
                _dict[guid] = val;
                OnAddedItem?.Invoke(guid, val);
            }
        }
        
        /// <summary>
        /// Remove item from registry
        /// </summary>
        /// <param name="guid"></param>
        public static void UnRegister(System.Guid guid)
        {
            if (_dict.Remove(guid))
            {
                OnRemovedItem?.Invoke(guid);
            }
        }

        /// <summary>
        /// Remove all items from registry
        /// </summary>
        public static void Clear()
        {
            _dict.Clear();
            OnAllItemsCleared?.Invoke();
        }
        
        /// <summary>
        /// Check if registry contains item with given id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool Contains(System.Guid guid)
        {
            return _dict.ContainsKey(guid);
        }
    }
}