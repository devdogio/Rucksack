using System;

namespace Devdog.Rucksack.Crafting
{
    public sealed class CraftResult<T> : EventArgs
    {
        /// <summary>
        /// Used items to craft this item.
        /// </summary>
        public T[] usedItems { get; }

        public IFuel usedFuel { get; }
        public int usedFuelAmount { get; }
        
        
        public T[] rewards { get; }
        
        // Currency reward / cost?
        
        public CraftResult(T[] rewards, T[] usedItems, IFuel usedFuel = null, int usedFuelAmount = 0)
        {
            this.rewards = rewards;
            this.usedItems = usedItems;

            this.usedFuel = usedFuel;
            this.usedFuelAmount = usedFuelAmount;
        }
    }
}