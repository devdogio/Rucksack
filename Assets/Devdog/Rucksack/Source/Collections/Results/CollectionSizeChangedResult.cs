using System;

namespace Devdog.Rucksack.Collections
{
    public class CollectionSizeChangedResult : EventArgs
    {
        public readonly int sizeBefore;
        public readonly int sizeAfter;

        public CollectionSizeChangedResult(int sizeBefore, int sizeAfter)
        {
            this.sizeBefore = sizeBefore;
            this.sizeAfter = sizeAfter;
        }

        public override string ToString()
        {
            return $"{sizeBefore} to {sizeAfter}";
        }
    }
}