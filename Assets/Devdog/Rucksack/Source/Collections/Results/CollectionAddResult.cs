using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionAddResult : EventArgs
    {
        public SlotAmount[] affectedSlots { get; }
        public CollectionAddResult(SlotAmount[] affectedSlots)
        {
            this.affectedSlots = affectedSlots;
        }
    }
}