using Devdog.Rucksack.Collections;
using System.Collections.Generic;

namespace Devdog.Rucksack.Currencies
{
    public static class CurrencyCollectionRegistry
    {
        private static CollectionRegistry.Helper<System.Guid, ICurrencyCollection> _idCols = new CollectionRegistry.Helper<System.Guid, ICurrencyCollection>();
        private static CollectionRegistry.Helper<string, ICurrencyCollection> _nameCols = new CollectionRegistry.Helper<string, ICurrencyCollection>();

        public static CollectionRegistry.Helper<System.Guid, ICurrencyCollection> byID
        {
            get { return _idCols; }
        }

        public static CollectionRegistry.Helper<string, ICurrencyCollection> byName
        {
            get { return _nameCols; }
        }
    }
}