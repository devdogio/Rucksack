using System;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TElementType">The type used for this collection.</typeparam>
    public class Collection<TElementType> : CollectionBase<CollectionSlot<TElementType>, TElementType>
        where TElementType : class, IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable
    {
        public Collection(int slotCount, ILogger logger = null)
            : base(slotCount, logger)
        {
            GenerateSlots<CollectionSlot<TElementType>>();
        }
    }
}
