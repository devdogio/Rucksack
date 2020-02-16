using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.Characters
{
    public sealed class PUN2DropHandler : MonoBehaviourPun, IDropHandler
    {
        [SerializeField]
        private float _pickupDistance = 10f;
        public float pickupDistance
        {
            get { return _pickupDistance; }
        }

        [SerializeField]
        private float _maxDropDistance = 10f;
        public float maxDropDistance
        {
            get { return _maxDropDistance; }
        }

        private static readonly ILogger _logger;
        static PUN2DropHandler()
        {
            _logger = new UnityLogger("[PUN2][Character] ");
        }

        public Result<GameObject> Drop(Character character, IUnityItemInstance item, Vector3 worldPosition)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return new Result<GameObject>(null, Errors.ItemCanNotBeDropped);

            if (item?.collectionEntry == null)
            {
                return new Result<GameObject>(null, Errors.ItemCanNotBeDropped);
            }

            var canSet = item.collectionEntry.CanSetAmountAndUpdateCollection(0);
            if (canSet.error != null)
            {
                return new Result<GameObject>(null, canSet.error);
            }

            if (Vector3.Distance(worldPosition, character.transform.position) > maxDropDistance)
            {
                _logger.LogVerbose("[Server] Player suggested position for drop is too far away from player; Forcing drop position", this);
                worldPosition = character.transform.position + (character.transform.forward * 3f);
            }

            var obj = PhotonNetwork.Instantiate(item.itemDefinition.worldModel.name, worldPosition, Quaternion.identity, 0, new object[] { this.pickupDistance });

            _logger.Log("[Server] Spawned object - Relaying to clients", obj);

            var pickup = obj.GetOrAddComponent<PUN2ItemInstancePickup>();
            pickup.itemInstance = item;
            pickup.amount = item.collectionEntry.amount;
            
            // Remove the item from the collection; Fires events on the collection.
            var removed = item.collectionEntry.SetAmountAndUpdateCollection(0);
            if (removed.error != null)
            {
                return new Result<GameObject>(null, removed.error);
            }
            
            // Clear collection entry to ensure we're no longer registered in a collection.
            item.collectionEntry = null;
            
            return obj;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // TODO: check weather _prefabs is in a \Resources folder!
        }
#endif
    }
}