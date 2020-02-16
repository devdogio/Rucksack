using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;

namespace Devdog.Rucksack.Tests
{
    public class MockedEquippableItemInstance : EquippableItemInstance, IEquatable<MockedEquippableItemInstance>
    {
        public int onEquippedCallCount { get; private set; }
        public int onUnEquippedCallCount { get; private set; }
     
        private ILogger _logger = new Logger("[EquippableItem] ");
        
        public MockedEquippableItemInstance(Guid ID, EquippableItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        {
        }

        public override void OnEquipped(int index, IEquipmentCollection<IEquippableItemInstance> collection)
        {
            base.OnEquipped(index, collection);
            onEquippedCallCount++;
            
            _logger.Log($"{ToString()} equipped {onEquippedCallCount} times");
        }

        public override void OnUnEquipped(int index, IEquipmentCollection<IEquippableItemInstance> collection)
        {
            base.OnUnEquipped(index, collection);
            onUnEquippedCallCount++;
            
            _logger.Log($"{ToString()} unEquipped {onUnEquippedCallCount} times");
        }

        public override object Clone()
        {
            var clone = (MockedEquippableItemInstance)base.Clone();
            clone.onEquippedCallCount = 0;
            clone.onUnEquippedCallCount = 0;
            return clone;
        }

        public bool Equals(MockedEquippableItemInstance other)
        {
            return Equals((IEquippableItemInstance) other);
        }
    }
}