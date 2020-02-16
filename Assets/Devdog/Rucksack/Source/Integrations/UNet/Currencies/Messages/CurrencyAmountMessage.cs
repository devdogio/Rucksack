using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public sealed class CurrencyAmountMessage : MessageBase
    {
        public GuidMessage collectionGuid;
        public GuidMessage currencyGuid;
        public double amount;
    }
}