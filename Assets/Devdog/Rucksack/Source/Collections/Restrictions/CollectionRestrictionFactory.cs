using System;
using System.Reflection;

namespace Devdog.Rucksack.Collections
{
    public static class CollectionRestrictionFactory
    {
        public static object Create<TElementType>(Type restrictionType, ICollection<TElementType> collection)
            where TElementType : IEquatable<TElementType>
        {
            return Create(restrictionType, (ICollection)collection);
        }
        
        public static TRestrictionType Create<TRestrictionType>()
        {
            return (TRestrictionType)Create(typeof(TRestrictionType), null);
        }

        public static TRestrictionType Create<TRestrictionType>(ICollection collection)
        {
            return (TRestrictionType)Create(typeof(TRestrictionType), collection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static object Create(Type restrictionType, ICollection collection = null)
        {
            ConstructorInfo constructor = null;
            if (collection != null)
            {
                constructor = restrictionType.GetConstructor(new Type[]{ collection.GetType() });
                if (constructor != null)
                {
                    return constructor.Invoke(new object[] {collection});
                }   
            }
            
            constructor = restrictionType.GetConstructor(new Type[] { });
            if (constructor != null)
            {
                return constructor.Invoke(new object[] {});
            }
            
            throw new MissingMethodException($"Couldn't find constructor with a parameter for {collection?.GetType()} or an empty constructor. Restriction could not be created.");
        }
    }
}