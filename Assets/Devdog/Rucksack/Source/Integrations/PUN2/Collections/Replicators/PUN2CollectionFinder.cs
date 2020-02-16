using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Collections
{
    public class PUN2CollectionFinder
    {
        public IPUN2Collection GetServerCurrencyCollection(System.Guid guid)
        {
            return ServerCurrencyCollectionRegistry.byID.Get(guid) as PUN2ServerCurrencyCollection;
        }

        public IPUN2Collection GetClientCurrencyCollection(System.Guid guid)
        {
            return CurrencyCollectionRegistry.byID.Get(guid) as PUN2ClientCurrencyCollection;
        }

        public IPUN2Collection GetServerCollection(System.Guid guid)
        {
            return ServerCollectionRegistry.byID.Get(guid) as IPUN2Collection;
        }

        public IPUN2Collection GetClientCollection(System.Guid guid)
        {
            return CollectionRegistry.byID.Get(guid) as IPUN2Collection;
        }
    }
}