using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionGroupIndexOfResult<T>
        where T : IEquatable<T>
    {
        public ICollection<T> collection { get; }
        public int index { get; }

        public CollectionGroupIndexOfResult(ICollection<T> collection, int index)
        {
            this.collection = collection;
            this.index = index;
        }

        public override string ToString()
        {
            return $"{index} - {collection}";
        }
    }
}