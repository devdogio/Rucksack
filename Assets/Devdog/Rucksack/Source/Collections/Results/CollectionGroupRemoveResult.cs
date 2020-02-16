using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionGroupRemoveResult<T>
        where T : IEquatable<T>
    {
        public ICollection<T> collection { get; }
        public SlotAmountItem<T>[] affectedSlots { get; }

        public CollectionGroupRemoveResult(ICollection<T> collection, SlotAmountItem<T>[] affectedSlots)
        {
            this.collection = collection;
            this.affectedSlots = affectedSlots;
        }
    }
}