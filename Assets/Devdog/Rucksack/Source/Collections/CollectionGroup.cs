using System;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionGroup<T> : CollectionGroup<T, ICollection<T>>
        where T : IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        public CollectionGroup(params ICollection<T>[] collections)
            : base(collections)
        { }

        public CollectionGroup(Slot[] collections)
            : base(collections)
        { }
    }

    /// <summary>
    /// CollectionGroups combine the behavior of collections. 
    /// When you want to add a stack of items that don't fit in a single collection the 
    /// CollectionGroup will handle the splitting of the stack between collections.
    /// Each collection can have it's own priority in the CollectionGroup. The CollectionGroup will
    /// auto. place items in the highest priority collections whenever possible.
    /// <seealso cref="Collection{T}"/>
    /// <seealso cref="CollectionSlot{T}"/>
    /// </summary>
    /// <typeparam name="TElementType"></typeparam>
    /// <typeparam name="TCollectionType"></typeparam>
    public class CollectionGroup<TElementType, TCollectionType> : ICloneable
        where TElementType : IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable
        where TCollectionType : ICollection<TElementType>
    {
        public class Slot
        {
            public TCollectionType collection { get; }
            public ICollectionPriority<TElementType> priority { get; }

            public Slot(TCollectionType collection)
                : this(collection, new CollectionPriority<TElementType>())
            { }

            public Slot(TCollectionType collection, ICollectionPriority<TElementType> priority)
            {
                this.collection = collection;
                this.priority = priority;
            }
        }

        public Slot[] collections { get; protected set; }
        private Slot[] _collectionsAddOrdered;
        private Slot[] _collectionsRemoveOrdered;
        public CollectionGroup(params TCollectionType[] collections)
            : this(collections.Select(o => new Slot(o)).ToArray())
        { }
        
        public CollectionGroup(Slot[] collections)
        {
            this.collections = collections ?? new Slot[0];
            SortArrays();
        }

        public int collectionCount
        {
            get { return collections.Length; }
        }

        private void SortArrays()
        {
            collections = collections.OrderByDescending(o => o.priority.GetGeneralPriority()).ToArray();
            _collectionsAddOrdered = collections.OrderByDescending(o => o.priority.GetAddPriority(default(TElementType))).ToArray();
            _collectionsRemoveOrdered = collections.OrderByDescending(o => o.priority.GetRemovePriority(default(TElementType))).ToArray();
        }

        public void Set(params TCollectionType[] newCollections)
        {
            Set(newCollections.Select(o => new Slot(o)).ToArray());
        }
        
        public void Set(Slot[] newCollections)
        {
            collections = newCollections;
            SortArrays();
        }
        
        public void AddCollection(Slot collection)
        {
            var l = new List<Slot>(collections);
            l.Add(collection);

            collections = l.ToArray();
            SortArrays();
        }
        
        public void RemoveCollection(TCollectionType collection)
        {
            var l = new List<Slot>(collections);
            l.RemoveAll(o => o.collection.Equals(collection));

            collections = l.ToArray();
            SortArrays();
        }

        /// <summary>
        /// MoveAuto the item from 1 collection group to another collection group.
        /// </summary>
        public Result<bool> Move(TElementType item, int amount, CollectionGroup<TElementType> target)
        {
            var canAdd = target.CanAdd(item, amount);
            if (canAdd.result == false)
            {
                return canAdd;
            }

            var canRemove = CanRemove(item, amount);
            if (canRemove.result == false)
            {
                return canRemove;
            }

            target.Add(item, amount);
            Remove(item, amount);

            return true;
        }
        
        public Result<bool> CanAdd(TElementType item, int amount = 1)
        {            
            if (GetCanAddAmount(item, amount) < amount)
            {
                return new Result<bool>(false, Errors.CollectionFull);
            }

            return new Result<bool>(true);
        }

        public Result<IEnumerable<CollectionGroupAddResult<TElementType>>> Add(TElementType item, int amount = 1)
        {
            var canAdd = CanAdd(item, amount);
            if (canAdd.result == false)
            {
                return new Result<IEnumerable<CollectionGroupAddResult<TElementType>>>(null, canAdd.error);
            }

            var l = new List<CollectionGroupAddResult<TElementType>>();
            foreach (var slot in _collectionsAddOrdered)
            {
                var canAddToCol = slot.collection.GetCanAddAmount(item, amount);
                if (canAddToCol.error == null && canAddToCol.result > 0)
                {
                    var addAmount = Math.Min(amount, canAddToCol.result);
                    var added = slot.collection.Add(item, addAmount);
                    
                    // CanAdd check passed, so error should always be empty.
                    if (added.error != null)
                    {
                        throw new ArgumentException(added.error.ToString());
                    }
                    
                    l.Add(new CollectionGroupAddResult<TElementType>(slot.collection, added.result.affectedSlots));

                    amount -= addAmount;
                    if (amount <= 0)
                    {
                        break;
                    }
                }
            }
            
            return l;
        }

        public Result<bool> CanRemove(TElementType item, int amount = 1)
        {
            int removableAmount = 0;
            foreach (var slot in _collectionsRemoveOrdered)
            {
                var containsAmount = slot.collection.GetAmount(item);
                var canRemove = slot.collection.CanRemove(item, containsAmount);
                if (canRemove.error == null)
                {
                    removableAmount += containsAmount;
                }
            }

            if (removableAmount < amount)
            {
                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
            }

            return true;
        }

        public Result<IEnumerable<CollectionGroupRemoveResult<TElementType>>> Remove(TElementType item, int amount = 1)
        {
            var canRemove = CanRemove(item, amount);
            if (canRemove.result == false)
            {
                return new Result<IEnumerable<CollectionGroupRemoveResult<TElementType>>>(null, canRemove.error);
            }
            
            var l = new List<CollectionGroupRemoveResult<TElementType>>();
            foreach (var slot in _collectionsRemoveOrdered)
            {
                var canRemoveFromCol = slot.collection.GetAmount(item);
                if (canRemoveFromCol > 0)
                {
                    var removeAmount = Math.Min(amount, canRemoveFromCol);
                    var removed = slot.collection.Remove(item, removeAmount);
                    l.Add(new CollectionGroupRemoveResult<TElementType>(slot.collection, removed.result.affectedSlots));
                    
                    amount -= removeAmount;
                    if (amount <= 0)
                    {
                        break;
                    }
                }
            }

            return l;
        }

        public int GetAmount(TElementType item)
        {
            return collections.Sum(o => o.collection.GetAmount(item));
        }

        /// <summary>
        /// Get the amount of all items that match the predicate.
        /// </summary>
        public int GetAmount(Predicate<TElementType> predicate)
        {
            int totalAmount = 0;
            foreach (var collection in collections)
            {
                totalAmount += collection.collection.GetAmount(predicate);
            }

            return totalAmount;
        }

        public int GetCanAddAmount(TElementType item, int bailEarly = int.MaxValue)
        {
            int amount = 0;
            foreach (var slot in _collectionsAddOrdered)
            {
                amount += slot.collection.GetCanAddAmount(item, bailEarly).result;
                if (amount >= bailEarly)
                {
                    break;
                }
            }

            return amount;
        }
        
        public bool Contains(TElementType item)
        {
            return collections.Any(o => o.collection.Contains(item));
        }

        public Result<CollectionGroupIndexOfResult<TElementType>> IndexOf(TElementType item)
        {
            foreach (var slot in collections)
            {
                var index = slot.collection.IndexOf(item);
                if (index != -1)
                {
                    return new Result<CollectionGroupIndexOfResult<TElementType>>(new CollectionGroupIndexOfResult<TElementType>(slot.collection, index));
                }
            }

            return new Result<CollectionGroupIndexOfResult<TElementType>>(null, Errors.CollectionDoesNotContainItem);
        }

        public Result<CollectionGroupIndexOfResult<TElementType>> IndexOf(Predicate<TElementType> predicate)
        {
            foreach (var slot in collections)
            {
                var index = slot.collection.IndexOf(predicate);
                if (index != -1)
                {
                    return new Result<CollectionGroupIndexOfResult<TElementType>>(new CollectionGroupIndexOfResult<TElementType>(slot.collection, index));
                }
            }

            return new Result<CollectionGroupIndexOfResult<TElementType>>(null, Errors.CollectionDoesNotContainItem);
        }

        public Result<CollectionGroupIndexOfResult<TElementType>> IndexOfSpecificInstance(TElementType item)
        {
            foreach (var slot in collections)
            {
                var index = slot.collection.IndexOf(o => ReferenceEquals(o, item));
                if (index != -1)
                {
                    return new Result<CollectionGroupIndexOfResult<TElementType>>(new CollectionGroupIndexOfResult<TElementType>(slot.collection, index));
                }
            }

            return new Result<CollectionGroupIndexOfResult<TElementType>>(null, Errors.CollectionDoesNotContainItem);
        }

        public IEnumerable<CollectionGroupIndexOfResult<TElementType>> IndexOfAll(TElementType item)
        {
            var l = new List<CollectionGroupIndexOfResult<TElementType>>();
            foreach (var slot in collections)
            {
                var indices = slot.collection.IndexOfAll(item);
                foreach (var index in indices)
                {
                    l.Add(new CollectionGroupIndexOfResult<TElementType>(slot.collection, index));
                }
            }

            return l;
        }

        public IEnumerable<CollectionGroupIndexOfResult<TElementType>> IndexOfAll(Predicate<TElementType> predicate)
        {
            var l = new List<CollectionGroupIndexOfResult<TElementType>>();
            foreach (var slot in collections)
            {
                var indices = slot.collection.IndexOfAll(predicate);
                foreach (var index in indices)
                {
                    l.Add(new CollectionGroupIndexOfResult<TElementType>(slot.collection, index));
                }
            }

            return l;
        }

        public object Clone()
        {
//            return MemberwiseClone();
            var clones = new Slot[collections.Length];
            for (var i = 0; i < collections.Length; i++)
            {
                clones[i] = new Slot((TCollectionType)collections[i].collection.Clone(), collections[i].priority);
            }
            return new CollectionGroup<TElementType, TCollectionType>(clones);
        }
    }
}
