namespace Devdog.Rucksack.Currencies
{
    public interface ICurrencyCollectionPriority<in T>
        where T : IIdentifiable
    {
        /// <summary>
        /// The priority used for general actions such as Contains and IndexOf.
        /// </summary>
        int GetGeneralPriority();
        
        /// <summary>
        /// The priority used when adding items.
        /// </summary>
        int GetAddPriority(T item);

        /// <summary>
        /// The priority used when removing items.
        /// </summary>
        int GetRemovePriority(T item);
    }
}
