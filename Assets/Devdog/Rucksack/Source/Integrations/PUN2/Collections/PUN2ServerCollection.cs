using System;
using Devdog.Rucksack.Items;

using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public class PUN2ServerCollection<T> : PUN2CollectionBase<T>
        where T : class, IItemInstance, IEquatable<T>
    {
        public PUN2ServerCollection(PhotonView owner, int slotCount, ILogger logger = null)
            : base(owner, slotCount, logger)
        {

            OnSlotsChanged += NotifyOnSlotsChanged;
            OnSizeChanged += NotifyOnSizeChanged;
        }

        protected void NotifyOnSlotsChanged(object sender, CollectionSlotsChangedResult data)
        {
            if (PhotonNetwork.IsMasterClient /*owner.IsServer() /*owner.isServer*/)
            {
                var clients = PUN2PermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
                foreach (var client in clients)
                {
                    var actionBridge = client.GetComponent<PUN2ActionsBridge>();
                    if (actionBridge != null)
                    {
                        logger.LogVerbose($"[Server] Notify client with ViewID: {actionBridge.photonView.ViewID} of changed itemGuid: {data.affectedSlots.ToSimpleString()} on collection: {collectionName}", this);

                        // TODO: Combine all affected slots in single call to client!
                        foreach (var slot in data.affectedSlots)
                        {
                            if (this[slot] != null)
                            {
                                // TODO: Remove this line - Client should request it by itself, or the server should check if the client already knows the item instance...
                                actionBridge.Server_TellClientToRegisterItemInstance(this[slot]);
                            }

                            actionBridge.Server_SetSlotOnClient(collectionGuid: ID, index: (ushort)slot, amount: (ushort)GetAmount(slot), itemInstanceGuid: this[slot]?.ID ?? System.Guid.Empty);
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