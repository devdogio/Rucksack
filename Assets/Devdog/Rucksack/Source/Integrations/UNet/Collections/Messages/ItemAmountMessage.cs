using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public class ItemAmountMessage : MessageBase
    {
        public RegisterItemInstanceMessage itemInstance;
        public ushort amount;
    }
}