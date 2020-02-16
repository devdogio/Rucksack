using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    public class EquippableItemDefinition : ItemDefinition, IEquippableItemDefinition
    {
        public IEquipmentType equipmentType { get; set; }
        public string mountPoint { get; set; }

        public EquippableItemDefinition(System.Guid guid)
            : base(guid)
        { }
    }
}