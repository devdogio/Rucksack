using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Characters
{
    public sealed class DropHandler : MonoBehaviour, IDropHandler
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

//        [SerializeField]
//        private bool _raycastToGround = true;
        
        private static readonly ILogger _logger;
        static DropHandler()
        {
            _logger = new UnityLogger("[Character] ");
        }

        public Result<GameObject> Drop(Character character, IUnityItemInstance item, Vector3 worldPosition)
        {
            if (Vector3.Distance(worldPosition, character.transform.position) > maxDropDistance)
            {
                _logger.LogVerbose("Player suggested position for drop is too far away from player; Forcing drop position", this);
                worldPosition = character.transform.position + (character.transform.forward * 3f);
            }
            
            // Create 3D model in the world.
            var obj = UnityEngine.Object.Instantiate<GameObject>(item.itemDefinition.worldModel, worldPosition, Quaternion.identity);
            obj.GetOrAddComponent<Trigger>();
            
            var rangeHandler = new GameObject("_RangeHandler");
            rangeHandler.transform.SetParent(obj.transform);
            rangeHandler.transform.localPosition = Vector3.zero;
            rangeHandler.transform.localRotation = Quaternion.identity;
            var handler = rangeHandler.AddComponent<TriggerRangeHandler>();
            handler.useRange = _pickupDistance;

            obj.GetOrAddComponent<TriggerInputHandler>();
            var pickup = obj.GetOrAddComponent<TriggerItemInstancePickup>();
            pickup.itemInstance = item;
            pickup.amount = item.collectionEntry?.amount ?? 1;

            // Remove the item from the collection; Fires events on the collection.
            var removed = item.collectionEntry.SetAmountAndUpdateCollection(0);
            if (removed.error != null)
            {
                return new Result<GameObject>(null, removed.error);
            }

            // Clear collection entry to ensure we're no longer registered in a collection.
            item.collectionEntry = null;
            
            _logger.Log("Spawned object", obj);
            return obj;
        }
    }
}