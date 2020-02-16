using System;
using System.Collections.Generic;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    internal class ItemTests
    {

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;
        }
        
        
        [Test]
        public void ItemEqualityTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var itemDef3 = new ItemDefinition(Guid.NewGuid());
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef3);
            

            var instanceComparer = EqualityComparer<IItemInstance>.Default;
            var definitionComparer = EqualityComparer<ItemDefinition>.Default;
            
            // Act
            
            
            // Assert
            // Item - Item2 comparision (equal)
            Assert.AreEqual(item, item2); // Items should be equal, because they're based on the same definition.
            Assert.IsTrue(instanceComparer.Equals(item, item2));
            Assert.IsTrue(item == item2);
            Assert.IsFalse(item != item2);
            Assert.IsTrue(item.Equals(item2));
            
            // Item - Item3 comparision (not equal)
            Assert.AreNotEqual(item, item3);
            Assert.IsFalse(instanceComparer.Equals(item, item3));
            Assert.IsFalse(item == item3);
            Assert.IsTrue(item != item3);
            Assert.IsFalse(item.Equals(item3));
            
            // Definition comparision (not equal)
            Assert.IsFalse(definitionComparer.Equals(itemDef, itemDef3));
            Assert.IsFalse(itemDef == itemDef3);
            Assert.IsTrue(itemDef != itemDef3);
            Assert.IsFalse(itemDef.Equals(itemDef3));          
        }

        [Test]
        public void ItemCanNotHaveZeroMaxStackSizeTest()
        {
            // Arrange

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                var itemDef = new ItemDefinition(Guid.NewGuid());
                itemDef.maxStackSize = 0;
            });
        }
        
        [Test]
        public void GetIDTest()
        {
            var id = Guid.NewGuid();
            var item = new ItemDefinition(id){ maxStackSize = 3 };
            
            Assert.IsNull(item.parent);
            Assert.AreEqual(id, item.ID);            
            Assert.AreEqual(id, item.GetRootID());
            Assert.AreEqual(0, item.GetParentCount());
        }

        [Test]
        public void GetRootIDTest()
        {
            var parentID = Guid.NewGuid();
            var childID = Guid.NewGuid();
            var parentItem = new ItemDefinition(parentID)
            {
                maxStackSize = 5,
                name = "ParentItem"
            };

            var childItem = new ItemDefinition(childID, parentItem)
            {
                maxStackSize = 3,
                name = "ChildItem"
            };

            Assert.AreEqual(3, childItem.maxStackSize);
            Assert.AreEqual(5, childItem.parent.maxStackSize);
            Assert.AreEqual(5, parentItem.maxStackSize);

            Assert.AreEqual("ParentItem", parentItem.name);
            Assert.AreEqual("ParentItem", childItem.parent.name);
            Assert.AreEqual("ChildItem", childItem.name);
            
            Assert.AreEqual(parentItem, childItem.parent);
            Assert.AreEqual(childID, childItem.ID);            
            Assert.AreEqual(parentID, childItem.GetRootID());
            
            Assert.AreEqual(1, childItem.GetParentCount());
            Assert.AreEqual(0, parentItem.GetParentCount());
        }
    }
}