using System;

namespace Devdog.Rucksack.Crafting
{
    public abstract class Fuel : IFuel
    {
        public Guid ID { get; }

        /// <summary>
        /// The temperature in celcius at which this fuel burns.
        /// </summary>
        public int temperature = 100;

        public virtual string name { get; }
        public virtual string description { get; }

        public Fuel(Guid guid)
        {
            ID = guid;
        }
        
        
        public Result<FuelUseResult> Use(CraftContext context)
        {
            throw new NotImplementedException();
        }
        
        
        
        public bool Equals(IFuel other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IFuel other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Fuel other)
        {
            throw new NotImplementedException();
        }
    }
}