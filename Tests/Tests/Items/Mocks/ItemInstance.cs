using System;
using System.Collections.Generic;
using Devdog.General2;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    [System.Serializable]
    public class ItemInstance : IItemInstance, IEquatable<ItemInstance>
    {
        public Guid ID { get; protected set; }
        public IItemDefinition itemDefinition { get; }
        public IShape2D layoutShape
        {
            get { return itemDefinition.layoutShape; }
        }


        // TODO: Create cooldownContainer that can be shared between instances / by category, etc.
//        public float cooldownTime { get; }

        public int maxStackSize
        {
            get { return itemDefinition.maxStackSize; } 
        }

        
        // For (de)serialization...
        private ItemInstance()
        { }
        
        public ItemInstance(Guid ID, IItemDefinition itemDefinition)
        {
            if (itemDefinition == null)
            {
                throw new ArgumentException("Given ItemDefintiion is null!");
            }
            
            this.ID = ID;
            this.itemDefinition = itemDefinition;
        }
        
        public static bool operator ==(ItemInstance left, ItemInstance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ItemInstance left, ItemInstance right)
        {
            return !Equals(left, right);
        }
        
        public virtual Result<bool> CanUse(Character character, ItemContext useContext)
        {
            return true;
        }

        public virtual Result<ItemUsedResult> Use(Character character, ItemContext useContext)
        {
            var canUse = CanUse(character, useContext);
            if (canUse.result == false)
            {
                return new Result<ItemUsedResult>(null, canUse.error);
            }

            return DoUse(character, useContext);
        }

        protected virtual Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            return new ItemUsedResult(useContext.useAmount, true, 0f);
        }

        public virtual int CompareTo(IItemInstance other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Comparer<IItemDefinition>.Default.Compare(itemDefinition, other.itemDefinition);
        }
        
        public virtual bool Equals(ItemInstance other)
        {
            return Equals((IItemInstance) other);
        }
        
        public virtual bool Equals(IItemInstance other)
        {
            if (ReferenceEquals(null, other)) return false;
            return itemDefinition.Equals(other.itemDefinition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IItemInstance) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ID.GetHashCode() * 397) ^ (itemDefinition != null ? itemDefinition.GetHashCode() : 0);
            }
        }

        public virtual object Clone()
        {
            return new ItemInstance(Guid.NewGuid(), itemDefinition);
        }

        public override string ToString()
        {
            return itemDefinition.name;
        }

    }
}