using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Items;
using Photon.Pun;

namespace Devdog.Rucksack.Collections.CharacterEquipment
{
    public sealed class PUN2ServerEquipmentCollection<TEquippableType> : EquipmentCollection<TEquippableType>, IPUN2Collection
        where TEquippableType : class, IItemInstance, IEquatable<TEquippableType>, IEquippable<TEquippableType>, ICollectionSlotEntry
    {
        public Guid ID { get; set; }
        public PhotonView owner { get; }

        public PUN2ServerEquipmentCollection(PhotonView owner, int slotCount, IEquippableCharacter<TEquippableType> character, ILogger logger = null)
            : base(slotCount, character, logger)
        {
            this.owner = owner;

            OnSlotsChanged += NotifyOnSlotsChanged;
            OnSizeChanged += NotifyOnSizeChanged;
        }

        public void Register()
        {
            // Ignored
        }

        public void Server_Register()
        {
            ServerCollectionRegistry.byID.Register(ID, this);
            ServerCollectionRegistry.byName.Register(collectionName, this);
        }

        public void UnRegister()
        {
            // Ignored
        }

        public void Server_UnRegister()
        {
            ServerCollectionRegistry.byID.UnRegister(ID);
            ServerCollectionRegistry.byName.UnRegister(collectionName);
        }


        // TODO: consider overwriting Add / Set methods for equipment -> Check if item we're trying to set / add is in a player collection.
        // TODO: Clients that join later need to get read permission to equipment collection of other clients and replicate the visually equipped items.

        public override Result<CollectionAddResult> Add(TEquippableType item, int amount, CollectionContext context)
        {
            var result = base.Add(item, amount, context);

            logger.Log($"Item {item?.ID} added to PUN2ServerEquipmentCollection with result {result}", this);

            return result;
        }

        public override Result<bool> Set(int index, TEquippableType item, int amount, CollectionContext context)
        {
            var result = base.Set(index, item, amount, context);

            logger.Log($"Item {item?.ID} Set on PUN2ServerEquipmentCollection with result {result}", this);

            return result;
        }

        private void NotifyOnSlotsChanged(object sender, CollectionSlotsChangedResult data)
        {
            if (PhotonNetwork.IsMasterClient /*owner.IsServer() /*owner.isServer*/)
            {
                var clients = PUN2PermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
                foreach (var client in clients)
                {
                    // TODO: Validate if there's at least read access

                    var actionBridge = client.GetComponent<PUN2ActionsBridge>();
                    if (actionBridge != null)
                    {
                        logger.LogVerbose($"[Server] Notify client with ViewID: {actionBridge.photonView.ViewID} of changed itemGuid: {data.affectedSlots.ToSimpleString()} on equipment collection: {collectionName}", this);

                        // TODO: Combine all affected slots in single call to client!
                        foreach (var slot in data.affectedSlots)
                        {
                            if (this[slot] != null)
                            {
                                // TODO: Remove this line - Client should request it by itself, or the server should check if the client already knows the item instance...
                                actionBridge.Server_TellClientToRegisterItemInstance(this[slot]);
                            }

                            actionBridge.Server_SetSlotOnClient(
                                collectionGuid: ID,
                                itemInstanceGuid: this[slot]?.ID ?? System.Guid.Empty,
                                index: slot,
                                amount: GetAmount(slot)
                            );
                        }
                    }
                }
            }
        }

        private void NotifyOnSizeChanged(object sender, CollectionSizeChangedResult data)
        {
            // TODO... implement this...
        }

        public override string ToString()
        {
            return collectionName;
        }
    }
}
