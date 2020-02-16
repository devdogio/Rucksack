using System;
using System.Linq;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    internal class CollectionRestrictionTests
    {
        private CollectionAccessibleMethods<IItemInstance> _collection;
        private const int CollectionSize = 10;
        
        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _collection = new CollectionAccessibleMethods<IItemInstance>(CollectionSize, new Logger("[Collection] "));
//            _collection.GenerateSlots<ItemInstanceLayoutCollectionSlot>();
        }
        
        [Test]
        public void CollectionAddRestrictionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Add(item);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.IsTrue(!_collection.slots[0].isOccupied);
        }
        
        [Test]
        public void CollectionSetAddRestrictionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Set(0, item);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.IsTrue(!_collection.slots[0].isOccupied);
        }
        
        [Test]
        public void CollectionSetAddRestrictionTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 2);
            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Set(0, item, 3);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(2, _collection.slots[0].amount);
        }

        [Test]
        public void GetCanAddAmountTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.GetCanAddAmount(item);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(0, result.result);
        }
        
        
        
        [Test]
        public void CollectionRemoveRestrictionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 1);
            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Remove(item);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(1, _collection.slots[0].amount);
        }
        
        [Test]
        public void CollectionSetOverwriteRestrictionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid());
            var itemDef2 = new ItemDefinition(Guid.NewGuid());
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item);
            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Set(0, item2, 1);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(1, _collection.slots[0].amount);
        }
        
        [Test]
        public void CollectionSetOverwriteRestrictionTest2()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var itemDef2 = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 3);
            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Set(0, item2, 2);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);
        }
        
        [Test]
        public void CollectionSetAmountRemoveRestrictionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 10 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Set(0, item, 2);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);
        }
        
        [Test]
        public void CollectionSetRemoveRestrictionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            _collection.Set(0, item, 3);
            _collection.restrictions.Add(new FakeCollectionRestriction());
            
            // Act
            var result = _collection.Set(0, item, 2);
            
            // Assert
            Assert.AreEqual(Errors.CollectionRestrictionPreventedAction, result.error);
            Assert.AreEqual(item, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);
        }

        [Test]
        public void ReadOnlyCollectionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            int addedItemCount = 0;
            int removedItemCount = 0;
            int slotsChangedCount = 0;
            
            var set1 = _collection.Set(0, item, 3);

            _collection.isReadOnly = true;

            _collection.OnAddedItem += (sender, result) => addedItemCount++;
            _collection.OnRemovedItem += (sender, result) => removedItemCount++;
            _collection.OnSlotsChanged += (sender, result) => slotsChangedCount++;
            
            // Act
            var canSetResult = _collection.CanSet(0, item);
            var setResult = _collection.Set(0, item, 2);
            var canAddAmount = _collection.GetCanAddAmount(item);
            var canAdd = _collection.CanAdd(item);
            var addResult = _collection.Add(item);
            var swap = _collection.SwapPublic(0, _collection, 1);
            var merge = _collection.MergePublic(0, _collection, 1, _collection.GetAmount(0));
            var canRemoveResult = _collection.CanRemove(item);
            var removeResult = _collection.Remove(item);

            _collection.Clear();
            _collection.Expand<FakeCollectionSlot>(2);
            _collection.Shrink(5);
            _collection.Sort(null);

            _collection.GenerateSlots<FakeCollectionSlot>();
            _collection.GenerateSlotsRange<FakeCollectionSlot>(0, 5);
            
            // Assert
            Assert.IsNull(set1.error);
            
            Assert.AreEqual(item, _collection[0]);
            Assert.AreEqual(3, _collection.slots[0].amount);
            
            Assert.AreEqual(0, addedItemCount);
            Assert.AreEqual(0, removedItemCount);
            Assert.AreEqual(0, slotsChangedCount);
            
            Assert.AreEqual(Errors.CollectionIsReadOnly, canSetResult.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, setResult.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, canAdd.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, addResult.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, canRemoveResult.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, removeResult.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, merge.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, swap.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, canAddAmount.error);
            
            Assert.AreEqual(10, _collection.slotCount);
            Assert.IsTrue(_collection.slots.All(o => o.GetType() == typeof(CollectionSlot<IItemInstance>)));
        }
        
        [Test]
        public void FactoryEmptyConstructorTest()
        {
            var inst = CollectionRestrictionFactory.Create<FakeCollectionRestriction>();
            Assert.IsNotNull(inst);            
            Assert.IsNull(inst.collection);            
        }
        
        [Test]
        public void FactoryEmptyConstructorTest2()
        {
            var inst = CollectionRestrictionFactory.Create<FakeCollectionRestriction>();
            Assert.IsNotNull(inst);            
            Assert.IsNull(inst.collection);            
        }
        
        [Test]
        public void FactoryEmptyConstructorTest3()
        {
            var inst = (FakeCollectionRestriction)CollectionRestrictionFactory.Create(typeof(FakeCollectionRestriction));
            Assert.IsNotNull(inst);
            Assert.IsNull(inst.collection);
        }
        
        [Test]
        public void FactoryCollectionConstructorTest()
        {
            var inst = CollectionRestrictionFactory.Create<FakeCollectionRestriction>(_collection);
            Assert.IsNotNull(inst);
            Assert.IsNotNull(inst.collection);
        }
        
        [Test]
        public void FactoryCollectionConstructorTest2()
        {
            var inst = (FakeCollectionRestriction)CollectionRestrictionFactory.Create(typeof(FakeCollectionRestriction), _collection);
            Assert.IsNotNull(inst);
            Assert.IsNotNull(inst.collection);
        }
        
        [Test]
        public void FactoryCollectionWrongTypeTest()
        {
            Assert.Catch<MissingMethodException>(() =>
            {
                CollectionRestrictionFactory.Create(typeof(IItemInstance));
            });
        }
        
        [Test]
        public void FactoryCollectionWrongTypeTest2()
        {
            Assert.Catch<MissingMethodException>(() =>
            {
                CollectionRestrictionFactory.Create(typeof(IItemInstance), _collection);
            });
        }
        
        [Test]
        public void FactoryCollectionWrongTypeTest3()
        {
            var inst = (FakeCollectionRestriction)CollectionRestrictionFactory.Create(typeof(FakeCollectionRestriction), (ICollection)null); // Defaults to empty constructor because given type is not a collection
            Assert.IsNotNull(inst);
            Assert.IsNull(inst.collection);
        }
    }
}