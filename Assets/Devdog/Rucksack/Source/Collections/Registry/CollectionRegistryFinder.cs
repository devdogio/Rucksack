using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionRegistryFinder<TCollectionType>
        where TCollectionType : class
    {
        public event Action<TCollectionType, TCollectionType> OnCollectionChanged;
        
        public string collectionName { get; }

        private TCollectionType _collection;
        public TCollectionType collection
        {
            get { return _collection; }
            set
            {
                var before = _collection;
                _collection = value;

                if (before != _collection)
                {
                    OnCollectionChanged?.Invoke(before, _collection);
                }
            }
        }

        private ILogger _logger;
        public CollectionRegistryFinder(string collectionName, ILogger logger = null)
        {
            _logger = logger ?? new Logger("[Collection] ");
            this.collectionName = collectionName;
            
            collection = FindCollection();
            
            CollectionRegistry.byName.OnAddedItem += OnLocalCollectionAdded;
            CollectionRegistry.byName.OnRemovedItem += OnLocalCollectionRemoved;
        }

        private void OnLocalCollectionAdded(string key, ICollection col)
        {
            if (collectionName == key)
            {
                collection = FindCollection();
            }
        }
        
        private void OnLocalCollectionRemoved(string key, ICollection col)
        {
            if (collectionName == key)
            {
                collection = FindCollection();
            }
        }

        private TCollectionType FindCollection()
        {
            var col = CollectionRegistry.byName.Get(collectionName) as TCollectionType;
            if (col != null)
            {          
                _logger.LogVerbose($"Local collection found with name: {collectionName} and type {col.GetType().Name}", this);
                return col;
            }

            _logger.Warning($"Collection with name {collectionName} not found!", this);
            return null;
        }
    }
}