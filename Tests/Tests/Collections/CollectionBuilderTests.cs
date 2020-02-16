using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class CollectionBuilderTests
    {

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

        }


        [Test]
        public void BuildBasicCollectionTest()
        {
            var builder = new CollectionBuilder<IItemInstance>();
            
            var collection = (Collection<IItemInstance>)builder
                .SetSize(10)
                .SetSlotType<LayoutCollectionSlot<IItemInstance>>()
                .Build();

            Assert.AreEqual(10, collection.slotCount);
            Assert.AreEqual(typeof(LayoutCollectionSlot<IItemInstance>), collection.GetSlot(0).GetType());
        }
        
        
    }
}