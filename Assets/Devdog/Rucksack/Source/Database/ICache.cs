namespace Devdog.Rucksack.Database
{
    public interface ICache<T>
    {
        bool Contains(IIdentifiable identifier);

        T Get(IIdentifiable identifier, T defaultValue = default(T));
        void Set(IIdentifiable identifier, T item);

        void Remove(IIdentifiable identifier);
    }
}