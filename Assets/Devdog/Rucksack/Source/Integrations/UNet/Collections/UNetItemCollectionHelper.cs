using System;
using Devdog.Rucksack.UI;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    public sealed class UNetItemCollectionHelper : MonoBehaviour
    {
        [SerializeField]
        private SerializedGuid _collectionGuid;

        [SerializeField]
        private UnityItemDefinition _itemDef;
        
        [SerializeField]
        private UnityEquippableItemDefinition _equippableItemDef;
        
        private readonly ILogger _logger;
        public UNetItemCollectionHelper()
        {
            _logger = new UnityLogger("[Collection][Helper] ");
        }

        private ICollection<IItemInstance> GetCollection()
        {
            var collection = ServerCollectionRegistry.byID.Get(_collectionGuid.guid) as ICollection<IItemInstance>;
            if (collection != null)
            {
                return collection;
            }
            
            collection = CollectionRegistry.byID.Get(_collectionGuid.guid) as ICollection<IItemInstance>;
            if (collection != null)
            {
                return collection;
            }
            
            _logger.Warning($"Couldn't find collection with name {_collectionGuid.guid}", this);
            return null;
        }
        
        public void AddItem()
        {
            var collection = GetCollection();
            var inst = ItemFactory.CreateInstance(_itemDef, System.Guid.NewGuid());
            
            var added = collection.Add(inst);
            if (added.error != null)
            {
                _logger.Error("Failed to add item to collection", added.error, this);
            }
        }
        
        public void AddEquippableItem()
        {
            var collection = GetCollection();
            var inst = ItemFactory.CreateInstance(_equippableItemDef, Guid.NewGuid());

            var added = collection.Add(inst);
            if (added.error != null)
            {
                _logger.Error("Failed to add item to collection", added.error, this);
            }
        }

        public void ToggleReadOnly()
        {
            var collection = (CollectionBase<CollectionSlot<IItemInstance>, IItemInstance>)GetCollection();
            collection.isReadOnly = !collection.isReadOnly;
        }
    }
}