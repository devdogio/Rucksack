using System;

namespace Devdog.Rucksack.Crafting
{
    public sealed class FuelUseResult : EventArgs
    {
        public IFuel fuel { get; }
        public int amount { get; }
        
        public FuelUseResult(IFuel fuel, int amount)
        {
            this.fuel = fuel;
            this.amount = amount;
        }
    }
}