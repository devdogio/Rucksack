using UnityEngine;

namespace Devdog.Rucksack.Currencies
{
    public sealed class CurrencyCollectionHelper : MonoBehaviour
    {
        [SerializeField]
        private SerializedGuid _collectionGuid;

        [SerializeField]
        private UnityCurrency _currencyDef;
        
        private readonly ILogger _logger;
        public CurrencyCollectionHelper()
        {
            _logger = new UnityLogger("[Collection][Helper] ");
        }

        private ICurrencyCollection<ICurrency, double> GetCollection()
        {
           var collection = ServerCurrencyCollectionRegistry.byID.Get(_collectionGuid.guid) as ICurrencyCollection<ICurrency, double>;
           if (collection != null)
           {
               return collection;
           }
            
            collection = CurrencyCollectionRegistry.byID.Get(_collectionGuid.guid) as ICurrencyCollection<ICurrency, double>;
            if (collection != null)
            {
                return collection;
            }
            
            _logger.Warning($"Couldn't find collection with name {_collectionGuid.guid}", this);
            return null;
        }
        
        public void AddGold()
        {
            var collection = GetCollection();
            var added = collection.Add(_currencyDef, 100f);
            if (added.error != null)
            {
                _logger.Error("Failed to add item to collection", added.error, this);
            }
            else
            {
                _logger.LogVerbose($"Added 100 {_currencyDef} to collection {_collectionGuid.guid}");
            }
        }
    }
}