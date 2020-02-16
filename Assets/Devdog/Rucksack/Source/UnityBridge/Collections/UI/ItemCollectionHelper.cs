using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    public class ItemCollectionHelper : MonoBehaviour
    {
        [SerializeField]
        protected string collectionName;

        [SerializeField]
        protected UnityItemDefinition itemDef;
        
        [SerializeField]
        protected UnityEquippableItemDefinition equippableItemDef;
        
        protected readonly ILogger logger;
        public ItemCollectionHelper()
        {
            logger = new UnityLogger("[Collection][Helper] ");
        }

        protected virtual ICollection<IItemInstance> GetCollection()
        {
            var collection = CollectionRegistry.byName.Get(collectionName) as ICollection<IItemInstance>;
            if (collection != null)
            {
                return collection;
            }
            
            logger.Warning($"Couldn't find collection with name {collectionName}", this);
            return null;
        }
        
        public void AddItem()
        {
            var collection = GetCollection();
            var inst = ItemFactory.CreateInstance(itemDef, System.Guid.NewGuid());
            var added = collection.Add(inst);
            if (added.error != null)
            {
                logger.Error("Failed to add item to collection", added.error, this);
            }
        }
        
        public void AddEquippableItem()
        {
            var collection = GetCollection();
            var added = collection.Add(ItemFactory.CreateInstance(equippableItemDef, Guid.NewGuid()));
            if (added.error != null)
            {
                logger.Error("Failed to add item to collection", added.error, this);
            }
        }

        public void ToggleReadOnly()
        {
            var collection = (CollectionBase<CollectionSlot<IItemInstance>, IItemInstance>)GetCollection();
            collection.isReadOnly = !collection.isReadOnly;
        }
    }
}