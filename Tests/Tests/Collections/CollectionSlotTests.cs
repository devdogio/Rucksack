using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    internal class CollectionSlotTests
    {
        
        [Test]
        public void CollectionSlotTest()
        {
            // Arrange
            var slot = new CollectionSlot<IItemInstance>();
            
            // Act, Assert
            Assert.IsTrue(!slot.isOccupied);
            Assert.AreEqual(0, slot.amount);
            Assert.IsNull(slot.item);
        }
        
    }
}