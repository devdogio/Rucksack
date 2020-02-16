using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public sealed class AddCurrencyCollectionMessage : MessageBase
    {
        /// <summary>
        /// The owner of this collection.
        /// </summary>
        public NetworkIdentity owner;
        
        public string collectionName;
        public GuidMessage collectionGuid;
    }
}