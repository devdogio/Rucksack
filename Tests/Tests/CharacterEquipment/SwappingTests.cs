using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class SwappingTests
    {
        private EquipmentCollectionAccessibleMethods<IEquippableItemInstance> _equipmentCollection;
        private EquippableCharacter<IItemInstance, IEquippableItemInstance> _character;
        private Collection<IItemInstance> _restoreToCollection;

        private MockedEquippableItemInstance _headItem;
        private MockedEquippableItemInstance _bodyItem;
        private MockedEquippableItemInstance _feetItem;
        private MockedEquippableItemInstance _swordItem;
        private MockedEquippableItemInstance _shieldItem;
        private MockedEquippableItemInstance _twoHandedSwordItem;
        private MockedEquippableItemInstance _arrowsItem;

        private IEquipmentType _headEquipmentType;
        private IEquipmentType _bodyEquipmentType;
        private IEquipmentType _feetEquipmentType;
        private IEquipmentType _swordEquipmentType;
        private IEquipmentType _shieldEquipmentType;
        private IEquipmentType _twoHandedSwordEquipmentType;
        private IEquipmentType _arrowsEquipmentType;

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _restoreToCollection = new Collection<IItemInstance>(10, new Logger("[RestoreCollection] "));
            _equipmentCollection = new EquipmentCollectionAccessibleMethods<IEquippableItemInstance>(5, null, new Logger("[Equipment] "));
            _character = new EquippableCharacter<IItemInstance, IEquippableItemInstance>(_equipmentCollection, new CollectionGroup<IItemInstance>(_restoreToCollection), new Logger("[EquippableCharacter] "));
            _equipmentCollection.characterOwner = _character;
            
            _headEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Head"};
            _bodyEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Body"};
            _feetEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Feet"};
            _swordEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Sword"};
            _shieldEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Shield"};
            _twoHandedSwordEquipmentType = new EquipmentType(Guid.NewGuid(), _swordEquipmentType, _shieldEquipmentType, _arrowsEquipmentType) {name = "2 Handed sword"};
            _arrowsEquipmentType = new EquipmentType(Guid.NewGuid(), _swordEquipmentType, _shieldEquipmentType) {name = "Arrows"};

            _headItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_headEquipmentType));
            _bodyItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_bodyEquipmentType));
            _feetItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_feetEquipmentType));
            _swordItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_swordEquipmentType));
            _shieldItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_shieldEquipmentType));
            _twoHandedSwordItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_twoHandedSwordEquipmentType));
            _arrowsItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_arrowsEquipmentType, 999));

            _equipmentCollection.SetAllEquipmentTypes(new IEquipmentType[][]
            {
                new[] {_headEquipmentType}, // Head 
                new[] {_bodyEquipmentType}, // Body 
                new[] {_feetEquipmentType}, // Feet 
                new[] {_swordEquipmentType, _twoHandedSwordEquipmentType}, // Right Hand 
                new[] {_swordEquipmentType, _shieldEquipmentType, _arrowsEquipmentType}, // Left Hand 
            });
            
//            _equipmentCollection.SetAllMountPoints(new IMountPoint<IEquippableItemInstance>[]
//            {
//                new MockedMountPoint("1"), 
//                new MockedMountPoint("2"), 
//                new MockedMountPoint("3"), 
//                new MockedMountPoint("4"), 
//                new MockedMountPoint("5"), 
//            });
        }
        
        private EquippableItemDefinition CreateDemoItemDef(IEquipmentType type, int maxStackSize = 1)
        {
            return new EquippableItemDefinition(Guid.NewGuid())
            {
                maxStackSize = maxStackSize,
                equipmentType = type
            };
        }
        
        [Test]
        public void SwapIncompatibleTest()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);

            var set = col.Set(0, _swordItem, 1);
            var swapped = col.SwapPublic(0, _equipmentCollection, 2);
            
            Assert.IsNull(set.error);
            Assert.AreEqual(Errors.CharacterCollectionEquipmentTypeInvalid, swapped.error);

            Assert.AreSame(_swordItem, col[0]);
            Assert.IsNull(_equipmentCollection[4]);
            Assert.AreEqual(0, _swordItem.onEquippedCallCount);
            Assert.AreEqual(0, _swordItem.onUnEquippedCallCount);
        }

        [Test]
        public void SwapStackable()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            var arrowsClone = (MockedEquippableItemInstance) _arrowsItem.Clone();
            
            var set = col.Set(0, _arrowsItem, 5);
            var set2 = _character.EquipAt(4, arrowsClone, 2);
            var swapped = col.SwapPublic(0, _equipmentCollection, 4);
            
            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(swapped.error);

            Assert.AreSame(arrowsClone, col[0]);
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            
            // Events aren't fired because we don't use the IEquippableCharacter, and move to the collection directly.
            Assert.AreEqual(0, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(0, _arrowsItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, arrowsClone.onEquippedCallCount);
            Assert.AreEqual(0, arrowsClone.onUnEquippedCallCount);
        }
        
        [Test]
        public void SwapSameItem()
        {
            var swordClone = (MockedEquippableItemInstance) _swordItem.Clone();
            
            var set1 = _character.EquipAt(3, swordClone, 1);
            var set2 = _character.EquipAt(4, _swordItem, 1);
            var swapped = _character.SwapOrMerge(4, 3);
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(swapped.error);

            Assert.AreSame(swordClone, _equipmentCollection[4]);
            Assert.AreSame(_swordItem, _equipmentCollection[3]);
            
            Assert.AreEqual(1, _swordItem.onEquippedCallCount);
            Assert.AreEqual(0, _swordItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, swordClone.onEquippedCallCount);
            Assert.AreEqual(0, swordClone.onUnEquippedCallCount);
        }
        
        [Test]
        public void MergeStackableItemsFullStack()
        {
            var arrowsClone = (MockedEquippableItemInstance) _arrowsItem.Clone();
            
            var set = _restoreToCollection.Set(0, _arrowsItem, 5);
            var set2 = _restoreToCollection.Set(1, arrowsClone, 5);
            var set3 = _character.EquipAt(4, _arrowsItem, 5);
            var merged = _character.SwapOrMerge(0, 4);
            
            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(set3.error);
            Assert.IsNull(merged.error);

            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            
            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(0, _arrowsItem.onUnEquippedCallCount);
            
            Assert.AreEqual(0, arrowsClone.onEquippedCallCount);
            Assert.AreEqual(0, arrowsClone.onUnEquippedCallCount);
        }
        
        [Test]
        public void MergeStackableItemsPartialStack()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            var arrowsClone = (MockedEquippableItemInstance) _arrowsItem.Clone();
            
            _equipmentCollection.SetEquipmentTypes(0, new []{ _arrowsEquipmentType });
            
            var set = col.Set(0, _arrowsItem, 5);
            var set1 = _character.EquipAt(0, _arrowsItem, 5);
            var set2 = _character.EquipAt(4, arrowsClone, 2);
            var merged = _character.SwapOrMerge(0, 4);
            
            Assert.IsNull(set.error);
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(merged.error);

            Assert.IsNull(col[0]);
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            Assert.AreEqual(7, _equipmentCollection.GetAmount(4));
            
            // _arrowsItem never gets an OnEquipped because we're moving a partial stack, which duplicates the instance.
            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(0, _arrowsItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, arrowsClone.onEquippedCallCount);
            Assert.AreEqual(0, arrowsClone.onUnEquippedCallCount);
        }

        [Test]
        public void UnstackToSlotIntoChar()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            
            var set = col.Set(0, _arrowsItem, 100);
            var merged = col.MergePublic(0, _equipmentCollection, 4, 40);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            
            Assert.AreEqual(col[0], _equipmentCollection[4]);
            Assert.AreNotSame(col[0], _equipmentCollection[4]);
            
            Assert.AreEqual(60, col.GetAmount(0));
            Assert.AreEqual(40, _equipmentCollection.GetAmount(4));
        }

        [Test]
        public void UnstackToSlotFromChar()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            
            var set = _character.EquipAt(4, _arrowsItem, 100);
            var merged = _equipmentCollection.MergePublic(4, col, 0, 40);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            
            Assert.AreEqual(col[0], _equipmentCollection[4]);
            Assert.AreNotSame(col[0], _equipmentCollection[4]);
            
            Assert.AreEqual(40, col.GetAmount(0));
            Assert.AreEqual(60, _equipmentCollection.GetAmount(4));
        }
        
        [Test]
        public void MoveAutoPartialToEquipTest()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            
            var set = col.Set(0, _arrowsItem, 100);
            var merged = col.MoveAuto(0, _equipmentCollection, 40);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            
            Assert.AreEqual(col[0], _equipmentCollection[4]);
            Assert.AreNotSame(col[0], _equipmentCollection[4]);
            
            Assert.AreEqual(60, col.GetAmount(0));
            Assert.AreEqual(40, _equipmentCollection.GetAmount(4));
        }

        [Test]
        public void MoveAutoFullStackToEquipTest()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            
            var set = col.Set(4, _arrowsItem, 100);
            var merged = col.MoveAuto(4, _equipmentCollection, 100);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            
            Assert.IsNull(col[0]);
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            
            Assert.AreEqual(0, col.GetAmount(0));
            Assert.AreEqual(100, _equipmentCollection.GetAmount(4));
        }

        [Test]
        public void MoveAutoFromEquipPartialTest()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            
            var set = _character.EquipAt(4, _arrowsItem, 100);
            var merged = _equipmentCollection.MoveAuto(4, col, 40);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            Assert.AreNotSame(_arrowsItem, col[0]);
            
            Assert.AreEqual(40, col.GetAmount(0));
            Assert.AreEqual(60, _equipmentCollection.GetAmount(4));
        }

        [Test]
        public void MoveAutoFromEquipFullStackTest()
        {
            var col = new CollectionAccessibleMethods<IItemInstance>(10);
            
            var set = _character.EquipAt(4, _arrowsItem, 100);
            var merged = _equipmentCollection.MoveAuto(4, col, 100);
            
            Assert.IsNull(set.error);
            Assert.IsNull(merged.error);
            
            Assert.IsNull(_equipmentCollection[4]);
            Assert.AreSame(_arrowsItem, col[0]);
            
            Assert.AreEqual(100, col.GetAmount(0));
            Assert.AreEqual(0, _equipmentCollection.GetAmount(4));
        }
        
    }
}