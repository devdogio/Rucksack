using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class ItemUsedMessage : MessageBase
    {
        public GuidMessage itemID;
        public ushort amountUsed;
        public short targetIndex;
    }
}