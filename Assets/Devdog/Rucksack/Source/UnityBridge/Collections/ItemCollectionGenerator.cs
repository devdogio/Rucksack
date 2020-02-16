using System;
using System.Collections;
using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    [RequireComponent(typeof(ItemCollectionCreator))]
    public sealed class ItemCollectionGenerator : MonoBehaviour
    {
        [Serializable]
        private class ItemDefAmountPair
        {
            [Required]
            public UnityItemDefinition itemDefinition;
            public int amount = 1;
        }

        [SerializeField]
        private ItemDefAmountPair[] _items = new ItemDefAmountPair[0];

        private ILogger _logger;
        public ItemCollectionGenerator()
        {
            _logger = new UnityLogger("[Collection] ");
        }
        
        private IEnumerator Start()
        {
            yield return null;

            var c = GetComponent<ItemCollectionCreator>();
            foreach (var item in _items)
            {
                var inst = ItemFactory.CreateInstance(item.itemDefinition, System.Guid.NewGuid());
                var added = c.collection.Add(inst, item.amount);

                if (added.error != null)
                {
                    HandleError(added.error);
                }
            }
        }

        private void HandleError(Error error)
        {
            _logger.Warning(error.ToString(), this);
        }
    }
}