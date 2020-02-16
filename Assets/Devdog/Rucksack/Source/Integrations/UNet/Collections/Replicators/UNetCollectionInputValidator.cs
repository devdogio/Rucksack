using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public class UNetCollectionInputValidator : NetworkInputValidatorBase
    {
        protected readonly ILogger logger;

        public UNetCollectionInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[UNet][Validation] ");
        }

        public Result<bool> ValidateSwapOrMerge(NetworkIdentity identity, RequestSwapOrMergeMessage data, out ICollection fromOut, out ICollection toOut)
        {
            fromOut = null;
            toOut = null;
            if (CheckGuidBytes(data.collectionGuid.bytes) == false ||
                CheckGuidBytes(data.toCollectionGuid.bytes) == false)
            {
                return Failed();
            }
            
            if (CheckCollectionPermission(identity, data.collectionGuid, data.toCollectionGuid, out fromOut, out toOut) == false)
            {
                return Failed();
            }
            
            if(CheckClamped(data.fromIndex, 0, fromOut.slotCount - 1) == false ||
               CheckClamped(data.toIndex, 0, toOut.slotCount - 1) == false ||
               CheckClamped(data.amount, 1, fromOut.GetAmount(data.fromIndex)) == false)
            {
               return Failed(); 
            }

            return true;
        }
        
        public Result<bool> ValidateMoveAuto(NetworkIdentity identity, RequestMoveAutoMessage data, out ICollection fromOut, out ICollection toOut)
        {
            fromOut = null;
            toOut = null;
            if (CheckGuidBytes(data.collectionGuid.bytes) == false ||
                CheckGuidBytes(data.toCollectionGuid.bytes) == false)
            {
                return Failed();
            }
            
            if (CheckCollectionPermission(identity, data.collectionGuid, data.toCollectionGuid, out fromOut, out toOut) == false)
            {
                return Failed();
            }
            
            if(CheckClamped(data.fromIndex, 0, fromOut.slotCount - 1) == false ||
               CheckClamped(data.amount, 1, fromOut.GetAmount(data.fromIndex)) == false)
            {
                return Failed(); 
            }

            return true;
        }

        public Result<bool> ValidateUseItem(NetworkIdentity identity, RequestUseItemMessage data, out INetworkItemInstance outItem)
        {
            outItem = null;
            if (CheckGuidBytes(data.itemGuid.bytes) == false)
            {
                return Failed();
            }
            
            if (CheckItemPermission(identity, data.itemGuid, ReadWritePermission.ReadWrite, out outItem) == false)
            {
                return Failed();
            }
            
            // TODO: Consider creating a distance check to Use items directly from the world, rather than from a collection (requires permission changes on items).
            
            if (outItem.collectionEntry.amount < data.useAmount)
            {
                return Failed();
            }

            return true;
        }
        
        public Result<bool> ValidateDropItem(NetworkIdentity identity, RequestDropItemMessage data, Player player, out INetworkItemInstance outItem)
        {
            outItem = null;
            if (CheckGuidBytes(data.itemGuid.bytes) == false)
            {
                return Failed();
            }

            if (CheckItemPermission(identity, data.itemGuid, ReadWritePermission.ReadWrite, out outItem) == false)
            {
                return Failed();
            }
            
            if (Vector3.Distance(data.worldPosition, player.transform.position) > player.GetComponent<IDropHandler>().maxDropDistance)
            {
                logger.LogVerbose("[Server] player suggested position for drop is too far away from player; Forcing drop position", this);
                data.worldPosition = player.transform.position + (player.transform.forward * 3f); // TODO: Use drop handler.
            }

            return true;
        }



        

        protected bool CheckItemPermission(NetworkIdentity identity, System.Guid itemGuid, ReadWritePermission minimalPermission, out INetworkItemInstance outItem)
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
        
        protected bool CheckCollectionPermission(NetworkIdentity identity, ICollection collection, ReadWritePermission minimalPermission)
        {
            var c = collection as IUNetCollection;
            if (c == null)
            {
                return false;
            }
            
            // Make sure the player has read/write access on the collection the item is in.
            var permission = UNetPermissionsRegistry.collections.GetPermission(c, identity);
            if (permission < minimalPermission)
            {
                return false;
            }

            return true;
        }
        
        protected bool CheckCollectionPermission(NetworkIdentity identity, System.Guid fromGuid, System.Guid toGuid, out ICollection fromOut, out ICollection toOut)
        {
            toOut = null;
            
            fromOut = UNetActionsBridge.collectionFinder.GetServerCollection(fromGuid) as ICollection;
            if (fromOut == null)
            {
                logger.Error($"[Server] Couldn't find collection with name {fromGuid} - netID: {identity.netId}");
                return false;
            }

            toOut = UNetActionsBridge.collectionFinder.GetServerCollection(toGuid) as ICollection;
            if (toOut == null)
            {
                logger.Error($"[Server] Couldn't find collection with name {toGuid} - netID: {identity.netId}");
                return false;
            }
            
            var fromAccess = UNetPermissionsRegistry.collections.GetPermission(fromOut as IUNetCollection, identity);
            var toAccess = UNetPermissionsRegistry.collections.GetPermission(toOut as IUNetCollection, identity);
            if (fromAccess < ReadWritePermission.ReadWrite || toAccess < ReadWritePermission.ReadWrite)
            {
                logger.Warning("[Server] Client doesn't have authority over collection(s)", this);
                return false;
            }

            return true;
        }
    }
}