using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionSlotsChangedResult : EventArgs
    {
        public int[] affectedSlots { get; }
        public CollectionSlotsChangedResult(int[] affectedSlots)
        {
            this.affectedSlots = affectedSlots;
        }
    }
}