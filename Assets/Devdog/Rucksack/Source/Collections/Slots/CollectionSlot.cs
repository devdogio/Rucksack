
using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// The default collection slot.
    /// </summary>
    public class CollectionSlot<T> : ICollectionEntry
        where T : class, IEquatable<T>, IStackable
    {
        public ICollection collection { get; set; }
        public int index { get; set; }
        
        private T _item;
        public T item
        {
            get { return _item; }
            private set
            {
                _item = value;
                if (!isOccupied)
                {
                    amount = 0;
                }
            }
        }
        
        public bool setCollectionEntry { get; set; }

        public int amount { get; private set; }
        public virtual bool isOccupied
        {
            get
            {
                return IsNull(item) == false;
            }
        }

        public CollectionSlot()
        {
            setCollectionEntry = true;
        }

        protected bool IsNull(T checkItem)
        {
            return checkItem == null || checkItem.Equals(default(T));
        }

        public virtual void SetWithoutValidation(T newItem, int newAmount)
        {
            // If we're setting the slot to null make sure to clean up the slot entry
            if (setCollectionEntry)
            {
                // If the same item is already in this slot and we're overwriting it, don't clear the current item's slotEntry.
                // Users might still have a reference to the old object, so clearing it can give strange behavior.
                if (EqualityComparer<T>.Default.Equals(item, newItem) == false)
                {
                    var entryOwner = this.item as ICollectionSlotEntry;
                    // NOTE: Only clear if the collectionEntry reference is still pointing to us...
                    if (entryOwner != null && entryOwner.collectionEntry == this)
                    {
                        entryOwner.collectionEntry = null;
                    }   
                }
            }
            
            item = newItem;
            amount = newAmount;
            if (isOccupied == false)
            {
                amount = 0;
            }
            
            // Set the new slot entry
            if (setCollectionEntry)
            {
                var entryOwner2 = item as ICollectionSlotEntry;
                if (entryOwner2 != null)
                {
                    entryOwner2.collectionEntry = this;
                }
            }
        }

        public Result<bool> CanSetAmountAndUpdateCollection(int setAmount)
        {
            return collection.CanSetBoxed(index, item, setAmount);
        }
        
        public Result<bool> SetAmountAndUpdateCollection(int setAmount)
        {
            return collection.SetBoxed(index, item, setAmount);
        }
    
        public virtual void Clear()
        {
            if (setCollectionEntry)
            {
                var entry = item as ICollectionSlotEntry;
                // NOTE: Only clear if the collectionEntry reference is still pointing to us...
                if (entry != null && entry.collectionEntry == this)
                {
                    entry.collectionEntry = null;
                }
            }
            
            item = default(T);
            amount = 0;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{item}:{amount}";
        }
    }
}