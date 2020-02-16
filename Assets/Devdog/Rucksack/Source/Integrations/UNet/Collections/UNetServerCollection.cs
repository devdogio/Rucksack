using System;
using Devdog.Rucksack.Items;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public class UNetServerCollection<T> : UNetCollectionBase<T>
        where T: class, IItemInstance, IEquatable<T>
    {
        public UNetServerCollection(NetworkIdentity owner, int slotCount, ILogger logger = null)
            : base(owner, slotCount, logger)
        {

            OnSlotsChanged += NotifyOnSlotsChanged;
            OnSizeChanged += NotifyOnSizeChanged;
        }

        protected void NotifyOnSlotsChanged(object sender, CollectionSlotsChangedResult data)
        {
            if (owner.isServer)
            {
                var clients = UNetPermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
                foreach (var client in clients)
                {
                    var actionBridge = client.GetComponent<UNetActionsBridge>();
                    if (actionBridge != null)
                    {
                        logger.LogVerbose($"[Server] Notify client with NetID: {actionBridge.identity.netId} of changed itemGuid: {data.affectedSlots.ToSimpleString()} on collection: {collectionName}", this);
                
                        // TODO: Combine all affected slots in single call to client!
                        foreach (var slot in data.affectedSlots)
                        {
                            if (this[slot] != null)
                            {
                                // TODO: Remove this line - Client should request it by itself, or the server should check if the client already knows the item instance...
                                actionBridge.Server_TellClientToRegisterItemInstance(this[slot]);
                            }
                        
                            actionBridge.Server_SetSlotOnClient(new SlotDataMessage()
                            {
                                collectionGuid = ID,
                                index = (ushort) slot,
                                amount = (ushort) GetAmount(slot),
                                itemInstanceGuid = this[slot]?.ID ?? System.Guid.Empty
                            });
                        }
                    }
                }
            }
        }

        protected void NotifyOnSizeChanged(object sender, CollectionSizeChangedResult data)
        {
            // TODO... implement this...
        }
    }
}