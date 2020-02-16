using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestSwapOrMergeMessage : MessageBase
    {

        public GuidMessage collectionGuid;
        public GuidMessage toCollectionGuid;
        public ushort fromIndex;
        public ushort toIndex;
        public short amount = -1;

    }
}