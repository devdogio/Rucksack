using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections.CharacterEquipment;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public static class UNetCollectionUtility
    {
        private static readonly ILogger _logger;


        static UNetCollectionUtility()
        {
            _logger = new UnityLogger("[UNet][Collection] ");
        }

        public static UNetServerItemVendorCollection CreateServerVendorItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, NetworkIdentity owner)
        {
            var collection = new UNetServerItemVendorCollection(owner, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Server_Register();

            // The server's owner object always has read/write permission.
            UNetPermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);
            
            _logger.Log($"[Server] Created and registered vendor item collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }
        
        public static UNetClientCollection<IVendorProduct<IItemInstance>> CreateClientVendorItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, NetworkIdentity owner, UNetActionsBridge playerBridge)
        {
            var collection = new UNetClientCollection<IVendorProduct<IItemInstance>>(owner, playerBridge, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Register();
            
            // NOTE: Do not give the player read+write access to the vendor collection; That way the client could set items directly into the vendor collection.
            UNetPermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.Read);
            
            _logger.Log($"[Client] Created and registered vendor item collection with name {collectionName} and guid {collectionGuid} for netID: {owner.netId}", collection);
            return collection;
        }
        
        public static UNetServerCollection<IItemInstance> CreateServerItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, NetworkIdentity owner)
        {
            var collection = new UNetServerCollection<IItemInstance>(owner, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Server_Register();

            // The server's owner object always has read/write permission.
            UNetPermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);
            
            _logger.Log($"[Server] Created and registered collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }

        public static UNetClientCollection<IItemInstance> CreateClientItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, NetworkIdentity owner, UNetActionsBridge playerBridge)
        {
            var collection = new UNetClientCollection<IItemInstance>(owner, playerBridge, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Register();

            // Set permission for this client; If we own it it will be added to the registry list.
            UNetPermissionsRegistry.collections.SetPermission(collection, playerBridge.identity, ReadWritePermission.None);
            
            _logger.Log($"[Client] Created and registered collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }
        
        
        
        public static UNetServerEquipmentCollection<IEquippableItemInstance> CreateServerEquipmentCollection(
            string collectionName, 
            System.Guid collectionGuid, 
            NetworkIdentity owner, 
            EquipmentCollectionSlot<IEquippableItemInstance>[] slots,
            IEquippableCharacter<IEquippableItemInstance> character)
        {
            var collection = new UNetServerEquipmentCollection<IEquippableItemInstance>(owner, 0, character)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.slots = slots;
            for (int i = 0; i < collection.slots.Length; i++)
            {
                collection.slots[i].collection = collection;
                collection.slots[i].index = i;
            }

            collection.Server_Register();

            // The server's owner object always has read/write permission.
            UNetPermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);
            
            _logger.Log($"[Server] Created and registered equipment collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }
        
        public static UNetClientEquipmentCollection<IEquippableItemInstance> CreateClientEquipmentCollection(
            string collectionName, 
            System.Guid collectionGuid, 
            NetworkIdentity owner, 
            UNetActionsBridge playerBridge,
            EquipmentCollectionSlot<IEquippableItemInstance>[] slots,
            IEquippableCharacter<IEquippableItemInstance> character)
        {
            var collection = new UNetClientEquipmentCollection<IEquippableItemInstance>(owner, playerBridge, 0, character)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.slots = slots;
            for (int i = 0; i < collection.slots.Length; i++)
            {
                collection.slots[i].collection = collection;
                collection.slots[i].index = i;
            }

            collection.Register();

            // The server's owner object always has read/write permission.
            UNetPermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.None);
            
            _logger.Log($"[Client] Created and registered equipment collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }
    }
}