namespace Devdog.Rucksack.Collections
{
    public static class ServerCollectionRegistry
    {
        private static CollectionRegistry.Helper<System.Guid, ICollection> _idCols = new CollectionRegistry.Helper<System.Guid, ICollection>();
        private static CollectionRegistry.Helper<string, ICollection> _nameCols = new CollectionRegistry.Helper<string, ICollection>();

        public static CollectionRegistry.Helper<System.Guid, ICollection> byID
        {
            get { return _idCols; }
        }

        public static CollectionRegistry.Helper<string, ICollection> byName
        {
            get { return _nameCols; }
        }
    }
}