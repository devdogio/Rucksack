namespace Devdog.Rucksack
{
    public interface INetworkObject<out TIdentity> : IIdentifiable
    {
        TIdentity owner { get; }
    }
}