using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public sealed class RequestUnEquipItemMessage : MessageBase
    {
        public GuidMessage collectionGuid;
        public GuidMessage itemGuid;
    }
}