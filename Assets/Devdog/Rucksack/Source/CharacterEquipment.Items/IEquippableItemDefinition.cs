﻿using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.CharacterEquipment.Items
{
    public interface IEquippableItemDefinition : IItemDefinition
    {
        IEquipmentType equipmentType { get; }
        string mountPoint { get; set; }
    }
}