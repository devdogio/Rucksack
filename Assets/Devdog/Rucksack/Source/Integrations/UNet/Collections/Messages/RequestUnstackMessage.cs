using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestUnstackMessage : MessageBase
    {
        public GuidMessage collectionGuid;
        public GuidMessage toCollectionGuid;
        public ushort fromIndex;
        public ushort toIndex;
        public ushort amount;
    }
}