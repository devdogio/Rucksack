using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class MountPointTests
    {
        private EquipmentCollection<IEquippableItemInstance> _equipmentCollection;
        private Collection<IItemInstance> _restoreToCollection;

        private MockedEquippableItemInstance _headItem;
        private MockedEquippableItemInstance _headItem2;
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

        private MockedMountPoint _mountPoint1;
        private MockedMountPoint _mountPoint2;
        private MockedMountPoint _mountPoint3;
        private MockedMountPoint _mountPoint4;
        private MockedMountPoint _mountPoint5;

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _restoreToCollection = new Collection<IItemInstance>(10, new Logger("[RestoreCollection] "));
            _equipmentCollection = new EquipmentCollection<IEquippableItemInstance>(5, null, new Logger("[Equipment] "));

            _headEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Head"};
            _bodyEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Body"};
            _feetEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Feet"};
            _swordEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Sword"};
            _shieldEquipmentType = new EquipmentType(Guid.NewGuid()) {name = "Shield"};
            _twoHandedSwordEquipmentType = new EquipmentType(Guid.NewGuid(), _swordEquipmentType, _shieldEquipmentType, _arrowsEquipmentType) {name = "2 Handed sword"};
            _arrowsEquipmentType = new EquipmentType(Guid.NewGuid(), _swordEquipmentType, _shieldEquipmentType) {name = "Arrows"};

            _headItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_headEquipmentType));
            _headItem2 = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_headEquipmentType));
            _bodyItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_bodyEquipmentType));
            _feetItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_feetEquipmentType));
            _swordItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_swordEquipmentType));
            _shieldItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_shieldEquipmentType));
            _twoHandedSwordItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_twoHandedSwordEquipmentType));
            _arrowsItem = new MockedEquippableItemInstance(Guid.NewGuid(), CreateDemoItemDef(_arrowsEquipmentType, 999));

            _mountPoint1 = new MockedMountPoint("1");
            _mountPoint2 = new MockedMountPoint("2");
            _mountPoint3 = new MockedMountPoint("3");
            _mountPoint4 = new MockedMountPoint("4");
            _mountPoint5 = new MockedMountPoint("5");
            
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
//                _mountPoint1, 
//                _mountPoint2, 
//                _mountPoint3, 
//                _mountPoint4, 
//                _mountPoint5, 
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

//        [Test]
//        public void SettingIncorrectMountPointCountThrowsExceptionTest()
//        {
//            Assert.Catch<ArgumentException>(() =>
//            {
//                _equipmentCollection.SetAllMountPoints(new IMountPoint<IEquippableItemInstance>[]
//                {
//                    new MockedMountPoint("asd"), 
//                });
//            });
//        }
//        
//        [Test]
//        public void MountItemTest()
//        {
//            var added = _equipmentCollection.Add(_headItem);
//            Assert.IsNull(added.error);
//            Assert.AreEqual(_mountPoint1, _equipmentCollection.GetSlot(0).mountPoint);
//            Assert.IsTrue(_equipmentCollection.GetSlot(0).mountPoint.hasMountedItem);
//            Assert.AreEqual(_headItem, ((MockedMountPoint)_equipmentCollection.GetSlot(0).mountPoint).mountedItem);
//        }
//
//        [Test]
//        public void ForceAddReplaceTest()
//        {
//            var added = _equipmentCollection.ForceAdd(_headItem2);
//            Assert.IsNull(added.error);
//            Assert.AreEqual(_mountPoint1, _equipmentCollection.GetSlot(0).mountPoint);
//            Assert.IsTrue(_equipmentCollection.GetSlot(0).mountPoint.hasMountedItem);
//            Assert.AreEqual(_headItem2, ((MockedMountPoint)_equipmentCollection.GetSlot(0).mountPoint).mountedItem);
//        }
//        
//        [Test]
//        public void ClearMountTest()
//        {
//            var added = _equipmentCollection.Add(_headItem);
//            Assert.IsNull(added.error);
//            _equipmentCollection.GetSlot(0).Clear();
//            
//            Assert.AreEqual(_mountPoint1, _equipmentCollection.GetSlot(0).mountPoint);
//            Assert.IsFalse(_equipmentCollection.GetSlot(0).mountPoint.hasMountedItem);
//        }
    }
}