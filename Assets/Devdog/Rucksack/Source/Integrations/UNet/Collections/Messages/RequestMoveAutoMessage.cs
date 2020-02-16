using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestMoveAutoMessage : MessageBase
    {

        public GuidMessage collectionGuid;
        public GuidMessage toCollectionGuid;
        public ushort fromIndex;
        public ushort amount;

    }
}