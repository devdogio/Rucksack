using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using NUnit.Framework;
using UnityEngine.Rendering;

namespace Devdog.Rucksack.Tests
{
    public class CharacterEquipmentTests
    {
        private EquippableCharacter<IItemInstance, IEquippableItemInstance> _equippableCharacter;
        private EquipmentCollectionAccessibleMethods<IEquippableItemInstance> _equipmentCollection;
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
            _equipmentCollection = new EquipmentCollectionAccessibleMethods<IEquippableItemInstance>(5, _equippableCharacter, new Logger("[Equipment] "));
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

//            _restoreToCollection.Add(_headItem);
//            _restoreToCollection.Add(_bodyItem);
//            _restoreToCollection.Add(_feetItem);
//            _restoreToCollection.Add(_swordItem);
//            _restoreToCollection.Add(_swordItem2);
//            _restoreToCollection.Add(_shieldItem);
//            _restoreToCollection.Add(_twoHandedSwordItem);
//            _restoreToCollection.Add(_arrowsItem, 100);

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
        public void GetEquippableSlotsTest1()
        {
            var slots = _equippableCharacter.GetEquippableSlots(_arrowsItem, 3);
            
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(3, slots[0].amount);
            Assert.AreEqual(4, slots[0].slot);
        }

        [Test]
        public void EquipAtWrongSlotTest()
        {
            _restoreToCollection.Set(0, _arrowsItem);
            var equipped = _equippableCharacter.EquipAt(1, _arrowsItem);
            
            Assert.AreEqual(Errors.CharacterCollectionEquipmentTypeInvalid, equipped.error);
            Assert.IsNull(_equipmentCollection[0]);
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreSame(_arrowsItem, _restoreToCollection[0]);
            Assert.AreEqual(1, _restoreToCollection.GetAmount(_arrowsItem));
        }
        
        [Test]
        public void GetEquippableSlotsTestSword()
        {
            var slots = _equippableCharacter.GetEquippableSlots(_swordItem, 1);
            var slots2 = _equippableCharacter.GetEquippableSlots(_swordItem2, 1);
            
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(1, slots[0].amount);
            Assert.AreEqual(3, slots[0].slot);
            
            Assert.AreEqual(1, slots2.Count);
            Assert.AreEqual(1, slots2[0].amount);
            Assert.AreEqual(3, slots2[0].slot);
        }
        
        [Test]
        public void GetEquippableSlotsTestSwordMultiple()
        {
            var slots = _equippableCharacter.GetEquippableSlots(_swordItem, 2);
            
            Assert.AreEqual(2, slots.Count);
            Assert.AreEqual(1, slots[0].amount);
            Assert.AreEqual(3, slots[0].slot);
            Assert.AreEqual(1, slots[1].amount);
            Assert.AreEqual(4, slots[1].slot);
        }
        
        [Test]
        public void GetEquippableSlotsTestSword2()
        {
            var equipped = _equippableCharacter.EquipAt(3, _swordItem);
            var slots = _equippableCharacter.GetEquippableSlots(_swordItem, 1);
            var slots2 = _equippableCharacter.GetEquippableSlots(_swordItem2, 1);
            
            Assert.IsNull(equipped.error);
            
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(1, slots[0].amount);
            Assert.AreEqual(4, slots[0].slot);
            
            Assert.AreEqual(1, slots2.Count);
            Assert.AreEqual(1, slots2[0].amount);
            Assert.AreEqual(4, slots2[0].slot);
        }
        
        [Test]
        public void GetEquippableSlotsTest2()
        {
            var equipped = _equippableCharacter.EquipAt(4, _arrowsItem, 999);
            var slots = _equippableCharacter.GetEquippableSlots(_arrowsItem, 3);
            
            Assert.IsNull(equipped.error);
            
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(3, slots[0].amount);
            Assert.AreEqual(4, slots[0].slot);
        }

        [Test]
        public void SetIncorrectEquipmentTypeCountTest()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                _equipmentCollection.SetAllEquipmentTypes(new IEquipmentType[][]
                {
                    new[] {_headEquipmentType}, // Head 
                });
            });
        }

        [Test]
        public void SetEquipmentTypesTest()
        {
            _equipmentCollection.SetAllEquipmentTypes(new IEquipmentType[][]
            {
                new[] {_headEquipmentType}, // Head 
                new[] {_bodyEquipmentType}, // Body 
                new[] {_feetEquipmentType}, // Feet 
                new[] {_swordEquipmentType, _twoHandedSwordEquipmentType}, // Right Hand 
                new[] {_shieldEquipmentType}, // Left Hand 
            });
        }

        [Test]
        public void SetEquipmentTypesInvalidTest()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                _equipmentCollection.SetAllEquipmentTypes(new IEquipmentType[][]
                {
                    new[] {_headEquipmentType}, // Head 
                    new[] {_bodyEquipmentType}, // Body 
                    new[] {_feetEquipmentType}, // Feet 
                });
            });
        }
        
        
        [Test]
        public void EquipItemToCharacterTest1()
        {
            var added = _equipmentCollection.Add(_headItem, 1);

            Assert.IsNull(added.error);
            Assert.AreEqual(1, added.result.affectedSlots.Length);
            Assert.AreEqual(0, added.result.affectedSlots[0].slot);
            Assert.AreEqual(1, added.result.affectedSlots[0].amount);
        }

        [Test]
        public void EquipItemToCharacterTest2()
        {
            var added = _equipmentCollection.Add(_headItem, 2); // Can't wear 2 head pieces

            Assert.AreEqual(Errors.CollectionFull, added.error);
            Assert.IsNull(added.result);
        }
        
        [Test]
        public void UnEquipItemFromCharacterTest()
        {
            var added = _equipmentCollection.Set(0, _headItem, 1);
            var removed = _equipmentCollection.Set(0, null, 0);
            
            Assert.IsNull(added.error);
            Assert.IsNull(removed.error);
            
            Assert.IsNull(_equipmentCollection[0]);
        }

        [Test]
        public void EquipSwordsToCharacterTest()
        {
            var added = _equippableCharacter.Equip(_swordItem, 2); // CAN wear 2 swords

            Assert.AreEqual(Errors.ItemIsExceedingMaxStackSize, added.error);
            Assert.AreEqual(0, _swordItem.onEquippedCallCount);
            Assert.AreEqual(0, _swordItem.onUnEquippedCallCount);
        }

        [Test]
        public void GetCanAddAmountTest()
        {
            var canAdd = _equipmentCollection.GetCanAddAmount(_headItem);

            Assert.IsNull(canAdd.error);
            Assert.AreEqual(1, canAdd.result);

            Assert.AreEqual(0, _headItem.onEquippedCallCount);
            Assert.AreEqual(0, _headItem.onUnEquippedCallCount);
        }

        [Test]
        public void GetCanAddAmountTest2()
        {
            _equipmentCollection.Add(_headItem, 1);
            var canAdd = _equipmentCollection.GetCanAddAmount(_headItem);

            Assert.IsNull(canAdd.error);
            Assert.AreEqual(0, canAdd.result);
        }

        [Test]
        public void GetCanAddAmountTest3()
        {
            var canAdd = _equipmentCollection.GetCanAddAmount(_swordItem);

            Assert.IsNull(canAdd.error);
            Assert.AreEqual(2, canAdd.result);
        }

        [Test]
        public void GetCanAddAmountTest4()
        {
            _equipmentCollection.Add(_swordItem, 1);
            var canAdd = _equipmentCollection.GetCanAddAmount(_swordItem);

            Assert.IsNull(canAdd.error);
            Assert.AreEqual(1, canAdd.result);
        }

        [Test]
        public void GetCanAddAmountTest5()
        {
            _equipmentCollection.Add(_swordItem, 2);
            var canAdd = _equipmentCollection.GetCanAddAmount(_swordItem);

            Assert.IsNull(canAdd.error);
            Assert.AreEqual(0, canAdd.result);
        }

        [Test]
        public void CanEquipSameItemMultipleTimesTest()
        {
            var canAdd = _equippableCharacter.Equip(_swordItem, 1);
            var canAdd2 = _equippableCharacter.Equip(_swordItem, 1);
            
            Assert.IsNull(canAdd.error);
            Assert.AreEqual(Errors.CollectionAlreadyContainsSpecificInstance, canAdd2.error);
        }

        [Test]
        public void EquipItemToCharacterMultipleTimesTestShouldUnEquipExistingItem()
        {
            var bodyItemClone = (MockedEquippableItemInstance) ((IItemInstance) _bodyItem).Clone();

            var added = _equippableCharacter.Equip(_bodyItem);
            var added2 = _equippableCharacter.Equip(bodyItemClone);

            Assert.IsNull(added.error);
            Assert.IsNull(added2.error);

            Assert.AreEqual(1, _bodyItem.onEquippedCallCount);
            Assert.AreEqual(1, _bodyItem.onUnEquippedCallCount);

            Assert.AreEqual(1, bodyItemClone.onEquippedCallCount);
            Assert.AreEqual(0, bodyItemClone.onUnEquippedCallCount);
            
            Assert.AreSame(_bodyItem, _restoreToCollection[0]);
            Assert.AreSame(bodyItemClone, _equippableCharacter.collection[1]);
        }

        [Test]
        public void IncompatibleEquipmentShouldUnEquipExistingItemsRestoreGroupFull()
        {
            _restoreToCollection.Add(_headItem, 10);

            var equipped = _equippableCharacter.EquipAt(4, _shieldItem);
            var equipped2 = _equippableCharacter.EquipAt(4, _swordItem); // Should un-equip shield

            Assert.IsNull(equipped.error);
            Assert.AreEqual(Errors.CollectionFull, equipped2.error);
        }
        
        [Test]
        public void GetCanAddAmountTest1()
        {
            var equipped = _equippableCharacter.EquipAt(4, _shieldItem);

            // We can't add the item because the restore collection is full, and thus the two handed sword can't be unequipped.
            var canAdd = _equipmentCollection.GetCanAddAmount(_arrowsItem);

            Assert.IsNull(equipped.error);
            Assert.IsNull(canAdd.error);
            Assert.AreEqual(0, canAdd.result);
        }

        [Test]
        public void SwapCompatibleItemsTest()
        {
            var set = _equippableCharacter.EquipAt(3, _swordItem);
            var swapped = _equippableCharacter.SwapOrMerge(3, 4);

            Assert.IsNull(set.error);
            Assert.IsNull(swapped.error);
        }

        [Test]
        public void SwapCompatibleItemsTest2()
        {
            var set = _equippableCharacter.EquipAt(3, _swordItem);
            var set2 = _equippableCharacter.EquipAt(4, (EquippableItemInstance)_swordItem.Clone());
            var swapped = _equippableCharacter.SwapOrMerge(3, 4);

            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(swapped.error);
        }

        [Test]
        public void SwapCompatibleItemsTest3()
        {
            var set = _equippableCharacter.EquipAt(3, _swordItem);
            var swapped = _equippableCharacter.SwapOrMerge(4, 3);

            Assert.IsNull(set.error);
            Assert.IsNull(swapped.error);
        }

        [Test]
        public void SwapIncompatibleItemsTest()
        {
            var set = _equippableCharacter.EquipAt(3, _swordItem);
            var swapped = _equippableCharacter.SwapOrMerge(3, 2);

            Assert.IsNull(set.error);
            Assert.AreEqual(Errors.CollectionCanNotMoveItem, swapped.error);
        }

        [Test]
        public void SwapIncompatibleItemsTest2()
        {
            var set = _equippableCharacter.EquipAt(3, _swordItem);
            var set2 = _equippableCharacter.EquipAt(4, _shieldItem);
            var swapped = _equippableCharacter.SwapOrMerge(3, 4); // Shield can't be placed in itemGuid 3, but sword can fit in itemGuid 4.

            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.AreEqual(Errors.CollectionCanNotMoveItem, swapped.error);
        }

        [Test]
        public void SwapIncompatibleItemsTest3()
        {
            var set = _equippableCharacter.EquipAt(3, _swordItem);
            var set2 = _equippableCharacter.EquipAt(4, _shieldItem);
            var swapped = _equippableCharacter.SwapOrMerge(4, 3);

            Assert.IsNull(set.error);
            Assert.IsNull(set2.error);
            Assert.AreEqual(Errors.CollectionCanNotMoveItem, swapped.error);
        }

        [Test]
        public void SetIncompatibleEquipmentType()
        {
            var set1 = _equippableCharacter.EquipAt(3, _arrowsItem, 10);
            
            Assert.AreEqual(Errors.CharacterCollectionEquipmentTypeInvalid, set1.error);
        }

        [Test]
        public void MergeCompatibleItemsTest()
        {
            var arrowsClone = (EquippableItemInstance) _arrowsItem.Clone();
            _equipmentCollection.SetEquipmentTypes(3, new[] {_arrowsEquipmentType});
            var set1 = _equippableCharacter.EquipAt(3, _arrowsItem, 10);
            var set2 = _equippableCharacter.EquipAt(4, arrowsClone, 10);

            var merged = _equipmentCollection.MergePublic(3, _equipmentCollection, 4, _equipmentCollection.GetAmount(3));

            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            Assert.IsNull(merged.error);

            Assert.AreEqual(20, _equipmentCollection.GetAmount(4));
        }

        [Test]
        public void MergeCompatibleItemsTest3()
        {
            _equipmentCollection.SetEquipmentTypes(3, new[] {_arrowsEquipmentType});
            var set1 = _equippableCharacter.EquipAt(3, _arrowsItem, 10);

            var merged = _equipmentCollection.MergePublic(4, _equipmentCollection, 3, _equipmentCollection.GetAmount(4));

            Assert.IsNull(set1.error);
            Assert.IsNull(merged.error);

            Assert.AreEqual(10, _equipmentCollection.GetAmount(3));
        }

        [Test]
        public void MergeIncompatibleItemsTest2()
        {
            _equipmentCollection.SetEquipmentTypes(3, new[] {_arrowsEquipmentType});
            var set1 = _equippableCharacter.EquipAt(2, _feetItem);
            var set2 = _equippableCharacter.EquipAt(3, _arrowsItem, 10);

            var merged = _equipmentCollection.MergePublic(2, _equipmentCollection, 3, _equipmentCollection.GetAmount(2));

            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.AreEqual(1, _equipmentCollection.GetAmount(2));
            Assert.AreEqual(10, _equipmentCollection.GetAmount(3));
            
            Assert.AreEqual(Errors.ItemsAreNotEqual, merged.error);
        }

        [Test]
        public void MergeNullOntoItemTest()
        {
            _equipmentCollection.SetEquipmentTypes(3, new []{ _arrowsEquipmentType });
            var set1 = _equippableCharacter.EquipAt(3, _arrowsItem, 10);

            var merged = _equipmentCollection.MergePublic(2, _equipmentCollection, 3, _equipmentCollection.GetAmount(2));
            
            Assert.IsNull(set1.error);
            Assert.IsNull(merged.error);
            
            Assert.AreEqual(10, _equipmentCollection.GetAmount(3));
        }

        [Test]
        public void ForceAddSingleItemTest()
        {
            var added = _equipmentCollection.ForceAdd(_shieldItem);
            Assert.IsNull(added.error);
        }
        
        [Test]
        public void ForceAddForceUnEquipTest2()
        {
            var added1 = _equippableCharacter.Equip(_arrowsItem);
            var added2 = _equippableCharacter.Equip(_shieldItem);
            
            Assert.IsNull(added1.error);
            Assert.IsNull(added2.error);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(1, _restoreToCollection.GetAmount(_arrowsItem));
            
            Assert.AreEqual(1, _equipmentCollection.GetAmount(_shieldItem));
            Assert.AreEqual(1, _equipmentCollection.GetAmount(4));
        }
        
        [Test]
        public void AddStackableArrowsTest()
        {
            var item2 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            var item3 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            
            var added1 = _equippableCharacter.Equip(_arrowsItem);
            var added2 = _equippableCharacter.Equip(item2);
            var added3 = _equippableCharacter.Equip(item3);

            Assert.IsNull(added1.error);
            Assert.IsNull(added2.error);
            Assert.IsNull(added3.error);
            
            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(1, item2.onEquippedCallCount);
            Assert.AreEqual(1, item3.onEquippedCallCount);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(3, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(3, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(0, _restoreToCollection.GetAmount(_arrowsItem));
        }
        
        [Test]
        public void AddStackableArrowsTest2()
        {
            _restoreToCollection.Add(_arrowsItem, 100);
            
            var item2 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            var item3 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            
            var added1 = _equippableCharacter.Equip(_arrowsItem);
            var added2 = _equippableCharacter.Equip(item2);
            var added3 = _equippableCharacter.Equip(item3);

            Assert.IsNull(added1.error);
            Assert.IsNull(added2.error);
            Assert.IsNull(added3.error);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(3, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(3, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(97, _restoreToCollection.GetAmount(_arrowsItem));
        }

        [Test]
        public void AddStackableArrowsTest3()
        {
            _restoreToCollection.Add(_arrowsItem, 100);
            
            var item2 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            var item3 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            
            var added1 = _equippableCharacter.Equip(_arrowsItem);
            var added2 = _equippableCharacter.Equip(item2);
            var added3 = _equippableCharacter.Equip(item3);

            var unEquipped = _equippableCharacter.UnEquip(_arrowsItem, 3);
            
            Assert.IsNull(added1.error);
            Assert.IsNull(added2.error);
            Assert.IsNull(added3.error);
            Assert.IsNull(unEquipped.error);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(0, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(100, _restoreToCollection.GetAmount(_arrowsItem));
        }
        
        [Test]
        public void EquipMultipleRestoreCollectionAmountTest()
        {
            _restoreToCollection.Add(_arrowsItem, 100);
            var added1 = _equippableCharacter.Equip(_arrowsItem, 10);
            
            // Not the same, as we didn't move the entire stack (duplicated item is placed in equipment collection).
            Assert.AreNotSame(_arrowsItem, _equipmentCollection[4]);
            Assert.IsNull(added1.error);

            var dupedItem = (MockedEquippableItemInstance) _equipmentCollection[4];
            Assert.AreEqual(1, dupedItem.onEquippedCallCount);
            Assert.AreEqual(0, dupedItem.onUnEquippedCallCount);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(10, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(10, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(90, _restoreToCollection.GetAmount(_arrowsItem));
            
            Assert.AreNotSame(_arrowsItem.collectionEntry, _equipmentCollection[4].collectionEntry);            
        }

        [Test]
        public void EquipEntireStackTest()
        {
            var added = _equippableCharacter.Equip(_arrowsItem, 10);
            
            Assert.IsNull(added.error);
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            Assert.AreSame(_arrowsItem.collectionEntry, _equipmentCollection[4].collectionEntry);            
        }
        
        [Test]
        public void UnEquipEntireStackTest()
        {
            _restoreToCollection.Add(_arrowsItem, 100);
            var added1 = _equippableCharacter.Equip(_arrowsItem, 10);
            var dupedItem = (MockedEquippableItemInstance) _equipmentCollection[4];
            var removed = _equippableCharacter.UnEquip(_arrowsItem, 10);
            
            Assert.IsNull(added1.error);
            Assert.IsNull(removed.error);
            
            Assert.IsNull(_equipmentCollection[4]);

            Assert.AreEqual(1, dupedItem.onEquippedCallCount);
            Assert.AreEqual(1, dupedItem.onUnEquippedCallCount);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(0, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(100, _restoreToCollection.GetAmount(_arrowsItem));
        }
        
        [Test]
        public void UnEquipPartialStackTest()
        {
            _restoreToCollection.Add(_arrowsItem, 100);
            var added1 = _equippableCharacter.Equip(_arrowsItem, 10);
            var removed = _equippableCharacter.UnEquip(_arrowsItem, 5);
            
            // Split of from main stack
            Assert.AreNotSame(_arrowsItem, _equipmentCollection[4]);
            Assert.IsNull(added1.error);
            Assert.IsNull(removed.error);
            
            Assert.AreEqual(0, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(0, _arrowsItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, ((MockedEquippableItemInstance)_equipmentCollection[4]).onEquippedCallCount);
            Assert.AreEqual(1, ((MockedEquippableItemInstance)_restoreToCollection[0]).onUnEquippedCallCount);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(5, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(5, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(95, _restoreToCollection.GetAmount(_arrowsItem));
        }
        
        [Test]
        public void ResettingStackTo0Test()
        {
            var item2 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            var item3 = (MockedEquippableItemInstance)_arrowsItem.Clone();
            
            var added1 = _equippableCharacter.Equip(_arrowsItem);
            Assert.AreSame(_arrowsItem, _equipmentCollection[4]);
            Assert.AreEqual(1, _equipmentCollection.GetAmount(4));

            var added2 = _equippableCharacter.Equip(item2);
            Assert.AreSame(item2, _equipmentCollection[4]);
            Assert.AreEqual(2, _equipmentCollection.GetAmount(4));
            
            var added3 = _equippableCharacter.Equip(item3);
            Assert.AreSame(item3, _equipmentCollection[4]);
            Assert.AreEqual(3, _equipmentCollection.GetAmount(4));
            
            var removed = _equippableCharacter.UnEquipAt(4, 3);
            
            Assert.IsNull(removed.error);
            
            Assert.IsNull(added1.error);
            Assert.IsNull(added2.error);
            Assert.IsNull(added3.error);
            
            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(1, _arrowsItem.onUnEquippedCallCount);
            Assert.AreEqual(1, item2.onEquippedCallCount);
            Assert.AreEqual(1, item2.onUnEquippedCallCount);
            Assert.AreEqual(1, item3.onEquippedCallCount);
            Assert.AreEqual(1, item3.onUnEquippedCallCount);
            
            // Arrows should've been unequipped by ForceAdd action.
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(0, _equipmentCollection.GetAmount(4));
            Assert.AreEqual(3, _restoreToCollection.GetAmount(_arrowsItem));
        }
        
        [Test]
        public void RestrictionPreventsItemIntoRestoreGroupShouldFailTest()
        {
            // Prevent items from being added to the restore collection
            _restoreToCollection.restrictions.Add(new FakeCollectionRestriction());

            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem);
            var set2 = _equippableCharacter.EquipAt(4, _shieldItem); // Unequips arrows (blocked by restore collection)
            
            Assert.IsNull(set1.error);
            Assert.AreEqual(Errors.CollectionFull, set2.error); // Restore collection can't be written to
            
            Assert.AreEqual(1, _equipmentCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(0, _equipmentCollection.GetAmount(_shieldItem));
            
            Assert.AreEqual(0, _restoreToCollection.GetAmount(_arrowsItem));
        }

        [Test]
        public void EquipmentCollectionOnEquipEventTest()
        {
            CollectionAddResult equipmentAddResult = null;
            CollectionRemoveResult<IEquippableItemInstance> equipmentRemoveResult = null;
            CollectionSlotsChangedResult equipmentChangeResult = null;
            
            int equipmentAddEventCount = 0;
            int equipmentRemoveEventCount = 0;
            int equipmentChangeEventCount = 0;
            
            
            CollectionAddResult restoreAddResult = null;
            CollectionRemoveResult<IItemInstance> restoreRemoveResult = null;
            CollectionSlotsChangedResult restoreChangeResult = null;
            
            int restoreAddEventCount = 0;
            int restoreRemoveEventCount = 0;
            int restoreChangeEventCount = 0;
            
            _equipmentCollection.OnAddedItem += (sender, result) =>
            {
                equipmentAddResult = result;
                equipmentAddEventCount++;
            };
                        
            _equipmentCollection.OnRemovedItem += (sender, result) =>
            {
                equipmentRemoveResult = result;
                equipmentRemoveEventCount++;
            };
            
            _equipmentCollection.OnSlotsChanged += (sender, result) =>
            {
                equipmentChangeResult = result;
                equipmentChangeEventCount++;
            };
            
            
            _restoreToCollection.OnAddedItem += (sender, result) =>
            {
                restoreAddResult = result;
                restoreAddEventCount++;
            };
                        
            _restoreToCollection.OnRemovedItem += (sender, result) =>
            {
                restoreRemoveResult = result;
                restoreRemoveEventCount++;
            };
            
            _restoreToCollection.OnSlotsChanged += (sender, result) =>
            {
                restoreChangeResult = result;
                restoreChangeEventCount++;
            };
            
            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem); // Add event
            var set2 = _equippableCharacter.EquipAt(4, _shieldItem); // Unequips arrows + add event for shield
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);

            Assert.AreEqual(2, equipmentAddEventCount); // Add arrows + shield
            Assert.AreEqual(1, equipmentRemoveEventCount); // Remove arrows
            Assert.AreEqual(3, equipmentChangeEventCount); // Arrow equip, unequip, shield equip
            
            Assert.AreEqual(1, restoreAddEventCount); // Add arrows
            Assert.AreEqual(0, restoreRemoveEventCount);
            Assert.AreEqual(1, restoreChangeEventCount); // Changed for arrows
            
            Assert.AreEqual(1, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(1, _equipmentCollection.GetAmount(_shieldItem));
        }
        
        [Test]
        public void EquipmentCollectionForceAddUnequipsItemEventTest()
        {
            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem); // Add event
            var set2 = _equippableCharacter.EquipAt(4, _shieldItem); // Unequips arrows + add event for shield
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);

            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(1, _arrowsItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, _shieldItem.onEquippedCallCount);
            Assert.AreEqual(0, _shieldItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(1, _equipmentCollection.GetAmount(_shieldItem));
        }

        [Test]
        public void RestoreTestItemCollectionSlotTest()
        {
            var set1 = _equippableCharacter.EquipAt(0, _headItem);
            var set2 = _equippableCharacter.EquipAt(3, _swordItem);
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.AreEqual(_equipmentCollection, _headItem.collectionEntry.collection);
            Assert.AreEqual(_equipmentCollection, _swordItem.collectionEntry.collection);

            var removed = _equippableCharacter.UnEquip(_headItem);
            
            Assert.IsNull(removed.error);
            Assert.AreEqual(_restoreToCollection, _headItem.collectionEntry.collection);
            Assert.AreEqual(_equipmentCollection, _swordItem.collectionEntry.collection);
        }
        
        [Test]
        public void RestoreTestItemCollectionSlotTest2()
        {
            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem);
            var set2 = _equippableCharacter.EquipAt(3, _swordItem);
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.AreEqual(_equipmentCollection, _arrowsItem.collectionEntry.collection);
            Assert.AreEqual(_equipmentCollection, _swordItem.collectionEntry.collection);

            var removed = _equippableCharacter.UnEquip(_arrowsItem);
            var removed2 = _equippableCharacter.UnEquip(_swordItem);
            
            Assert.IsNull(removed.error);
            Assert.IsNull(removed2.error);
            Assert.AreEqual(_restoreToCollection, _arrowsItem.collectionEntry.collection);
            Assert.AreEqual(_restoreToCollection, _swordItem.collectionEntry.collection);
        }
        
        [Test]
        public void EquipmentTest1()
        {
            var set1 = _equippableCharacter.EquipAt(3, _swordItem);
            var set2 = _equippableCharacter.EquipAt(3, _swordItem2);

            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.AreEqual(_swordItem, _restoreToCollection[0]);
            Assert.AreEqual(_restoreToCollection, _swordItem.collectionEntry.collection);
            Assert.AreEqual(_equipmentCollection, _swordItem2.collectionEntry.collection);
            
            Assert.AreEqual(1, _swordItem.onEquippedCallCount);
            Assert.AreEqual(1, _swordItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, _swordItem2.onEquippedCallCount);
            Assert.AreEqual(0, _swordItem2.onUnEquippedCallCount);
        }

        [Test]
        public void EquipOverwriteTest()
        {
            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem, 3);
            var set2 = _equippableCharacter.EquipAt(4, _arrowsItem, 4);
            
            Assert.IsNull(set1.error);
            Assert.AreEqual(Errors.CollectionAlreadyContainsSpecificInstance, set2.error);
            
            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreEqual(0, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(_equipmentCollection, _arrowsItem.collectionEntry.collection);
            
            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(0, _arrowsItem.onUnEquippedCallCount);
        }
        
        [Test]
        public void EquipOverwriteCloneTest()
        {
            var clone = (MockedEquippableItemInstance) _arrowsItem.Clone();
            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem, 3);
            var set2 = _equippableCharacter.EquipAt(4, clone, 5);
            
            Assert.IsNull(set1.error);
            Assert.IsNull(set2.error);
            
            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreEqual(0, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreSame(clone, _equipmentCollection[4]);
            Assert.AreEqual(8, _equipmentCollection.GetAmount(4));
            
            Assert.AreEqual(_equipmentCollection, _arrowsItem.collectionEntry.collection);
            Assert.AreEqual(_equipmentCollection, clone.collectionEntry.collection);
            
            Assert.AreEqual(1, _arrowsItem.onEquippedCallCount);
            Assert.AreEqual(1, _arrowsItem.onUnEquippedCallCount);
            
            Assert.AreEqual(1, clone.onEquippedCallCount);
            Assert.AreEqual(0, clone.onUnEquippedCallCount);
            

        }

        [Test]
        public void EquipTooManyToEquippableCharacter()
        {
            _restoreToCollection.Set(0, _arrowsItem, 10);
            
            var set1 = _equippableCharacter.EquipAt(4, _arrowsItem, 11);

            Assert.AreEqual(Errors.CollectionDoesNotContainItem, set1.error);
            Assert.AreEqual(10, _restoreToCollection.GetAmount(0));
            Assert.AreEqual(0, _equippableCharacter.collection.GetAmount(4));
        }

        [Test]
        public void EquippingPartialStackDuplicatesItemReference()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.EquipAt(4, _arrowsItem, 5);
            
            Assert.AreEqual(6, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(5, _equippableCharacter.collection.GetAmount(_arrowsItem));
            Assert.AreSame(_arrowsItem, _restoreToCollection[0]);
            
            // Duplicated item reference
            Assert.AreNotSame(_arrowsItem, _equippableCharacter.collection[4]);
        }
        
        [Test]
        public void EquippingPartialStackDuplicatesItemReference2()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.Equip(_arrowsItem, 5);
            
            Assert.AreEqual(6, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(5, _equippableCharacter.collection.GetAmount(_arrowsItem));
            Assert.AreSame(_arrowsItem, _restoreToCollection[0]);
            
            // Duplicated item reference
            Assert.AreNotSame(_arrowsItem, _equippableCharacter.collection[4]);
        }
        
        [Test]
        public void UnEquipPartialStackShouldSplit()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.Equip(_arrowsItem, 11);
            _equippableCharacter.UnEquip(_arrowsItem, 5);
            
            Assert.AreEqual(5, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(6, _equippableCharacter.collection.GetAmount(_arrowsItem));

            // Duplicated item reference
            Assert.AreNotSame(_arrowsItem, _restoreToCollection[0]);
            Assert.AreSame(_arrowsItem, _equippableCharacter.collection[4]);
        }
        
        [Test]
        public void UnEquipAtPartialStackShouldSplit()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.EquipAt(4, _arrowsItem, 11);
            _equippableCharacter.UnEquipAt(4, 5);
            
            Assert.AreEqual(5, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(6, _equippableCharacter.collection.GetAmount(_arrowsItem));

            // Duplicated item reference
            Assert.AreNotSame(_arrowsItem, _restoreToCollection[0]);
            Assert.AreSame(_arrowsItem, _equippableCharacter.collection[4]);
        }
        
        [Test]
        public void EquipAtEntireStackItemReference()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.EquipAt(4, _arrowsItem, 11);
            
            Assert.AreEqual(0, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(11, _equippableCharacter.collection.GetAmount(_arrowsItem));
            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreSame(_arrowsItem, _equippableCharacter.collection[4]);
        }
        
        [Test]
        public void EquipEntireStackItemReference()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.Equip(_arrowsItem, 11);
            
            Assert.AreEqual(0, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(11, _equippableCharacter.collection.GetAmount(_arrowsItem));
            Assert.IsNull(_restoreToCollection[0]);
            Assert.AreSame(_arrowsItem, _equippableCharacter.collection[4]);
        }
        
        [Test]
        public void UnEquipEntiretackShouldNotSplit()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.Equip(_arrowsItem, 11);
            _equippableCharacter.UnEquip(_arrowsItem, 11);
            
            Assert.AreEqual(11, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(0, _equippableCharacter.collection.GetAmount(_arrowsItem));

            // Duplicated item reference
            Assert.AreSame(_arrowsItem, _restoreToCollection[0]);
            Assert.IsNull(_equippableCharacter.collection[4]);
        }
        
        [Test]
        public void UnEquipAtEntiretackShouldNotSplit()
        {
            _restoreToCollection.Add(_arrowsItem, 11);
            _equippableCharacter.Equip(_arrowsItem, 11);
            _equippableCharacter.UnEquipAt(4, 11);
            
            Assert.AreEqual(11, _restoreToCollection.GetAmount(_arrowsItem));
            Assert.AreEqual(0, _equippableCharacter.collection.GetAmount(_arrowsItem));

            // Duplicated item reference
            Assert.AreSame(_arrowsItem, _restoreToCollection[0]);
            Assert.IsNull(_equippableCharacter.collection[4]);
        }
    }
}