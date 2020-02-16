using System;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class ItemFactoryTests
    {
        private IItemDefinition _itemDefinition;
        private IEquippableItemDefinition _equippableItemDefinition;
        
        [SetUp]
        public void Setup()
        {
            _itemDefinition = new ItemDefinition(System.Guid.NewGuid());
            _equippableItemDefinition = new EquippableItemDefinition(System.Guid.NewGuid());
            
            ItemFactory.Bind<ItemDefinition, ItemInstance>();
        }

        [TearDown]
        public void Teardown()
        {
            ItemFactory.RemoveAllBindings();
        }

        [Test]
        public void CreateDerivedBindingTest()
        {
            ItemFactory.Bind<EquippableItemDefinition, EquippableItemInstance>();
        }
        
        [Test]
        public void CreateItemInstanceTypeShouldSucceed()
        {
            var inst = ItemFactory.CreateInstance(_itemDefinition, System.Guid.NewGuid());
            
            Assert.IsNotNull(inst);
            Assert.AreEqual(typeof(ItemInstance), inst.GetType());
        }
        
        [Test]
        public void CreateItemOfInvalidTypeShouldFail()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                var inst = ItemFactory.CreateInstance(_equippableItemDefinition, System.Guid.NewGuid());
                Assert.IsNull(inst);
            });
        }
    }
}