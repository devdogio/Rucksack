using System;
using System.Collections.Generic;
using System.Reflection;

namespace Devdog.Rucksack.Items
{
    /// <summary>
    /// This class uses generics with a UnityEngine.Object restriction, because the standard extension methods use an EqualityComparer<T>.
    /// Because Unity wrapped the native objects with some C# code to make it equate to null an actual EqualityComparer<T> won't work, and won't recognize items to be equal.
    /// </summary>
    public static class UnityItemDefinitionExtensionMethods
    {
        public delegate T GetValueDelegate<out T>(IItemDefinition target);
        public delegate T GetValueDelegate<out T, in TDefType>(TDefType target) where TDefType : IItemDefinition;

        public static TValueType GetUnityObjectValue<TValueType, TDefType>(this TDefType target, GetValueDelegate<TValueType, TDefType> get, TValueType ignoreValue = default(TValueType))
            where TValueType : UnityEngine.Object
            where TDefType : IItemDefinition
        {
            var val = get(target);
            if (val == ignoreValue)
            {
                if (target.parent != null)
                {
                    return GetUnityObjectValue<TValueType, TDefType>((TDefType)target.parent, get, ignoreValue);
                }
            }
            
            return val;
        }
        
        public static TValueType GetUnityObjectValue<TValueType>(this IItemDefinition itemDef, GetValueDelegate<TValueType> get, TValueType ignoreValue = default(TValueType))
            where TValueType : UnityEngine.Object
        {
            return GetUnityObjectValue<TValueType>(itemDef, get, itemDef, ignoreValue);
        }
        
        public static TValueType GetUnityObjectValue<TValueType>(this IItemDefinition itemDef, GetValueDelegate<TValueType> get, IItemDefinition target, TValueType ignoreValue)
            where TValueType : UnityEngine.Object
        {
            var val = get(target);
            if (val == ignoreValue)
            {
                if (target.parent != null)
                {
                    return GetUnityObjectValue<TValueType>(target, get, target.parent, ignoreValue);
                }
            }
            
            return val;
        }
    }
}