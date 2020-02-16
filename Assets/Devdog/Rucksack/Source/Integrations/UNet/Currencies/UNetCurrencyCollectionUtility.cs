using Devdog.Rucksack.Collections;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public static class UNetCurrencyCollectionUtility
    {
        private static ILogger _logger;

        static UNetCurrencyCollectionUtility()
        {
            _logger = new UnityLogger("[UNet][Collection] ");
        }

        public static UNetServerCurrencyCollection CreateServerCurrencyCollection(string collectionName, System.Guid collectionGuid, NetworkIdentity owner)
        {
            var collection = new UNetServerCurrencyCollection(owner, _logger)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Server_Register();
            
            // The server's owner object always has read/write permission.
            UNetPermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);
                
            _logger.Log($"[Server] Created and registered currency collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }

        public static UNetClientCurrencyCollection CreateClientCurrencyCollection(string collectionName, System.Guid collectionGuid, NetworkIdentity owner, UNetActionsBridge playerBridge)
        {
            var collection = new UNetClientCurrencyCollection(owner, playerBridge, _logger)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Register();

            // Set permission for this client; If we own it it will be added to the registry list.
            UNetPermissionsRegistry.collections.SetPermission(collection, playerBridge.identity, ReadWritePermission.None);
                
            _logger.Log($"[Client] Created and registered currency collection with name {collection.collectionName} and guid {collection.ID} for netID: {owner.netId}", collection);
            return collection;
        }
    }
}