using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public sealed class AddEquipmentCollectionMessage : MessageBase
    {
        public NetworkIdentity owner;
        public string collectionName;
        public GuidMessage collectionGuid;
        public EquipmentSlotDataMessage[] slots;
    }
}