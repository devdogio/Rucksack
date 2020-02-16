using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;

namespace Devdog.Rucksack.Currencies
{
    public sealed class PUN2CurrencyReplicator
    {
        private readonly ILogger logger;
        private readonly PUN2ActionsBridge bridge;

        private static PUN2CurrencyInputValidator inputValidator;
        static PUN2CurrencyReplicator()
        {
            inputValidator = new PUN2CurrencyInputValidator(new UnityLogger("[PUN2][Validation] "));
        }

        public PUN2CurrencyReplicator(PUN2ActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[PUN2] ");
        }

        public void TargetRpc_AddCurrencyCollection(PhotonView owner, string collectionName, Guid collectionGuid)
        {
            var col = PUN2ActionsBridge.collectionFinder.GetClientCurrencyCollection(collectionGuid);
            if (col == null)
            {
                col = PUN2CurrencyCollectionUtility.CreateClientCurrencyCollection(collectionName, collectionGuid, owner, bridge);
            }
        }

        public void TargetRpc_SetCurrency(PhotonView target, Guid collectionGuid, Guid currencyGuid, double amount)
        {
            var col = PUN2ActionsBridge.collectionFinder.GetClientCurrencyCollection(collectionGuid) as PUN2ClientCurrencyCollection;
            if (col != null)
            {
                var currency = bridge.currencyDatabase.Get(new Identifier(currencyGuid));
                if (currency.error == null)
                {
                    col.Set(currency.result, amount);
                    logger.LogVerbose($"[Client] Server set currency {currency} to {amount}", bridge);
                }
                else
                {
                    logger.Warning("[Client] Requested currency ID not found in local database", bridge);
                }
            }
            else
            {
                logger.Warning($"[Client] Couldn't find currency collection with name {collectionGuid}", bridge);
            }
        }
    }
}
