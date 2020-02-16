using System;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.Tests
{
    public sealed class LayoutCollectionAccessibleMethods<T> : LayoutCollection<T> 
        where T : class, IEquatable<T>, IStackable, IIdentifiable, ICloneable, IShapeOwner2D
    {
        public LayoutCollectionAccessibleMethods(int slotCount, int columnCount, ILogger logger = null)
            : base(slotCount, columnCount, logger)
        {
        }
        
        public Result<bool> CanSwap(int fromIndex, ICollection toCol, int toIndex, CollectionContext context = null)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            context = context ?? new CollectionContext();
            context.originalIndex = fromIndex;
            return CanSet2Collections(this, fromIndex, toCol, toIndex, toCol.GetBoxed(toIndex), toCol.GetAmount(toIndex), this[fromIndex], GetAmount(fromIndex), context);
        }

        public Result<bool> Swap(int fromIndex, ICollection toCol, int toIndex, bool f, CollectionContext context = null)
        {
            return this.Swap(fromIndex, toCol, toIndex, context);
        }

        public Result<bool> Merge(int fromIndex, ICollection toCol, int toIndex, int amount, bool f, CollectionContext context = null)
        {
            return this.Merge(fromIndex, toCol, toIndex, amount, context);
        }
    }
}