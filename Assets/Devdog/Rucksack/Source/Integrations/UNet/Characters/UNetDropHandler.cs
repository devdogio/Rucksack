using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Characters
{
    public sealed class UNetDropHandler : NetworkBehaviour, IDropHandler
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
        static UNetDropHandler()
        {
            _logger = new UnityLogger("[UNet][Character] ");
        }

        [ServerCallback]
        public Result<GameObject> Drop(Character character, IUnityItemInstance item, Vector3 worldPosition)
        {
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
            
            // Create 3D model in the world.
            var obj = UnityEngine.Object.Instantiate<GameObject>(item.itemDefinition.worldModel, worldPosition, Quaternion.identity);
            obj.GetOrAddComponent<NetworkIdentity>();
            obj.GetOrAddComponent<UNetTrigger>();
            
            var rangeHandler = new GameObject("_RangeHandler");
            rangeHandler.transform.SetParent(obj.transform);
            rangeHandler.transform.localPosition = Vector3.zero;
            rangeHandler.transform.localRotation = Quaternion.identity;
            var handler = rangeHandler.AddComponent<TriggerRangeHandler>();
            handler.useRange = _pickupDistance;

            obj.GetOrAddComponent<TriggerInputHandler>();
            var pickup = obj.GetOrAddComponent<UNetItemInstancePickup>();
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
            
            _logger.Log("[Server] Spawned object - Relaying to clients", obj);
            NetworkServer.Spawn(obj);

            return obj;
        }
    }
}