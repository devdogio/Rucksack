using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionGroupAddResult<T>
        where T : IEquatable<T>
    {
        public ICollection<T> collection { get; }
        
        /// <summary>
        /// All affected slots by the add operation.
        /// </summary>
        public SlotAmount[] affectedSlots { get; }
        
        public CollectionGroupAddResult(ICollection<T> collection, SlotAmount[] affectedSlots)
        {
            this.collection = collection;
            this.affectedSlots = affectedSlots;
        }
    }
}