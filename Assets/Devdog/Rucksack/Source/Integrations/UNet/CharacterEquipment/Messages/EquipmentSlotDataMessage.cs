using System.Linq;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Database;
using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class EquipmentSlotDataMessage : MessageBase
    {
        public GuidMessage[] equipmentTypeGuids;
        
        public EquipmentCollectionSlot<IEquippableItemInstance> ToSlotInstance(IDatabase<UnityEquipmentType> database, IEquippableCharacter<IEquippableItemInstance> character)
        {
            var slot = new EquipmentCollectionSlot<IEquippableItemInstance>
            {
                equipmentTypes = equipmentTypeGuids.Select(o => database.Get(new Identifier(o.guid)).result).Cast<IEquipmentType>().ToArray()
            };

            return slot;
        }
    }
}