using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.Currencies
{
    public static class ServerCurrencyCollectionRegistry
    {
        private static CollectionRegistry.Helper<System.Guid, ICurrencyCollection> _idCols = new CollectionRegistry.Helper<System.Guid, ICurrencyCollection>();
        //private static CurrencyCollectionRegistry.CollectionRegisteryHelper<string, ICurrencyCollection> _nameCols = new CurrencyCollectionRegistry.CollectionRegisteryHelper<string, ICurrencyCollection>();

        public static CollectionRegistry.Helper<System.Guid, ICurrencyCollection> byID
        {
            get { return _idCols; }
        }

        //public static CurrencyCollectionRegistry.CollectionRegisteryHelper<string, ICurrencyCollection> byName
        //{
        //    get { return _nameCols; }
        //}
    }
}