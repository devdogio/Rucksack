using System;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Database;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Vendors
{
    [System.Serializable]
    public class CurrencyDecoratorMessage : MessageBase
    {
        public float amount;
        public GuidMessage currencyGuid;

        public CurrencyDecoratorMessage()
        {
            
        }

        public CurrencyDecoratorMessage(CurrencyDecorator<double> dec)
        {
            amount = (float)dec.amount;
            currencyGuid = dec.currency?.ID ?? Guid.Empty;
        }
        
        public CurrencyDecorator<double> ToDecorator(IDatabase<UnityCurrency> database)
        {
            var currency = database.Get(new Identifier(currencyGuid.guid));
            return new CurrencyDecorator(currency.result, amount);
        }
    }
}