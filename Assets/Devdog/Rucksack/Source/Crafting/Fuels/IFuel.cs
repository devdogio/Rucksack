using System;

namespace Devdog.Rucksack.Crafting
{
    public interface IFuel : IEquatable<IFuel>, IComparable<IFuel>, IIdentifiable
    {
        string name { get; }
        string description { get; }
        
        
        Result<FuelUseResult> Use(CraftContext context);
    }
}