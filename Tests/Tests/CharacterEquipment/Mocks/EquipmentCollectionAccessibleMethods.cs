using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.Tests
{
    public class EquipmentCollectionAccessibleMethods<TEquippableType> : EquipmentCollection<TEquippableType>
        where TEquippableType : class, IEquatable<TEquippableType>, IEquippable<TEquippableType>, ICollectionSlotEntry, ICloneable, IIdentifiable, IStackable
    {
        public EquipmentCollectionAccessibleMethods(int slotCount, IEquippableCharacter<TEquippableType> character, ILogger logger = null)
            : base(slotCount, character, logger)
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