using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    
    internal class LayoutCollectionTests
    {

        private LayoutCollectionAccessibleMethods<IItemInstance> _collection;
        private const int CollectionSize = ColumnCount * 4;
        private const int ColumnCount = 4;

        private ItemInstance _item;
        private ItemInstance _item2;
        private ItemInstance _item3;
        private ItemInstance _item41x1;

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _collection = new LayoutCollectionAccessibleMethods<IItemInstance>(CollectionSize, ColumnCount, new Logger("[Collection] "));
            
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10, layoutShape = new SimpleShape2D(2, 2) };
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 10, layoutShape = new SimpleShape2D(2, 2) };
            var itemDef41x1 = new ItemDefinition(Guid.NewGuid()) {maxStackSize = 10, layoutShape = new SimpleShape2D(1, 1)};
            _item = new ItemInstance(Guid.NewGuid(), itemDef);
            _item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            _item3 = new ItemInstance(Guid.NewGuid(), itemDef2);
            _item41x1 = new ItemInstance(Guid.NewGuid(), itemDef41x1);
            
        }
        
        [Test]
        public void CreateNewCollectionSizeTest()
        {
            // Arrange, Act, Assert
            Assert.AreEqual(CollectionSize, _collection.slots.Length);
            Assert.AreEqual(CollectionSize, _collection.slotCount);
            Assert.AreEqual(4, _collection.rowCount);
        }

        [Test]
        public void CollectionSlotsShouldNotBeNullAndEmptyTest()
        {
            // Arrange, Act, Assert
            for (int i = 0; i < _collection.slots.Length; i++)
            {
                Assert.NotNull(_collection.slots[i]);
                Assert.IsNull(_collection.slots[i].occupiedBy);
                Assert.IsFalse(_collection.slots[i].isOccupied);
            }
        }

        [Test]
        public void CanSwapTest()
        {
            // Arrange
            
            // [0,0,3,3]
            // [0,0,3,3]
            // [?,?,?,?]
            // [?,?,?,?]
            var add1 = _collection.Add(_item, 2);
            var add2 = _collection.Add(_item2, 2);
            var add3 = _collection.Add(_item3, 2);

            // Act
            var canSwap0 = _collection.CanSwap(0, _collection, 0);
            var canSwap1 = _collection.CanSwap(0, _collection, 1);
            var canSwap2 = _collection.CanSwap(0, _collection, 2);
            var canSwap3 = _collection.CanSwap(0, _collection, 3);
            
            var canSwap4 = _collection.CanSwap(0, _collection, 0 + ColumnCount);
            var canSwap5 = _collection.CanSwap(0, _collection, 1 + ColumnCount);
            var canSwap6 = _collection.CanSwap(0, _collection, 2 + ColumnCount);
            var canSwap7 = _collection.CanSwap(0, _collection, 3 + ColumnCount);
            
            var canSwap8 = _collection.CanSwap(0, _collection, 0 + ColumnCount * 2);
            var canSwap9 = _collection.CanSwap(0, _collection, 1 + ColumnCount * 2);
            var canSwap10 = _collection.CanSwap(0, _collection, 2 + ColumnCount * 2);
            var canSwap11 = _collection.CanSwap(0, _collection, 3 + ColumnCount * 2);
            
            var canSwap12 = _collection.CanSwap(0, _collection, 0 + ColumnCount * 3);
            var canSwap13 = _collection.CanSwap(0, _collection, 1 + ColumnCount * 3);
            var canSwap14 = _collection.CanSwap(0, _collection, 2 + ColumnCount * 3);
            var canSwap15 = _collection.CanSwap(0, _collection, 3 + ColumnCount * 3);
            
            // Assert
            Assert.IsNull(add1.error);
            Assert.IsNull(add2.error);
            Assert.IsNull(add3.error);
            
            Assert.AreEqual(0, add1.result.affectedSlots[0].slot);
            Assert.AreEqual(0, add2.result.affectedSlots[0].slot);
            Assert.AreEqual(2, add3.result.affectedSlots[0].slot);
            
            Assert.IsNull(canSwap0.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap1.error);
            Assert.IsNull(canSwap2.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap3.error);
            
            Assert.IsNull(canSwap4.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap5.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap6.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap7.error);
            
            Assert.IsNull(canSwap8.error);
            Assert.IsNull(canSwap9.error);
            Assert.IsNull(canSwap10.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap11.error);
            
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap12.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap13.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap14.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap15.error);
        }
        
        [Test]
        public void CanSwapWithSmallerItemTest()
        {
            // Arrange
            
            // [0,0,1,1]
            // [0,0,1,1]
            // [4,?,?,?]
            // [?,?,?,?]
            var add1 = _collection.Set(0, _item, 2);
            var add2 = _collection.Set(2, _item2, 2);
            var add3 = _collection.Set(8, _item41x1, 2);
            
            // Act
            var canSwap0 = _collection.CanSwap(0, _collection, 0);
            var canSwap1 = _collection.CanSwap(0, _collection, 1);
            var canSwap2 = _collection.CanSwap(0, _collection, 2);
            var canSwap3 = _collection.CanSwap(0, _collection, 3);
            
            var canSwap4 = _collection.CanSwap(0, _collection, 0 + ColumnCount);
            var canSwap5 = _collection.CanSwap(0, _collection, 1 + ColumnCount);
            var canSwap6 = _collection.CanSwap(0, _collection, 2 + ColumnCount);
            var canSwap7 = _collection.CanSwap(0, _collection, 3 + ColumnCount);
            
            var canSwap8 = _collection.CanSwap(0, _collection, 0 + ColumnCount * 2);
            var canSwap9 = _collection.CanSwap(0, _collection, 1 + ColumnCount * 2);
            var canSwap10 = _collection.CanSwap(0, _collection, 2 + ColumnCount * 2);
            var canSwap11 = _collection.CanSwap(0, _collection, 3 + ColumnCount * 2);
            
            var canSwap12 = _collection.CanSwap(0, _collection, 0 + ColumnCount * 3);
            var canSwap13 = _collection.CanSwap(0, _collection, 1 + ColumnCount * 3);
            var canSwap14 = _collection.CanSwap(0, _collection, 2 + ColumnCount * 3);
            var canSwap15 = _collection.CanSwap(0, _collection, 3 + ColumnCount * 3);
            
            // Assert
            Assert.IsNull(add1.error);
            Assert.IsNull(add2.error);
            Assert.IsNull(add3.error);
            
            Assert.IsNull(canSwap0.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap1.error);
            Assert.IsNull(canSwap2.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap3.error);
            
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap4.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap5.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap6.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap7.error);
            
            Assert.IsNull(canSwap8.error);
            Assert.IsNull(canSwap9.error);
            Assert.IsNull(canSwap10.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap11.error);
            
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap12.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap13.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap14.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap15.error);
        }

        [Test]
        public void CanSwapWithSmallerItemTest2()
        {
            // Arrange
            
            // [0,0,1,1]
            // [0,0,1,1]
            // [4,?,?,?]
            // [?,?,?,?]
            var add1 = _collection.Add(_item, 2);
            var add2 = _collection.Add(_item2, 2);
            var add3 = _collection.Add(_item3, 2);
            var add4 = _collection.Add(_item41x1, 2);
            
            // Act
            var canSwap0 = _collection.CanSwap(0, _collection, 0);
            var canSwap1 = _collection.CanSwap(1, _collection, 0);
            var canSwap2 = _collection.CanSwap(2, _collection, 0);
            var canSwap3 = _collection.CanSwap(3, _collection, 0);
            
            var canSwap4 = _collection.CanSwap(0 + ColumnCount, _collection, 0);
            var canSwap5 = _collection.CanSwap(1 + ColumnCount, _collection, 0);
            var canSwap6 = _collection.CanSwap(2 + ColumnCount, _collection, 0);
            var canSwap7 = _collection.CanSwap(3 + ColumnCount, _collection, 0);
            
            var canSwap8 = _collection.CanSwap (0 + ColumnCount * 2, _collection, 0);
            var canSwap9 = _collection.CanSwap (1 + ColumnCount * 2, _collection, 0);
            var canSwap10 = _collection.CanSwap( 2 + ColumnCount * 2, _collection, 0);
            var canSwap11 = _collection.CanSwap( 3 + ColumnCount * 2, _collection, 0);
            
            var canSwap12 = _collection.CanSwap(0 + ColumnCount * 3, _collection, 0);
            var canSwap13 = _collection.CanSwap(1 + ColumnCount * 3, _collection, 0);
            var canSwap14 = _collection.CanSwap(2 + ColumnCount * 3, _collection, 0);
            var canSwap15 = _collection.CanSwap(3 + ColumnCount * 3, _collection, 0);
            
            // Assert
            Assert.IsNull(add1.error);
            Assert.IsNull(add2.error);
            Assert.IsNull(add3.error);
            Assert.IsNull(add4.error);
            
            Assert.AreEqual(0, add1.result.affectedSlots[0].slot);
            Assert.AreEqual(0, add2.result.affectedSlots[0].slot);
            Assert.AreEqual(2, add3.result.affectedSlots[0].slot);
            
            Assert.IsNull(canSwap0.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap1.error);
            Assert.IsNull(canSwap2.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap3.error);
            
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap4.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap5.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap6.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap7.error);
            
            Assert.IsNull(canSwap8.error);
            Assert.IsNull(canSwap9.error);
            Assert.IsNull(canSwap10.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap11.error);
            
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap12.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap13.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap14.error);
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, canSwap15.error);
        }
        
        [Test]
        public void GetCanAddAmountTest1()
        {
            var c = _collection.GetCanAddAmount(_item);
            
            Assert.IsNull(c.error);
            Assert.AreEqual(40, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest2()
        {
            // Arrange
            
            // [0,0,?,?]
            // [0,0,?,?]
            // [?,?,?,?]
            // [?,?,?,?]
            _collection.Set(0, _item, 3);

            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(37, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest3()
        {
            // Arrange
            
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            // [?,?,?,?]
            _collection.Set(1, _item, 3);
            
            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(27, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest4()
        {
            // Arrange
            
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            // [?,?,?,?]
            _collection.Set(1, _item3, 5);
            
            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(20, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest5()
        {
            // Arrange
            
            // [?,?,?,?]
            // [?,?,?,?]
            // [0,0,?,?]
            // [0,0,?,?]
            _collection.Set(ColumnCount * 2, _item, 3);
            
            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(37, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest6()
        {
            // Arrange
            
            // [?,?,?,?]
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            _collection.Set(1 + ColumnCount, _item, 3);
            
            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(7, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest7()
        {
            // Arrange
            
            // [?,?,?,4]
            // [?,4,?,?]
            // [?,?,4,?]
            // [4,?,?,?]
            _collection.Set(3, _item41x1, 1);
            _collection.Set(5, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(10, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(12, (IItemInstance)_item41x1.Clone(), 1);
            
            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(0, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest8()
        {
            // Arrange
            
            // [4,?,?,?]
            // [?,4,?,?]
            // [?,?,4,?]
            // [?,?,?,4]
            _collection.Set(0, _item41x1, 1);
            _collection.Set(5, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(10, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(15, (IItemInstance)_item41x1.Clone(), 1);
            
            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(20, c.result);
        }
        
        [Test]
        public void GetCanAddAmountTest9()
        {
            // Arrange
            
            // [4,4,4,4]
            // [?,?,?,?]
            // [4,4,4,4]
            // [?,?,?,0]
            _collection.Set(0, _item41x1, 1);
            _collection.Set(1, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(2, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(3, (IItemInstance)_item41x1.Clone(), 1);

            _collection.Set(8, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(9, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(10, (IItemInstance)_item41x1.Clone(), 1);
            _collection.Set(11, (IItemInstance)_item41x1.Clone(), 1);

            // Act
            var c = _collection.GetCanAddAmount(_item);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.AreEqual(0, c.result);
        }
        
        [Test]
        public void SetItemToNullShouldResetOccupiedBySlotsTest()
        {
            // Arrange
            
            // [?,?,?,?]
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            _collection.Set(1 + ColumnCount, _item, 3);
            
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[1 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[2 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[1 + ColumnCount * 2].occupiedBy);
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[2 + ColumnCount * 2].occupiedBy);
            
            // Act
            var c = _collection.Set(1 + ColumnCount, null);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.IsTrue(c.result);

            Assert.IsNull(_collection.slots[1 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[2 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[1 + ColumnCount * 2].occupiedBy);
            Assert.IsNull(_collection.slots[2 + ColumnCount * 2].occupiedBy);
        }
        
        [Test]
        public void SetItemToNewItemShouldResetOccupiedBySlotsTest()
        {
            // Arrange
            
            // [?,?,?,?]
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            
            // Act
            var set1 = _collection.Set(1 + ColumnCount, _item, 3);
            var set2 = _collection.Set(1 + ColumnCount, _item3);
            
            // Assert
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            Assert.IsTrue(set2.result);

            Assert.AreEqual(_item3, _collection[1 + ColumnCount]);
            
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[1 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[2 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[1 + ColumnCount * 2].occupiedBy);
            Assert.AreEqual(_collection.slots[1 + ColumnCount], _collection.slots[2 + ColumnCount * 2].occupiedBy);
        }
        
        [Test]
        public void SetItemToSmallerItemShouldResetOccupiedBySlotsTest()
        {
            // Arrange
            
            // [?,?,?,?]
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            _collection.Set(1 + ColumnCount, _item, 3);
            
            // Act
            var c = _collection.Set(1 + ColumnCount, _item41x1);
            
            // Assert
            Assert.IsNull(c.error);
            Assert.IsTrue(c.result);

            Assert.AreEqual(_item41x1, _collection[1 + ColumnCount]);
            
            Assert.IsNotNull(_collection.slots[1 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[2 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[1 + ColumnCount * 2].occupiedBy);
            Assert.IsNull(_collection.slots[2 + ColumnCount * 2].occupiedBy);
        }
        
        
        [Test]
        public void CanSetTest()
        {
            // Arrange
            
            // [?,?,?,?]
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            _collection.Set(1 + ColumnCount, _item, 3);

            // Act           
            // Assert
            Assert.IsTrue(_collection.CanSet(0, _item41x1).result);
            Assert.IsTrue(_collection.CanSet(1, _item41x1).result);
            Assert.IsTrue(_collection.CanSet(2, _item41x1).result);
            Assert.IsTrue(_collection.CanSet(3, _item41x1).result);
            
            Assert.IsTrue (_collection.CanSet(0 + ColumnCount, _item41x1).result);
            Assert.IsTrue (_collection.CanSet(1 + ColumnCount, _item41x1).result);
            Assert.IsFalse(_collection.CanSet(2 + ColumnCount, _item41x1).result);
            Assert.IsTrue (_collection.CanSet(3 + ColumnCount, _item41x1).result);

            Assert.IsTrue (_collection.CanSet(0 + ColumnCount * 2, _item41x1).result);
            Assert.IsFalse(_collection.CanSet(1 + ColumnCount * 2, _item41x1).result);
            Assert.IsFalse(_collection.CanSet(2 + ColumnCount * 2, _item41x1).result);
            Assert.IsTrue (_collection.CanSet(3 + ColumnCount * 2, _item41x1).result);

            Assert.IsTrue(_collection.CanSet(0 + ColumnCount * 3, _item41x1).result);
            Assert.IsTrue(_collection.CanSet(1 + ColumnCount * 3, _item41x1).result);
            Assert.IsTrue(_collection.CanSet(2 + ColumnCount * 3, _item41x1).result);
            Assert.IsTrue(_collection.CanSet(3 + ColumnCount * 3, _item41x1).result);
        }

        [Test]
        public void CanSetTest2()
        {
            // Arrange

            // [?,?,?,?]
            // [?,0,0,?]
            // [?,0,0,?]
            // [?,?,?,?]
            _collection.Set(1 + ColumnCount, _item, 3);

            // Act           
            // Assert
            for (int i = 0; i < _collection.slotCount; i++)
            {
                if (i == 1 + ColumnCount)
                {
                    continue;
                }
                
                Assert.IsFalse(_collection.CanSet(i, _item2).result, $"{i} failed.");
            }
        }

        [Test]
        public void AddObjectToCollectionTest1()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999, layoutShape = new SimpleShape2D(2,2) };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            // Act
            var canAdd = _collection.CanAdd(item);
            var result = _collection.Add(item);

            // Assert
            Assert.IsTrue(canAdd.result, "CanAdd returned false with message \"" + canAdd.error + "\"");
            Assert.AreEqual(16, _collection.slots.Length, "Collection length should not have been affected by add operation.");

            // Result values
            Assert.IsNull(result.error);
            Assert.AreEqual(1, result.result.affectedSlots.Length);
            Assert.AreEqual(0, result.result.affectedSlots[0].slot, "Result itemGuid does not match expected itemGuid");
            
            // Collection values
            Assert.AreEqual(item, _collection.slots[0].item, "Object has not been placed where expected ( [0] )");
            Assert.AreEqual(1, _collection.slots[result.result.affectedSlots[0].slot].amount, "Item amount does not match.");
            
            Assert.AreEqual(_collection.slots[1].occupiedBy, _collection.slots[0]);
            Assert.AreEqual(_collection.slots[0 + ColumnCount].occupiedBy, _collection.slots[0]);
            Assert.AreEqual(_collection.slots[1 + ColumnCount].occupiedBy, _collection.slots[0]);
        }

        [Test]
        public void GetCanAddAmountTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 1 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            // Act
            var result = _collection.GetCanAddAmount(item);

            // Assert
            Assert.AreEqual(16, result.result);
        }
        
        [Test]
        public void AddExceedingCollectionSize()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);

            // Act
            var result = _collection.Add(item, 5 * 16 + 1);

            // Assert
            Assert.AreEqual(Errors.CollectionFull, result.error);
        }

        [Test]
        public void AddObjectToCollectionMaxStackSizeTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 1 };
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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999, name = "ItemDef1" };
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999, name = "ItemDef2" };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            var item3 = new ItemInstance(Guid.NewGuid(), itemDef2);
            
            // Act
            var added = _collection.Add(item);
            var added2 = _collection.Add(item3);
            var added3 = _collection.Add(item2);

            // Assert
            Assert.IsNull(added.error);
            Assert.IsNull(added2.error);
            Assert.IsNull(added3.error);
            
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

            _collection.Set(0, (IItemInstance)item2.Clone(), 2);
            _collection.Set(1, (IItemInstance)item2.Clone(), 3);
            
            // Act
            var amount = _collection.GetCanAddAmount(item);

            // Collection values
            Assert.AreEqual((itemDef.maxStackSize * CollectionSize) - (itemDef.maxStackSize * 2), amount.result);
        }
        
        [Test]
        public void AddMoreItemsThanMaxStackSizeAllowsItemsShouldBeSplitOverSlotsTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            
            // Act
            _collection.Set(0, (IItemInstance)item.Clone(), 2);
            _collection.Set(6, (IItemInstance)item.Clone(), 1);
            _collection.Set(7, (IItemInstance)item.Clone(), itemDef.maxStackSize);
            
            _collection.Add(item2, 8);

            // Assert
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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
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
        public void SetSameItemInSlotReduceItemEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            _collection.Set(0, item, 5);

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
            Assert.AreEqual(_collection.slots[0], _collection.slots[removeResult.affectedSlots[0].slot]);
            
            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }
        
        [Test]
        public void SetSameItemInSlotIncreaseItemEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            _collection.Set(0, item, 2);

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
            var set2 = _collection.Set(0, item, 5);

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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef);
            
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
            var set1 = _collection.Set(0, item, 2);
            var set2 = _collection.Set(0, item2, 5);

            // Assert
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            Assert.AreEqual(5, _collection.slots[0].amount);
            
            Assert.AreEqual(2, addEventCount);
            Assert.AreEqual(0, removeEventCount);
            Assert.AreEqual(2, changeEventCount);
            
            Assert.AreEqual(1, addResult.affectedSlots.Length);
            Assert.AreEqual(3, addResult.affectedSlots[0].amount);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);
            
            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }
        
        [Test]
        public void SetSlotWithExceedingMaxStackSizeShouldThrowExceptionTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
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
            
            _collection.Set(0, _item, 2);

            // Assert
            Assert.AreEqual(_item, _collection.slots[0].item);
            
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
            
            _collection.Set(0, _item, 2);
            var setResult = _collection.Set(0, null);

            // Assert
            Assert.IsTrue(setResult.result);
            
            Assert.IsNull(_collection.slots[0].item);
            Assert.AreEqual(0, _collection.slots[0].amount);
            
            Assert.AreEqual(1, addEventCount);
            Assert.AreEqual(1, removeEventCount);
            Assert.AreEqual(2, changeEventCount);
            
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

            var set1 = _collection.Set(0, _item, 3);
            var set2 = _collection.Set(0, _item2, 2);

            // Assert
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.AreEqual(1, addEventCount);
            Assert.AreEqual(1, removeEventCount);
            Assert.AreEqual(2, changeEventCount);
            
            Assert.AreEqual(1, addResult.affectedSlots.Length);
            Assert.AreEqual(3, addResult.affectedSlots[0].amount);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);
            
            Assert.AreEqual(1, removeResult.affectedSlots.Length);
            Assert.AreEqual(1, removeResult.affectedSlots[0].amount);
            Assert.AreEqual(_collection.slots[0], _collection.slots[addResult.affectedSlots[0].slot]);
            
            Assert.AreEqual(1, changeResult.affectedSlots.Length);
            Assert.AreEqual(_collection.slots[0], _collection.slots[changeResult.affectedSlots[0]]);
        }
        
        [Test]
        public void SwapElementWithEmptySlotThatIsBlockedTest()
        {
            // Arrange
            _collection.Set(0, _item, 2);
            _collection.Set(2, _item2, 3);
            
            // Act
            var moved = _collection.Swap(2, _collection, 1, false);

            // Assert
            Assert.AreEqual(Errors.LayoutCollectionItemBlocked, moved.error);
        }
        
        [Test]
        public void SwapTwoElementsInCollectionTest()
        {
            // Arrange
            _collection.Set(0, _item, 2);
            _collection.Set(2, _item2, 3);
            
            // Act
            var moved = _collection.Swap(0, _collection, 2, false);

            // Assert
            Assert.IsNull(moved.error);

            Assert.AreEqual(_item2, _collection.slots[0].item);
            Assert.AreEqual(3, _collection.slots[0].amount);
            
            Assert.AreEqual(_item, _collection.slots[2].item);
            Assert.AreEqual(2, _collection.slots[2].amount);
        }
        
        [Test]
        public void SwapToNewSlotCheckOccupiedByTest()
        {
            // Arrange
            _collection.Set(0, _item, 2);
            _collection.Set(2, _item2, 3);
            
            // Act
            var moved = _collection.Swap(0, _collection, ColumnCount * 2, false);

            // Assert
            Assert.IsNull(moved.error);
            
            Assert.IsNull(_collection.slots[0].occupiedBy);
            Assert.IsNull(_collection.slots[1].occupiedBy);
            Assert.IsNull(_collection.slots[0 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[1 + ColumnCount].occupiedBy);
            
            Assert.AreEqual(_collection.slots[0 + ColumnCount * 2], _collection.slots[0 + ColumnCount * 2].occupiedBy);
            Assert.AreEqual(_collection.slots[0 + ColumnCount * 2], _collection.slots[1 * ColumnCount * 2].occupiedBy);
            Assert.AreEqual(_collection.slots[0 + ColumnCount * 2], _collection.slots[0 + ColumnCount * 3].occupiedBy);
            Assert.AreEqual(_collection.slots[0 + ColumnCount * 2], _collection.slots[1 + ColumnCount * 3].occupiedBy);

            Assert.AreEqual(_item2, _collection.slots[0 + ColumnCount * 2].item);
            Assert.AreEqual(2, _collection.slots[0 + ColumnCount * 2].amount);
            
            Assert.AreEqual(_item, _collection.slots[2].item);
            Assert.AreEqual(3, _collection.slots[2].amount);
        }
        
        [Test]
        public void SwapTwoEmptySlotsTest()
        {
            // Arrange
            var moved = _collection.Swap(0, _collection, 2, false);

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
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var item2 = new ItemInstance(Guid.NewGuid(), itemDef2);

            _collection.Set(0, item, 2);
            
            _collection.Set(2, item2, 2);
            
            // Act
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                _collection.Swap(2, _collection, -1, false);
            });

            // Assert
        }
        
        [Test]
        public void SwapElementInCollectionShouldFireSlotsChangedEventTest()
        {
            // Arrange
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 3 };
            var itemDef2 = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5 };
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
            _collection.Swap(2, _collection, 1, false);

            // Assert
            Assert.AreEqual(1, changedEventCount);
            
            Assert.AreEqual(2, changedResult.affectedSlots.Length);
            Assert.AreEqual(2, changedResult.affectedSlots[0]);
            Assert.AreEqual(1, changedResult.affectedSlots[1]);
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
            Assert.AreEqual(16, _collection.slotCount);
            _collection.Shrink(5);
            Assert.AreEqual(11, _collection.slotCount);
        }

        [Test]
        public void SetAmounToZeroShouldClearTest()
        {
            var set = _collection.Set(0, _item);

            Assert.AreEqual(_collection.slots[0], _collection.slots[0].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[0 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1 + ColumnCount].occupiedBy);
            
            var setAmount = _collection.Set(0, _item, 0);

            Assert.IsNull(set.error);
            Assert.IsNull(setAmount.error);
            
            Assert.IsNull(_collection.slots[0].occupiedBy);
            Assert.IsNull(_collection.slots[1].occupiedBy);
            Assert.IsNull(_collection.slots[0 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[1 + ColumnCount].occupiedBy);
        }
        
        [Test]
        public void SetAmounToZeroShouldClearTest2()
        {
            var set = _collection.Set(0, _item);

            Assert.AreEqual(_collection.slots[0], _collection.slots[0].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[0 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1 + ColumnCount].occupiedBy);
            
            var setAmount = _collection.Remove(_item);

            Assert.IsNull(set.error);
            Assert.IsNull(setAmount.error);
            
            Assert.IsNull(_collection.slots[0].occupiedBy);
            Assert.IsNull(_collection.slots[1].occupiedBy);
            Assert.IsNull(_collection.slots[0 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[1 + ColumnCount].occupiedBy);
        }
        
        [Test]
        public void SetAmounToZeroShouldClearTest3()
        {
            var set = _collection.Set(0, _item, 2);

            Assert.AreEqual(_collection.slots[0], _collection.slots[0].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[0 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1 + ColumnCount].occupiedBy);
            
            var setAmount = _collection.Remove(_item, 1);

            Assert.IsNull(set.error);
            Assert.IsNull(setAmount.error);
            
            Assert.AreEqual(_collection.slots[0], _collection.slots[0].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[0 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1 + ColumnCount].occupiedBy);
        }
        
        [Test]
        public void SetAmounToZeroShouldClearTest4()
        {
            var set = _collection.Set(0, _item);

            Assert.AreEqual(_collection.slots[0], _collection.slots[0].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[0 + ColumnCount].occupiedBy);
            Assert.AreEqual(_collection.slots[0], _collection.slots[1 + ColumnCount].occupiedBy);
            
            var setAmount = _collection.Set(0, null);

            Assert.IsNull(set.error);
            Assert.IsNull(setAmount.error);
            
            Assert.IsNull(_collection.slots[0].occupiedBy);
            Assert.IsNull(_collection.slots[1].occupiedBy);
            Assert.IsNull(_collection.slots[0 + ColumnCount].occupiedBy);
            Assert.IsNull(_collection.slots[1 + ColumnCount].occupiedBy);
        }
        
        [Test]
        public void CloneTest()
        {
            var clone = (LayoutCollection<IItemInstance>)_collection.Clone();
            
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
        
        
        
        
        
        // TODO: Test Swap a negative state (A test where items can't be moved due to some restriction)
        
        // TODO: Test Sorting
        // TODO: Test collection config (read-only / readwrite, name)
        
        // TODO: Test collection replicator
        // TODO: Test collections with differently sized items (item layout sizes)
    }
}
