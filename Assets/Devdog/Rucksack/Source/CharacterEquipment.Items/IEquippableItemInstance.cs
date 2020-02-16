using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.CharacterEquipment.Items
{
    public interface IEquippableItemInstance : IItemInstance, IEquippable<IEquippableItemInstance>, IEquatable<IEquippableItemInstance>, ICollectionSlotEntry
    {
        new IEquippableItemDefinition itemDefinition { get; }
    }
}