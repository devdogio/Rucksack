using System;
using System.Collections.Generic;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.CharacterEquipment
{
    public abstract class EquipmentCollectionBase<TSlotType, TEquippableType> : CollectionBase<TSlotType, TEquippableType>, IEquipmentCollection<TEquippableType>
        where TSlotType : EquipmentCollectionSlot<TEquippableType>, new()
        where TEquippableType : class, IEquatable<TEquippableType>, ICloneable, IEquippable<TEquippableType>, ICollectionSlotEntry, IStackable, IIdentifiable
    {
        public IEquippableCharacter<TEquippableType> characterOwner { get; set; }
        
        public EquipmentCollectionBase(int slotCount, IEquippableCharacter<TEquippableType> characterOwner, ILogger logger = null)
            : base(slotCount, logger)
        {
            this.characterOwner = characterOwner;
            GenerateSlots<TSlotType>();
        }

        public void SetEquipmentTypes(int index, IEquipmentType[] equipmentTypes)
        {
            slots[index].equipmentTypes = equipmentTypes;
        }

        public void SetAllEquipmentTypes(IEquipmentType[][] equipmentTypes)
        {
            if (equipmentTypes.Length != slots.Length)
            {
                throw new ArgumentOutOfRangeException($"equipmentTypes.Length has to be equal in length to the slots in the collection.");
            }

            for (int i = 0; i < equipmentTypes.Length; i++)
            {
                SetEquipmentTypes(i, equipmentTypes[i]);
            }
        }
        
        public Result<CollectionAddResult> ForceAdd(TEquippableType item, int amount = 1)
        {
            return ForceAdd(item, amount, new CollectionContext());
        }

        public virtual Result<CollectionAddResult> ForceAdd(TEquippableType item, int amount, CollectionContext context)
        {
            // TODO: Force add should be able to add to existing stacks
            // TODO: 1. Note that, when force adding it should try to add to existing stacks,
            // TODO: 2. if these stacks are at their max capacity it should try to use empty slots,
            // TODO: 3. if there's no empty slots, replace an existing item.

            var added = Add(item, amount, context);
            if (added.error == null)
            {
                // Can add all directly
                return added;
            }
            
            // TODO: Can't add everything directly; have to un-equip something
            
            for (var i = 0; i < slots.Length; i++) 
            { 
                int setAmount = amount;
                var placeItem = item;
                if (AreEqual(slots[i].item, item))
                {
                    setAmount = GetAmount(i) + amount;
                    placeItem = slots[i].item; // Keep existing item in slot
                }
                
                var set = Set(i, placeItem, setAmount, context); 
                if (set.result) 
                { 
                    logger.LogVerbose($"Added {placeItem} x {amount} to collection", this); 
                    return new Result<CollectionAddResult>(new CollectionAddResult(new []{ new SlotAmount(i, amount) })); 
                } 
            }
             
            return new Result<CollectionAddResult>(null, Errors.CollectionFull); 
        }

        public override Result<bool> CanSet(int index, TEquippableType item, int amount, CollectionContext context)
        {
            var canSet = base.CanSet(index, item, amount, context);
            if (canSet.result == false)
            {
                return canSet;
            }

            if (IsNull(item))
            {
                return true;
            }

            // Can the item be placed in this slot (Restriction types)?
            bool equipmentTypeConstraintIsValid = false;
            foreach (var equipmentType in slots[index].equipmentTypes)
            {
                if (equipmentType.Equals(item.equipmentType))
                {
                    equipmentTypeConstraintIsValid = true;
                    break;
                }                
            }
                
            if (equipmentTypeConstraintIsValid == false)
            {
                return new Result<bool>(false, Errors.CharacterCollectionEquipmentTypeInvalid);
            }
            
            return true;
        }
        
        protected override void SetInternal(int index, TEquippableType item, int amount, CollectionContext context)
        {
            base.SetInternal(index, item, amount, context);
        }
    }
}