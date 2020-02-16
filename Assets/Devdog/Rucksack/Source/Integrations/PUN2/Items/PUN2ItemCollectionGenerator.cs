using System;
using System.Collections;
using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    [RequireComponent(typeof(PUN2ItemCollectionCreator))]
    public sealed class PUN2ItemCollectionGenerator : MonoBehaviourPun
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
        public PUN2ItemCollectionGenerator()
        {
            _logger = new UnityLogger("[Collection] ");
        }

        //[ServerCallback]
        private IEnumerator Start()
        {
            if (!PhotonNetwork.IsMasterClient)
                yield break;

            yield return null;

            var c = (ICollection<IItemInstance>)GetComponent<PUN2ItemCollectionCreator>().collection;
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