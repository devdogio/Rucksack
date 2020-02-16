using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections.CharacterEquipment;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;

using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public static class PUN2CollectionUtility
    {
        private static readonly ILogger _logger;

        static PUN2CollectionUtility()
        {
            _logger = new UnityLogger("[PUN2][Collection] ");
        }

        public static PUN2ServerCollection<IItemInstance> CreateServerItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, PhotonView owner)
        {
            var collection = new PUN2ServerCollection<IItemInstance>(owner, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Server_Register();

            // The server's owner object always has read/write permission.
            PUN2PermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);

            _logger.Log($"[Server] Created and registered collection with name '{collection.collectionName}' and guid '{collection.ID}' for ViewID: {owner.ViewID}", collection);
            return collection;
        }

        public static PUN2ClientCollection<IItemInstance> CreateClientItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, PhotonView owner, PUN2ActionsBridge playerBridge)
        {
            var collection = new PUN2ClientCollection<IItemInstance>(owner, playerBridge, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Register();

            // Set permission for this client; If we own it it will be added to the registry list.
            PUN2PermissionsRegistry.collections.SetPermission(collection, playerBridge.photonView, ReadWritePermission.None);

            _logger.Log($"[Client] Created and registered collection with name '{collection.collectionName}' and guid '{collection.ID}' for ViewID: {owner.ViewID}", collection);
            return collection;
        }

        public static PUN2ServerEquipmentCollection<IEquippableItemInstance> CreateServerEquipmentCollection(
            string collectionName,
            System.Guid collectionGuid,
            PhotonView owner,
            EquipmentCollectionSlot<IEquippableItemInstance>[] slots,
            IEquippableCharacter<IEquippableItemInstance> character)
        {
            var collection = new PUN2ServerEquipmentCollection<IEquippableItemInstance>(owner, 0, character)
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
            PUN2PermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);

            _logger.Log($"[Server] Created and registered equipment collection with name '{collection.collectionName}' and guid '{collection.ID}' for ViewID: {owner.ViewID}", collection);
            return collection;
        }

        public static PUN2ClientEquipmentCollection<IEquippableItemInstance> CreateClientEquipmentCollection(
                    string collectionName,
                    System.Guid collectionGuid,
                    PhotonView owner,
                    PUN2ActionsBridge playerBridge,
                    EquipmentCollectionSlot<IEquippableItemInstance>[] slots,
                    IEquippableCharacter<IEquippableItemInstance> character)
        {
            var collection = new PUN2ClientEquipmentCollection<IEquippableItemInstance>(owner, playerBridge, 0, character)
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
            PUN2PermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.None);

            _logger.Log($"[Client] Created and registered equipment collection with name '{collection.collectionName}' and guid '{collection.ID}' for ViewID: {owner.ViewID}", collection);

            return collection;
        }

        public static PUN2ServerItemVendorCollection CreateServerVendorItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, PhotonView owner)
        {
            var collection = new PUN2ServerItemVendorCollection(owner, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Server_Register();

            // The server's owner object always has read/write permission.
            PUN2PermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);

            _logger.Log($"[Server] Created and registered vendor item collection with name '{collection.collectionName}' and guid '{collection.ID}' for ViewID: {owner.ViewID}", collection);
            return collection;
        }

        public static PUN2ClientCollection<IVendorProduct<IItemInstance>> CreateClientVendorItemCollection(int slotCount, string collectionName, System.Guid collectionGuid, PhotonView owner, PUN2ActionsBridge playerBridge)
        {
            var collection = new PUN2ClientCollection<IVendorProduct<IItemInstance>>(owner, playerBridge, slotCount)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Register();

            // NOTE: Do not give the player read+write access to the vendor collection; That way the client could set items directly into the vendor collection.
            PUN2PermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.Read);

            _logger.Log($"[Client] Created and registered vendor item collection with name '{collectionName}' and guid '{collectionGuid}' for ViewID: {owner.ViewID}", collection);
            return collection;
        }
    }
}
