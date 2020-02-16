using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class EquippableCharacterTests
    {
        private EquipmentCollection<IEquippableItemInstance> _equipmentCollection;
        private EquippableCharacter<IItemInstance, IEquippableItemInstance> _equippableCharacter;
        private Collection<IItemInstance> _restoreToCollection;

        private MockedEquippableItemInstance _headItem;
        private MockedEquippableItemInstance _bodyItem;
        private MockedEquippableItemInstance _feetItem;
        private MockedEquippableItemInstance _swordItem;
        private MockedEquippableItemInstance _swordItem2;
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
            _equipmentCollection = new EquipmentCollection<IEquippableItemInstance>(5, _equippableCharacter, new Logger("[Equipment] "));
            _equippableCharacter = new EquippableCharacter<IItemInstance, IEquippableItemInstance>(_equipmentCollection, new CollectionGroup<IItemInstance>(_restoreToCollection), new Logger("[EquippableCharacter] "));
            _equipmentCollection.characterOwner = _equippableCharacter;
            
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
            _swordItem2 = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_swordEquipmentType));
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

            var mountPoints = new IMountPoint<IEquippableItemInstance>[]
            {
                new MockedMountPoint("1"),
                new MockedMountPoint("2"),
                new MockedMountPoint("3"),
                new MockedMountPoint("4"),
                new MockedMountPoint("5"),
            };
            
//            _equipmentCollection.SetAllMountPoints(mountPoints);
            _equippableCharacter.mountPoints = mountPoints;
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
        public void EquippableCharacterEquipTests()
        {
            var equipped = _equippableCharacter.Equip(_headItem, 1);

            Assert.IsNull(equipped.error);

            Assert.AreEqual(1, equipped.result.Length);
            Assert.AreEqual(1, equipped.result[0].equippedAmount);
            Assert.AreSame(_headItem, equipped.result[0].equippedItem);
            Assert.AreEqual(0, equipped.result[0].index);

            Assert.AreEqual(_headItem, _equipmentCollection[0]);
            Assert.AreEqual(1, _equipmentCollection.GetAmount(0));
        }
        
        [Test]
        public void EquippableCharacterEquipAtTests()
        {
            var equipped = _equippableCharacter.EquipAt(0, _headItem, 1);

            Assert.IsNull(equipped.error);
            Assert.AreEqual(1, equipped.result.equippedAmount);
            Assert.AreSame(_headItem, equipped.result.equippedItem);
            Assert.AreEqual(0, equipped.result.index);
            
            Assert.AreEqual(_headItem, _equipmentCollection[0]);
            Assert.AreEqual(1, _equipmentCollection.GetAmount(0));
        }
        
        [Test]
        public void EquippableCharacterEquipAtWrongSlotTests()
        {
            var equipped = _equippableCharacter.EquipAt(1, _headItem, 1);

            Assert.AreEqual(Errors.CharacterCollectionEquipmentTypeInvalid, equipped.error);
            
            Assert.IsNull(_equipmentCollection[0]);
            Assert.AreEqual(0, _equipmentCollection.GetAmount(0));
            
            Assert.IsNull(_equipmentCollection[1]);
            Assert.AreEqual(0, _equipmentCollection.GetAmount(1));
        }
        
        [Test]
        public void EquippableCharacterEquipShouldRemoveFromSourceCollectionTests()
        {
            var set = _restoreToCollection.Set(0, _headItem, 1);
            var equipped = _equippableCharacter.Equip(_headItem, 1);

            Assert.IsNull(set.error);
            Assert.IsNull(equipped.error);
            Assert.AreEqual(1, equipped.result[0].equippedAmount);
            Assert.AreSame(_headItem, equipped.result[0].equippedItem);
            Assert.AreEqual(0, equipped.result[0].index);
            
            Assert.AreEqual(_headItem, _equipmentCollection[0]);
            Assert.AreEqual(1, _equipmentCollection.GetAmount(0));
            
            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreEqual(0, _restoreToCollection.GetAmount(0));
        }
        
        [Test]
        public void EquippableCharacterEquipShouldRemoveFromSourceCollectionEquipAtTests()
        {
            var set = _restoreToCollection.Set(0, _headItem, 1);
            var equipped = _equippableCharacter.EquipAt(0, _headItem, 1);

            Assert.IsNull(set.error);
            Assert.IsNull(equipped.error);
            Assert.AreEqual(1, equipped.result.equippedAmount);
            Assert.AreSame(_headItem, equipped.result.equippedItem);
            Assert.AreEqual(0, equipped.result.index);
            
            Assert.AreEqual(_headItem, _equipmentCollection[0]);
            Assert.AreEqual(1, _equipmentCollection.GetAmount(0));
            
            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreEqual(0, _restoreToCollection.GetAmount(0));
        }
        
        [Test]
        public void EquipSmallAmountOfStackTest()
        {
            var set = _restoreToCollection.Set(0, _arrowsItem, 10);
            var equipped = _equippableCharacter.EquipAt(4, _arrowsItem, 6);

            Assert.IsNull(set.error);
            Assert.IsNull(equipped.error);
            
            Assert.AreEqual(_arrowsItem, _equipmentCollection[4]);
            Assert.AreEqual(6, _equipmentCollection.GetAmount(4));
            
            Assert.AreEqual(_arrowsItem, _restoreToCollection[0]);
            Assert.AreEqual(4, _restoreToCollection.GetAmount(0));
            
            // Item should've been duplicated and can't be same reference.
            Assert.AreNotSame(_equipmentCollection[0], _restoreToCollection[0]);
        }
        
        [Test]
        public void EquipFromMultipleStacksTest()
        {
            // Add to start collection
            var arrowClone = (IEquippableItemInstance) _arrowsItem.Clone();
            var set = _restoreToCollection.Set(0, _arrowsItem, 3);
            var set2 = _restoreToCollection.Set(1, arrowClone, 4);
            
            // Equip should remove items from startCollection and add to equipment collection
            var equipped = _equippableCharacter.EquipAt(4, _arrowsItem, 5);

            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, equipped.error);
            
            Assert.IsNull(_equipmentCollection[4]);
            Assert.AreEqual(0, _equipmentCollection.GetAmount(4));

            Assert.AreEqual(3, _restoreToCollection.GetAmount(0));
            Assert.AreEqual(4, _restoreToCollection.GetAmount(1));
            
            Assert.AreSame(arrowClone, _restoreToCollection[1]);
        }
                
        [Test]
        public void EquipExceedingStackSize()
        {
            var equipped = _equippableCharacter.EquipAt(0, _headItem, 3);
            Assert.AreEqual(Errors.ItemIsExceedingMaxStackSize, equipped.error);
            
            Assert.IsNull(_equipmentCollection[0]);
            Assert.AreEqual(0, _equipmentCollection.GetAmount(0));
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_headItem));
        }
        
        [Test]
        public void UnEquipFullRestoreCollectionTest()
        {
            var clone = (IEquippableItemInstance) _headItem.Clone();
            
            _restoreToCollection.Add(_headItem, _headItem.maxStackSize * _restoreToCollection.slotCount); // Fill collection
            var canAdd = _restoreToCollection.CanAdd((IEquippableItemInstance)_headItem.Clone(), 1);
            
            var equipped = _equippableCharacter.Equip(clone, 1);
            var unEquipped = _equippableCharacter.UnEquip(clone);
            
            Assert.AreEqual(Errors.CollectionFull, canAdd.error);
            Assert.IsNull(equipped.error);
            Assert.AreEqual(Errors.CollectionFull, unEquipped.error);

            Assert.AreEqual(_equipmentCollection, clone.collectionEntry?.collection);

            Assert.AreEqual(_headItem, _equipmentCollection[0]);
            Assert.AreEqual(1, _equipmentCollection.GetAmount(0));
        }
        
        [Test]
        public void UnEquipNonEquippedItem()
        {
            var unEquipped = _equippableCharacter.UnEquip(_headItem);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, unEquipped.error);
        }
        
        [Test]
        public void UnEquipAtEmptySlotTest()
        {
            var unEquipped = _equippableCharacter.UnEquipAt(0, 1);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, unEquipped.error);
        }
        
        [Test]
        public void UnEquipItemTest()
        {
            var equipped = _equippableCharacter.EquipAt(0, _headItem, 1);
            var unEquipped = _equippableCharacter.UnEquipAt(0, 1);
            
            Assert.IsNull(equipped.error);
            Assert.IsNull(unEquipped.error);
        }
                
        [Test]
        public void UnEquipTooManyItemsTest()
        {
            var equipped = _equippableCharacter.EquipAt(0, _headItem, 1);
            var unEquipped = _equippableCharacter.UnEquipAt(0, 2);
            
            Assert.IsNull(equipped.error);
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, unEquipped.error);
        }
    }
}