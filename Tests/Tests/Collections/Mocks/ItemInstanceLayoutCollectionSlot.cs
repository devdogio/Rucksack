using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    public class ItemInstanceLayoutCollectionSlot<T> : LayoutCollectionSlot<T>
        where T: class, IItemInstance, IEquatable<T>
    {
        public override bool isOccupied
        {
            get
            {
                return base.isOccupied && occupiedBy == null;
            }
        }

        public override void Clear()
        {
            base.Clear();
            occupiedBy = null;
        }
    }
}