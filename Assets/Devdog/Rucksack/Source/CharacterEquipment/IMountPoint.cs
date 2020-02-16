namespace Devdog.Rucksack.CharacterEquipment
{
    public interface IMountPoint<in T>
    {
        string name { get; set; }
        bool hasMountedItem { get; }
        
        /// <summary>
        /// Mount an item to this mount point.
        /// </summary>
        void Mount(T item);

        /// <summary>
        /// Clear any items that might've been mounted.
        /// </summary>
        void Clear();
    }
}