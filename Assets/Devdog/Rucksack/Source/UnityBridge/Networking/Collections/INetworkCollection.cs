namespace Devdog.Rucksack.Collections
{
    public interface INetworkCollection<out TIdentity> : INetworkCollection, INetworkObject<TIdentity>
    {
//        TIdentity owner { get; }
    }

    public interface INetworkCollection : IIdentifiable
    {
        string collectionName { get; }
//        System.Guid collectionGuid { get; }
    }
}