using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public sealed class ItemEquippedMessage : MessageBase
    {
        public GuidMessage itemDefinitionID;
        public string mountPoint;
    }
}