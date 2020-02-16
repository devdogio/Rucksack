using System;

namespace Devdog.Rucksack.CharacterEquipment
{
    public interface IEquippable<T>
        where T : IEquatable<T>
    {
        IEquipmentCollection<T> equippedTo { get; }
        IEquipmentType equipmentType { get; }

        // Callbacks
        void OnEquipped(int index, IEquipmentCollection<T> collection);
        void OnUnEquipped(int index, IEquipmentCollection<T> collection);
    }
}