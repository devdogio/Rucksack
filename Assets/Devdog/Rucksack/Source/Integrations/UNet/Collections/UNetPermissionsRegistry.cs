using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public static class UNetPermissionsRegistry
    {
        public static readonly NetworkPermissionsMap<IUNetCollection, NetworkIdentity> collections = new NetworkPermissionsMap<IUNetCollection, NetworkIdentity>();
        public static readonly NetworkPermissionsMap<INetworkObject<NetworkIdentity>, NetworkIdentity> objects = new NetworkPermissionsMap<INetworkObject<NetworkIdentity>, NetworkIdentity>();

//        public static readonly NetworkCollectionPermissions<TriggerBase, NetworkIdentity> triggers = new NetworkCollectionPermissions<TriggerBase, NetworkIdentity>();
//        public static readonly NetworkCollectionPermissions<IUNetCollection, NetworkIdentity> currencyCollections = new NetworkCollectionPermissions<IUNetCollection, NetworkIdentity>();
    }
}