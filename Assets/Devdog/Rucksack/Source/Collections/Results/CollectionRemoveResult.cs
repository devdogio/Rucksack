using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionRemoveResult<T> : EventArgs
    {
        public SlotAmountItem<T>[] affectedSlots { get; }
        public CollectionRemoveResult(SlotAmountItem<T>[] affectedSlots)
        {
            this.affectedSlots = affectedSlots;
        }
    }
}