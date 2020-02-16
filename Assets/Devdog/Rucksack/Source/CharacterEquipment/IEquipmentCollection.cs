using System;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.CharacterEquipment
{
    public interface IEquipmentCollection<TElementType> : ICollection<TElementType>
        where TElementType: IEquatable<TElementType>
    {
        IEquippableCharacter<TElementType> characterOwner { get; }
        
        void SetEquipmentTypes(int index, IEquipmentType[] equipmentTypes);
        void SetAllEquipmentTypes(IEquipmentType[][] equipmentTypes);

//        /// <summary>
//        /// Force add an item to this collection. This will un-equip any items that are incompatible.
//        /// </summary>
//        Result<CollectionAddResult> ForceAdd(TElementType item, int amount = 1);

    }
}