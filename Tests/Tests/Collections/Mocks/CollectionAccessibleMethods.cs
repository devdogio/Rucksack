using System;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.Tests
{
    public sealed class CollectionAccessibleMethods<T> : Collection<T> 
        where T : class, IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        public CollectionAccessibleMethods(int slotCount, ILogger logger = null)
            : base(slotCount, logger)
        {
            
        }
        
        public Result<bool> CanSwapPublic(int fromIndex, ICollection toCol, int toIndex, CollectionContext context = null)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            context = context ?? new CollectionContext();
            context.originalIndex = fromIndex;
            return CanSet2Collections(this, fromIndex, toCol, toIndex, toCol.GetBoxed(toIndex), toCol.GetAmount(toIndex), this[fromIndex], GetAmount(fromIndex), context);
        }

        public Result<bool> SwapPublic(int fromIndex, ICollection toCol, int toIndex, CollectionContext context = null)
        {
            return this.Swap(fromIndex, toCol, toIndex, context);
        }

        public Result<bool> MergePublic(int fromIndex, ICollection toCol, int toIndex, int amount, CollectionContext context = null)
        {
            return this.Merge(fromIndex, toCol, toIndex, amount, context);
        }
    }
}