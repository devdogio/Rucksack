namespace Devdog.Rucksack.Collections
{
    public interface ICollectionEntry
    {
        ICollection collection { get; }
        int index { get; }
        int amount { get; }
        
        /// <summary>
        /// Should this slot write entry data for the item that it stores. 
        /// When writing the entry data the item stored in this collection is mutated, which can cause strange behaviors when trying to simulate behavior.
        /// Marking setCollectionEntry to false will not write entry data, and thus, the item stored in the slot is never affected for being stored in it.
        /// <remarks>When false the item will not receive it's entry data, and thus, won't know in which collection it is!</remarks>
        /// </summary>
        bool setCollectionEntry { get; set; }

        /// <summary>
        /// Is the slot occupied in any way? 
        /// This could be occuped when it contains an item, is blocked by another item, or is blocked by a layout item.
        /// <remarks>The field could be empty, but still be occupied.</remarks>
        /// </summary>
        bool isOccupied { get; }
        
        Result<bool> CanSetAmountAndUpdateCollection(int setAmount);
        Result<bool> SetAmountAndUpdateCollection(int setAmount);
        void Clear();
    }
}