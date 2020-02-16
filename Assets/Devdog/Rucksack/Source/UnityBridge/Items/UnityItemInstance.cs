using System;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.Rucksack.Items
{
    public partial class UnityItemInstance : IUnityItemInstance, IEquatable<UnityItemInstance>
    {
        public virtual ICollectionEntry collectionEntry { get; set; }

        protected static ILogger logger = new UnityLogger("[Item] ");
        
        public Guid ID { get; protected set; }
        public IUnityItemDefinition itemDefinition { get; protected set; }
        IItemDefinition IItemInstance.itemDefinition
        {
            get { return itemDefinition; }
        }

        public virtual IShape2D layoutShape
        {
            get { return itemDefinition.layoutShape; }
        }
        
        public virtual int maxStackSize
        {
            get { return itemDefinition.maxStackSize; }
        }

        protected UnityItemInstance()
        { }

        protected UnityItemInstance(Guid ID, IUnityItemDefinition itemDefinition)
        {
            this.ID = ID;
            this.itemDefinition = itemDefinition;
        }

        public virtual Result<bool> CanUse(Character character, ItemContext useContext)
        {
            if (collectionEntry != null)
            {
                if (useContext.useAmount > collectionEntry.amount)
                {
                    return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
                }
                
                var canSet = collectionEntry.CanSetAmountAndUpdateCollection(collectionEntry.amount - useContext.useAmount);
                if (canSet.result == false)
                {
                    return canSet;
                }

                return true;
            }

            // NOTE: Item has to be in a collection to be usable.
            return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
        }

        public Result<ItemUsedResult> Use(Character character, ItemContext useContext)
        {
            var canUse = CanUse(character, useContext);
            if (canUse.result == false)
            {
                return new Result<ItemUsedResult>(null, canUse.error);
            }

            var useResult = DoUse(character, useContext);
            if (useResult.error == null)
            {
                if (collectionEntry != null)
                {
                    if (useResult.result.reduceStackSize)
                    {
                        var success = collectionEntry.SetAmountAndUpdateCollection(collectionEntry.amount - useResult.result.usedAmount);
                        if (success.result == false)
                        {
                            return new Result<ItemUsedResult>(null, success.error);
                        }
                    }   
                }
            }
            
            return useResult;
        }

        public virtual Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            return new ItemUsedResult(useContext.useAmount, false, 0f);
        }

        public virtual Result<GameObject> Drop(Character character, Vector3 worldPosition)
        {
            if (collectionEntry == null)
            {
                return new Result<GameObject>(null, Errors.ItemCanNotBeDropped);
            }

            var dropHandler = character.GetComponent<IDropHandler>();
            Assert.IsNotNull(dropHandler, $"No {typeof(IDropHandler).Name} component found on character. Can't drop item.");
            return dropHandler.Drop(character, this, worldPosition);
        }

        public int CompareTo(IItemInstance other)
        {
            return ID.CompareTo(other.ID);
        }
        
        public bool Equals(IUnityItemInstance other)
        {
            return Equals((IItemInstance) other);
        }
        
        public bool Equals(UnityItemInstance other)
        {
            return Equals((IItemInstance) other);
        }

        public virtual bool Equals(IItemInstance other)
        {
            return itemDefinition.Equals(other?.itemDefinition);
        }

        public virtual object Clone()
        {
            var clone = (UnityItemInstance) MemberwiseClone();
            clone.ID = System.Guid.NewGuid();
            ItemRegistry.Register(clone.ID, clone);
            return clone;
        }
        
        public override string ToString()
        {
            return itemDefinition.name;
        }

    }
}