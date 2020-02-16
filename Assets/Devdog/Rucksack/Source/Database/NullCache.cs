namespace Devdog.Rucksack.Database
{
    public sealed class NullCache<T> : ICache<T>
    {
//        private readonly ILogger _logger;
        public NullCache(ILogger logger)
        {
//            _logger = logger;
        }
        
        public bool Contains(IIdentifiable identifier)
        {
            return false;
        }

        public T Get(IIdentifiable identifier, T defaultValue = default(T))
        {
            return defaultValue;
        }

        public void Set(IIdentifiable identifier, T item)
        {
//            _logger.Warning("Trying to set value in null cache. Using: " + GetType().Name);
        }

        public void Remove(IIdentifiable identifier)
        {
//            _logger.Warning("Trying to remove value from null cache. Using: " + GetType().Name);
        }
    }
}