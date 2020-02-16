using System;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class EquipmentCollectionSlot<TElementType> : CollectionSlot<TElementType>
        where TElementType: class, IIdentifiable, IEquatable<TElementType>, IStackable, IEquippable<TElementType>
    {
        public IEquipmentType[] equipmentTypes { get; set; }
        
        public EquipmentCollectionSlot()
        {
            equipmentTypes = new IEquipmentType[0];
        }
    }
}