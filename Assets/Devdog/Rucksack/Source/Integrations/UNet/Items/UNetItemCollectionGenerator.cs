using System;
using System.Collections;
using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    [RequireComponent(typeof(UNetItemCollectionCreator))]
    public sealed class UNetItemCollectionGenerator : NetworkBehaviour
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
        public UNetItemCollectionGenerator()
        {
            _logger = new UnityLogger("[Collection] ");
        }
        
        [ServerCallback]
        private IEnumerator Start()
        {
            yield return null;

            var c = (ICollection<IItemInstance>)GetComponent<UNetItemCollectionCreator>().collection;
            foreach (var item in _items)
            {
                var inst = ItemFactory.CreateInstance(item.itemDefinition, System.Guid.NewGuid());
                var added = c.Add(inst, item.amount);

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