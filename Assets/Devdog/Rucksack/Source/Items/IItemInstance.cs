using System;
using Devdog.General2;

namespace Devdog.Rucksack.Items
{
    public interface IItemInstance : 
        IComparable<IItemInstance>, 
        IEquatable<IItemInstance>, 
        IIdentifiable, IStackable, ICloneable, IShapeOwner2D
    {
        IItemDefinition itemDefinition { get; }

        Result<bool> CanUse(Character character, ItemContext useContext);
        Result<ItemUsedResult> Use(Character character, ItemContext useContext);
    }
}