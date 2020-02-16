using System;
using System.Collections.Generic;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class EquippableCharacter<TBaseType, TEquippableType> : IEquippableCharacter<TEquippableType>
        where TBaseType : class, IEquatable<TBaseType>, IStackable, IIdentifiable, ICloneable
        where TEquippableType : class, TBaseType, IEquatable<TEquippableType>, IEquippable<TEquippableType>, Collections.ICollectionSlotEntry, ICloneable
    {
        public struct EquippableSlotAmount
        {
            public int slot;
            public int amount;
            public bool unEquipContents;

            public EquippableSlotAmount(int slot, int amount, bool unEquipContents)
            {
                this.slot = slot;
                this.amount = amount;
                this.unEquipContents = unEquipContents;
            }
        }
        
        /// <summary>
        /// When items are unequipped or removed due to item conflicts they will be restored to the restoreItemsGroup
        /// </summary>
        public CollectionGroup<TBaseType> restoreItemsToGroup;
        
        public IMountPoint<TEquippableType>[] mountPoints { get; set; }
        public IEquipmentCollection<TEquippableType> collection { get; set; }
        protected readonly ILogger logger;

        public EquippableCharacter(IEquipmentCollection<TEquippableType> collection, CollectionGroup<TBaseType> restoreItemsToGroup, ILogger logger = null)
        {
            this.restoreItemsToGroup = restoreItemsToGroup ?? new CollectionGroup<TBaseType>();
            this.logger = logger ?? new NullLogger();
            this.collection = collection;
        }

        public IEnumerable<TEquippableType> GetAll()
        {
            if (collection == null)
            {
                return new TEquippableType[0];
            }
            
            return collection;
        }

        public TEquippableType Get(int index)
        {
            return collection?[index];
        }

        /// <summary>
        /// Return a list of slots where this item can be equipped to + the amount that can be equipped.
        /// <remarks>Note that the slot might still be filled and the item in it might have to be un-equipped.</remarks>
        /// </summary>
        public virtual List<EquippableSlotAmount> GetEquippableSlots(TEquippableType item, int equipAmount)
        {
            var slots = new List<EquippableSlotAmount>();
            var setContext = new CollectionContext();
            setContext.validationFlags = ~(CollectionContext.Validations.SpecificInstance);
            if (collection?.GetCanAddAmount(item, equipAmount).result >= equipAmount)
            {
                // Can add to an existing stacks
                var toAdd = equipAmount;
                for (int i = 0; i < collection.slotCount; i++)
                {
                    int added = 0;
                    if (EqualityComparer<TEquippableType>.Default.Equals(collection[i], item) && collection.GetAmount(i) + equipAmount <= item.maxStackSize)
                    {
                        added = Math.Min(equipAmount, Math.Min(equipAmount, item.maxStackSize));
                        slots.Add(new EquippableSlotAmount(i, added, false));
                    }
                    else if (collection[i] == null && collection.CanSetBoxed(i, item, Math.Min(equipAmount, item.maxStackSize), setContext).error == null)
                    {
                        added = Math.Min(equipAmount, item.maxStackSize);
                        slots.Add(new EquippableSlotAmount(i, added, false));
                    }
                    
                    toAdd -= added;
                    if (toAdd <= 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                // Need to un-equip something
                for (int i = 0; i < collection?.slotCount; i++)
                {
                    if (collection.CanSetBoxed(i, item, equipAmount, setContext).error == null)
                    {
                        slots.Add(new EquippableSlotAmount(i, equipAmount, true));
                        return slots;
                    }
                }
            }

            return slots;
        }

        protected virtual Result<bool> CanEquip(TEquippableType item, int amount)
        {
            if (amount > item.maxStackSize)
            {
                return new Result<bool>(false, Errors.ItemIsExceedingMaxStackSize);
            }
            
            if (collection?.IndexOf(o => ReferenceEquals(item, o)) != -1)
            {
                return new Result<bool>(false, Errors.CollectionAlreadyContainsSpecificInstance);
            }

            if (item.collectionEntry != null)
            {
                if (item.collectionEntry.amount < amount)
                {
                    return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
                }
            }

            return true;
        }
        
        public virtual Result<EquipmentResult<TEquippableType>[]> Equip(TEquippableType item, int amount = 1)
        {
            var canEquip = CanEquip(item, amount);
            if (canEquip.error != null)
            {
                return new Result<EquipmentResult<TEquippableType>[]>(null, canEquip.error);
            }
            
            var slots = GetEquippableSlots(item, amount);
            var e = new List<EquipmentResult<TEquippableType>>();
            foreach (var slot in slots)
            {
                var equipped = DoEquipAt(slot.slot, item, slot.amount, slot.unEquipContents);
                if (equipped.error != null)
                {
                    return new Result<EquipmentResult<TEquippableType>[]>(null, equipped.error);
                }
                
                e.Add(equipped.result);
            }

            return new Result<EquipmentResult<TEquippableType>[]>(e.ToArray());
        }

        public virtual Result<EquipmentResult<TEquippableType>> EquipAt(int index, TEquippableType item, int amount = 1)
        {
            var canEquip = CanEquip(item, amount);
            if (canEquip.error != null)
            {
                return new Result<EquipmentResult<TEquippableType>>(null, canEquip.error);
            }

            return DoEquipAt(index, item, amount, collection?[index] != item && collection?.GetAmount(index) + amount > item.maxStackSize);
        }

        private Result<EquipmentResult<TEquippableType>> DoEquipAt(int index, TEquippableType item, int amount, bool unEquipContents)
        {
            // Remove item from the source collection (the collection it's currently in).
            if (unEquipContents) //  || collection.GetAmount(index) + amount > item.maxStackSize
            {
                // Already an item equipped here, and we're exceeding the stack size. Un-equip item
                var unEquipped = UnEquipAt(index, collection.GetAmount(index));
                if (unEquipped.error != null)
                {
                    return new Result<EquipmentResult<TEquippableType>>(null, unEquipped.error);
                }
            }

            var previousItem = collection?[index];
            var previousEntry = item.collectionEntry;
            
            // Not moving entire stack; Make a clone of the object.
            if (previousEntry?.amount > amount)
            {
                item = (TEquippableType) item.Clone();
            }
            
            var set2 = collection?.Set(index, item, collection.GetAmount(index) + amount);
            if (set2.HasValue && set2.Value.error != null)
            {
                return new Result<EquipmentResult<TEquippableType>>(null, set2.Value.error);
            }

            // Previous (partial) stack, notify it got un-equipped (replaced) by other item.
            previousItem?.OnUnEquipped(index, collection);
            previousEntry?.SetAmountAndUpdateCollection(previousEntry.amount - amount);

            item.OnEquipped(index, collection);
            return new EquipmentResult<TEquippableType>()
            {
                equippedAmount = amount,
                equippedItem = item, 
                index = index,
            };
        }


        public virtual Result<bool> CanUnEquip(int index, int amount)
        {
            if (collection == null)
            {
                return true;
            }
            
            if (collection.GetAmount(index) < amount)
            {
                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
            }
            
            var canRestore = restoreItemsToGroup.CanAdd(collection[index], amount);
            if (canRestore.error != null)
            {
                return canRestore;
            }
            
            return true;
        }
        
        public virtual Result<UnEquipmentResult> UnEquip(TEquippableType item, int amount = 1)
        {
            int index = item.collectionEntry?.index ?? -1;
            if (index == -1 || item.collectionEntry?.collection != collection)
            {
                index = collection?.IndexOf(item) ?? -1;
            }
            
            if (index == -1)
            {
                return new Result<UnEquipmentResult>(null, Errors.CollectionDoesNotContainItem);
            }
            
            return UnEquipAt(index, amount);
        }
        
        public virtual Result<UnEquipmentResult> UnEquipAt(int index, int amount = 1)
        {
            var canUnEquip = CanUnEquip(index, amount);
            if (canUnEquip.error != null)
            {
                return new Result<UnEquipmentResult>(null, canUnEquip.error);
            }
            
            // Not moving entire stack, duplicate item to be restored.
            if (collection != null)
            {
                var restoreItem = collection[index];
                if (amount < collection.GetAmount(index))
                {
                    restoreItem = (TEquippableType)restoreItem.Clone();
                }

                restoreItemsToGroup.Add(restoreItem, amount);
                restoreItem.OnUnEquipped(index, collection);
                
                collection.Set(index, collection[index], collection.GetAmount(index) - amount);
            }
            
            return new Result<UnEquipmentResult>(new UnEquipmentResult()
            {
                slot = index
            });
        }

        public Result<bool> SwapOrMerge(int from, int to)
        {
            return collection?.SwapOrMerge(from, collection, to, collection.GetAmount(from)) ?? false;
        }
    }
}