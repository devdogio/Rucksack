using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{

    internal class CollectionTests
    {

        private CollectionAccessibleMethods<IItemInstance> _collection;
        private const int CollectionSize = 10;

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _collection = new CollectionAccessibleMethods<IItemInstance>(CollectionSize, new Logger("[Collection] "));
//            _collection.GenerateSlots<ItemInstanceLayoutCollectionSlot>();

            UnityEngine.Assertions.Assert.raiseExceptions = true;
        }

        [Test]
        public void CreateNewCollectionSizeTest()
        {
            // Arrange, Act, Assert
            Assert.AreEqual(CollectionSize, _collection.slots.Length);
        }

        [Test]
        public void CollectinoSlotsShouldNotBeNullAndEmptyTest()
        {
            // Arrange, Act, Assert
            for (int i = 0; i < _collection.slots.Length; i++)
            {
                Assert.NotNull(_collection.slots[i]);
                Assert.IsTrue(!_collection.slots[i].isOccupied);
            }
        }

        [Test]
        public void AddObjectToCollectionTest1()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var canAdd = _collection.CanAdd(item);
            var result = _collection.Add(item);

            // Assert
            Assert.IsTrue(canAdd.result, "CanAdd returned false with message \"" + canAdd.error + "\"");
            Assert.AreEqual(10, _collection.slots.Length, "Collection length should not have been affected by add operation.");

            // Result values
            Assert.IsNull(result.error);
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(0, result.result.affectedSlots[0].slot);

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(1, _collection.slots[result.result.affectedSlots[0].slot].amount);
        }

        [Test]
        public void GetCanAddAmountTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 1};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var result = _collection.GetCanAddAmount(item);

            // Assert
            Assert.AreEqual(10, result.result);
        }

        [Test]
        public void CanRemoveAmountTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 50};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, item, 5);
            _collection.Set(2, item, 3);
            _collection.Set(5, item, 12);

            var canRemoveAll = _collection.CanRemove(item, _collection.GetAmount(item));

            // Assert
            Assert.IsTrue(canRemoveAll.result);
            Assert.IsNull(canRemoveAll.error);
        }

        [Test]
        public void AddExceedingCollectionSize()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var result = _collection.Add(item, 51);

            // Assert
            Assert.AreEqual(Errors.CollectionFull, result.error);
            Assert.IsNull(result.result);
        }

        [Test]
        public void AddObjectToCollectionMaxStackSizeTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 1};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var result = _collection.Add(item);
            var result2 = _collection.Add(item2);

            // Assert
            Assert.IsNull(result.error);
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[result.result.affectedSlots[0].slot], "Result itemGuid does not match expected itemGuid");

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Object has not been placed where expected ( [0] )");
            Assert.AreEqual(item2, _collection.slots[1].item, "Object has not been placed where expected ( [1] )");
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(1, _collection.slots[result.result.affectedSlots[0].slot].amount, "Item amount does not match.");
            Assert.AreEqual(1, result2.result.affectedSlots.Length);
            Assert.AreEqual(1, _collection.slots[result2.result.affectedSlots[0].slot].amount, "Item amount does not match.");
        }

        [Test]
        public void AddSameObjectMulitpleTimesShouldFailTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var result = _collection.Add(item);
            var result2 = _collection.Add(item);

            // Assert (Result values)
            Assert.IsNull(result.error);
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[result.result.affectedSlots[0].slot], "Result itemGuid does not match expected itemGuid");

            Assert.AreEqual(Errors.CollectionAlreadyContainsSpecificInstance, result2.error);

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Object has not been placed where expected ( [0] )");
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(1, _collection.slots[result.result.affectedSlots[0].slot].amount, "Item amount does not match.");
        }

        [Test]
        public void AddMultipleItemsToCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            // Act
            var result = _collection.Add(item);
            var result2 = _collection.Add(item2);

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Object has not been placed where expected ( [0] )");
            Assert.AreEqual(item2, _collection.slots[1].item, "Object has not been placed where expected ( [0] )");
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(1, _collection.slots[result.result.affectedSlots[0].slot].amount, "Item amount does not match.");
            Assert.AreEqual(1, result2.result.affectedSlots.Length);
            Assert.AreEqual(1, _collection.slots[result2.result.affectedSlots[0].slot].amount, "Item amount does not match.");
        }

        [Test]
        public void AddMultipleOfSameItemShouldStackTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            var item2 = new ItemInstance(Guid.NewGuid(), itemDef); // TODO: Consider implementing IDisposable? < Dispose of item once it's merged / used up.

            // Act
            var result = _collection.Add(item);
            var result2 = _collection.Add(item2);

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Object has not been placed where expected ( [0] )");
            Assert.IsNull(_collection.slots[1].item, "Slot with itemGuid 1 should remain null. Item should've been stacked on 0.");
            Assert.AreEqual(1, result.result.affectedSlots.Length, "Item amount does not match.");
            Assert.AreEqual(2, _collection.slots[result.result.affectedSlots[0].slot].amount, "Item amount does not match.");
            Assert.AreEqual(1, result2.result.affectedSlots.Length, "Item amount does not match.");
            Assert.AreEqual(2, _collection.slots[result2.result.affectedSlots[0].slot].amount, "Item amount does not match.");
//            Assert.AreEqual(0, item2.amount, "Item2 amount should've been reset to 0");
        }

        [Test]
        public void AddItemsInIllogicalOrderShouldStackTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef2);

            // Act
            var add1 = _collection.Add(item);
            var add2 = _collection.Add(item3);
            var add3 = _collection.Add(item2);

            // Collection values
            Assert.IsNull(add1.error);
            Assert.IsNull(add2.error);
            Assert.IsNull(add3.error);

            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(2, _collection.slots[0].amount);

            Assert.AreEqual(item3, _collection.slots[1].item);
        }

        [Test]
        public void CollectionCanAddAmountTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var amount = _collection.GetCanAddAmount(item);

            // Collection values
            Assert.AreEqual(itemDef.maxStackSize * CollectionSize, amount.result);
        }

        [Test]
        public void CollectionCanAddAmountTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            // Act
            _collection.Set(0, item2, 2);
            _collection.Set(1, (IItemInstance) item2.Clone(), 3);

            var amount = _collection.GetCanAddAmount(item);

            // Collection values
            Assert.AreEqual((itemDef.maxStackSize * CollectionSize) - (itemDef.maxStackSize * 2), amount.result);
        }

        [Test]
        public void CollectionAmountTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef2);

            // Act
            _collection.Set(0, item, 2);
            _collection.Set(1, item2, 3);
            _collection.Set(4, item3, 2);

            // Collection values
            Assert.AreEqual(2, _collection.GetAmount(item));
            Assert.AreEqual(5, _collection.GetAmount(item2));
            Assert.AreEqual(5, _collection.GetAmount(item3));
        }

        [Test]
        public void RemoveSingleItemFromCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Add(item);
            _collection.Add(item2);
            var canRemove = _collection.CanRemove(item2);
            var result = _collection.Remove(item2);

            // Collection values
            Assert.IsNull(canRemove.error);
            Assert.IsTrue(canRemove.result);

            Assert.IsNull(result.error);
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(1, result.result.affectedSlots[0].amount);

            Assert.AreEqual(item, _collection.slots[0].item, "Object has not been placed where expected ( [0] )");
            Assert.AreEqual(1, _collection.slots[0].amount, "Slot should've been depleted to 0");
        }

        [Test]
        public void RemoveMultipleItemFromCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Add(item);
            _collection.Add(item2);
            var canRemove = _collection.CanRemove(item2, 2);
            var result = _collection.Remove(item2, 2);

            // Collection values
            Assert.IsNull(canRemove.error);
            Assert.IsTrue(canRemove.result);

            Assert.IsNull(result.error);
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(2, result.result.affectedSlots[0].amount);

            Assert.IsNull(_collection.slots[0].item, "Object should've been removed when stack got depleted.");
            Assert.AreEqual(0, _collection.slots[0].amount, "Slot should've been depleted to 0");
        }

        [Test]
        public void RemoveTooManyItemsShouldFailTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Add(item);
            _collection.Add(item2);
            var canRemove = _collection.CanRemove(item2, 3);
            var result = _collection.Remove(item2, 3);

            // Collection values
            Assert.IsFalse(canRemove.result);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, canRemove.error);

            Assert.IsNull(result.result);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, result.error);

            Assert.AreEqual(item, _collection.slots[0].item, "Item should've remained in itemGuid [0]");
            Assert.AreEqual(2, _collection.slots[0].amount, "Amount should be 2. Remove action should fail.");
        }

        [Test]
        public void RemoveItemsSeperatedInDifferentSlotsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, item, 3);
            _collection.Set(2, item2, 2);
            _collection.Set(7, item3, 3);

            var canRemove = _collection.CanRemove(item2, 7);
            var result = _collection.Remove(item2, 7);

            // Collection values
            Assert.IsNull(canRemove.error);
            Assert.IsTrue(canRemove.result);

            Assert.IsNull(result.error);
            Assert.AreEqual(3, result.result.affectedSlots.Length);
            // First try to remove the exact item instance, therefore it comes first in the array
            Assert.AreEqual(2, result.result.affectedSlots[0].slot);
            Assert.AreEqual(0, result.result.affectedSlots[1].slot);
            Assert.AreEqual(7, result.result.affectedSlots[2].slot);
            
            // First try to remove the exact item instance, therefore it comes first in the array
            Assert.AreEqual(2, result.result.affectedSlots[0].amount);
            Assert.AreEqual(3, result.result.affectedSlots[1].amount);
            Assert.AreEqual(2, result.result.affectedSlots[2].amount);


            Assert.IsNull(_collection.slots[0].item);
            Assert.AreEqual(0, _collection.slots[0].amount);
            
            Assert.IsNull(_collection.slots[2].item);
            Assert.AreEqual(0, _collection.slots[2].amount);

            Assert.IsNotNull(_collection.slots[7].item);
            Assert.AreEqual(1, _collection.slots[7].amount);
        }

        [Test]
        public void ClearCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            // Act
            _collection.Add(item);
            _collection.Add(item2);

            _collection.Clear();

            // Collection values
            Assert.IsNull(_collection.slots[0].item, "Slot should've been cleared.");
            Assert.AreEqual(0, _collection.slots[0].amount, "Slot amount should've been reset to 0");

            Assert.IsNull(_collection.slots[1].item, "Slot should've been cleared.");
            Assert.AreEqual(0, _collection.slots[1].amount, "Slot amount should've been reset to 0");
        }

        [Test]
        public void ClearEmptyCollection()
        {
            // Arrange
            int removeCount = 0;
            int changedCount = 0;

            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changedResult = null;
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeResult = result;
                removeCount++;
            };

            _collection.OnSlotsChanged += (sender, result) =>
            {
                changedResult = result;
                changedCount++;
            };

            // Act
            _collection.Clear();

            // Assert
            Assert.AreEqual(0, removeCount);
            Assert.AreEqual(0, changedCount);
        }

        [Test]
        public void ClearCollectionEventsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);
            var item3 = (ItemInstance)item.Clone();

            int removeCount = 0;
            int changedCount = 0;

            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changedResult = null;

            _collection.Set(0, item, 3);
            _collection.Set(1, item2, 5);
            _collection.Set(4, item3, 1);

            _collection.OnRemovedItem += (sender, result) =>
            {
                removeResult = result;
                removeCount++;
            };

            _collection.OnSlotsChanged += (sender, result) =>
            {
                changedResult = result;
                changedCount++;
            };

            // Act
            _collection.Clear();

            // Assert
            Assert.AreEqual(1, removeCount);
            Assert.AreEqual(1, changedCount);

            Assert.AreEqual(3, removeResult.affectedSlots.Length);
            Assert.AreEqual(0, removeResult.affectedSlots[0].slot);
            Assert.AreEqual(1, removeResult.affectedSlots[1].slot);
            Assert.AreEqual(4, removeResult.affectedSlots[2].slot);
            
            Assert.AreEqual(3, removeResult.affectedSlots[0].amount);
            Assert.AreEqual(5, removeResult.affectedSlots[1].amount);
            Assert.AreEqual(1, removeResult.affectedSlots[2].amount);

            Assert.AreSame(item, removeResult.affectedSlots[0].item);
            Assert.AreSame(item2, removeResult.affectedSlots[1].item);
            Assert.AreSame(item3, removeResult.affectedSlots[2].item);

            
            Assert.AreEqual(3, changedResult.affectedSlots.Length);
            Assert.AreEqual(0, changedResult.affectedSlots[0]);
            Assert.AreEqual(1, changedResult.affectedSlots[1]);
            Assert.AreEqual(4, changedResult.affectedSlots[2]);

            for (int i = 0; i < _collection.slots.Length; i++)
            {
                Assert.IsNull(_collection.slots[i].item);
                Assert.AreEqual(0, _collection.slots[i].amount);
            }
        }

        [Test]
        public void CollectionContainsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            // Act
            _collection.Add(item);
            var contains = _collection.Contains(item);
            var contains2 = _collection.Contains(item2);

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Item should be in this itemGuid.");
            Assert.IsNull(_collection.slots[1].item, "Item should be in this itemGuid.");

            Assert.IsTrue(contains);
            Assert.IsFalse(contains2);
        }

        [Test]
        public void CollectionIndexOfTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef3 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef3);

            // Act
            _collection.Add(item);
            _collection.Add(item3);
            var contains = _collection.IndexOf(item);
            var contains2 = _collection.IndexOf(item2);
            var contains3 = _collection.IndexOf(item3);

            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Item should be in this itemGuid.");
            Assert.AreEqual(item3, _collection.slots[1].item, "Item should be in this itemGuid.");

            Assert.AreEqual(0, contains);
            Assert.AreEqual(-1, contains2);
            Assert.AreEqual(1, contains3);
        }


        [Test]
        public void CollectionAmountOfItemsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var itemDef3 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 999};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef2);
            var item4 = new ItemInstance(Guid.NewGuid(), itemDef3);

            // Act
            _collection.Add(item);
            _collection.Add(item2);
            _collection.Add(item3);

            // Assert
            Assert.AreEqual(2, _collection.GetAmount(item));
            Assert.AreEqual(2, _collection.GetAmount(item2));
            Assert.AreEqual(1, _collection.GetAmount(item3));
            Assert.AreEqual(0, _collection.GetAmount(item4));
        }


        [Test]
        public void GenerateSlotsTest()
        {
            // Arrange

            // Act
            _collection.GenerateSlots<FakeCollectionSlot>();

            // Assert
            foreach (var slot in _collection.slots)
            {
                Assert.AreEqual(slot.GetType(), typeof(FakeCollectionSlot));
                Assert.IsTrue(!slot.isOccupied);
            }
        }

        [Test]
        public void GenerateSlotsRangeTest()
        {
            // Arrange

            // Act
            _collection.GenerateSlotsRange<FakeCollectionSlot>(0, 3);

            // Assert
            Assert.AreEqual(_collection.slots[0].GetType(), typeof(FakeCollectionSlot));
            Assert.AreEqual(_collection.slots[1].GetType(), typeof(FakeCollectionSlot));
            Assert.AreEqual(_collection.slots[2].GetType(), typeof(FakeCollectionSlot));
            Assert.AreEqual(_collection.slots[3].GetType(), typeof(FakeCollectionSlot));

            for (int i = 4; i < _collection.slots.Length; i++)
            {
                Assert.AreEqual(_collection.slots[i].GetType(), typeof(CollectionSlot<IItemInstance>));
                Assert.IsTrue(!_collection.slots[i].isOccupied);
            }
        }

        [Test]
        public void GenerateSlotsOutOfRangeShouldThrowExceptionTest()
        {
            // Arrange

            // Act
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                _collection.GenerateSlotsRange<FakeCollectionSlot>(8, 11);
            });

            // Assert

        }

        [Test]
        public void RemoveMultipleElementsSpreadOverSlotsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 1};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Add(item);
            _collection.Add(item2);

            var removed = _collection.Remove(item, 2);

            // Assert
            Assert.IsNull(_collection.slots[0].item);
            Assert.IsNull(_collection.slots[1].item);

            Assert.IsNull(removed.error);
            Assert.AreEqual(2, removed.result.affectedSlots.Length);
            Assert.AreEqual(1, removed.result.affectedSlots[0].amount);
            Assert.AreEqual(1, removed.result.affectedSlots[1].amount);
        }

        [Test]
        public void AddMoreItemsThanMaxStackSizeAllowsItemsShouldBeSplitOverSlotsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Add(item, 8);

            // Assert
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);
            Assert.AreEqual(item, _collection.slots[1].item);
            Assert.AreEqual(3, _collection.slots[1].amount);
            Assert.AreEqual(item, _collection.slots[2].item);
            Assert.AreEqual(2, _collection.slots[2].amount);
        }

        [Test]
        public void AddMoreItemsThanMaxStackSizeAllowsItemsShouldBeSplitOverSlotsTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, (IItemInstance) item.Clone(), 2);
            _collection.Set(6, (IItemInstance) item.Clone(), 1);
            _collection.Set(7, (IItemInstance) item.Clone(), itemDef.maxStackSize);

            var added = _collection.Add(item2, 8);

            // Assert
            Assert.IsNull(added.error);

            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);

            Assert.AreEqual(item, _collection.slots[6].item);
            Assert.AreEqual(3, _collection.slots[6].amount);

            Assert.AreEqual(item, _collection.slots[7].item);
            Assert.AreEqual(3, _collection.slots[7].amount);

            Assert.AreEqual(item, _collection.slots[1].item);
            Assert.AreEqual(3, _collection.slots[1].amount);

            Assert.AreEqual(item, _collection.slots[2].item);
            Assert.AreEqual(2, _collection.slots[2].amount);
        }

        [Test]
        public void SetSlotInCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var canSet = _collection.CanSet(0, item, 2);
            var set = _collection.Set(0, item, 2);

            // Assert
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(2, _collection.slots[0].amount);

            Assert.IsNull(set.error);
            Assert.IsTrue(set.result);
        }

        [Test]
        public void SetSlotToNullTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(0, item, 2);
            var set2 = _collection.Set(0, null, 0);

            Assert.IsTrue(set.result);
            Assert.IsTrue(set2.result);

            Assert.AreEqual(null, _collection[0]);
            Assert.AreEqual(0, _collection.GetAmount(0));
        }

        [Test]
        public void GetItemFromCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, item, 5);

            var getItem = _collection[0];
            var getItem2 = _collection[1];

            // Assert
            Assert.IsTrue(ReferenceEquals(item, getItem));
            Assert.IsNull(getItem2);
        }

        [Test]
        public void GetItemFromCollectionOutOfRangeTest()
        {
            // Arrange
            // Act
            // Assert
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                var item = _collection[50];
            });
        }

        [Test]
        public void GetAmountIndexTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, item, 5);

            var amount = _collection.GetAmount(0);
            var amount2 = _collection.GetAmount(1);

            // Assert
            Assert.AreEqual(5, amount);
            Assert.AreEqual(0, amount2);
        }

        [Test]
        public void SetSameItemInSlotReduceItemEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = (IItemInstance) item.Clone();
            
            _collection.Set(0, item2, 5);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionAddResult addResult = null;
            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changeResult = null;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
                addResult = result;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
                removeResult = result;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            // Act

            var set2 = _collection.Set(0, item, 3);

            // Assert
            Assert.IsNull(set2.error);
            Assert.AreEqual(3, _collection.slots[0].amount);

            Assert.AreEqual(0, addEventCount);
            Assert.AreEqual(1, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(1, removeResult.affectedSlots.Length);
            Assert.AreEqual(2, removeResult.affectedSlots[0].amount);
            Assert.AreSame(item2, removeResult.affectedSlots[0].item);
            
            Assert.AreEqual(_collection.slots[0], _collection.slots[removeResult.affectedSlots[0].slot]);

            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }

        [Test]
        public void SetSameItemInSlotIncreaseItemEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, (IItemInstance) item.Clone(), 2);


            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionAddResult addResult = null;
            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changeResult = null;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
                addResult = result;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
                removeResult = result;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            // Act

            var set2 = _collection.Set(0, (IItemInstance) item.Clone(), 5);

            // Assert
            Assert.IsNull(set2.error);
            Assert.AreEqual(5, _collection.slots[0].amount);

            Assert.AreEqual(1, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(1, addResult.affectedSlots.Length);
            Assert.AreEqual(3, addResult.affectedSlots[0].amount);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);

            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }

        [Test]
        public void SetEqualItemInSlotIncreaseItemEventTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, (IItemInstance) item.Clone(), 2);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionAddResult addResult = null;
            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changeResult = null;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
                addResult = result;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
                removeResult = result;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            // Act

            var set2 = _collection.Set(0, (IItemInstance) item2.Clone(), 5);

            // Assert
            Assert.IsNull(set2.error);
            Assert.AreEqual(5, _collection.slots[0].amount);

            Assert.AreEqual(1, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(1, addResult.affectedSlots.Length);
            Assert.AreEqual(3, addResult.affectedSlots[0].amount);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);

            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }

        [Test]
        public void RemoveEventsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionAddResult addResult = null;
            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changeResult = null;

            _collection.Set(1, item, 3);

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
                addResult = result;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
                removeResult = result;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            // Act
            var removed = _collection.Remove(item, 2);

            // Assert
            Assert.IsNull(removed.error);
            Assert.AreEqual(1, _collection.GetAmount(1));

            Assert.AreEqual(0, addEventCount);
            Assert.AreEqual(1, removeEventCount);
            Assert.AreEqual(1, changeEventCount);
            
            Assert.AreEqual(1, removeResult.affectedSlots.Length);
            Assert.AreEqual(1, removeResult.affectedSlots[0].slot);
            Assert.AreEqual(2, removeResult.affectedSlots[0].amount);
            Assert.AreSame(item, removeResult.affectedSlots[0].item);
            
            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(1, changeResult.affectedSlots[0]);
        }


        [Test]
        public void SetSlotWithExceedingMaxStackSizeShouldTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var set = _collection.Set(0, item, 5);

            // Assert
            Assert.AreEqual(Errors.ItemIsExceedingMaxStackSize, set.error);
        }

        [Test]
        public void SetSlotInCollectionEventsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionAddResult addResult = null;
            CollectionSlotsChangedResult changeResult = null;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
                addResult = result;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            _collection.Set(0, item, 2);

            // Assert
            Assert.AreEqual(item, _collection.slots[0].item);

            Assert.AreEqual(1, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(1, addResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);
            Assert.AreEqual(2, addResult.affectedSlots[0].amount);

            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }

        [Test]
        public void SetSlotInCollectionToNullEventsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, (IItemInstance) item.Clone(), 2);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changeResult = null;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
                removeResult = result;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };


            var setResult = _collection.Set(0, null);

            // Assert
            Assert.IsNull(setResult.error);
            Assert.IsTrue(setResult.result);

            Assert.IsNull(_collection.slots[0].item);
            Assert.AreEqual(0, _collection.slots[0].amount);

            Assert.AreEqual(0, addEventCount);
            Assert.AreEqual(1, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(1, removeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[removeResult.affectedSlots[0].slot]);
            Assert.AreEqual(2, removeResult.affectedSlots[0].amount);

            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }

        [Test]
        public void SetSlotInCollectionFromNullToNullEventsTest()
        {
            // Arrange
            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
            };

            _collection.Set(0, null);

            // Assert
            Assert.AreEqual(0, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(0, changeEventCount);
        }

        [Test]
        public void SetSlotOverwriteItemEventsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 3);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionAddResult addResult = null;
            CollectionRemoveResult<IItemInstance> removeResult = null;
            CollectionSlotsChangedResult changeResult = null;

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
                addResult = result;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
                removeResult = result;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            _collection.Set(0, item2, 2);

            // Assert
            Assert.AreEqual(1, addEventCount);
            Assert.AreEqual(1, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(1, addResult.affectedSlots.Length);
            Assert.AreEqual(2, addResult.affectedSlots[0].amount);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);

            Assert.AreEqual(1, removeResult.affectedSlots.Length);
            Assert.AreEqual(3, removeResult.affectedSlots[0].amount);
            Assert.AreSame(item, removeResult.affectedSlots[0].item);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);

            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }

        [Test]
        public void SetElementThatIsAlreadyInCollectionOnSameSlotShouldSucceedTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, item, 2);

            var setResult = _collection.Set(0, item, 1);

            // Assert
            Assert.IsTrue(setResult.result);

            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(1, _collection.slots[0].amount);
        }

        [Test]
        public void SetElementThatIsAlreadyInCollectionOnDifferentSlotShouldFailTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            _collection.Set(0, item, 2);

            var setResult = _collection.Set(1, item, 1);

            // Assert
            Assert.IsFalse(setResult.result);
            Assert.AreEqual(Errors.CollectionAlreadyContainsSpecificInstance, setResult.error);

            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(2, _collection.slots[0].amount);

            Assert.IsNull(_collection.slots[1].item);
            Assert.AreEqual(0, _collection.slots[1].amount);
        }

        [Test]
        public void IndexOfAllTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, (IItemInstance) item.Clone(), 2);
            _collection.Set(2, (IItemInstance) item.Clone(), 2);
            _collection.Set(3, (IItemInstance) item2.Clone(), 2);
            _collection.Set(7, (IItemInstance) item.Clone(), 2);

            // Act
            var indices = _collection.IndexOfAll(item).ToArray();

            // Assert
            Assert.AreEqual(3, indices.Length);
            Assert.AreEqual(0, indices[0]);
            Assert.AreEqual(2, indices[1]);
            Assert.AreEqual(7, indices[2]);
        }

        [Test]
        public void IndexOfAllPredicateTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 4};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, (IItemInstance) item.Clone(), 2);
            _collection.Set(2, (IItemInstance) item.Clone(), 2);
            _collection.Set(3, (IItemInstance) item2.Clone(), 2);
            _collection.Set(7, (IItemInstance) item.Clone(), 2);

            // Act
            var indices = _collection.IndexOfAll(t => t?.maxStackSize == 3).ToArray();

            // Assert
            Assert.AreEqual(3, indices.Length);
            Assert.AreEqual(0, indices[0]);
            Assert.AreEqual(2, indices[1]);
            Assert.AreEqual(7, indices[2]);
        }

        [Test]
        public void IndexOfAllPredicateNullTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, (IItemInstance) item.Clone(), 2);
            _collection.Set(2, (IItemInstance) item.Clone(), 2);
            _collection.Set(3, (IItemInstance) item.Clone(), 2);
            _collection.Set(7, (IItemInstance) item.Clone(), 2);

            // Act
            var indices = _collection.IndexOfAll(t => t == null);

            // Assert
            Assert.AreEqual(6, indices.Count());
            Assert.AreEqual(1, indices.ElementAt(0));
            Assert.AreEqual(4, indices.ElementAt(1));
            Assert.AreEqual(5, indices.ElementAt(2));
            Assert.AreEqual(6, indices.ElementAt(3));
            Assert.AreEqual(8, indices.ElementAt(4));
            Assert.AreEqual(9, indices.ElementAt(5));
        }

        [Test]
        public void IndexOfPredicateTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);
            _collection.Set(2, item, 2);
            _collection.Set(3, item2, 2);
            _collection.Set(7, item, 2);

            // Act
            var index = _collection.IndexOf(t => t?.maxStackSize == 5);

            // Assert
            Assert.AreEqual(3, index);
        }

        [Test]
        public void SwapElementWithEmptySlotTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3, name = "ItemDef1"};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5, name = "ItemDef2"};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);
            _collection.Set(2, item2, 3);

            // Act
            var moved = _collection.SwapPublic(2, _collection, 1);

            // Assert
            Assert.IsNull(moved.error);

            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(2, _collection.slots[0].amount);

            Assert.AreEqual(item2, _collection.slots[1].item);
            Assert.AreEqual(3, _collection.slots[1].amount);

            Assert.IsNull(_collection.slots[2].item);
            Assert.AreEqual(0, _collection.slots[2].amount);
        }

        [Test]
        public void SwapTwoElementsInCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);

            _collection.Set(2, item2, 3);

            // Act
            var moved = _collection.SwapPublic(0, _collection, 2);

            // Assert
            Assert.IsNull(moved.error);

            Assert.AreEqual(item2, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);

            Assert.AreEqual(item, _collection.slots[2].item);
            Assert.AreEqual(2, _collection.slots[2].amount);
        }

        [Test]
        public void SwapTwoEmptySlotsTest()
        {
            // Arrange
            // Act
            var moved = _collection.SwapPublic(0, _collection, 2);

            // Assert
            Assert.IsNull(moved.error);

            Assert.IsNull(_collection.slots[0].item);
            Assert.AreEqual(0, _collection.slots[0].amount);

            Assert.IsNull(_collection.slots[2].item);
            Assert.AreEqual(0, _collection.slots[2].amount);
        }

        [Test]
        public void SwapElementInCollectionShouldFailTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);

            _collection.Set(2, item2, 2);

            // Act
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                _collection.SwapPublic(2, _collection, -1);
            });

            // Assert
        }

        [Test]
        public void SwapElementInCollectionShouldFireSlotsChangedEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);
            CollectionSlotsChangedResult changedResult = null;
            int changedEventCount = 0;

            _collection.Set(0, item, 2);

            _collection.Set(2, item2, 2);

            _collection.OnSlotsChanged += (sender, result) =>
            {
                changedResult = result;
                changedEventCount++;
            };

            // Act
            var swap = _collection.SwapPublic(2, _collection, 1);

            // Assert
            Assert.IsNull(swap.error);

            Assert.AreEqual(1, changedEventCount);

            Assert.AreEqual(2, changedResult.affectedSlots.Length);
            Assert.AreEqual(2, changedResult.affectedSlots[0]);
            Assert.AreEqual(1, changedResult.affectedSlots[1]);
        }

        [Test]
        public void ExpandCollectionTest()
        {
            _collection.Expand<CollectionSlot<IItemInstance>>(5);

            // Assert
            Assert.AreEqual(CollectionSize + 5, _collection.slots.Length);

            Assert.AreEqual(typeof(CollectionSlot<IItemInstance>), _collection.slots[10].GetType());
            Assert.AreEqual(typeof(CollectionSlot<IItemInstance>), _collection.slots[11].GetType());
            Assert.AreEqual(typeof(CollectionSlot<IItemInstance>), _collection.slots[12].GetType());
            Assert.AreEqual(typeof(CollectionSlot<IItemInstance>), _collection.slots[13].GetType());
            Assert.AreEqual(typeof(CollectionSlot<IItemInstance>), _collection.slots[14].GetType());
        }

        [Test]
        public void ExpandCollectionTest2()
        {
            _collection.Expand<CollectionSlot<IItemInstance>>(0);

            // Assert
            Assert.AreEqual(CollectionSize, _collection.slots.Length);
        }

        [Test]
        public void ShrinkCollectionTest()
        {
            _collection.Shrink(5);

            // Assert
            Assert.AreEqual(CollectionSize - 5, _collection.slots.Length);
        }

        [Test]
        public void ShrinkCollectionTest2()
        {
            _collection.Shrink(0);

            // Assert
            Assert.AreEqual(CollectionSize, _collection.slots.Length);
        }

        [Test]
        public void SlotCountTest()
        {
            Assert.AreEqual(10, _collection.slotCount);
            _collection.Shrink(5);
            Assert.AreEqual(5, _collection.slotCount);
        }

        [Test]
        public void CloneTest()
        {
            var clone = (Collection<IItemInstance>) _collection.Clone();

            Assert.IsFalse(ReferenceEquals(_collection, clone));
            Assert.AreEqual(clone.slotCount, _collection.slotCount);

            for (int i = 0; i < clone.slotCount; i++)
            {
                Assert.IsFalse(ReferenceEquals(clone.slots[i], _collection.slots[i]));
                Assert.IsTrue(ReferenceEquals(clone.slots[i].item, _collection.slots[i].item));
                Assert.AreEqual(clone.slots[i].amount, _collection.slots[i].amount);
                Assert.AreEqual(clone.slots[i].item, _collection.slots[i].item);
            }
        }

        [Test]
        public void MergeWithSelfTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(0, item, 5);

            // Act
            var merged = _collection.MergePublic(0, _collection, 0, 5);

            // Assert
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            Assert.AreEqual(5, _collection.GetAmount(0));
        }

        [Test]
        public void MergeWithSellNullTest()
        {
            // Arrange
            // Act
            var merged = _collection.MergePublic(0, _collection, 0, _collection.GetAmount(0));

            // Assert
            Assert.IsNull(merged.error);
            Assert.AreEqual(0, _collection.GetAmount(0));
        }

        [Test]
        public void MergeSlotShouldSucceedTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 1);
            _collection.Set(1, item2, 2);

            // Act
            var merge = _collection.MergePublic(0, _collection, 1, _collection.GetAmount(0));

            // Assert
            Assert.IsNull(merge.error);

            Assert.IsTrue(!_collection.slots[0].isOccupied);
            Assert.AreEqual(0, _collection.slots[0].amount);

            Assert.AreEqual(3, _collection.GetAmount(1));
        }

        [Test]
        public void MergeSlotsEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            CollectionSlotsChangedResult changeResult = null;

            _collection.Set(0, item, 1);
            _collection.Set(2, item2, 2);

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
                changeResult = result;
            };

            // Act
            _collection.MergePublic(0, _collection, 2, _collection.GetAmount(0));

            // Assert
            Assert.AreEqual(0, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(1, changeEventCount);

            Assert.AreEqual(2, changeResult.affectedSlots.Length);
            Assert.AreEqual(0, changeResult.affectedSlots[0]);
            Assert.AreEqual(2, changeResult.affectedSlots[1]);
        }

        [Test]
        public void MergeSlotsFailedEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            int addEventCount = 0;
            int removeEventCount = 0;
            int changeEventCount = 0;

            _collection.Set(0, item, 3);
            _collection.Set(2, item2, 2);

            // Act
            _collection.OnAddedItem += (sender, result) =>
            {
                addEventCount++;
            };
            _collection.OnRemovedItem += (sender, result) =>
            {
                removeEventCount++;
            };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                changeEventCount++;
            };

            // Act
            _collection.MergePublic(0, _collection, 2, _collection.GetAmount(0));

            // Assert
            Assert.AreEqual(0, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(0, changeEventCount);
        }

        [Test]
        public void MergeSlotFromNullTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(2, item2, 2);

            // Act
            var merged = _collection.MergePublic(0, _collection, 2, _collection.GetAmount(0));

            // Assert
            Assert.IsNull(merged.error);

            Assert.AreEqual(2, _collection.GetAmount(2));
            Assert.AreEqual(0, _collection.GetAmount(0));
        }

        [Test]
        public void MergeSlotNullToNullTest()
        {
            // Arrange
            // Act
            var merge = _collection.MergePublic(0, _collection, 2, _collection.GetAmount(0));

            // Assert
            Assert.IsNull(merge.error);
        }

        [Test]
        public void CanSetTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);

            var can = _collection.CanSet(0, item, 2);

            Assert.IsNull(can.error);
            Assert.IsTrue(can.result);
        }

        [Test]
        public void CanSetTest2()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            var set1 = _collection.Set(0, item, 1);
            var can = _collection.CanSet(0, item, -1);

            Assert.IsNull(set1.error);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, can.error);
            Assert.IsFalse(can.result);
        }

        private static IEnumerable CanTest3Inputs()
        {
            return new[]
            {
                new [] {0},
                new [] {1},
                new [] {2},
                new [] {3},
            };
        }
        
        [Test]
        [TestCaseSource(nameof(CanTest3Inputs))]
        public void CanSetTest3(int canSetIndex)
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            var set1 = _collection.Set(0, item, 5);
            var set2 = _collection.CanSet(0, item2, canSetIndex);
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.AreSame(item, _collection[0]);
            Assert.AreEqual(5, _collection.GetAmount(0));
        }

        [Test]
        public void CanSetExceedingMaxStackSizeTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 5};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            var canSet = _collection.CanSet(0, item, 6);
            
            Assert.AreEqual(Errors.ItemIsExceedingMaxStackSize, canSet.error);
            
            Assert.IsNull(_collection[0]);
            Assert.AreEqual(0, _collection.GetAmount(0));
        }

        [Test]
        public void CollectionSimulationTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            using (var sim = new CollectionSimulation<Collection<IItemInstance>>(_collection))
            {
                sim.collection.Add(item);
                sim.collection.Add(item2, 3);
            }

            Assert.IsNull(_collection[0]);
            Assert.IsNull(_collection[1]);
        }

        [Test]
        public void CollectionSimulationTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 1);
            _collection.Set(1, (IItemInstance) item.Clone(), 2);
            _collection.Set(3, item2, 2);
            _collection.Set(4, (IItemInstance) item2.Clone(), 3);

            // Act
            using (var sim = new CollectionSimulation<Collection<IItemInstance>>(_collection))
            {
                sim.collection.Clear();
            }

            // Assert
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(item, _collection[1]);
            Assert.AreEqual(item2, _collection[3]);
            Assert.AreEqual(item2, _collection[4]);
        }

        [Test]
        public void EnumerateCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, (IItemInstance) item.Clone(), 1);
            _collection.Set(1, (IItemInstance) item.Clone(), 2);
            _collection.Set(3, (IItemInstance) item.Clone(), 2);
            _collection.Set(4, (IItemInstance) item.Clone(), 3);

            int index = 0;
            foreach (var iterItem in _collection)
            {
                if (index == 0 || index == 1 || index == 3 || index == 4)
                    Assert.AreEqual(item, _collection.slots[index].item);
                else
                    Assert.IsNull(_collection.slots[index].item);

                index++;
            }
        }


        [Test]
        public void ItemCollectionEntryPropertyTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            Assert.IsNull(item.collectionEntry);

            _collection.Set(0, item, 2);

            Assert.AreEqual(_collection, item.collectionEntry.collection);
            Assert.AreEqual(0, item.collectionEntry.index);
            Assert.AreEqual(2, item.collectionEntry.amount);
        }

        [Test]
        public void ItemCollectionEntryPropertyTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            Assert.IsNull(item.collectionEntry);

            _collection.Set(0, item, 2);

            Assert.AreEqual(_collection, item.collectionEntry.collection);
            Assert.AreEqual(0, item.collectionEntry.index);
            Assert.AreEqual(2, item.collectionEntry.amount);

            _collection.Set(0, null, 0);

            Assert.IsNull(item.collectionEntry);
        }

        [Test]
        public void SwapItemEntrySlotTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);

            Assert.IsNull(item.collectionEntry);
            Assert.IsNull(item2.collectionEntry);

            _collection.Set(0, item, 2);
            _collection.Set(1, item2, 3);

            Assert.AreEqual(_collection, item.collectionEntry.collection);
            Assert.AreEqual(0, item.collectionEntry.index);
            Assert.AreEqual(2, item.collectionEntry.amount);

            Assert.AreEqual(_collection, item2.collectionEntry.collection);
            Assert.AreEqual(1, item2.collectionEntry.index);
            Assert.AreEqual(3, item2.collectionEntry.amount);

            _collection.SwapPublic(0, _collection, 1);

            Assert.AreEqual(_collection, item2.collectionEntry.collection);
            Assert.AreEqual(0, item2.collectionEntry.index);
            Assert.AreEqual(3, item2.collectionEntry.amount);

            Assert.AreEqual(_collection, item.collectionEntry.collection);
            Assert.AreEqual(1, item.collectionEntry.index);
            Assert.AreEqual(2, item.collectionEntry.amount);
        }

        [Test]
        public void SwapItemWithEmptyEntrySlotTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            Assert.IsNull(item.collectionEntry);

            _collection.Set(0, item, 2);
            _collection.SwapPublic(0, _collection, 1);

            Assert.AreEqual(_collection, item.collectionEntry.collection);
            Assert.AreEqual(1, item.collectionEntry.index);
            Assert.AreEqual(2, item.collectionEntry.amount);
        }

        [Test]
        public void FindNextSlotTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (CollectionItemInstance) item.Clone();
            var clone2 = (CollectionItemInstance) item.Clone();
            var clone3 = (CollectionItemInstance) item.Clone();

            _collection.Set(1, item, 2);
            _collection.Set(4, clone2, 2);
            _collection.Set(6, clone3, 2);

            var slot = _collection.GetAddItemEnumerator(clone, 1, new CollectionContext());
            slot.MoveNext();
            Assert.AreEqual(1, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(4, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(6, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(0, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(2, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(3, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(5, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(7, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(8, slot.Current);

            slot.MoveNext();
            Assert.AreEqual(9, slot.Current);

            var next = slot.MoveNext();
            Assert.IsFalse(next);
        }

        [Test]
        public void MergeIntoEmptySlotTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 3};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            var merged = _collection.MergePublic(0, _collection, 1, 1);
            Assert.IsNull(merged.error);

            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(1, _collection.GetAmount(1));
            Assert.AreEqual(item, _collection[1]);
        }

        [Test]
        public void MergeTooManyIntoEmptySlotTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            var merged = _collection.MergePublic(0, _collection, 1, 4);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, merged.error);
        }

        [Test]
        public void MergeIntoExistingStackTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            _collection.Set(1, item2, 5);
            var merged = _collection.MergePublic(0, _collection, 1, 3);

            Assert.IsNull(merged.error);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(8, _collection.GetAmount(1));
        }

        [Test]
        public void MergeEntireStackIntoEmptySlotTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            var merged = _collection.MergePublic(0, _collection, 1, 3);

            Assert.IsNull(merged.error);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(3, _collection.GetAmount(1));
            
            Assert.AreSame(item, _collection[1]);
            Assert.IsNull(_collection[0]);
        }

        [Test]
        public void MergePartIntoExistingStackTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            _collection.Set(1, item2, 5);
            var merged = _collection.MergePublic(0, _collection, 1, 1);

            Assert.IsNull(merged.error);
            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(6, _collection.GetAmount(1));
        }

        [Test]
        public void MergeTooMuchIntoExistingStackTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            _collection.Set(1, item2, 5);
            var merged = _collection.MergePublic(0, _collection, 1, 4);

            Assert.AreEqual(Errors.CollectionDoesNotContainItem, merged.error);
            Assert.AreEqual(3, _collection.GetAmount(0));
            Assert.AreEqual(5, _collection.GetAmount(1));
        }

        [Test]
        public void CollectionEntryFieldTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item);

            Assert.AreEqual(_collection, item.collectionEntry.collection);
            Assert.AreEqual(null, item2.collectionEntry);

            _collection.Set(0, item2);
            Assert.AreEqual(null, item.collectionEntry);
            Assert.AreEqual(_collection, item2.collectionEntry.collection);

            _collection.Set(0, null);
            Assert.AreEqual(null, item.collectionEntry);
            Assert.AreEqual(null, item2.collectionEntry);
            Assert.AreEqual(null, _collection[0]);
        }

        [Test]
        public void EntryFieldOverwriteTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);

            Assert.AreEqual(_collection, item.collectionEntry.collection);

            _collection.Set(0, item, 0);

            Assert.AreEqual(null, item.collectionEntry);
        }




        [Test]
        public void SwapTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);
            _collection.Set(3, item2, 1);

            var swapped = _collection.SwapPublic(0, _collection, 3);

            Assert.IsNull(swapped.error);
            Assert.AreSame(item2, _collection[0]);
            Assert.AreSame(item, _collection[3]);
            Assert.AreEqual(1, _collection.GetAmount(0));
            Assert.AreEqual(2, _collection.GetAmount(3));
        }

        [Test]
        public void SwapMergableItems()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (IItemInstance) item.Clone();

            _collection.Set(0, item, 2);
            _collection.Set(3, clone, 1);

            var swapped = _collection.SwapPublic(0, _collection, 3);

            Assert.IsNull(swapped.error);
            Assert.AreSame(clone, _collection[0]);
            Assert.AreSame(item, _collection[3]);
            Assert.AreEqual(1, _collection.GetAmount(0));
            Assert.AreEqual(2, _collection.GetAmount(3));
        }

        [Test]
        public void SwapToEmptyTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 2);

            var swapped = _collection.SwapPublic(0, _collection, 3);

            Assert.IsNull(swapped.error);
            Assert.IsNull(_collection[0]);
            Assert.AreSame(item, _collection[3]);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(2, _collection.GetAmount(3));
        }

        [Test]
        public void SwapFromEmptyTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 2);

            var swapped = _collection.SwapPublic(3, _collection, 0);

            Assert.IsNull(swapped.error);
            Assert.IsNull(_collection[0]);
            Assert.AreSame(item, _collection[3]);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(2, _collection.GetAmount(3));
        }

        [Test]
        public void SwapWithSelfTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            
            _collection.Set(0, item, 2);

            var swapped = _collection.SwapPublic(0, _collection, 0);

            Assert.IsNull(swapped.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(2, _collection.GetAmount(0));
        }

        [Test]
        public void MergeTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (IItemInstance) item.Clone();

            _collection.Set(0, item, 2);
            _collection.Set(3, clone, 1);

            var merged = _collection.MergePublic(0, _collection, 3, 2);

            Assert.IsNull(merged.error);
            Assert.IsNull(_collection[0]);
            Assert.AreSame(item, _collection[3]);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(3, _collection.GetAmount(3));
        }

        [Test]
        public void MergeNonMergableTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);
            _collection.Set(3, item2, 1);

            var merged = _collection.MergePublic(0, _collection, 3, 2);

            Assert.AreEqual(Errors.ItemsAreNotEqual, merged.error);
            Assert.AreSame(item, _collection[0]);
            Assert.AreSame(item2, _collection[3]);
            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(1, _collection.GetAmount(3));
        }

        [Test]
        public void MergeWithEmptySlot()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 5);

            var merged = _collection.MergePublic(0, _collection, 3, 5);

            Assert.IsNull(merged.error);
            Assert.IsNull(_collection[0]);
            Assert.AreSame(item, _collection[3]);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(5, _collection.GetAmount(3));
        }

        [Test]
        public void MergePartialStackTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (IItemInstance) item.Clone();

            _collection.Set(0, item, 5);
            _collection.Set(3, clone, 3);

            var merged = _collection.MergePublic(0, _collection, 3, 3);

            Assert.IsNull(merged.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(item, _collection[3]);
            Assert.AreNotSame(_collection[0], _collection[3]); // Should be equal, but not same ref.
            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(6, _collection.GetAmount(3));
        }

        [Test]
        public void MergePartialStackToEmptyTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10};
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 5);

            var merged = _collection.MergePublic(0, _collection, 3, 3);

            Assert.IsNull(merged.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(item, _collection[3]);
            Assert.AreNotSame(_collection[0], _collection[3]); // items shoudl be equal, but not the same ref.
            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(3, _collection.GetAmount(3));
        }

        [Test]
        public void MergeTooManyTest1()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (IItemInstance) item.Clone();
                        
            _collection.Set(0, item, 8);
            _collection.Set(3, clone, 9);

            var merged = _collection.MergePublic(0, _collection, 3, 3);
            
            Assert.AreEqual(Errors.ItemIsExceedingMaxStackSize, merged.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(clone, _collection[3]);
            Assert.AreEqual(8, _collection.GetAmount(0));
            Assert.AreEqual(9, _collection.GetAmount(3));
        }

        [Test]
        public void MergeTooManyTest2()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (IItemInstance) item.Clone();
                        
            _collection.Set(0, item, 8);
            _collection.Set(3, clone, 9);

            var merged = _collection.MergePublic(0, _collection, 3, 9);
            
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, merged.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(clone, _collection[3]);
            Assert.AreEqual(8, _collection.GetAmount(0));
            Assert.AreEqual(9, _collection.GetAmount(3));
        }

        [Test]
        public void MergeOntoSelfTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
                        
            _collection.Set(0, item, 8);

            var merged = _collection.MergePublic(0, _collection, 0, 8);
            
            Assert.IsNull(merged.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(8, _collection.GetAmount(0));
        }

        [Test]
        public void MergeOntoSelfPartialStackTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
                        
            _collection.Set(0, item, 8);

            var merged = _collection.MergePublic(0, _collection, 0, 5);
            
            Assert.IsNull(merged.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(8, _collection.GetAmount(0));
        }
        
        [Test]
        public void Merge2CollectionsAmountTest()
        {
            var col2 = new CollectionAccessibleMethods<IItemInstance>(4);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = col2.Set(0, item, 3);
            var merged = col2.MergePublic(0, _collection, 5, 3);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            Assert.AreSame(item, _collection[5]);
            Assert.IsNull(col2[0]);
            
            Assert.AreEqual(3, _collection.GetAmount(5));
            Assert.AreEqual(0, col2.GetAmount(0));
        }
        
        [Test]
        public void Merge2CollectionsAmountPartialStackTest()
        {
            var col2 = new CollectionAccessibleMethods<IItemInstance>(4);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10, name = "ItemDef1" };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var clone = (IItemInstance) item.Clone();
            
            var set = col2.Set(0, item, 5);
            var set2 = _collection.Set(5, clone, 3);
            var merged = col2.MergePublic(0, _collection, 5, 3);
            
            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(merged.error);
            Assert.AreSame(clone, _collection[5]);
            Assert.AreSame(item, col2[0]);
            
            Assert.AreEqual(6, _collection.GetAmount(5));
            Assert.AreEqual(2, col2.GetAmount(0));
        }
        
        [Test]
        public void Swap2CollectionsAmountTest()
        {
            var col2 = new CollectionAccessibleMethods<IItemInstance>(4);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = col2.Set(0, item, 3);
            var swapped = col2.SwapPublic(0, _collection, 5);
            
            Assert.IsNull(set.error);
            Assert.IsNull(swapped.error);
            Assert.AreSame(item, _collection[5]);
            Assert.IsNull(col2[0]);
            
            Assert.AreEqual(3, _collection.GetAmount(5));
            Assert.AreEqual(0, col2.GetAmount(0));
        }
        
        [Test]
        public void Move2CollectionsAmountTest()
        {
            var col2 = new Collection<IItemInstance>(4);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = col2.Set(0, item, 3);
            var moved = col2.MoveAuto(0, _collection, 3);
            
            Assert.IsNull(set.error);
            Assert.IsNull(moved.error);
            Assert.AreSame(item, _collection[0]);
            Assert.IsNull(col2[0]);
            
            Assert.AreEqual(3, _collection.GetAmount(0));
            Assert.AreEqual(0, col2.GetAmount(0));
        }
        
        [Test]
        public void MoveAutoTest()
        {
            var col2 = new Collection<IItemInstance>(10);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 5);
            var moved = _collection.MoveAuto(0, col2, 5);
            
            Assert.IsNull(moved.error);
            Assert.IsNull(_collection[0]);
            Assert.AreSame(item, col2[0]);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.AreEqual(5, col2.GetAmount(0));
        }

        [Test]
        public void MoveAutoPartialStackTest()
        {
            var col2 = new Collection<IItemInstance>(10);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 5);
            var moved = _collection.MoveAuto(0, col2, 3);
            
            Assert.IsNull(moved.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(item, col2[0]);
            Assert.AreNotSame(_collection[0], col2[0]);
            
            Assert.AreEqual(2, _collection.GetAmount(0));
            Assert.AreEqual(3, col2.GetAmount(0));
        }
        
        [Test]
        public void MoveTooManyAutoTest()
        {
            var col2 = new Collection<IItemInstance>(10);
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 5);
            var moved = _collection.MoveAuto(0, col2, 6);
            
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, moved.error);
            Assert.AreEqual(item, _collection[0]);
            Assert.IsNull(col2[0]);
            
            Assert.AreEqual(5, _collection.GetAmount(0));
            Assert.AreEqual(0, col2.GetAmount(0));
        }

        [Test]
        public void SetNegativeAmountValueCollectionSetTest()
        {           
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(0, item, 3);

            Assert.IsNull(set.error);

            var set2 = _collection.Set(0, item, -1);

            Assert.AreEqual(Errors.CollectionDoesNotContainItem, set2.error);
            Assert.AreEqual(3, _collection.GetAmount(0));
        }
        
        [Test]
        public void SetNegativeAmountValueCollectionSetTest2()
        {           
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set2 = _collection.Set(0, item, -3);
            
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, set2.error);
            Assert.AreEqual(0, _collection.GetAmount(0));
            Assert.IsNull(_collection[0]);
        }

        [Test]
        public void Bug_FillStackTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(1, item, 2);
            
            // Should re-supply slot 1, and add the 2 remaining to slot 0
            var add = _collection.Add(item2, 3);
            
            Assert.IsTrue(set.result);
            Assert.IsNull(add.error);
            
            Assert.AreEqual(item.ID, _collection[1].ID);
            Assert.AreEqual(item2.ID, _collection[0].ID);
            
//            Assert.AreEqual(3, add.result.affectedSlots[0].amount);
            Assert.AreEqual(2, add.result.affectedSlots.Length);
            Assert.AreEqual(1, add.result.affectedSlots[0].slot);
            Assert.AreEqual(0, add.result.affectedSlots[1].slot);
            
            Assert.AreEqual(1, add.result.affectedSlots[0].amount);
            Assert.AreEqual(2, add.result.affectedSlots[1].amount);
        }

        [Test]
        public void ItemInstanceClonesTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(2, item, 2);
            
            Assert.IsTrue(set.result);

            var add = _collection.Add(item2, 1);
            
            Assert.IsNull(add.error);
            Assert.AreSame(item2, _collection[2]);
        }
        
        [Test]
        public void ItemInstanceClonesTest2()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(2, item, 2);
            
            Assert.IsTrue(set.result);

            var add = _collection.Add(item2, 2);
            
            Assert.IsNull(add.error);
            Assert.AreSame(item, _collection[2]);
            Assert.AreEqual(3, _collection.GetAmount(2));
            Assert.AreEqual(item2, _collection[0]);
            Assert.AreEqual(1, _collection.GetAmount(0));
        }
        
        [Test]
        public void ItemInstanceClonesTest3()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);

            var set = _collection.Set(2, item, 2);
            
            Assert.IsTrue(set.result);

            var add = _collection.Add(item2, 5);
            
            Assert.IsNull(add.error);
            Assert.AreSame(item, _collection[2]);
            Assert.AreEqual(3, _collection.GetAmount(2));
            Assert.AreEqual(item2, _collection[0]);
            Assert.AreEqual(3, _collection.GetAmount(0));
            
            // Not the same, make sure its' a clone of the object, but not the same as this one.
            Assert.AreNotSame(item, _collection[1]);
            Assert.AreNotSame(item2, _collection[1]);

            // Make sure the ItemID's are also different.
            Assert.AreNotEqual(item.ID, _collection[1].ID);
            Assert.AreNotEqual(item2.ID, _collection[1].ID);
            
            Assert.AreEqual(1, _collection.GetAmount(1));
        }

        [Test]
        public void SortCollectionTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var set = _collection.Set(0, item, 2);
            var set2 = _collection.Set(1, item2, 1);

            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);

            var addedItemEvent = 0;
            var removedItemEvent = 0;
            CollectionSlotsChangedResult eventData = null;
            var slotsChangedEvent = 0;
            _collection.OnAddedItem += (sender, result) => { addedItemEvent++; };
            _collection.OnRemovedItem += (sender, result) => { removedItemEvent++; };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                slotsChangedEvent++;
                eventData = result;
            };
            
            _collection.Sort(new DefaultItemComparer());
            
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.GetAmount(0));
            
            Assert.IsNull(_collection.slots[1].item);
            Assert.AreEqual(0, _collection.GetAmount(1));
            
            Assert.AreEqual(0, addedItemEvent);
            Assert.AreEqual(0, removedItemEvent);
            Assert.AreEqual(1, slotsChangedEvent);
            Assert.AreEqual(_collection.slotCount, eventData.affectedSlots.Length);
        }
        
        [Test]
        public void SortCollectionTest2()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3, name = "itemdef1" };
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 8, name = "itemdef2" };
            var itemDef3 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 1, name = "itemdef3" };
            var item = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new CollectionItemInstance(Guid.NewGuid(), itemDef);
            
            var item3 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);
            var item4 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);
            var item5 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);
            var item6 = new CollectionItemInstance(Guid.NewGuid(), itemDef2);
            
            var item7 = new CollectionItemInstance(Guid.NewGuid(), itemDef3);
            var item8 = new CollectionItemInstance(Guid.NewGuid(), itemDef3);
            
            _collection.Set(0, item, 2);
            _collection.Set(1, item2, 1);
            _collection.Set(2, item8, 1);

            _collection.Set(3, item3, 3);
            _collection.Set(4, item4, 4);
            _collection.Set(6, item5, 2);
            
            _collection.Set(8, item7, 1);
            _collection.Set(9, item6, 6);

            var addedItemEvent = 0;
            var removedItemEvent = 0;
            CollectionSlotsChangedResult eventData = null;
            var slotsChangedEvent = 0;
            _collection.OnAddedItem += (sender, result) => { addedItemEvent++; };
            _collection.OnRemovedItem += (sender, result) => { removedItemEvent++; };
            _collection.OnSlotsChanged += (sender, result) =>
            {
                slotsChangedEvent++;
                eventData = result;
            };
            
            _collection.Sort(new DefaultItemComparer());
            
            Assert.AreEqual(itemDef, _collection.slots[0].item.itemDefinition);
            Assert.AreEqual(3, _collection.GetAmount(0));
            
            Assert.AreSame(itemDef2, _collection.slots[1].item.itemDefinition);
            Assert.AreEqual(8, _collection.GetAmount(1));
            Assert.AreSame(itemDef2, _collection.slots[2].item.itemDefinition);
            Assert.AreEqual(7, _collection.GetAmount(2));


            Assert.AreSame(itemDef3, _collection.slots[3].item.itemDefinition);
            Assert.AreEqual(1, _collection.GetAmount(3));
            Assert.AreSame(itemDef3, _collection.slots[4].item.itemDefinition);
            Assert.AreEqual(1, _collection.GetAmount(4));
            
            Assert.AreEqual(0, _collection.GetAmount(5));
            Assert.AreEqual(0, _collection.GetAmount(6));
            Assert.AreEqual(0, _collection.GetAmount(7));
            Assert.AreEqual(0, _collection.GetAmount(8));
            Assert.AreEqual(0, _collection.GetAmount(9));

            
            Assert.AreEqual(0, addedItemEvent);
            Assert.AreEqual(0, removedItemEvent);
            Assert.AreEqual(1, slotsChangedEvent);
            Assert.AreEqual(_collection.slotCount, eventData.affectedSlots.Length);
        }
        
        
        // TODO: Test collection config (read-only / readwrite, name)
    }
}
