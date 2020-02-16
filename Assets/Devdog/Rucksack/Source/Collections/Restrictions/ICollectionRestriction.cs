using System;

namespace Devdog.Rucksack.Collections
{
    public interface ICollectionRestriction<in T>
        where T: IEquatable<T>
    {
        Result<bool> CanAdd(T item, CollectionContext context);
//        Result<bool> CanMove(int fromIndex, ICollection toCollection, int toIndex, CollectionContext context);
        Result<bool> CanRemove(T item, CollectionContext context);
    }
}