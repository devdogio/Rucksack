using System;
using System.Linq;
using Devdog.Rucksack.CharacterEquipment.Items;

namespace Devdog.Rucksack.CharacterEquipment
{
    /// <summary>
    /// Simple helper class to serialize EquipmentCollectionSlot for the unity inspector.
    /// </summary>
    [Serializable]
    public sealed class UnitySerializedEquipmentCollectionSlot
    {
        public UnityEquipmentType[] equipmentTypes = new UnityEquipmentType[0];

        public EquipmentCollectionSlot<IEquippableItemInstance> ToSlotInstance(IEquippableCharacter<IEquippableItemInstance> character)
        {
            return new EquipmentCollectionSlot<IEquippableItemInstance>()
            {
                equipmentTypes = equipmentTypes.Cast<IEquipmentType>().ToArray(),
            };
        }
    }
}