using System;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class EquipmentCollection<TEquippableType> : EquipmentCollectionBase<EquipmentCollectionSlot<TEquippableType>, TEquippableType>
        where TEquippableType : class, IEquatable<TEquippableType>, ICloneable, IEquippable<TEquippableType>, ICollectionSlotEntry, IIdentifiable, IStackable
    {

        public EquipmentCollection(int slotCount, IEquippableCharacter<TEquippableType> character, ILogger logger = null)
            : base(slotCount, character, logger)
        {
        }
    }
}