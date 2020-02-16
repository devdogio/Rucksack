using System;

namespace Devdog.Rucksack.Crafting
{
    public interface IBlueprint<T> : IIdentifiable, IEquatable<IBlueprint<T>>
        where T: IIdentifiable
    {

//        Result<bool> CanCreate();
//        Result<CraftResult<T>> Create();
        
    }
}