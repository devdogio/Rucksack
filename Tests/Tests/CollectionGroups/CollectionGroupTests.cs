using System;
using System.Linq;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class CollectionGroupTests
    {
        private CollectionGroup<IItemInstance, Collection<IItemInstance>> _group;
        private ItemDefinition _itemDef;
        private ItemDefinition _itemDef2;
        private IItemInstance _item;
        private IItemInstance _item2;
        private IItemInstance _item3;

        private Collection<IItemInstance> _col0 { get; set; }
        private Collection<IItemInstance> _col1 { get; set; }

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _col0 = new Collection<IItemInstance>(5) {};
            _col1 = new Collection<IItemInstance>(5) {};
            
            _group = new CollectionGroup<IItemInstance, Collection<IItemInstance>>(new []
            {
                new CollectionGroup<IItemInstance, Collection<IItemInstance>>.Slot(_col0), 
                new CollectionGroup<IItemInstance, Collection<IItemInstance>>.Slot(_col1, new CollectionPriority<IItemInstance>(60, 60, 60)), 
            });
            
            // Arrange
            _itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            _itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10 };
            _item = new ItemInstance(Guid.NewGuid(), _itemDef);
            _item2 = new ItemInstance(Guid.NewGuid(), _itemDef);
            _item3 = new ItemInstance(Guid.NewGuid(), _itemDef2);
        }

        [Test]
        public void AddItemToCollectionShouldBeAddedToSecondCollectionTest()
        {
            // Act
            var added = _group.Add(_item, 3);
            var arr = added.result.ToArray();
            
            // Assert
            Assert.IsNull(added.error);
            
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(_col1, arr[0].collection);
            Assert.AreEqual(_item, arr[0].collection[0]);
            Assert.AreEqual(1, arr[0].affectedSlots.Length);
            Assert.AreEqual(0, arr[0].affectedSlots[0].slot);
            Assert.AreEqual(3, arr[0].affectedSlots[0].amount);
            
            Assert.AreEqual(_col1[0], _item);
            Assert.IsNull(_col0[0]);
        }
        
        [Test]
        public void AddItemToCollectionShouldBeAddedToBothCollectionTest()
        {
            // Act
            var added = _group.Add(_item, 65);
            var arr = added.result.ToArray();

            // Assert
            Assert.IsNull(added.error);

            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(_col1, arr[0].collection);            
            for (int i = 0; i < _col0.slotCount; i++)
            {
                Assert.AreEqual(_item, arr[0].collection[i]);
            }
            
            Assert.AreEqual(_col0, arr[1].collection);
            Assert.AreEqual(_item, arr[1].collection[0]);
            Assert.AreEqual(_item, arr[1].collection[1]);
            Assert.IsNull(arr[1].collection[2]);
            Assert.IsNull(arr[1].collection[3]);
            Assert.IsNull(arr[1].collection[4]);
            
            
            Assert.AreEqual(5, arr[0].affectedSlots.Length);
            Assert.AreEqual(2, arr[1].affectedSlots.Length);
            
            Assert.AreEqual(0, arr[0].affectedSlots[0].slot);
            Assert.AreEqual(1, arr[0].affectedSlots[1].slot);
            Assert.AreEqual(2, arr[0].affectedSlots[2].slot);
            Assert.AreEqual(3, arr[0].affectedSlots[3].slot);
            Assert.AreEqual(4, arr[0].affectedSlots[4].slot);
            
            Assert.AreEqual(10, arr[0].affectedSlots[0].amount);
            Assert.AreEqual(10, arr[0].affectedSlots[1].amount);
            Assert.AreEqual(10, arr[0].affectedSlots[2].amount);
            Assert.AreEqual(10, arr[0].affectedSlots[3].amount);
            Assert.AreEqual(10, arr[0].affectedSlots[4].amount);
            
            Assert.AreEqual(0, arr[1].affectedSlots[0].slot);
            Assert.AreEqual(1, arr[1].affectedSlots[1].slot);
            Assert.AreEqual(10, arr[1].affectedSlots[0].amount);
            Assert.AreEqual(5, arr[1].affectedSlots[1].amount);
            
            Assert.AreEqual(10, arr[0].collection.GetAmount(0));
            Assert.AreEqual(10, arr[0].collection.GetAmount(1));
            Assert.AreEqual(10, arr[0].collection.GetAmount(2));
            Assert.AreEqual(10, arr[0].collection.GetAmount(3));
            Assert.AreEqual(10, arr[0].collection.GetAmount(4));
            
            Assert.AreEqual(10, arr[1].collection.GetAmount(0));
            Assert.AreEqual(5, arr[1].collection.GetAmount(1));
            Assert.AreEqual(0, arr[1].collection.GetAmount(2));
            Assert.AreEqual(0, arr[1].collection.GetAmount(3));
            Assert.AreEqual(0, arr[1].collection.GetAmount(4));
        }
        
        [Test]
        public void RemoveItemFromCollectionTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item, 5);
            _col0.slots[4].SetWithoutValidation(_item, 5);
            
            _col1.slots[0].SetWithoutValidation(_item, 5);
            _col1.slots[2].SetWithoutValidation(_item, 5);
            _col1.slots[4].SetWithoutValidation(_item, 5);
            
            // Act
            var removed = _group.Remove(_item, 12);
            Assert.IsNull(removed.error);
            var arr = removed.result.ToArray();

            // Assert
            Assert.IsNull(removed.error);
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(_col1, arr[0].collection);
            
            Assert.IsNull(arr[0].collection[0]);
            Assert.IsNull(arr[0].collection[2]);
            Assert.AreEqual(_item, arr[0].collection[4]);
            
            Assert.AreEqual(3, arr[0].affectedSlots.Length);
            Assert.AreEqual(0, arr[0].affectedSlots[0].slot);
            Assert.AreEqual(2, arr[0].affectedSlots[1].slot);
            Assert.AreEqual(4, arr[0].affectedSlots[2].slot);
            Assert.AreEqual(5, arr[0].affectedSlots[0].amount);
            Assert.AreEqual(5, arr[0].affectedSlots[1].amount);
            Assert.AreEqual(2, arr[0].affectedSlots[2].amount);
            
            Assert.AreEqual(_item, _col0[0]);
            Assert.AreEqual(5, _col0.GetAmount(0));
            Assert.AreEqual(5, _col0.GetAmount(2));
            Assert.AreEqual(5, _col0.GetAmount(4));
            
            Assert.AreEqual(_item, _col1[4]);
            Assert.AreEqual(0, _col1.GetAmount(0));
            Assert.AreEqual(0, _col1.GetAmount(2));
            Assert.AreEqual(3, _col1.GetAmount(4));
        }
        
        [Test]
        public void RemoveItemFromCollectionTest2()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item, 5);
            _col0.slots[4].SetWithoutValidation(_item, 5);
            
            _col1.slots[0].SetWithoutValidation(_item, 5);
            _col1.slots[2].SetWithoutValidation(_item, 5);
            _col1.slots[4].SetWithoutValidation(_item, 5);
            
            // Act
            var removed = _group.Remove(_item, 27);
            var arr = removed.result.ToArray();

            // Assert
            Assert.IsNull(removed.error);
            Assert.AreEqual(2, arr.Length);
            Assert.AreEqual(_col1, arr[0].collection);
            Assert.AreEqual(_col0, arr[1].collection);
            
            Assert.IsNull(arr[0].collection[0]);
            Assert.IsNull(arr[0].collection[2]);
            Assert.IsNull(arr[0].collection[4]);
            
            Assert.IsNull(arr[1].collection[0]);
            Assert.IsNull(arr[1].collection[2]);
            Assert.AreEqual(_item, arr[1].collection[4]);
            Assert.AreEqual(3, arr[1].collection.GetAmount(4));
            
            
            Assert.AreEqual(3, arr[0].affectedSlots.Length);
            Assert.AreEqual(0, arr[0].affectedSlots[0].slot);
            Assert.AreEqual(2, arr[0].affectedSlots[1].slot);
            Assert.AreEqual(4, arr[0].affectedSlots[2].slot);

            Assert.AreEqual(5, arr[1].affectedSlots[0].amount);
            Assert.AreEqual(5, arr[0].affectedSlots[1].amount);
            Assert.AreEqual(5, arr[0].affectedSlots[2].amount);
            
            Assert.AreEqual(3, arr[1].affectedSlots.Length);
            Assert.AreEqual(0, arr[1].affectedSlots[0].slot);
            Assert.AreEqual(2, arr[1].affectedSlots[1].slot);
            Assert.AreEqual(4, arr[1].affectedSlots[2].slot);
            Assert.AreEqual(5, arr[1].affectedSlots[0].amount);
            Assert.AreEqual(5, arr[1].affectedSlots[1].amount);
            Assert.AreEqual(2, arr[1].affectedSlots[2].amount);
            
            Assert.AreEqual(0, _col0.GetAmount(0));
            Assert.AreEqual(0, _col0.GetAmount(2));
            Assert.AreEqual(3, _col0.GetAmount(4));
            
            Assert.AreEqual(0, _col1.GetAmount(0));
            Assert.AreEqual(0, _col1.GetAmount(2));
            Assert.AreEqual(0, _col1.GetAmount(4));
        }

        [Test]
        public void SlotCountTest()
        {
            Assert.AreEqual(2, _group.collectionCount);
        }

        [Test]
        public void GetAmountTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item3, 5);
            _col0.slots[4].SetWithoutValidation(_item3, 5);
            
            _col1.slots[0].SetWithoutValidation(_item3, 5);
            _col1.slots[2].SetWithoutValidation(_item, 5);
            _col1.slots[4].SetWithoutValidation(_item, 5);
            
            // Assert
            Assert.AreEqual(15, _group.GetAmount(_item));
        }
        
        [Test]
        public void GetAmountPredicateTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item3, 5);
            _col0.slots[4].SetWithoutValidation(_item3, 5);
            
            _col1.slots[0].SetWithoutValidation(_item3, 5);
            _col1.slots[2].SetWithoutValidation(_item, 5);
            _col1.slots[4].SetWithoutValidation(_item, 5);
            
            // Assert
            Assert.AreEqual(15, _group.GetAmount(o => o?.ID == _item3.ID));
        }
        
        [Test]
        public void ContainsTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item2, 5);
            _col0.slots[4].SetWithoutValidation(_item2, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5);
            _col1.slots[2].SetWithoutValidation(_item3, 5);
            _col1.slots[4].SetWithoutValidation(_item2, 5);
            
            // Assert
            Assert.IsTrue(_group.Contains(_item));
            Assert.IsTrue(_group.Contains(_item3));
        }
        
        [Test]
        public void IndexOfTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item2, 5);
            _col0.slots[4].SetWithoutValidation(_item2, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5);
            _col1.slots[2].SetWithoutValidation(_item3, 5);
            _col1.slots[4].SetWithoutValidation(_item2, 5);
            
            // Act
            var index = _group.IndexOf(_item);
            var index2 = _group.IndexOf(_item3);
            
            // Assert
            Assert.IsNull(index.error);
            Assert.AreEqual(_col1, index.result.collection); // _col1 because _item2 is equal to _item1.
            Assert.AreEqual(0, index.result.index);
            
            Assert.IsNull(index2.error);
            Assert.AreEqual(_col1, index2.result.collection);
            Assert.AreEqual(2, index2.result.index);
        }
        
        [Test]
        public void IndexOfPredicateTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item2, 5);
            _col0.slots[4].SetWithoutValidation(_item2, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5);
            _col1.slots[2].SetWithoutValidation(_item3, 5);
            _col1.slots[4].SetWithoutValidation(_item2, 5);
            
            // Act
            var index = _group.IndexOf(o => o?.ID == _item2.ID);
            var index2 = _group.IndexOf(o => o?.ID == _item3.ID);
            
            // Assert
            Assert.IsNull(index.error);
            Assert.AreEqual(_col1, index.result.collection); // _col1 because _item2 is equal to _item1.
            Assert.AreEqual(0, index.result.index);
            
            Assert.IsNull(index2.error);
            Assert.AreEqual(_col1, index2.result.collection);
            Assert.AreEqual(2, index2.result.index);
        }
        
        [Test]
        public void IndexOfSpecificInstance()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item2, 5);
            _col0.slots[4].SetWithoutValidation(_item2, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5);
            _col1.slots[2].SetWithoutValidation(_item3, 5);
            _col1.slots[4].SetWithoutValidation(_item2, 5);
            
            // Act
            var index = _group.IndexOfSpecificInstance(_item);
            var index2 = _group.IndexOfSpecificInstance(_item3);
            
            // Assert
            Assert.IsNull(index.error);
            Assert.AreEqual(_col0, index.result.collection);
            Assert.AreEqual(0, index.result.index);
            
            Assert.IsNull(index2.error);
            Assert.AreEqual(_col1, index2.result.collection);
            Assert.AreEqual(2, index2.result.index);
        }
        
        [Test]
        public void IndexOfAllTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item2, 5);
            _col0.slots[4].SetWithoutValidation(_item2, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5);
            _col1.slots[2].SetWithoutValidation(_item3, 5);
            _col1.slots[4].SetWithoutValidation(_item2, 5);
            
            // Act
            var index = _group.IndexOfAll(_item).ToArray();
            var index2 = _group.IndexOfAll(_item3).ToArray();
            
            // Assert
            Assert.AreEqual(5, index.Length);
            Assert.AreEqual(0, index[0].index);
            Assert.AreEqual(_col1, index[0].collection);
            Assert.AreEqual(4, index[1].index);
            Assert.AreEqual(_col1, index[1].collection);
            
            Assert.AreEqual(0, index[2].index);
            Assert.AreEqual(_col0, index[2].collection);
            Assert.AreEqual(2, index[3].index);
            Assert.AreEqual(_col0, index[3].collection);
            Assert.AreEqual(4, index[4].index);
            Assert.AreEqual(_col0, index[4].collection);
            
            Assert.AreEqual(1, index2.Length);
            Assert.AreEqual(2, index2[0].index);
            Assert.AreEqual(_col1, index2[0].collection);
        }
        
        [Test]
        public void IndexOfAllPredicateTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5);
            _col0.slots[2].SetWithoutValidation(_item2, 5);
            _col0.slots[4].SetWithoutValidation(_item3, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5);
            _col1.slots[2].SetWithoutValidation(_item3, 5);
            _col1.slots[4].SetWithoutValidation(_item2, 5);
            
            // Act
            var index = _group.IndexOfAll(o => o?.ID == _item2.ID).ToArray();
            var index2 = _group.IndexOfAll(o => o?.ID == _item3.ID).ToArray();
            
            // Assert
            Assert.AreEqual(3, index.Length);
            Assert.AreEqual(0, index[0].index);
            Assert.AreEqual(_col1, index[0].collection);
            Assert.AreEqual(4, index[1].index);
            Assert.AreEqual(_col1, index[1].collection);
            
            Assert.AreEqual(2, index[2].index);
            Assert.AreEqual(_col0, index[2].collection);
            
            Assert.AreEqual(2, index2.Length);
            Assert.AreEqual(2, index2[0].index);
            Assert.AreEqual(_col1, index2[0].collection);
            Assert.AreEqual(4, index2[1].index);
            Assert.AreEqual(_col0, index2[1].collection);
        }

        [Test]
        public void GetCanAddAmountTest()
        {
            // Arrange
            _col0.slots[0].SetWithoutValidation(_item, 5); // Add 5
            _col0.slots[2].SetWithoutValidation(_item2, 5); // Add 5
            _col0.slots[4].SetWithoutValidation(_item3, 5);
            
            _col1.slots[0].SetWithoutValidation(_item2, 5); // Add 5
            _col1.slots[2].SetWithoutValidation(_item3, 5); 
            _col1.slots[4].SetWithoutValidation(_item2, 5); // Add 5
            
            // Act
            var amount = _group.GetCanAddAmount(_item2);

            // Assert
            Assert.AreEqual(60, amount);
        }

        [Test]
        public void CollectionGroupSimulationTest()
        {
            using (var sim = new CollectionGroupSimulation<IItemInstance, Collections.Collection<IItemInstance>>(_group))
            {
                var success = sim.group.Add(_item, 61);
                Assert.IsNull(success.error);

                var r = success.result.ToArray();
                Assert.AreEqual(2, r.Length);
                
                Assert.AreEqual(5, r[0].affectedSlots.Length);
                Assert.AreEqual(10, r[0].collection.GetAmount(r[0].affectedSlots[0].slot));
                Assert.AreEqual(10, r[0].collection.GetAmount(r[0].affectedSlots[1].slot));
                Assert.AreEqual(10, r[0].collection.GetAmount(r[0].affectedSlots[2].slot));
                Assert.AreEqual(10, r[0].collection.GetAmount(r[0].affectedSlots[3].slot));
                Assert.AreEqual(10, r[0].collection.GetAmount(r[0].affectedSlots[4].slot));
                
                Assert.AreEqual(2, r[1].affectedSlots.Length);
                Assert.AreEqual(10, r[1].collection.GetAmount(r[1].affectedSlots[0].slot));
                Assert.AreEqual(1, r[1].collection.GetAmount(r[1].affectedSlots[1].slot));
            }

            foreach (var item in _group.collections[0].collection)
            {
                Assert.IsNull(item);
            }

            foreach (var item in _group.collections[1].collection)
            {
                Assert.IsNull(item);
            }
        }

        [Test]
        public void CollectionsReadOnlyGroupTest()
        {
            // Arrange
            int addedItemCount = 0;
            int removedItemCount = 0;
            int slotsChangedCount = 0;
            
            _col0.isReadOnly = true;
            _col1.isReadOnly = true;

            _col0.OnAddedItem += (sender, result) => addedItemCount++;
            _col0.OnRemovedItem += (sender, result) => removedItemCount++;
            _col0.OnSlotsChanged += (sender, result) => slotsChangedCount++;
            
            _col1.OnAddedItem += (sender, result) => addedItemCount++;
            _col1.OnRemovedItem += (sender, result) => removedItemCount++;
            _col1.OnSlotsChanged += (sender, result) => slotsChangedCount++;
            
            // Act
            var canAddAmount = _group.GetCanAddAmount(_item);
            var canAdd = _group.CanAdd(_item);
            var addResult = _group.Add(_item);
            var canRemoveResult = _group.CanRemove(_item);
            var removeResult = _group.Remove(_item);
            
            // Assert
            Assert.AreEqual(0, addedItemCount);
            Assert.AreEqual(0, removedItemCount);
            Assert.AreEqual(0, slotsChangedCount);
            
            Assert.AreEqual(Errors.CollectionFull, canAdd.error);
            Assert.AreEqual(Errors.CollectionFull, addResult.error);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, canRemoveResult.error);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, removeResult.error);
            Assert.AreEqual(0, canAddAmount);
        }
        
    }
}