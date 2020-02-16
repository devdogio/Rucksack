using System;
using Devdog.General2;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    public class CollectionItemInstance : ItemInstance, ICollectionSlotEntry, IEquatable<CollectionItemInstance>
    {
        public ICollectionEntry collectionEntry { get; set; }

        public CollectionItemInstance(Guid ID, IItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        {
            
        }
        
        public static bool operator ==(CollectionItemInstance left, CollectionItemInstance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CollectionItemInstance left, CollectionItemInstance right)
        {
            return !Equals(left, right);
        }
        
        public override Result<bool> CanUse(Character character, ItemContext useContext)
        {
            var canUse = base.CanUse(character, useContext);
            if (canUse.result == false)
            {
                return canUse;
            }
            
            if (collectionEntry != null)
            {
                var canSet = collectionEntry.CanSetAmountAndUpdateCollection(collectionEntry.amount - useContext.useAmount);
                if (canSet.result == false)
                {
                    return canSet;
                }   
            }

            return true;
        }
        
        public sealed override Result<ItemUsedResult> Use(Character character, ItemContext useContext)
        {
            var used = base.Use(character, useContext);
            if (used.error == null)
            {
                if (collectionEntry != null)
                {
                    var success = collectionEntry.SetAmountAndUpdateCollection(collectionEntry.amount - used.result.usedAmount);
                    if (success.result == false)
                    {
                        return new Result<ItemUsedResult>(null, success.error);
                    }
                }
            }

            return used;
        }
        
        public bool Equals(CollectionItemInstance other)
        {
            return Equals((IItemInstance) other);
        }
        
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override object Clone()
        {
            return new CollectionItemInstance(Guid.NewGuid(), itemDefinition)
            {
//                collectionEntry = collectionEntry
            };
        }
    }
}