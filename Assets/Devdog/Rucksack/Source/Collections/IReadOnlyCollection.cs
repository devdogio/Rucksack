using System;

namespace Devdog.Rucksack.Collections
{
    public interface IReadOnlyCollection<out TSlotType, out TElementType> : IReadOnlyCollection<TElementType>
        where TElementType : IEquatable<TElementType>
    {
        TSlotType GetSlot(int index);
    }
    
    public interface IReadOnlyCollection<out TElementType> : IReadOnlyCollection, System.Collections.Generic.IEnumerable<TElementType>
        where TElementType : IEquatable<TElementType>
    {
        TElementType this[int key] { get; }
        
        /// <summary>
        /// Get the amount of all items that match the predicate.
        /// </summary>
        int GetAmount(Predicate<TElementType> predicate);

        int IndexOf(Predicate<TElementType> predicate);

        System.Collections.Generic.IEnumerable<int> IndexOfAll(Predicate<TElementType> predicate);

//        System.Collections.Generic.IEnumerable<TElementType> GetAll();
    }

    public interface IReadOnlyCollection
    {
        /// <summary>
        /// The number of slots in this collection.
        /// </summary>
        int slotCount { get; }
                
        /// <summary>
        /// Get the amount of items at a given index.
        /// </summary>
        /// <returns>Returns the amount (stack size) of items in the given slot.</returns>
        int GetAmount(int index);
    }
}