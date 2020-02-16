using System;
using System.Collections;
using System.Collections.Generic;

namespace Devdog.Rucksack.Collections
{
    public class LayoutCollection<TElementType> : CollectionBase<LayoutCollectionSlot<TElementType>, TElementType>
        where TElementType : class, IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable, IShapeOwner2D
    {
        public int columnCount { get; }
        public int rowCount
        {
            get { return (int)Math.Ceiling((float) slotCount / columnCount); }
        }

        public LayoutCollection(int slotCount, int columnCount, ILogger logger = null)
            : base(slotCount, logger)
        {
            GenerateSlots<LayoutCollectionSlot<TElementType>>();
            this.columnCount = columnCount;
        }

        /// <summary>
        /// Can Set Axis Aligned Bounding Box. This will check if there's collision when trying to set the in a given slot.
        /// </summary>
        private bool CanSetAABB(int index, TElementType item, int amount, CollectionContext context)
        {
            // Given item is null; Can always be placed.
            if (IsNull(item))
            {
                return true;
            }
            
            // Exceeding the X or Y axis (out of bounds of grid)
            if ((index % columnCount) + Offset(item.layoutShape.convexX - 1, 0) >= columnCount ||
                index + Offset(item.layoutShape.convexX - 1, item.layoutShape.convexY - 1) >= slotCount)
            {
                return false;
            }
            
            var originalIndex = context.originalIndex;
            var allowCollisionSelf = context.allowAABBCollisionSelf;
            for (int x = 0; x < item.layoutShape.convexX; x++)
            {
                for (int y = 0; y < item.layoutShape.convexY; y++)
                {
                    // We're not occupying this specific slot.
                    if (item.layoutShape.IsBlocking(x, y) == false)
                    {
                        continue;
                    }

                    var startSlot = slots[index];
                    var target = slots[index + Offset(x, y)];
                    
                    if (target.isOccupied == false)
                    {
                        continue;
                    }

                    // Source slot is filled - Overwriting existing
                    if (startSlot.item != null)
                    {
                        continue;
                    }

                    if (allowCollisionSelf)
                    {
                        if (target.occupiedBy == startSlot ||
                            target.occupiedBy == slots[originalIndex])
                        {
                            continue;
                        }
                    }
                    
                    return false;
                }
            }
            
            return true;
        }
        
        private void SetAABB(int index, TElementType item, LayoutCollectionSlot<TElementType> occupyingSlot)
        {
            if (IsNull(item))
            {
                return;
            }
            
            for (int x = 0; x < item.layoutShape.convexX; x++)
            {
                for (int y = 0; y < item.layoutShape.convexY; y++)
                {
                    slots[index + Offset(x, y)].occupiedBy = occupyingSlot;
                }
            }
        }

        public override Result<bool> CanSet(int index, TElementType item, int amount, CollectionContext context)
        {
            var canSet = base.CanSet(index, item, amount, context);
            if (canSet.result == false)
            {
                return canSet;
            }
            
            if (CanSetAABB(index, item, amount, context) == false)
            {
                return new Result<bool>(false, Errors.LayoutCollectionItemBlocked);
            }

            return true;
        }

        protected override void SetInternal(int index, TElementType item, int amount, CollectionContext context)
        {
            SetAABB(index, slots[index].item, null); // Clear the old
            base.SetInternal(index, item, amount, context);

            if (IsNull(slots[index].item) == false)
            {
                SetAABB(index, item, slots[index]);
            }
        }

        public override Result<int> GetCanAddAmount(TElementType item, int bailEarly, CollectionContext context)
        {
            if (isReadOnly)
            {
                return new Result<int>(0, Errors.CollectionIsReadOnly);
            }
            
            var canAddRestriction = CheckAddRestriction(item, context);
            if (canAddRestriction.result == false)
            {
                return new Result<int>(0, canAddRestriction.error);
            }

            // Avoid chaning the caller's context.
            context = context.Clone();
            context.allowAABBCollisionSelf = false;
            context.validationFlags &= ~(CollectionContext.Validations.Restrictions | CollectionContext.Validations.SpecificInstance);
            
            // Keep track of which slots get 'occupied' when we set our item in a new slot (The item might occupy multiple slots).
            int addAmount = 0;
            var occupiedList = new BitArray(slotCount);
            for (int i = 0; i < slotCount; i++)
            {
                if (CanSet(i, item, item.maxStackSize, context).result)
                {
                    if (IsNull(slots[i].item) && occupiedList[i] == false && CanSetAABB(i, item, 1, context))
                    {
                        for (int x = 0; x < item.layoutShape.convexX; x++)
                        {
                            for (int y = 0; y < item.layoutShape.convexY; y++)
                            {
                                occupiedList[i + Offset(x, y)] = true;
                            }
                        }

                        addAmount += item.maxStackSize;   
                    }
                    else if (AreEqual(slots[i].item, item) && occupiedList[i] == false)
                    {
                        var canAdd = item.maxStackSize - slots[i].amount;
                        addAmount += canAdd;   
                    }
                }

                if (addAmount >= bailEarly)
                {
                    break;
                }
            }

            return addAmount;
        }
        
        public override void Sort(IComparer<TElementType> comparer, CollectionContext context)
        {
            throw new System.NotImplementedException();
        }
        
        private int Offset(int x, int y)
        {
            return columnCount * y + x;
        }
    }
}