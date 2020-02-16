using Devdog.Rucksack.Collections;

using Photon.Pun;

namespace Devdog.Rucksack.Currencies
{
    public static class PUN2CurrencyCollectionUtility
    {
        private static ILogger _logger;

        static PUN2CurrencyCollectionUtility()
        {
            _logger = new UnityLogger("[PUN2][Collection] ");
        }

        public static PUN2ServerCurrencyCollection CreateServerCurrencyCollection(string collectionName, System.Guid collectionGuid, PhotonView owner)
        {
            var collection = new PUN2ServerCurrencyCollection(owner, _logger)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Server_Register();

            // The server's owner object always has read/write permission.
            PUN2PermissionsRegistry.collections.SetPermission(collection, owner, ReadWritePermission.ReadWrite);

            _logger.Log($"[Server] Created and registered currency collection with name {collection.collectionName} and guid {collection.ID} for ViewID: {owner.ViewID}", collection);
            return collection;
        }

        public static PUN2ClientCurrencyCollection CreateClientCurrencyCollection(string collectionName, System.Guid collectionGuid, PhotonView owner, PUN2ActionsBridge playerBridge)
        {
            var collection = new PUN2ClientCurrencyCollection(owner, playerBridge, _logger)
            {
                collectionName = collectionName,
                ID = collectionGuid
            };

            collection.Register();

            // Set permission for this client; If we own it it will be added to the registry list.
            PUN2PermissionsRegistry.collections.SetPermission(collection, playerBridge.photonView, ReadWritePermission.None);

            _logger.Log($"[Client] Created and registered currency collection with name {collection.collectionName} and guid {collection.ID} for ViewID: {owner.ViewID}", collection);
            return collection;
        }
    }
}