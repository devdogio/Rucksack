using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class AddCollectionMessage : MessageBase
    {
        public NetworkIdentity owner;
        public string collectionName;
        public GuidMessage collectionGuid;
        public ushort slotCount;
    }
}