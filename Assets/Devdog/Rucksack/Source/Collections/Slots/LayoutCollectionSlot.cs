using System;

namespace Devdog.Rucksack.Collections
{
    public class LayoutCollectionSlot<T> : CollectionSlot<T>
        where T: class, IEquatable<T>, IStackable
    {
        public CollectionSlot<T> occupiedBy { get; set; }
        public override bool isOccupied
        {
            get
            {
                return base.isOccupied || occupiedBy != null;
            }
        }

        public override void Clear()
        {
            base.Clear();
            occupiedBy = null;
        }
    }
}