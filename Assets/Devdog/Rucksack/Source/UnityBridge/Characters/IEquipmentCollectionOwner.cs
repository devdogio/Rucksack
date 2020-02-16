using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.Characters
{
    public interface IEquipmentCollectionOwner
    {
        CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>> equipmentCollectionGroup { get; }
    }
}