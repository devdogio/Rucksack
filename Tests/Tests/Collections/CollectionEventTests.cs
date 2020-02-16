using System;
using NUnit.Framework;
using System.Linq;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    internal class CollectionEventTests
    {
        private Collection<IItemInstance> _collection;
        private const int CollectionSize = 10;
        
        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

//            _collection = new Collection<IItemInstance>(CollectionSize, new Logger("[Collection] "));
            _collection = new Collection<IItemInstance>(CollectionSize, new Logger("[Collection] "));
        }
        
        [Test]
        public void CollectionAddEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            CollectionAddResult eventResult = null;
            int eventCallCount = 0;
            
            // Act
            _collection.OnAddedItem += (sender, addResult) =>
            {
                eventResult = addResult;
                eventCallCount++;
            };
            var result = _collection.Add(item);
            var result2 = _collection.Add(item); // Action should fail (same instance error)
            
            // Assert
            Assert.IsNotNull(eventResult);
            Assert.AreEqual(1, eventResult.affectedSlots.Length);
            Assert.AreEqual(1, eventResult.affectedSlots[0].amount);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(Errors.CollectionAlreadyContainsSpecificInstance, result2.error);
            
            Assert.AreEqual(1, eventCallCount);
        }
        
        [Test]
        public void CollectionRemoveEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            CollectionRemoveResult<IItemInstance> eventResult = null;
            int eventCallCount = 0;
            
            // Act
            _collection.OnRemovedItem += (sender, removeResult) =>
            {
                eventResult = removeResult;
                eventCallCount++;
            };
            var result = _collection.Add(item);
            var result2 = _collection.Remove(item);
            
            // Assert
            Assert.IsNotNull(eventResult);
            Assert.AreEqual(1, eventResult.affectedSlots.Length);
            Assert.AreEqual(1, eventResult.affectedSlots[0].amount);
            
            Assert.IsNull(result.error);
            Assert.IsNull(result2.error);
            
            Assert.AreEqual(1, eventCallCount);
        }

        [Test]
        public void SetAddEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            CollectionAddResult eventResult = null;
            int eventCallCount = 0;
            
            // Act
            _collection.OnAddedItem += (sender, addResult) =>
            {
                eventResult = addResult;
                eventCallCount++;
            };
            
            var result = _collection.Set(0, item, 2);
            
            // Assert
            Assert.IsNotNull(eventResult);
            Assert.AreEqual(1, eventResult.affectedSlots.Length);
            Assert.AreEqual(2, eventResult.affectedSlots[0].amount);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(1, eventCallCount);
        }
        
        [Test]
        public void SetRemoveEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            CollectionRemoveResult<IItemInstance> eventResult = null;
            int eventCallCount = 0;
            var addResult = _collection.Set(0, item, 3);
            
            // Act
            _collection.OnRemovedItem += (sender, removeResult) =>
            {
                eventResult = removeResult;
                eventCallCount++;
            };
            
            var setRemoveResult = _collection.Set(0, item, 1);
            
            // Assert
            Assert.IsNotNull(eventResult);
            Assert.AreEqual(1, eventResult.affectedSlots.Length);
            Assert.AreEqual(2, eventResult.affectedSlots[0].amount);
            
            Assert.IsNull(addResult.error);
            Assert.IsNull(setRemoveResult.error);
            Assert.AreEqual(1, eventCallCount);
        }
        
        [Test]
        public void SetNewItemEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var itemDef2 = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);
            
            CollectionRemoveResult<IItemInstance> removeEventResult = null;
            CollectionAddResult addEventResult = null;
            int removeEventCallCount = 0;
            int addEventCallCount = 0;
            var addResult = _collection.Set(0, item, 3);
            
            // Act
            _collection.OnAddedItem += (sender, addResult1) =>
            {
                addEventResult = addResult1;
                addEventCallCount++;
            };
            
            _collection.OnRemovedItem += (sender, removeResult) =>
            {
                removeEventResult = removeResult;
                removeEventCallCount++;
            };
            
            var setRemoveResult = _collection.Set(0, item2, 2);
            
            // Assert
            Assert.IsNull(addResult.error);
            Assert.IsNull(setRemoveResult.error);
            
            Assert.AreEqual(1, removeEventResult.affectedSlots.Length);
            Assert.AreEqual(3, removeEventResult.affectedSlots[0].amount);
            
            Assert.AreEqual(1, addEventResult.affectedSlots.Length);
            Assert.AreEqual(2, addEventResult.affectedSlots[0].amount);
            
            Assert.AreEqual(1, removeEventCallCount);
            Assert.AreEqual(1, addEventCallCount);
        }

        [Test]
        public void ClearEmptyCollectionEventTest()
        {
            CollectionSlotsChangedResult changedResult = null;
            int changedEventCount = 0;
            
            _collection.OnSlotsChanged += (sender, e) =>
            {
                changedResult = e;
                changedEventCount++;
            };

            _collection.Clear();
            
            Assert.AreEqual(0, changedEventCount);
        }
        
        [Test]
        public void ClearCollectionEventTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            CollectionSlotsChangedResult changedResult = null;
            int changedEventCount = 0;

            _collection.Set(0, item, 3);
            _collection.Set(1, (ItemInstance)item.Clone(), 2);
            _collection.Set(3, (ItemInstance)item.Clone(), 4);
            
            _collection.OnSlotsChanged += (sender, e) =>
            {
                changedResult = e;
                changedEventCount++;
            };

            _collection.Clear();
            
            Assert.AreEqual(1, changedEventCount);
            Assert.AreEqual(3, changedResult.affectedSlots.Length);
            
            Assert.AreEqual(0, changedResult.affectedSlots[0]);
            Assert.AreEqual(1, changedResult.affectedSlots[1]);
            Assert.AreEqual(3, changedResult.affectedSlots[2]);
            
            for (int i = 0; i < _collection.slotCount; i++)
            {
                Assert.AreEqual(0, _collection.GetAmount(i));
            }
        }

        [Test]
        public void GenerateSlotsChangedEventTest()
        {
            CollectionSlotsChangedResult changedResult = null;
            int changedEventCount = 0;

            _collection.OnSlotsChanged += (sender, e) =>
            {
                changedResult = e;
                changedEventCount++;
            };

            _collection.GenerateSlotsRange<CollectionSlot<IItemInstance>>(5, 9);
            
            Assert.AreEqual(1, changedEventCount);
            Assert.AreEqual(5, changedResult.affectedSlots.Length);
            
            Assert.AreEqual(5, changedResult.affectedSlots[0]);
            Assert.AreEqual(6, changedResult.affectedSlots[1]);
            Assert.AreEqual(7, changedResult.affectedSlots[2]);
            Assert.AreEqual(8, changedResult.affectedSlots[3]);
            Assert.AreEqual(9, changedResult.affectedSlots[4]);
        }
        
        [Test]
        public void ExpandEventTest()
        {
            CollectionSizeChangedResult changedResult = null;
            int changedEventCount = 0;

            _collection.OnSizeChanged += (sender, e) =>
            {
                changedResult = e;
                changedEventCount++;
            };

            _collection.Expand<CollectionSlot<IItemInstance>>(5);

            Assert.AreEqual(1, changedEventCount);
            
            Assert.AreEqual(10, changedResult.sizeBefore);
            Assert.AreEqual(15, changedResult.sizeAfter);
        }
        
        [Test]
        public void ShrinkEventTest()
        {
            CollectionSizeChangedResult changedResult = null;
            int changedEventCount = 0;

            _collection.OnSizeChanged += (sender, e) =>
            {
                changedResult = e;
                changedEventCount++;
            };

            _collection.Shrink(5);

            Assert.AreEqual(1, changedEventCount);
            
            Assert.AreEqual(10, changedResult.sizeBefore);
            Assert.AreEqual(5, changedResult.sizeAfter);
        }

        [Test]
        public void ItemUseCollectionUpdateTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = (CollectionItemInstance)item.Clone();
            
            _collection.Set(0, item);
            _collection.Set(1, item2);


            CollectionRemoveResult<IItemInstance> removeResult = null;
            int removeCallCount = 0;
            
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeResult = result;
                removeCallCount++;
            };

            var used = item.Use(null, new ItemContext());
            
            Assert.IsNull(used.error);
            
            Assert.AreEqual(1, removeResult.affectedSlots[0].amount);
            Assert.AreEqual(1, removeCallCount);
            
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.IsNull(_collection[0]);
        }
        
        [Test]
        public void ItemUseNotEnoughInCollectionTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            
            _collection.Set(0, item, 2);

            CollectionRemoveResult<IItemInstance> removeResult = null;
            int removeCallCount = 0;

            _collection.OnRemovedItem += (sender, result) =>
            {
                removeResult = result;
                removeCallCount++;
            };

            var context1 = new ItemContext
            {
                useAmount = 3
            };
            var usedResult = item.Use(null, context1);

            Assert.AreEqual(Errors.CollectionDoesNotContainItem, usedResult.error);
            Assert.AreEqual(0, removeCallCount);
            Assert.AreEqual(2, _collection.GetAmount(0));
        }
        
        [Test]
        public void ItemUseNotInCollectionTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
//            item.amountContainer.SetAmount(10);
            
            var used = item.Use(null, new ItemContext());
            
            Assert.IsNull(used.error);
            Assert.AreEqual(1, used.result.usedAmount);
        }
        
        [Test]
        public void ItemUseAmountTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = (CollectionItemInstance)item.Clone();
            
            _collection.Set(0, item, 5);
            _collection.Set(1, item2, 4);

            var context1 = new ItemContext
            {
                useAmount = 3
            };

            var used = item.Use(null, context1);
            
            Assert.IsNull(used.error);
            Assert.AreEqual(3, used.result.usedAmount);
            Assert.AreEqual(2, _collection.GetAmount(0));
        }
        
        [Test]
        public void ItemUseAmountReadOnlyCollectionTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = (CollectionItemInstance)item.Clone();
            
            _collection.Set(0, item, 5);
            _collection.Set(1, item2, 4);

            var context1 = new ItemContext
            {
                useAmount = 3
            };

            _collection.isReadOnly = true;
            
            var used = item.Use(null, context1);
            
            Assert.AreEqual(Errors.CollectionIsReadOnly, used.error);
            Assert.AreEqual(5, _collection.GetAmount(0));
        }
        
        [Test]
        public void SwapOrMergeSlotsChangedEventsTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            CollectionSlotsChangedResult changeResult = null;
            object changeObj = null;
            int changeCallCount = 0;
            
            var set1 = _collection.Set(0, item, 3);
            var set2 = _collection.Set(1, item2, 2);

            Assert.IsTrue(set1.result);
            Assert.IsTrue(set2.result);

            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeObj = sender;
                changeResult = result;

                changeCallCount++;
            };
            
            var swap = _collection.SwapOrMerge(0, _collection, 1, 3);
            
            Assert.IsTrue(swap.result);
            
            Assert.AreSame(item2, _collection[0]);
            Assert.AreSame(item, _collection[1]);
            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(3, _collection.GetAmount(1));
            
            Assert.AreEqual(1, changeCallCount);
            Assert.AreSame(changeObj, _collection);
            Assert.AreEqual(2, changeResult.affectedSlots.Length);
            Assert.IsTrue(changeResult.affectedSlots.Contains(0));
            Assert.IsTrue(changeResult.affectedSlots.Contains(1));
        }
    }
}