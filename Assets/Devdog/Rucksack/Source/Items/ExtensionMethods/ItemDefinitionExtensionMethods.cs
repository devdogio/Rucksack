using System;
using System.Collections.Generic;
using System.Reflection;

namespace Devdog.Rucksack.Items
{
    public static class ItemDefinitionExtensionMethods
    {
        public delegate T GetValueDelegate<out T>(IItemDefinition target);
        public delegate T GetValueDelegate<out T, in TDefType>(TDefType target) where TDefType : IItemDefinition;
        
        public static IEnumerable<T> ToItemInstances<T>(this IEnumerable<ItemDefinition> definitions)
            where T: IItemInstance, IIdentifiable
        {
            foreach (var def in definitions)
            {
                yield return (T) def.CreateInstance(Guid.NewGuid());
            }
        }

        public static TValueType GetValue<TValueType, TDefType>(this TDefType target, GetValueDelegate<TValueType, TDefType> get, TValueType ignoreValue = default(TValueType))
            where TDefType : IItemDefinition
        {
            var val = get(target);
            if (EqualityComparer<TValueType>.Default.Equals(val, ignoreValue))
            {
                if (target.parent != null)
                {
                    return GetValue<TValueType, TDefType>((TDefType)target.parent, get, ignoreValue);
                }
            }
            
            return val;
        }
        
        public static TValueType GetValue<TValueType>(this IItemDefinition itemDef, GetValueDelegate<TValueType> get, TValueType ignoreValue = default(TValueType))
        {
            return GetValue<TValueType>(itemDef, get, itemDef, ignoreValue);
        }
        
        public static TValueType GetValue<TValueType>(this IItemDefinition itemDef, GetValueDelegate<TValueType> get, IItemDefinition target, TValueType ignoreValue)
        {
            var val = get(target);
            if (EqualityComparer<TValueType>.Default.Equals(val, ignoreValue))
            {
                if (target.parent != null)
                {
                    return GetValue<TValueType>(target, get, target.parent, ignoreValue);
                }
            }
            
            return val;
        }

        public static IItemDefinition GetRoot(this IItemDefinition itemDefinition)
        {
            var p = itemDefinition;
            while (p.parent != null)
            {
                p = p.parent;
            }

            return p;
        }

        public static System.Guid GetRootID(this IItemDefinition itemDefinition)
        {
            return GetRoot(itemDefinition).ID;
        }

        public static int GetParentCount(this IItemDefinition itemDefinition)
        {
            int i = 0;
            var p = itemDefinition;
            while (p.parent != null)
            {
                p = p.parent;
                i++;
            }

            return i;
        }
    }
}