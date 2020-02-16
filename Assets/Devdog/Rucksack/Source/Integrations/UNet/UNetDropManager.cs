using System.Collections.Generic;
using Devdog.General2;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack
{
    public class UNetDropManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject[] _prefabs = new GameObject[0];

        [SerializeField]
        private float _defaultPickupDistance = 5f;

        private readonly Dictionary<NetworkHash128, GameObject> _prefabLookup = new Dictionary<NetworkHash128, GameObject>();
        
        private readonly ILogger _logger;
        public UNetDropManager()
        {
            _logger = new UnityLogger("[UNet] ");
        }

        public override void OnStartClient()
        {
            foreach (var prefab in _prefabs)
            {
                if (prefab != null)
                {
                    var identity = prefab.GetOrAddComponent<NetworkIdentity>();
                    _logger.LogVerbose("[Client] Registering prefab with assetId: " + identity.assetId);

                    _prefabLookup[identity.assetId] = prefab;
                    ClientScene.RegisterPrefab(prefab, OnSpawn, OnUnSpawn);
                }
            }
        }
        
#if UNITY_EDITOR

        private void OnValidate()
        {
            foreach (var prefab in _prefabs)
            {
                if (prefab == null)
                {
                    continue;
                }
                
                prefab.GetOrAddComponent<NetworkIdentity>();
                UnityEditor.EditorUtility.SetDirty(prefab);
            }
        }

#endif

        protected virtual GameObject OnSpawn(Vector3 position, NetworkHash128 assetID)
        {
            _logger.Log("[Client] Dropping item with assetId: " + assetID);
            
            var obj = Instantiate(_prefabLookup[assetID], position, Quaternion.identity);
            
            // Create 3D model in the world.
            obj.GetOrAddComponent<UNetTrigger>();
            
            var rangeHandler = new GameObject("_RangeHandler");
            rangeHandler.transform.SetParent(obj.transform);
            rangeHandler.transform.localPosition = Vector3.zero;
            rangeHandler.transform.localRotation = Quaternion.identity;
            var handler = rangeHandler.AddComponent<TriggerRangeHandler>();
            handler.useRange = _defaultPickupDistance;

            obj.GetOrAddComponent<TriggerInputHandler>();

            return obj;
        }
        
        protected virtual void OnUnSpawn(GameObject spawned)
        {
            spawned.SetActive(false);
        }
    }
}