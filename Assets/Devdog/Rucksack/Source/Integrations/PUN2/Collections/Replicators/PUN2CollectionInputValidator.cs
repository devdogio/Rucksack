using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;
using Devdog.Rucksack.Items;
using Devdog.General2;
using Devdog.Rucksack.Characters;

namespace Devdog.Rucksack.Collections
{
    public sealed class PUN2CollectionInputValidator : NetworkInputValidatorBase
    {
        private readonly ILogger logger;

        public PUN2CollectionInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[PUN2][Validation] ");
        }

        public Result<bool> ValidateSwapOrMerge(PhotonView identity, byte[] collectionGuidBytes, Guid collectionGuid, byte[] toCollectionGuidBytes, Guid toCollectionGuid, ushort fromIndex, ushort toIndex, short amount, out ICollection fromOut, out ICollection toOut)
        {
            fromOut = null;
            toOut = null;
            if (CheckGuidBytes(collectionGuidBytes) == false ||
                CheckGuidBytes(toCollectionGuidBytes) == false)
            {
                return Failed();
            }

            if (CheckCollectionPermission(identity, collectionGuid, toCollectionGuid, out fromOut, out toOut) == false)
            {
                return Failed();
            }

            if (CheckClamped(fromIndex, 0, fromOut.slotCount - 1) == false ||
               CheckClamped(toIndex, 0, toOut.slotCount - 1) == false ||
               CheckClamped(amount, 1, fromOut.GetAmount(fromIndex)) == false)
            {
                return Failed();
            }

            return true;
        }

        public Result<bool> ValidateMoveAuto(PhotonView identity, byte[] collectionGuidBytes, Guid collectionGuid, byte[] toCollectionGuidBytes, Guid toCollectionGuid, ushort fromIndex, short amount, out ICollection fromOut, out ICollection toOut)
        {
            fromOut = null;
            toOut = null;
            if (CheckGuidBytes(collectionGuidBytes) == false ||
                CheckGuidBytes(toCollectionGuidBytes) == false)
            {
                return Failed();
            }
        
            if (CheckCollectionPermission(identity, collectionGuid, toCollectionGuid, out fromOut, out toOut) == false)
            {
                return Failed();
            }
        
            if (CheckClamped(fromIndex, 0, fromOut.slotCount - 1) == false ||
               CheckClamped(amount, 1, fromOut.GetAmount(fromIndex)) == false)
            {
                return Failed();
            }
        
            return true;
        }
        
        public Result<bool> ValidateUseItem(PhotonView identity, Guid itemGuid, ushort useAmount, out INetworkItemInstance outItem)
        {
            outItem = null;
            if (CheckGuidBytes(itemGuid.ToByteArray()) == false)
            {
                return Failed();
            }
        
            if (CheckItemPermission(identity, itemGuid, ReadWritePermission.ReadWrite, out outItem) == false)
            {
                return Failed();
            }
        
            // TODO: Consider creating a distance check to Use items directly from the world, rather than from a collection (requires permission changes on items).
        
            if (outItem.collectionEntry.amount < useAmount)
            {
                return Failed();
            }
        
            return true;
        }
        
        public Result<bool> ValidateDropItem(PhotonView identity, Guid itemGuid, Player player, out INetworkItemInstance outItem, ref Vector3 worldPosition)
        {
            outItem = null;
            if (CheckGuidBytes(itemGuid.ToByteArray()) == false)
            {
                return Failed();
            }
        
            if (CheckItemPermission(identity, itemGuid, ReadWritePermission.ReadWrite, out outItem) == false)
            {
                return Failed();
            }
        
            if (Vector3.Distance(worldPosition, player.transform.position) > player.GetComponent<IDropHandler>().maxDropDistance)
            {
                logger.LogVerbose("[Server] player suggested position for drop is too far away from player; Forcing drop position", this);
                worldPosition = player.transform.position + (player.transform.forward * 3f); // TODO: Use drop handler.
            }
        
            return true;
        }
        
        
        private bool CheckItemPermission(PhotonView identity, System.Guid itemGuid, ReadWritePermission minimalPermission, out INetworkItemInstance outItem)
        {
            outItem = ServerItemRegistry.Get(itemGuid) as INetworkItemInstance;
            if (outItem?.collectionEntry?.collection == null)
            {
                return false;
            }
        
            if (CheckCollectionPermission(identity, outItem.collectionEntry.collection, minimalPermission) == false)
            {
                return false;
            }
        
            return true;
        }
        
        private bool CheckCollectionPermission(PhotonView identity, ICollection collection, ReadWritePermission minimalPermission)
        {
            var c = collection as IPUN2Collection;
            if (c == null)
            {
                return false;
            }
        
            // Make sure the player has read/write access on the collection the item is in.
            var permission = PUN2PermissionsRegistry.collections.GetPermission(c, identity);
            if (permission < minimalPermission)
            {
                return false;
            }
        
            return true;
        }

        private bool CheckCollectionPermission(PhotonView identity, System.Guid fromGuid, System.Guid toGuid, out ICollection fromOut, out ICollection toOut)
        {
            toOut = null;
        
            fromOut = PUN2ActionsBridge.collectionFinder.GetServerCollection(fromGuid) as ICollection;
            if (fromOut == null)
            {
                logger.Error($"[Server] Couldn't find collection with name {fromGuid} - ViewID: {identity.ViewID}");
                return false;
            }
        
            toOut = PUN2ActionsBridge.collectionFinder.GetServerCollection(toGuid) as ICollection;
            if (toOut == null)
            {
                logger.Error($"[Server] Couldn't find collection with name {toGuid} - ViewID: {identity.ViewID}");
                return false;
            }
        
            var fromAccess = PUN2PermissionsRegistry.collections.GetPermission(fromOut as IPUN2Collection, identity);
            var toAccess = PUN2PermissionsRegistry.collections.GetPermission(toOut as IPUN2Collection, identity);
            if (fromAccess < ReadWritePermission.ReadWrite || toAccess < ReadWritePermission.ReadWrite)
            {
                logger.Warning("[Server] Client doesn't have authority over collection(s)", this);
                return false;
            }
        
            return true;
        }
    }
}
