using System;
using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class UNetSpawnReplicator
    {
        protected readonly ILogger logger;
        protected readonly UNetActionsBridge bridge;

        public UNetSpawnReplicator(UNetActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[UNet] ");
        }
        
        // public void TargetRpc_SpawnItem(NetworkConnection target, SpawnItemMessage data)
        // {
        //     if (ClientScene.prefabs.ContainsKey(data.assetID) == false)
        //     {
        //         logger.Warning($"Server told to spawn asset with ID {data.assetID}, but no local asset with this ID was found!");
        //         return;
        //     }

        //     var prefab = ClientScene.prefabs[data.assetID];
        //     var obj = UnityEngine.Object.Instantiate<GameObject>(prefab, data.position, data.rotation);
            
        //     // Create 3D model in the world.
        //     obj.GetOrAddComponent<UNetTrigger>();
        //     obj.GetOrAddComponent<TriggerInputHandler>();

        //     // TODO: Sync up the item definition to the client, so the client can show data about the object.
            
        //     var rangeHandler = new GameObject("_RangeHandler");
        //     rangeHandler.transform.SetParent(obj.transform);
        //     rangeHandler.transform.localPosition = Vector3.zero;
        //     rangeHandler.transform.localRotation = Quaternion.identity;
        //     var handler = rangeHandler.AddComponent<TriggerRangeHandler>();
        //     handler.useRange = 10f; // TODO: Set this in a global location somewhere...
            
        //     ClientScene.SetLocalObject(data.networkInstanceID, obj);
            
        //     logger.LogVerbose($"Created object with instance ID: {data.networkInstanceID} and assetID: {data.assetID}");
        // }
        
        // public void TargetRpc_UnSpawnItem(NetworkConnection target, UnSpawnItemMessage data)
        // {
        //     var obj = ClientScene.FindLocalObject(data.networkInstanceID);
        //     UnityEngine.Object.Destroy(obj);

        //     logger.LogVerbose($"Destroyed object with instance ID: {data.networkInstanceID}");
        // }
    }
}