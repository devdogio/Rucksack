using Devdog.Rucksack.Collections;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public class UNetServerCurrencyCollection : UNetCurrencyCollectionBase
    {
        public UNetServerCurrencyCollection(NetworkIdentity owner, ILogger logger = null)
            : base(owner, logger)
        {
            OnCurrencyChanged += NotifyOnCurrencyChanged;
        }

        private void NotifyOnCurrencyChanged(object sender, CurrencyChangedResult<double> currencyChangedResult)
        {
            if (owner.isServer)
            {
                var clients = UNetPermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
                foreach (var client in clients)
                {
                    var actionBridge = client.GetComponent<UNetActionsBridge>();
                    if (actionBridge != null)
                    {
                        logger.LogVerbose($"[Server] Notify client with NetID: {actionBridge.identity.netId} of changed currency {currencyChangedResult.currency} on collection: {collectionName}", this);
                
                        actionBridge.Server_SetCurrencyOnClient(new CurrencyAmountMessage()
                        {
                            collectionGuid = ID,
                            currencyGuid = currencyChangedResult.currency.ID,
                            amount = currencyChangedResult.amountAfter,
                        });
                    }
                }
            }
        }
    }
}