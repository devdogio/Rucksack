using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Items;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections.CharacterEquipment
{
    public class UNetServerEquipmentCollection<TEquippableType> : EquipmentCollection<TEquippableType>, IUNetCollection 
        where TEquippableType : class, IItemInstance, IEquatable<TEquippableType>, IEquippable<TEquippableType>, ICollectionSlotEntry
    {
        public Guid ID { get; set; }
        public NetworkIdentity owner { get; }
        
        public UNetServerEquipmentCollection(NetworkIdentity owner, int slotCount, IEquippableCharacter<TEquippableType> character, ILogger logger = null)
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
//            ServerCollectionRegistry.byName.Register(collectionName, this);
        }

        public void UnRegister()
        {
            // Ignored
        }

        public void Server_UnRegister()
        {
            ServerCollectionRegistry.byID.UnRegister(ID);
//            ServerCollectionRegistry.byName.UnRegister(collectionName);
        }
        
        
        // TODO: consider overwriting Add / Set methods for equipment -> Check if item we're trying to set / add is in a player collection.
        // TODO: Clients that join later need to get read permission to equipment collection of other clients and replicate the visually equipped items.
        
        
        protected void NotifyOnSlotsChanged(object sender, CollectionSlotsChangedResult data)
        {
            if (owner.isServer)
            {
                var clients = UNetPermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
                foreach (var client in clients)
                {
                    // TODO: Validate if there's at least read access
                    
                    var actionBridge = client.GetComponent<UNetActionsBridge>();
                    if (actionBridge != null)
                    {
                        logger.LogVerbose($"[Server] Notify client with NetID: {actionBridge.identity.netId} of changed itemGuid: {data.affectedSlots.ToSimpleString()} on equipment collection: {collectionName}", this);
                
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
        
        public override string ToString()
        {
            return collectionName;
        }
    }
}