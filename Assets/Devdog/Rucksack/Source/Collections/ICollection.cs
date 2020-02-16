using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Collections
{
    public interface ICollection<out TSlotType, TElementType> : ICollection<TElementType>, IReadOnlyCollection<TSlotType, TElementType>
        where TSlotType : CollectionSlot<TElementType>
        where TElementType : class, IEquatable<TElementType>, IStackable
    { }
    
    public interface ICollection<TElementType> :  IReadOnlyCollection<TElementType>, ICollection, ICloneable
        where TElementType : IEquatable<TElementType>
    {
        event EventHandler<CollectionAddResult> OnAddedItem;
        event EventHandler<CollectionRemoveResult<TElementType>> OnRemovedItem;
        event EventHandler<CollectionSlotsChangedResult> OnSlotsChanged;
        event EventHandler<CollectionSizeChangedResult> OnSizeChanged;
        
        
        Result<bool> CanAdd(TElementType item, int amount = 1);
        Result<CollectionAddResult> Add(TElementType item, int amount = 1);

        Result<bool> CanRemove(TElementType item, int amount = 1);
        Result<CollectionRemoveResult<TElementType>> Remove(TElementType item, int amount = 1);
        

        /// <summary>
        /// Check if the item can be set in this slot.
        /// This will check restrictions and add / remove actions.
        /// </summary>
        /// <remarks>
        /// This also verifies if the item is already present in the collection in another slot. 
        /// If the exact same item already exists in this collection this method will return an error.
        /// </remarks>
        Result<bool> CanSet(int index, TElementType item, int amount);
        
        /// <summary>
        /// Set an item at a given location.
        /// </summary>
        /// <remarks>Note that if the given index already has an item it will be overwritten.</remarks>
        /// <remarks>If the object is blocked by another object (only possible if the item size exceeds 1x1), false will be returned and the object won't be placed.</remarks>
        /// <returns>True if the object has been inserted. False if the object could not be inserted.</returns>
        Result<bool> Set(int index, TElementType item, int amount);
        
        
        /// <summary>
        /// Get the moveAmount of items we have.
        /// This will sum up all the items in the collection and give you the total moveAmount of this item.
        /// </summary>
        int GetAmount(TElementType item);
        
        /// <summary>
        /// Contains this item or an item equal to this.
        /// </summary>
        bool Contains(TElementType item);
        
        /// <summary>
        /// Get the index of an item or an item that is equatable to 'item'.
        /// </summary>
        /// <returns>Returns the index or -1 if the item or an equatable item is not found.</returns>
        int IndexOf(TElementType item);
        
        IEnumerable<int> IndexOfAll(TElementType item);
        
        /// <summary>
        /// Get the moveAmount of {item} that can be added to this collection.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bailEarly">If you only want to know if you can store 20 items you can bail early at 20; Once 20 items can be placed this method will return straight away.
        /// This is faster then letting the method complete fully.</param>
        Result<int> GetCanAddAmount(TElementType item, int bailEarly = int.MaxValue);
        
        
        /// <summary>
        /// Sort this collection's contents based on the IComparer{T} <see cref="IComparer{T}"/>
        /// </summary>
        /// <param name="comparer"></param>
        void Sort(IComparer<TElementType> comparer);
    }

    public interface ICollection : IReadOnlyCollection
    {
        object GetBoxed(int index);


        Result<bool> CanAddBoxed(object item, int amount = 1, CollectionContext context = null);
        Result<bool> AddBoxed(object item, int amount = 1, CollectionContext context = null);
        
        Result<bool> CanRemoveBoxed(object item, int amount = 1, CollectionContext context = null);
        Result<CollectionRemoveResult<object>> RemoveBoxed(object item, int amount = 1, CollectionContext context = null);
        
        /// <summary>
        ///
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when TTargetType is not convertable to collection element type.</exception>
        Result<bool> CanSetBoxed(int index, object item, int amount = 1, CollectionContext context = null);
        
        /// <summary>
        /// Set an item at a given location.
        /// </summary>
        /// <remarks>Note that if the given index already has an item it will be overwritten.</remarks>
        /// <exception cref="ArgumentException">Thrown when TTargetType is not convertable to collection element type.</exception>
        /// <returns>True if the object has been inserted. False if the object could not be inserted.</returns>
        Result<bool> SetBoxed(int index, object item, int amount = 1, CollectionContext context = null);

        
        void ForceSetBoxed(int index, object item, int amount);
        
        /// <summary>
        /// Force set the items in this collection
        /// <remarks>
        /// Note: ForceSet doesn't check for restrictions or validations. It simply sets the given items
        /// NOTE: ForceSet doesn't do any validations, restrictions, etc. It only sets the items and fires events, that's it.
        /// </remarks>
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when TTargetType is not convertable to collection element type.</exception>
        /// <typeparam name="TTargetType">The item type we're trying to set. This type has to be convertible to the collection's element type.</typeparam>
        void ForceSetBoxed<TTargetType>(Tuple<TTargetType, int>[] items) where TTargetType : class, IEquatable<TTargetType>;




        Result<bool> SwapOrMerge(int fromIndex, ICollection toCol, int toIndex, int mergeAmount, CollectionContext context = null);

        /// <summary>
        /// Move the contents of one slot to another slot (auto picked target slot)
        /// <remarks>Will split the stack if needed to fit it in the <paramref name="toCol"/></remarks>
        /// </summary>
        /// <param name="fromIndex">The index of the slot we want to move</param>
        /// <param name="toCol">The collection we want to move the item to. Note that, if the target collection can not accept the item it will be ignored.</param>
        /// <param name="amount">The amount of items you want to move to the new slot. Entering something less than the stack size results in splitting the stack.</param>
        /// <param name="context"></param>
        Result<bool> MoveAuto(int fromIndex, ICollection toCol, int amount, CollectionContext context = null);
        

        
        /// <summary>
        /// Resize the collection to a specified size.
        /// <remarks>Note that this can remove slots and thus destroy items in the procress!</remarks>
        /// </summary>
        /// <param name="newSize">The new size of the collection.</param>
        void Resize(int newSize);
        
        /// <summary>
        /// Clear the entire collection of items.
        /// </summary>
        void Clear();
    }
}