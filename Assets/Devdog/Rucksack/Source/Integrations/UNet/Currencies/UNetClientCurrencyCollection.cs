using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public class UNetClientCurrencyCollection : UNetCurrencyCollectionBase
    {
        protected UNetActionsBridge actionsBridge;
        
        public UNetClientCurrencyCollection(NetworkIdentity owner, UNetActionsBridge actionsBridge, ILogger logger = null)
            : base(owner, logger)
        {
            this.actionsBridge = actionsBridge;
        }
    }
}