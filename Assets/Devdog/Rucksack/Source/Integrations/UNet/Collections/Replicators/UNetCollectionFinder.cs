using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Collections
{
    public class UNetCollectionFinder
    {
        public IUNetCollection GetServerCurrencyCollection(System.Guid guid)
        {
            return ServerCurrencyCollectionRegistry.byID.Get(guid) as UNetServerCurrencyCollection;
        }

        public IUNetCollection GetClientCurrencyCollection(System.Guid guid)
        {
            return CurrencyCollectionRegistry.byID.Get(guid) as UNetClientCurrencyCollection;
        }

        public IUNetCollection GetServerCollection(System.Guid guid)
        {
            return ServerCollectionRegistry.byID.Get(guid) as IUNetCollection;
        }

        public IUNetCollection GetClientCollection(System.Guid guid)
        {
            return CollectionRegistry.byID.Get(guid) as IUNetCollection;
        }
    }
}