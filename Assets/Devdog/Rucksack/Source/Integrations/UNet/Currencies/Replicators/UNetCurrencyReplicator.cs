using Devdog.Rucksack.Collections;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public class UNetCurrencyReplicator
    {
        protected readonly ILogger logger;
        protected readonly UNetActionsBridge bridge;

        protected static UNetCurrencyInputValidator inputValidator;
        static UNetCurrencyReplicator()
        {
            inputValidator = new UNetCurrencyInputValidator(new UnityLogger("[UNet][Validation] "));
        }
        
        public UNetCurrencyReplicator(UNetActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[UNet] ");
        }


        public void TargetRpc_AddCurrencyCollection(NetworkConnection target, AddCurrencyCollectionMessage data)
        {
            var col = UNetActionsBridge.collectionFinder.GetClientCurrencyCollection(data.collectionGuid);
            if (col == null)
            {
                col = UNetCurrencyCollectionUtility.CreateClientCurrencyCollection(data.collectionName, data.collectionGuid, data.owner, bridge);
            }
        }
        
        public void TargetRpc_SetCurrency(NetworkConnection target, CurrencyAmountMessage data)
        {
            var col = UNetActionsBridge.collectionFinder.GetClientCurrencyCollection(data.collectionGuid) as UNetClientCurrencyCollection;
            if (col != null)
            {
                var currencyGuid = data.currencyGuid;
                var currency = bridge.currencyDatabase.Get(new Identifier(currencyGuid));
                if (currency.error == null)
                {
                    col.Set(currency.result, data.amount);
                    logger.LogVerbose($"[Client] Server set currency {currency} to {data.amount}", bridge);
                }
                else
                {
                    logger.Warning("[Client] Requested currency ID not found in local database", bridge);
                }
            }
            else
            {
                logger.Warning($"[Client] Couldn't find currency collection with name {data.collectionGuid}", bridge);
            }
        }
    }
}