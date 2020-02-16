using System.Collections.Generic;

namespace Devdog.Rucksack.Items
{
    public static class ServerItemRegistry
    {
        private static readonly Dictionary<System.Guid, IItemInstance> _dict = new Dictionary<System.Guid, IItemInstance>();
        public static IItemInstance Get(System.Guid guid)
        {
            IItemInstance o;
            _dict.TryGetValue(guid, out o);
            return o;
        }

        public static IEnumerable<IItemInstance> GetAll()
        {
            foreach (var kvp in _dict)
            {
                yield return kvp.Value;
            }
        }
        
        public static void Register(System.Guid guid, IItemInstance val)
        {
            if (val != null)
            {
                _dict[guid] = val;
            }
        }
        
        public static void UnRegister(System.Guid guid)
        {
            _dict.Remove(guid);
        }

        public static void Clear()
        {
            _dict.Clear();
        }
        
        public static bool Contains(System.Guid guid)
        {
            return _dict.ContainsKey(guid);
        }
    }
}