using System;
using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public static class PUN2PermissionsRegistry
    {
        public static readonly NetworkPermissionsMap<IPUN2Collection, PhotonView> collections = new NetworkPermissionsMap<IPUN2Collection, PhotonView>();
        public static readonly NetworkPermissionsMap<INetworkObject<PhotonView>, PhotonView> objects = new NetworkPermissionsMap<INetworkObject<PhotonView>, PhotonView>();

        //public static readonly NetworkPermissionsMap<IUNetCollection, NetworkIdentity> collections = new NetworkPermissionsMap<IUNetCollection, NetworkIdentity>();
        //public static readonly NetworkPermissionsMap<INetworkObject<NetworkIdentity>, NetworkIdentity> objects = new NetworkPermissionsMap<INetworkObject<NetworkIdentity>, NetworkIdentity>();

        //        public static readonly NetworkCollectionPermissions<TriggerBase, NetworkIdentity> triggers = new NetworkCollectionPermissions<TriggerBase, NetworkIdentity>();
        //        public static readonly NetworkCollectionPermissions<IUNetCollection, NetworkIdentity> currencyCollections = new NetworkCollectionPermissions<IUNetCollection, NetworkIdentity>();
    }
}
