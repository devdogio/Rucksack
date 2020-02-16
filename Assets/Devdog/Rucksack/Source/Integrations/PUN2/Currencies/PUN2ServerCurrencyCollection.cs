using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.Rucksack.Collections;
using Photon.Pun;

namespace Devdog.Rucksack.Currencies
{
    public sealed class PUN2ServerCurrencyCollection : PUN2CurrencyCollectionBase
    {
        public PUN2ServerCurrencyCollection(PhotonView owner, ILogger logger = null)
            : base(owner, logger)
        {
            OnCurrencyChanged += NotifyOnCurrencyChanged;
        }

        private void NotifyOnCurrencyChanged(object sender, CurrencyChangedResult<double> currencyChangedResult)
        {
            if (PhotonNetwork.IsMasterClient /*base.owner.IsServer() /*owner.isServer*/)
            {
                var clients = PUN2PermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
                foreach (var client in clients)
                {
                    var actionBridge = client.GetComponent<PUN2ActionsBridge>();
                    if (actionBridge != null)
                    {
                        logger.LogVerbose($"[Server] PUN2ServerCurrencyCollection - Notify client with ViewID: {actionBridge.photonView.ViewID} of changed currency {currencyChangedResult.currency} on collection: {collectionName}", this);

                        actionBridge.Server_SetCurrencyOnClient(
                            collectionGuid: ID,
                            currencyGuid: currencyChangedResult.currency.ID,
                            amount: currencyChangedResult.amountAfter
                        );
                    }
                }
            }
        }
    }
}
