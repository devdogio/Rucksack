using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;
using Devdog.Rucksack.Items;
using System.Runtime.CompilerServices;

namespace Devdog.Rucksack.Collections
{
    public class PUN2CollectionReplicator
    {
        protected readonly ILogger logger;
        protected readonly PUN2ActionsBridge bridge;

        protected static PUN2CollectionInputValidator inputValidator;

        static PUN2CollectionReplicator()
        {
            inputValidator = new PUN2CollectionInputValidator(new UnityLogger("[PUN2][Validation] "));
        }

        public PUN2CollectionReplicator(PUN2ActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[PUN2] ");
        }

        public void TargetRpc_AddCollection(PhotonView owner, string collectionName, Guid collectionGuid, int slotCount)
        {
            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid);
            if (collection == null)
            {
                collection = PUN2CollectionUtility.CreateClientItemCollection(slotCount, collectionName, collectionGuid, owner, bridge);
            }
        }

        public void TargetRpc_SetCollectionPermission(Guid collectionGuid, ReadWritePermission permission)
        {
            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid);
            if (collection == null)
            {
                collection = PUN2ActionsBridge.collectionFinder.GetClientCurrencyCollection(collectionGuid);
            }

            if (collection != null)
            {
                PUN2PermissionsRegistry.collections.SetPermission(collection, bridge.photonView, permission);

                // NOTE: If we're setting permission to none AND we're not the owner of the collection just remove it from our registry.
                // NOTE: The server will have to re-send the collection to the client once the client interacts with it.
                if (permission == ReadWritePermission.None && collection.owner != bridge.photonView)
                {
                    collection.UnRegister();
                }

                logger.Log($"[Client] Set collection {collectionGuid} permission to {permission} with ViewId: {bridge.photonView.ViewID}", bridge);
            }
            else
            {
                logger.Warning($"[Client] Collection with guid: {collectionGuid} not found. Can't set permission", bridge);
            }
        }

        public void TargetRpc_SetSlot(Guid collectionGuid, Guid itemInstanceGuid, ushort index, ushort amount)
        {
            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid) as ICollection;
            if (collection != null)
            {
                IItemInstance item = null;
                //var item = ItemRegistry.Get(itemInstanceGuid);
                ItemRegistry.TryGet(itemInstanceGuid, out item);

                collection.ForceSetBoxed(index, item, item != null ? amount : 0);

                logger.Log($"[Client] Set index {index} to item with GUID: {itemInstanceGuid} x {amount} in collection '{collection}'({collectionGuid}) with viewId: {bridge.photonView.ViewID}", bridge);
            }
            else
            {
                logger.Warning($"[Client] Collection with guid: {collectionGuid} not found. Can't set slot {index}", bridge);
            }
        }

        public void Cmd_RequestSwapOrMerge(byte[] collectionGuidBytes, Guid collectionGuid, byte[] toCollectionGuidBytes, Guid toCollectionGuid, ushort fromIndex, ushort toIndex, short amount)
        {
            logger.LogVerbose($"[Server] Client requested to SwapOrMerge an item from {fromIndex} to {toIndex}. slot from collection {collectionGuid} to {toCollectionGuid}", bridge);

            ICollection fromCol, toCol;
            var error = inputValidator.ValidateSwapOrMerge(bridge.photonView, collectionGuidBytes, collectionGuid, toCollectionGuidBytes, toCollectionGuid, fromIndex, toIndex, amount, out fromCol, out toCol).error;
            if (error == null)
            {
                error = fromCol.SwapOrMerge(fromIndex, toCol, toIndex, amount).error;
            }

            HandleError(error);
        }

        public void Cmd_RequestMoveAuto(byte[] collectionGuidBytes, Guid collectionGuid, byte[] toCollectionGuidBytes, Guid toCollectionGuid, ushort fromIndex, short amount)
        {
            logger.LogVerbose($"[Server] Client requested to move an item from {fromIndex} to auto. slot from collection {collectionGuid} to {toCollectionGuid}", bridge);

            ICollection fromCol, toCol;
            var error = inputValidator.ValidateMoveAuto(bridge.photonView, collectionGuidBytes, collectionGuid, toCollectionGuidBytes, toCollectionGuid, fromIndex, amount, out fromCol, out toCol).error;
            if (error == null)
            {
                error = fromCol.MoveAuto(fromIndex, toCol, amount).error;
            }

            HandleError(error);
        }

        public void Cmd_RequestUseItem(Guid itemGuid, ushort useAmount, short targetIndex)
        {
            logger.LogVerbose($"[Server] Client requested to use item with guid {itemGuid} {useAmount}x", bridge);

            INetworkItemInstance item;
            var error = inputValidator.ValidateUseItem(bridge.photonView, itemGuid, useAmount, out item).error;
            if (error == null)
            {
                var ctx = new ItemContext
                {
                    useAmount = useAmount,
                    targetIndex = targetIndex,
                };

                error = item.Server_Use(bridge.player, ctx).error;
                if (error == null)
                {
                    bridge.photonView.RPC(nameof(bridge.TargetRpc_NotifyItemUsed), bridge.photonView.Owner,
                        /*itemID: */ item.ID.ToByteArray(),
                        /*amountUsed: */ (int)useAmount,
                        /*targetIndex: */ (int)targetIndex
                    );
                }
            }

            HandleError(error);
        }

        public void Cmd_RequestDropItem(Guid itemGuid, Vector3 worldPosition)
        {
            logger.LogVerbose($"[Server] Client requested to drop item with guid {itemGuid}", bridge);

            INetworkItemInstance item;
            var error = inputValidator.ValidateDropItem(bridge.photonView, itemGuid, bridge.player, out item, ref worldPosition).error;
            if (error == null)
            {
                error = item.Server_Drop(bridge.player, worldPosition).error;
            }

            HandleError(error);
        }

        public void TargetRpc_SetCollectionContents(PhotonView target, string collectionName, Guid collectionGuid, ItemAmountMessage[] items)
        {
            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid) as ICollection;
            if (collection != null)
            {
                collection.ForceSetBoxed<IItemInstance>(items.Select(o => new Tuple<IItemInstance, int>(o.itemInstance.TryCreateItemInstance(bridge.itemsDatabase), o.amount)).ToArray());
            }
            else
            {
                logger.Warning($"[Client] Collection with guid: {collectionGuid} not found. Can't set collection contents", bridge);
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
