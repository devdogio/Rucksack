using System;

namespace Devdog.Rucksack.Collections
{
    public static class CollectionExtensionMethods
    {
//        public static Result<bool> SwapOrMergeAuto(this ICollection fromCol, int fromIndex, ICollection toCol, int amount)
//        {
//            if (amount > fromCol.GetAmount(fromIndex))
//            {
//                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
//            }
//            
//            var canSetBoxed = fromCol.CanSetBoxed(fromIndex, fromCol.GetBoxed(fromIndex), amount);
//            if (canSetBoxed.result == false)
//            {
//                return canSetBoxed;
//            }
//
//            var added = toCol.AddBoxed(fromCol.GetBoxed(fromIndex), amount);
//            if (added.error != null)
//            {
//                return new Result<bool>(false, added.error);
//            }
//
//            fromCol.SetBoxed(fromIndex, fromCol.GetBoxed(fromIndex), fromCol.GetAmount(fromIndex) - amount);
//            return true;
//        }
        
        public static int GetFirstUnOccupiedSlot<TSlotType, T>(this ICollection<TSlotType, T> collection)
            where T : class, IEquatable<T>, IStackable
            where TSlotType : CollectionSlot<T>
        {
            for (int i = 0; i < collection.slotCount; i++)
            {
                if (collection.GetSlot(i).isOccupied == false)
                {
                    return i;
                }
            }

            return -1;
        }
        
//        public static int GetFirstEmptySlot(this ICollection collection)
//        {
//            for (int i = 0; i < collection.slotCount; i++)
//            {
//                if (collection.GetBoxed(i) == null)
//                {
//                    return i;
//                }
//            }
//
//            return -1;
//        }

    }
}