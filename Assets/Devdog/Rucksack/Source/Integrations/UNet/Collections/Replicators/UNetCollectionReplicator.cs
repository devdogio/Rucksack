using System;
using System.Runtime.CompilerServices;
using System.Linq;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public class UNetCollectionReplicator
    {
        protected readonly ILogger logger;
        protected readonly UNetActionsBridge bridge;
        
        protected static UNetCollectionInputValidator inputValidator;

        static UNetCollectionReplicator()
        {
            inputValidator = new UNetCollectionInputValidator(new UnityLogger("[UNet][Validation] "));
        }
        
        public UNetCollectionReplicator(UNetActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[UNet] ");
        }
        
        public void Cmd_RequestSwapOrMerge(RequestSwapOrMergeMessage data)
        {
            logger.LogVerbose($"[Server] Client requested to SwapOrMerge an item from {data.fromIndex} to {data.toIndex}. slot from collection {data.collectionGuid} to {data.toCollectionGuid}", bridge);

            ICollection fromCol, toCol;
            var error = inputValidator.ValidateSwapOrMerge(bridge.identity, data, out fromCol, out toCol).error;
            if (error == null)
            {
                error = fromCol.SwapOrMerge(data.fromIndex, toCol, data.toIndex, data.amount).error;
            }
            
            HandleError(error);
        }
        
        public void Cmd_RequestMoveAuto(RequestMoveAutoMessage data)
        {
            logger.LogVerbose($"[Server] Client requested to move an item from {data.fromIndex} to auto. slot from collection {data.collectionGuid} to {data.toCollectionGuid}", bridge);

            ICollection fromCol, toCol;
            var error = inputValidator.ValidateMoveAuto(bridge.identity, data, out fromCol, out toCol).error;
            if (error == null)
            {
                error = fromCol.MoveAuto(data.fromIndex, toCol, data.amount).error;
            }

            HandleError(error);
        }

        public void Cmd_RequestUseItem(RequestUseItemMessage data)
        {
            logger.LogVerbose($"[Server] Client requested to use item with guid {data.itemGuid} {data.useAmount}x", bridge);

            INetworkItemInstance item;
            var error = inputValidator.ValidateUseItem(bridge.identity, data, out item).error;
            if (error == null)
            {
                var ctx = new ItemContext
                {
                    useAmount = data.useAmount,
                    targetIndex = data.targetIndex,
                };
                
                error = item.Server_Use(bridge.player, ctx).error;
                if (error == null)
                {
                    bridge.TargetRpc_NotifyItemUsed(bridge.connectionToClient, new ItemUsedMessage()
                    {
                        itemID = item.ID,
                        amountUsed = data.useAmount,
                        targetIndex = data.targetIndex
                    });
                }
            }

            HandleError(error);
        }

        public void Cmd_RequestDropItem(RequestDropItemMessage data)
        {
            logger.LogVerbose($"[Server] Client requested to drop item with guid {data.itemGuid}", bridge);
            
            INetworkItemInstance item;
            var error = inputValidator.ValidateDropItem(bridge.identity, data, bridge.player, out item).error;
            if (error == null)
            {
                error = item.Server_Drop(bridge.player, data.worldPosition).error;
            }
            
            HandleError(error);
        }
        
        
        
        // TODO: Consider separating into client and server replicators.
        
        
        
        public void TargetRpc_AddCollection(NetworkConnection target, AddCollectionMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid);
            if (collection == null)
            {
                collection = UNetCollectionUtility.CreateClientItemCollection(data.slotCount, data.collectionName, data.collectionGuid, data.owner, bridge);
            }
        }
        
        public void TargetRpc_SetCollectionContents(NetworkConnection target, SetCollectionContentsMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid) as ICollection;
            if (collection != null)
            {
                collection.ForceSetBoxed<IItemInstance>(data.items.Select(o => new Tuple<IItemInstance, int>(o.itemInstance.TryCreateItemInstance(bridge.itemsDatabase), o.amount)).ToArray());
            }
            else
            {
                logger.Warning($"[Client] Collection with guid: {data.collectionGuid} not found. Can't set collection contents", bridge);
            }
        }

        public void TargetRpc_SetCollectionPermission(NetworkConnection target, SetCollectionPermissionMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid);
            if (collection == null)
            {
                collection = UNetActionsBridge.collectionFinder.GetClientCurrencyCollection(data.collectionGuid);
            }

            if (collection != null)
            {
                UNetPermissionsRegistry.collections.SetPermission(collection, bridge.identity, data.permission);

                // NOTE: If we're setting permission to none AND we're not the owner of the collection just remove it from our registry.
                // NOTE: The server will have to re-send the collection to the client once the client interacts with it.
                if (data.permission == ReadWritePermission.None && collection.owner != bridge.identity)
                {
                    collection.UnRegister();
                }

                logger.Log($"[Client] Set collection {data.collectionGuid} permission to {data.permission} with netId: {bridge.netId}", bridge);
            }
            else
            {
                logger.Warning($"[Client] Collection with guid: {data.collectionGuid} not found. Can't set permission", bridge);
            }
        }
        
        public void TargetRpc_SetSlot(NetworkConnection target, SlotDataMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid) as ICollection;
            if (collection != null)
            {
                var item = ItemRegistry.Get(data.itemInstanceGuid);
                collection.ForceSetBoxed(data.index, item, item != null ? data.amount : 0);

                logger.Log($"[Client] Set index {data.index} to item with GUID: {data.itemInstanceGuid} x {data.amount} in collection {data.collectionGuid} with netId: {bridge.netId}", bridge);
            }
            else
            {
                logger.Warning($"[Client] Collection with guid: {data.collectionGuid} not found. Can't set slot {data.index}", bridge);
            }
        }
        
        
        
        
        
        
        
        
        
        protected void HandleError(Error error, [CallerMemberName] string name = "")
        {
            if (error != null)
            {
                // TODO: Send message back to client about failed action...
                logger.Error($"Player action '{name}' failed: ", error, bridge.player);
            }
        }
    }
}