using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TSlotType">The collectionSlot type (wrapper type)</typeparam>
    /// <typeparam name="TElementType">The type used for the elements inside this collection.</typeparam>
    public abstract class CollectionBase<TSlotType, TElementType> : ICollection<TSlotType, TElementType>, ISimulatable
        where TSlotType : CollectionSlot<TElementType>, new()
        where TElementType : class, IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable
    {
        /// <summary>
        /// All slots available to this collection. Slots can be empty.
        /// </summary> 
        public TSlotType[] slots;

        /// <summary>
        /// The filters used to limit this collection. Filters can prevent an item from being stored, moved, etc.
        /// </summary>
        public List<ICollectionRestriction<TElementType>> restrictions;

        /// <summary>
        /// The equality comparer is used to find items in the collection, handle stacking, and replacing.
        /// </summary>
        public IEqualityComparer<TElementType> equalityComparer;

        
        public string collectionName { get; set; }
        
        protected ILogger logger { get; set; }
        public bool isReadOnly { get; set; }


        public int slotCount
        {
            get { return slots.Length; }
        }

        public event EventHandler<CollectionAddResult> OnAddedItem;
        public event EventHandler<CollectionRemoveResult<TElementType>> OnRemovedItem;
        public event EventHandler<CollectionSlotsChangedResult> OnSlotsChanged;
        public event EventHandler<CollectionSizeChangedResult> OnSizeChanged;

        public TElementType this[int key]
        {
            get { return slots[key].item; }
        }
        
        protected CollectionBase(int slotCount, ILogger logger = null)
        {
            slots = new TSlotType[slotCount];
            restrictions = new List<ICollectionRestriction<TElementType>>();
            equalityComparer = EqualityComparer<TElementType>.Default;
            
            this.logger = logger ?? new NullLogger();
        }

        protected void InvokeOnAddedItem(CollectionAddResult addResult) {
            OnAddedItem?.Invoke(this, addResult);
        }

        protected void InvokeOnRemovedItem(CollectionRemoveResult<TElementType> removeResult) {
            OnRemovedItem?.Invoke(this, removeResult);
        }

        protected void InvokeOnSlotsChanged(CollectionSlotsChangedResult slotsChangedResult) {
            OnSlotsChanged?.Invoke(this, slotsChangedResult);
        }

        protected void InvokeOnSizeChanged(CollectionSizeChangedResult sizeChangedResult) {
            OnSizeChanged?.Invoke(this, sizeChangedResult);
        }


        public object GetBoxed(int index)
        {
            return this[index];
        }

        public TSlotType GetSlot(int index)
        {
            return slots[index];
        }

        protected virtual TElementType CreateElementClone(TElementType original)
        {
            return (TElementType)original.Clone();
        }
        
        protected virtual void SetInternal(int index, TElementType item, int amount, CollectionContext context)
        {
            if (IsNull(item) || amount <= 0)
            {
                slots[index].Clear();
            }
            else
            {
                // Without validation because we've already done all required steps...
                slots[index].SetWithoutValidation(item, amount);
            }
        }

        /// <summary>
        /// </summary>
        public Result<bool> CanAdd(TElementType item, int amount = 1)
        {
            return CanAdd(item, amount, new CollectionContext());
        }

        public virtual Result<bool> CanAdd(TElementType item, int amount, CollectionContext context)
        {
            var canAddAmount = GetCanAddAmount(item, amount, context);
            if (canAddAmount.error != null)
            {
                return new Result<bool>(false, canAddAmount.error);
            }

            if (context.HasFlag(CollectionContext.Validations.SpecificInstance))
            {
                if (IndexOfSpecificInstance(item) != -1)
                {
                    return new Result<bool>(false, Errors.CollectionAlreadyContainsSpecificInstance);
                }
            }
            
            if (canAddAmount.result < amount)
            {
                return new Result<bool>(false, Errors.CollectionFull);
            }

            return true;
        }

        public virtual IEnumerator<int> GetAddItemEnumerator(TElementType item, int amount, CollectionContext context)
        {
            context.validationFlags &= ~CollectionContext.Validations.SpecificInstance;
            context.validationFlags &= ~CollectionContext.Validations.Restrictions;

            // First try stacking
            for (int i = 0; i < slots.Length; i++)
            {
                if (AreEqual(slots[i].item, item))
                {
                    var canAddAmount = Math.Min(item.maxStackSize - GetAmount(i), amount);
                    if (canAddAmount > 0 && CanSet(i, item, GetAmount(i) + canAddAmount, context).result)
                    {
                        yield return i;
                    }
                }
            } 

            // Then try finding a new empty slot
            for (var i = 0; i < slots.Length; i++)
            {
                var canAddAmount = Math.Min(item.maxStackSize, amount);
                if (slots[i].isOccupied == false && CanSet(i, item, canAddAmount, context).result)
                {
                    yield return i;
                }
            }
        }

        public Result<CollectionAddResult> Add(TElementType item, int amount = 1)
        {
            return Add(item, amount, new CollectionContext());
        }
        
        public virtual Result<CollectionAddResult> Add(TElementType item, int amount, CollectionContext context)
        {
            var canAdd = CanAdd(item, amount, context);
            if (canAdd.result == false)
            {
                return new Result<CollectionAddResult>(null, canAdd.error);
            }

            var totalAddAmount = amount;
            var affectedSlots = new List<SlotAmount>();
            
            // Can Add already checks all restrictions and requirements.
            var contextClone = context.Clone();
            contextClone.validationFlags &= ~CollectionContext.Validations.Restrictions;
            contextClone.validationFlags &= ~CollectionContext.Validations.SpecificInstance;

            // We'll handle events ourselves and bundle them
            contextClone.fireEventFlags = 0;

            var enumerator = GetAddItemEnumerator(item, amount, contextClone);
            while (enumerator.MoveNext())
            {
                var index = enumerator.Current;
                
                // Slot is occupied by other slot.
                if (slots[index].isOccupied && IsNull(slots[index].item))
                {
                    continue;
                }

                var isEmptySlot = this[index] == null;
                var canAddToStackAmount = Math.Min(item.maxStackSize - GetAmount(index), totalAddAmount);
                totalAddAmount -= canAddToStackAmount;
                affectedSlots.Add(new SlotAmount(index, canAddToStackAmount));

                if (totalAddAmount > 0)
                {
                    // Need to do another stack placement after this one.
                    if (isEmptySlot)
                    {
                        // Empty slot, so set the reference (and make a clone later if we need to place more).
                        SetInternal(index, item, GetAmount(index) + canAddToStackAmount, contextClone);
                    }
                    else
                    {
                        // If we're adding to an existing stack and still have to place more keep the existing item in the slot and just increase the amount.
                        SetInternal(index, this[index], GetAmount(index) + canAddToStackAmount, contextClone);
                    }
                }
                else
                {
                    // We don't want to place any more after this iteration.
                    SetInternal(index, item, GetAmount(index) + canAddToStackAmount, contextClone);
                }

                if (totalAddAmount <= 0)
                {
                    break;
                }
            
                if (isEmptySlot)
                {
                    item = CreateElementClone(item); // The next item we're placing needs to be a new instance.
                }
            }

            logger.LogVerbose($"Added {item} x {amount} to collection", this);
            var response = new Result<CollectionAddResult>(new CollectionAddResult(affectedSlots.ToArray()));

            if (context.HasFlag(CollectionContext.Events.Add))
            {
                InvokeOnAddedItem(response.result);
            }

            if (context.HasFlag(CollectionContext.Events.SlotChanged))
            {
                InvokeOnSlotsChanged(new CollectionSlotsChangedResult(response.result.affectedSlots.Select(o => o.slot).ToArray()));
            }
            
            return response;
        }

        public Result<bool> CanSet(int index, TElementType item, int amount = 1)
        {
            return CanSet(index, item, amount, new CollectionContext() {originalIndex = index});
        }
        
        public virtual Result<bool> CanSet(int index, TElementType item, int amount, CollectionContext context)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            if (amount < 0)
            {
                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
            }

            if (amount > item?.maxStackSize)
            {
                return new Result<bool>(false, Errors.ItemIsExceedingMaxStackSize);
            }
            
            // Slot is empty and item is empty, ignore call
            if (slots[index].isOccupied == false && IsNull(item))
            {
                return true;
            }

            Error error = null;
            var currentAmount = GetAmount(index);
            if (AreEqual(slots[index].item, item))
            {
                // Adding or removing
                
                if (amount < currentAmount)
                {
                    error = CheckRemoveRestriction(item, context).error;
                }
                else if (amount > currentAmount)
                {
                    error = CheckAddRestriction(item, context).error;
                    
                    if (context.HasFlag(CollectionContext.Validations.SpecificInstance) && IsNull(item) == false)
                    {
                        var i = IndexOfSpecificInstance(item);
                        if (i != -1 && index != i)
                        {
                            error = Errors.CollectionAlreadyContainsSpecificInstance;
                        }
                    }
                }
                else
                {
                    // Equal item, same amount
                    return true;
                }
            }
            else
            {
                // Swapping out item
                
                if (IsNull(item))
                {
                    error = CheckRemoveRestriction(item, context).error;
                }
                else
                {
                    if (IsNull(slots[index].item) == false)
                    {
                        error = CheckRemoveRestriction(item, context).error;
                        if (error != null)
                        {
                            return new Result<bool>(false, error);
                        }
                    }

                    error = CheckAddRestriction(item, context).error;
                    
                    if (context.HasFlag(CollectionContext.Validations.SpecificInstance) && IsNull(item) == false)
                    {
                        var i = IndexOfSpecificInstance(item);
                        if (i != -1 && index != i)
                        {
                            error = Errors.CollectionAlreadyContainsSpecificInstance;
                        }
                    }
                }
            }

            if (error != null)
            {
                return new Result<bool>(false, error);
            }
            
            return true;
        }
        
        /// <summary>
        /// Set an item at a given location.
        /// </summary>
        /// <remarks>Note that if the given index already has an item it will be overwritten.</remarks>
        /// <remarks>If the object is blocked by another object (only possible if the item size exceeds 1x1), false will be returned and the object won't be placed.</remarks>
        /// <remarks>Set ignores the CanAdd and CanRemove checks. Only use Set() if you're certain about your add or remove action.</remarks>
        /// <returns>True if the object has been inserted. False if the object could not be inserted.</returns>
        public Result<bool> Set(int index, TElementType item, int amount = 1)
        {
            return Set(index, item, amount, new CollectionContext() {originalIndex = index});
        }
        
        public virtual Result<bool> Set(int index, TElementType item, int amount, CollectionContext context)
        {
            var canSet = CanSet(index, item, amount, context);
            if (canSet.result == false)
            {
                return canSet;
            }

            // Slot is empty and item is empty, ignore call
            if (slots[index].isOccupied == false && IsNull(item))
            {
                return true;
            }

            if (AreEqual(slots[index].item, item) && GetAmount(index) == amount)
            {
                // Still set for *SpecificInstance
                SetInternal(index, item, amount, context);
                return true;
            }

            var currentItem = slots[index].item;
            var currentAmount = GetAmount(index);
            if (AreEqual(slots[index].item, item))
            {
                SetInternal(index, item, amount, context);

                var diff = Math.Abs(amount - currentAmount);
                if (amount < currentAmount)
                {
                    if (context.HasFlag(CollectionContext.Events.Remove))
                    {
                        InvokeOnRemovedItem(new CollectionRemoveResult<TElementType>(new [] {new SlotAmountItem<TElementType>(index, diff, currentItem)}));
                    }
                } 
                else if (amount > currentAmount)
                {
                    if (context.HasFlag(CollectionContext.Events.Add))
                    {
                        InvokeOnAddedItem(new CollectionAddResult(new SlotAmount[] {new SlotAmount(index, diff)}));
                    }
                }
            }
            else
            {
                if (IsNull(item))
                {
                    SetInternal(index, item, 0, context);
                    if (context.HasFlag(CollectionContext.Events.Remove))
                    {
                        InvokeOnRemovedItem(new CollectionRemoveResult<TElementType>(new [] {new SlotAmountItem<TElementType>(index, currentAmount, currentItem)}));
                    }
                }
                else
                {
                    if (IsNull(currentItem) == false)
                    {
                        SetInternal(index, default(TElementType), 0, context);
                        if (context.HasFlag(CollectionContext.Events.Remove))
                        {
                            InvokeOnRemovedItem(new CollectionRemoveResult<TElementType>(new [] {new SlotAmountItem<TElementType>(index, currentAmount, currentItem)}));
                        }
                    }

                    // Add the item
                    SetInternal(index, item, amount, context);
                    if (context.HasFlag(CollectionContext.Events.Add))
                    {
                        InvokeOnAddedItem(new CollectionAddResult(new [] {new SlotAmount(index, amount)}));
                    }
                }
            }

            if (context.HasFlag(CollectionContext.Events.SlotChanged))
            {
                InvokeOnSlotsChanged(new CollectionSlotsChangedResult(new int[]{ index }));
            }
            
            return true;
        }
        
        public void ForceSet(Tuple<TElementType, int>[] items)
        {
            var context = new CollectionContext();
            context.validationFlags = 0;
            
            Resize(items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                Set(i, items[i].Item1, items[i].Item2, context);
            }
        }
        
        public Result<bool> CanAddBoxed(object item, int amount = 1, CollectionContext context = null)
        {
            if (item is TElementType == false && item != null)
            {
                return new Result<bool>(false, Errors.CollectionInvalidItemType);
            }

            return CanAdd((TElementType) item, amount, context);
        }
        
        public Result<bool> AddBoxed(object item, int amount = 1, CollectionContext context = null)
        {
            if (item is TElementType == false && item != null)
            {
                return new Result<bool>(false, Errors.CollectionInvalidItemType);
            }

            context = context ?? new CollectionContext();
            var added = Add((TElementType) item, amount, context);
            if (added.error != null)
            {
                return new Result<bool>(false, added.error);
            }

            return true;
        }

        public Result<bool> CanRemoveBoxed(object item, int amount = 1, CollectionContext context = null)
        {
            if (item is TElementType == false && item != null)
            {
                return new Result<bool>(false, Errors.CollectionInvalidItemType);
            }
            
            return CanRemove((TElementType) item, amount, context);
        }

        public Result<CollectionRemoveResult<object>> RemoveBoxed(object item, int amount = 1, CollectionContext context = null)
        {
            if (item is TElementType == false && item != null)
            {
                return new Result<CollectionRemoveResult<object>>(null, Errors.CollectionInvalidItemType);
            }

            var remove = Remove((TElementType) item, amount, context);
            if (remove.error != null)
            {
                return new Result<CollectionRemoveResult<object>>(new CollectionRemoveResult<object>(new SlotAmountItem<object>[0]), remove.error);
            }
            
            var arr = remove.result.affectedSlots.Select(o => new SlotAmountItem<object>(o.slot, o.amount, o.item));
            return new CollectionRemoveResult<object>(arr.ToArray());
        }
        
        public Result<bool> CanSetBoxed(int index, object item, int amount = 1, CollectionContext context = null)
        {
            if (item is TElementType == false && item != null)
            {
                return new Result<bool>(false, Errors.CollectionInvalidItemType);
            }

            context = context ?? new CollectionContext();
//            context.originalIndex = index;
            return CanSet(index, (TElementType) item, amount, context);
        }
        
        public Result<bool> SetBoxed(int index, object item, int amount = 1, CollectionContext context = null)
        {
            if (item is TElementType == false && item != null)
            {
                return new Result<bool>(false, Errors.CollectionInvalidItemType);
            }

            context = context ?? new CollectionContext();
//            context.originalIndex = index;
            return Set(index, (TElementType) item, amount, context);
        }
                
        public void ForceSetBoxed(int index, object item, int amount)
        {
            if (item is TElementType == false && item != null)
            {
                return;
//                throw new ArgumentException($"Given item is not convertible to {typeof(TElementType).Name} - {item} :: {item.GetType().Name}");
            }

            var context = new CollectionContext
            {
                originalIndex = index,
                validationFlags = 0
            };
            
            Set(index, (TElementType) item, amount, context);
        }
        
        public void ForceSetBoxed<TTargetType>(Tuple<TTargetType, int>[] items)
            where TTargetType : class, IEquatable<TTargetType>
        {
            foreach (var item in items)
            {
                if (item.Item1 is TElementType == false && item.Item1 != null)
                {
                    throw new ArgumentException($"Given tuples are not convertible to {typeof(TElementType).Name} - {item.Item1}");
                }
            }

            var context = new CollectionContext()
            {
                validationFlags = 0
            };
            
            Resize(items.Length, context);
            for (var i = 0; i < items.Length; i++)
            {
                Set(i, items[i].Item1 as TElementType, items[i].Item2, context);
            }
        }
        
        public virtual Result<bool> SwapOrMerge(int fromIndex, ICollection toCol, int toIndex, int mergeAmount, CollectionContext context = null)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            context = context ?? new CollectionContext();
            var merged = Merge(fromIndex, toCol, toIndex, mergeAmount, context);
            if (merged.result)
            {
                return merged;
            }
            
            var swapped = Swap(fromIndex, toCol, toIndex, context);
            if (swapped.result)
            {
                return swapped;
            }
            
            return new Result<bool>(false, Errors.CollectionCanNotMoveItem);
        }

        protected Result<bool> Swap(int fromIndex, ICollection toCol, int toIndex, CollectionContext context = null)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            context = context ?? new CollectionContext();
            context.originalIndex = fromIndex;
            return Set2Collections(this, fromIndex, toCol, toIndex, toCol.GetBoxed(toIndex), toCol.GetAmount(toIndex), this[fromIndex], GetAmount(fromIndex), context);
        }

        protected Result<bool> Merge(int fromIndex, ICollection toCol, int toIndex, int amount, CollectionContext context = null)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            context = context ?? new CollectionContext();
            if (this == toCol && fromIndex == toIndex)
            {
                // Target is same as source
                return true;
            }

            if (this[fromIndex] == null)
            {
                // Merging nothing
                return true;
            }

            if (amount > GetAmount(fromIndex))
            {
                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
            }
            
            if (toCol.GetBoxed(toIndex) == null || AreEqual(this[fromIndex], toCol.GetBoxed(toIndex) as TElementType))
            {
                if (amount < GetAmount(fromIndex))
                {
                    // Not moving entire stack
                    var toItem = toCol.GetBoxed(toIndex);
                    if (toItem == null)
                    {
                        // Moving to an empty slot, we need to duplicate our item to avoid storing the safe ref twice.
                        toItem = CreateElementClone(this[fromIndex]);
                    }
                    
                    return Set2Collections(this, fromIndex, toCol, toIndex, this[fromIndex], GetAmount(fromIndex) - amount, toItem, toCol.GetAmount(toIndex) + amount, context);
                }
                else if (amount == GetAmount(fromIndex))
                {
                    // Moving entire stack
                    return Set2Collections(this, fromIndex, toCol, toIndex, default(TElementType), 0, this[fromIndex], GetAmount(fromIndex) + toCol.GetAmount(toIndex), context);
                }
            }

            return new Result<bool>(false, Errors.ItemsAreNotEqual);
        }
        
        public virtual Result<bool> MoveAuto(int fromIndex, ICollection toCol, int moveAmount, CollectionContext context = null)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            if (moveAmount > GetAmount(fromIndex))
            {
                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
            }
            
            if (this == toCol)
            {
                // Moving to a new auto. slot inside the same collection 
                return true;
            }

            context = context ?? new CollectionContext();
            var canAdd = toCol.CanAddBoxed(this[fromIndex], moveAmount, context);
            if (canAdd.result == false)
            {
                return canAdd;
            }

            var canSet = CanSet(fromIndex, this[fromIndex], GetAmount(fromIndex) - moveAmount);
            if (canSet.result == false)
            {
                return canSet;
            }

            var moveItem = this[fromIndex];
            if (moveAmount < GetAmount(fromIndex))
            {
                // Not moving entire stack, clone object.
                moveItem = CreateElementClone(this[fromIndex]);
            }
            
            toCol.AddBoxed(moveItem, moveAmount);
            Set(fromIndex, this[fromIndex], GetAmount(fromIndex) - moveAmount, context);

            return true;
        }

        protected Result<bool> CanSet2Collections(ICollection fromCol, int fromIndex, ICollection toCol, int toIndex, object fromItem, int fromAmount, object toItem, int toAmount, CollectionContext context)
        {
            if (fromCol == toCol)
            {
                // Staying in the same collection, avoid firing events
                context.validationFlags &= ~CollectionContext.Validations.SpecificInstance;
            }

            context.originalIndex = toIndex;
            var canSet2 = fromCol.CanSetBoxed(fromIndex, fromItem, fromAmount, context);
            context.originalIndex = fromIndex;
            var canSet1 = toCol.CanSetBoxed(toIndex, toItem, toAmount, context); 
 
            if (canSet1.result == false) 
            { 
                return canSet1; 
            } 
 
            if (canSet2.result == false) 
            { 
                return canSet2; 
            }

            return true;
        }

        protected Result<bool> Set2Collections(ICollection fromCol, int fromIndex, ICollection toCol, int toIndex, object fromItem, int fromAmount, object toItem, int toAmount, CollectionContext context)
        {
            // Create a clone to avoid ovewriting settings
            context = context.Clone();
            var fireEventsBefore = context.HasFlag(CollectionContext.Events.SlotChanged);
            if (fromCol == toCol)
            {
                // Staying in the same collection, avoid firing add / remove events
                context.fireEventFlags = 0;
                context.validationFlags &= ~CollectionContext.Validations.SpecificInstance;
            }
            
            var can = CanSet2Collections(fromCol, fromIndex, toCol, toIndex, fromItem, fromAmount, toItem, toAmount, context);
            if (can.result == false)
            {
                return can;
            }
            
            context.originalIndex = toIndex;
            fromCol.SetBoxed(fromIndex, fromItem, fromAmount, context);
            context.originalIndex = fromIndex;
            toCol.SetBoxed(toIndex, toItem, toAmount, context);

            if (fireEventsBefore && fromCol == toCol)
            {
                InvokeOnSlotsChanged(new CollectionSlotsChangedResult(new int[]{ fromIndex, toIndex }));
            }
            
            return true; 
        }
        
        public Result<bool> CanRemove(TElementType item, int amount = 1)
        {
            return CanRemove(item, amount, new CollectionContext());
        }

        public Result<bool> CanRemove(TElementType item, int amount, CollectionContext context)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            // Handle remove restriction here once to avoid doing it on every CanSet()
            var canRemoveRestriction = CheckRemoveRestriction(item, context);
            if (canRemoveRestriction.result == false)
            {
                return canRemoveRestriction;
            }

            context.validationFlags &= ~CollectionContext.Validations.Restrictions;
            
            int removable = 0;
            foreach (var i in IndexOfAll(item))
            {
                var canRemove = CanSet(i, slots[i].item, 0, context);
                if (canRemove.result == false)
                {
                    return canRemove;
                }
                
                removable += GetAmount(i);
            }

            if (removable < amount)
            {
                return new Result<bool>(false, Errors.CollectionDoesNotContainItem);
            }
            
            return true;
        }

        public Result<CollectionRemoveResult<TElementType>> Remove(TElementType item, int amount = 1)
        {
            return Remove(item, amount, new CollectionContext());
        }

        public virtual Result<CollectionRemoveResult<TElementType>> Remove(TElementType item, int amount, CollectionContext context)
        {
            var canRemove = CanRemove(item, amount, context);
            if (canRemove.result == false)
            {
                return new Result<CollectionRemoveResult<TElementType>>(null, canRemove.error);
            }

            // First try to remove the ReferenceEquals item instance
            var indices = IndexOfAll(o => ReferenceEquals(o, item));
            var affectedSlots = new List<SlotAmountItem<TElementType>>();
            var amountToRemove = amount;
            amountToRemove = RemoveFromIndices(indices, amountToRemove, affectedSlots, context);

            if (amountToRemove > 0)
            {
                indices = IndexOfAll(item);
                amountToRemove = RemoveFromIndices(indices, amountToRemove, affectedSlots, context);
            }

            var response = new Result<CollectionRemoveResult<TElementType>>(new CollectionRemoveResult<TElementType>(affectedSlots.ToArray()));
            if (context.HasFlag(CollectionContext.Events.Remove))
            {
                InvokeOnRemovedItem(response.result);
            }

            if (context.HasFlag(CollectionContext.Events.SlotChanged))
            {
                InvokeOnSlotsChanged(new CollectionSlotsChangedResult(response.result.affectedSlots.Select(o => o.slot).ToArray()));
            }
            
            return response;
        }

        protected int RemoveFromIndices(IEnumerable<int> indices, int amountToRemove, List<SlotAmountItem<TElementType>> affectedSlots, CollectionContext context)
        {
            foreach (int index in indices)
            {
                var currentItem = slots[index].item;
                if (amountToRemove - slots[index].amount < 0)
                {
                    // Just remove moveAmount, there's enough in this slot to complete
                    SetInternal(index, currentItem, slots[index].amount - amountToRemove, context);
                    affectedSlots.Add(new SlotAmountItem<TElementType>(index, amountToRemove, currentItem));
                    amountToRemove = 0;
                    break;
                }

                affectedSlots.Add(new SlotAmountItem<TElementType>(index, slots[index].amount, currentItem));
                amountToRemove -= slots[index].amount;
                SetInternal(index, default(TElementType), 0, context);
            }

            return amountToRemove;
        }

        /// <summary>
        /// Check if this collection contains the given item. This will use the equalityComparer.
        /// </summary>
        public bool Contains(TElementType item)
        {
            return IndexOf(item) != -1;
        }
        
        public int IndexOf(TElementType item)
        {
            return IndexOf(o => AreEqual(o, item));
        }

        public int IndexOf(Predicate<TElementType> predicate)
        {
            for (var i = 0; i < slots.Length; i++)
            {
                if (predicate(slots[i].item))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of a specific instance. This will use ReferenceEquals as opposed to equality comparisions.
        /// </summary>
        public int IndexOfSpecificInstance(TElementType item)
        {
            return IndexOf(t => ReferenceEquals(t, item));
        }
        
        public IEnumerable<int> IndexOfAll(TElementType item)
        {
            return IndexOfAll(o => AreEqual(o, item));
        }
        
        public IEnumerable<int> IndexOfAll(Predicate<TElementType> predicate)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (predicate(slots[i].item))
                {
                    yield return i;
                }
            }
        }

        public Result<int> GetCanAddAmount(TElementType item, int bailEarly = int.MaxValue)
        {
            return GetCanAddAmount(item, bailEarly, new CollectionContext());
        }

        public virtual Result<int> GetCanAddAmount(TElementType item, int bailEarly, CollectionContext context)
        {
            if (isReadOnly)
            {
                return new Result<int>(0, Errors.CollectionIsReadOnly);
            }
           
            // Handle restrictions here once to avoid doing it on every CanSet call
            var canAdd = CheckAddRestriction(item, context);
            if (canAdd.result == false)
            {
                return new Result<int>(0, canAdd.error);
            }
            
            // Avoid chaning the caller's context.
            context = context.Clone();
            context.validationFlags &= ~(CollectionContext.Validations.Restrictions | CollectionContext.Validations.SpecificInstance);
            
            int totalAmount = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (CanSet(i, item, item.maxStackSize, context).result)
                {
                    if (IsNull(slots[i].item))
                    {
                        totalAmount += item.maxStackSize;
                    }
                    else if (AreEqual(slots[i].item, item))
                    {
                        totalAmount += item.maxStackSize - slots[i].amount;
                    }
                }
    
                if (totalAmount >= bailEarly)
                {
                    break;
                }
            }
    
            return new Result<int>(totalAmount);
        }

        public int GetAmount(TElementType item)
        {
            return GetAmount(o => AreEqual(o, item));
        }

        public int GetAmount(int index)
        {
            return slots[index].amount;
        }
        
        public int GetAmount(Predicate<TElementType> predicate)
        {
            int totalAmount = 0;
            for (var i = 0; i < slots.Length; i++)
            {
                if (predicate(slots[i].item))
                {
                    totalAmount += slots[i].amount;
                }
            }

            return totalAmount;
        }
        
        public void Clear()
        {
            Clear(new CollectionContext());
        }

        public void Clear(CollectionContext context)
        {
            if (isReadOnly)
            {
                return;
            }
            
            var l = new List<SlotAmountItem<TElementType>>();
            int removedCount = 0;
            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot.isOccupied)
                {
                    l.Add(new SlotAmountItem<TElementType>(i, GetAmount(i), slot.item));
                    removedCount += slot.amount;
                    slot.Clear();
                }
            }

            if (l.Count > 0)
            {
                var affectedSlots = l.ToArray();

                if (context.HasFlag(CollectionContext.Events.Remove))
                {
                    InvokeOnRemovedItem(new CollectionRemoveResult<TElementType>(affectedSlots));
                }

                if (context.HasFlag(CollectionContext.Events.SlotChanged))
                {
                    InvokeOnSlotsChanged(new CollectionSlotsChangedResult(affectedSlots.Select(o => o.slot).ToArray()));
                }
            }
        }

        public void Resize(int newSize)
        {
            Resize(newSize, new CollectionContext());
        }
        
        public void Resize(int newSize, CollectionContext context)
        {
            if (newSize > slotCount)
            {
                Expand(newSize - slotCount, context);
            }
            else if (newSize < slotCount)
            {
                Shrink(slotCount - newSize, context);
            }
        }

        public void Expand(int expandBySlots)
        {
            Expand<TSlotType>(expandBySlots, new CollectionContext());
        }
        
        public void Expand(int expandBySlots, CollectionContext context)
        {
            Expand<TSlotType>(expandBySlots, context);
        }

        public void Expand<TNewSlotType>(int expandBySlots)
            where TNewSlotType : TSlotType, new()
        {
            Expand<TNewSlotType>(expandBySlots, new CollectionContext());
        }
        
        public void Expand<TNewSlotType>(int expandBySlots, CollectionContext context)
            where TNewSlotType : TSlotType, new()
        {
            if (isReadOnly)
            {
                return;
            }

            int currentSize = slotCount;
            Array.Resize(ref slots, currentSize + expandBySlots);
            GenerateSlotsRange<TNewSlotType>(currentSize - 1, currentSize + expandBySlots - 1, context);

            if (context.HasFlag(CollectionContext.Events.SizeChanged))
            {
                InvokeOnSizeChanged(new CollectionSizeChangedResult(currentSize, currentSize + expandBySlots));
            }
        }
        
        
        public void Shrink(int shrinkBySlots)
        {
            Shrink(shrinkBySlots, new CollectionContext());
        }
        
        public void Shrink(int shrinkBySlots, CollectionContext context)
        {
            if (isReadOnly)
            {
                return;
            }

            int currentSize = slotCount;
            Array.Resize(ref slots, slotCount - shrinkBySlots);

            if (context.HasFlag(CollectionContext.Events.SizeChanged))
            {
                InvokeOnSizeChanged(new CollectionSizeChangedResult(currentSize, currentSize - shrinkBySlots));
            }
        }
        
        public void Sort(IComparer<TElementType> comparer)
        {
            Sort(comparer, new CollectionContext());
        }
        
        public virtual void Sort(IComparer<TElementType> comparer, CollectionContext context)
        {
            if (isReadOnly)
            {
                return;
            }

            var l = new List<Tuple<TElementType, int>>();
            foreach (var slot in slots)
            {
                if (slot.isOccupied)
                {
                    l.Add(new Tuple<TElementType, int>(slot.item, slot.amount));
                
                    var before = slot.setCollectionEntry;
                    slot.setCollectionEntry = false;
                    slot.Clear();
                    slot.setCollectionEntry = before;
                }
            }
         
            l.Sort((a, b) => comparer.Compare(a?.Item1, b?.Item1));

            var ctx = new CollectionContext()
            {
                validationFlags = 0,
                fireEventFlags = 0,
            };
            foreach (var item in l)
            {
                var added = Add(item.Item1, item.Item2, ctx);
                if (added.error != null)
                {
                    throw new Exception("Couldn't re-add item after sort with error: " + added.error);
                }
            }

            if (context.HasFlag(CollectionContext.Events.SlotChanged))
            {
                InvokeOnSlotsChanged(new CollectionSlotsChangedResult(Enumerable.Range(0, slotCount).ToArray()));
            }
        }

        public IEnumerable<TElementType> GetAll()
        {
            foreach (var slot in slots)
            {
                if (IsNull(slot.item) == false)
                {
                    yield return slot.item;
                }
            }
        }
        
        
        protected Result<bool> CheckAddRestriction(TElementType item, CollectionContext context)
        {
            if (context.HasFlag(CollectionContext.Validations.Restrictions))
            {
                foreach (var restriction in restrictions)
                {
                    var canAdd = restriction.CanAdd(item, context);
                    if (canAdd.result == false)
                    {
                        return canAdd;
                    }
                }
            }

            return true;
        }
        
        protected Result<bool> CheckRemoveRestriction(TElementType item, CollectionContext context)
        {
            if (context.HasFlag(CollectionContext.Validations.Restrictions))
            {
                foreach (var restriction in restrictions)
                {
                    var canAdd = restriction.CanRemove(item, context);
                    if (canAdd.result == false)
                    {
                        return canAdd;
                    }
                }
            }

            return true;
        }

//        protected virtual bool CanStack(TElementType a, TElementType b)
//        {
//            return AreEqual(a, b);
//        }

//        protected virtual int GetCanStackAmount(TElementType a, int aAmount, TElementType b)
//        {
//            if (a != null && AreEqual(a, b) && aAmount <= a.maxStackSize)
//            {
//                return a.maxStackSize - aAmount;
//            }
//
//            return 0;
//        }

        protected bool AreEqual(TElementType a, TElementType b)
        {
            return equalityComparer.Equals(a, b);
        }
        
        protected bool IsNull(TElementType item)
        {
            return item == null || item.Equals(default(TElementType));
        }
        
        public void GenerateSlots<TNewSlotType>()
            where TNewSlotType : TSlotType, new()
        {
            GenerateSlotsRange<TNewSlotType>(0, slots.Length - 1);
        }

        public void GenerateSlots(Type slotType)
        {
            GenerateSlotsRange(slotType, 0, slots.Length - 1, new CollectionContext());
        }

        public void GenerateSlotsRange<TNewSlotType>(int startIndex, int endIndex)
            where TNewSlotType : TSlotType, new()
        {
            GenerateSlotsRange(typeof(TNewSlotType), startIndex, endIndex, new CollectionContext());
        }
        
        public void GenerateSlotsRange<TNewSlotType>(int startIndex, int endIndex, CollectionContext context)
            where TNewSlotType : TSlotType, new()
        {
            GenerateSlotsRange(typeof(TNewSlotType), startIndex, endIndex, context);
        }
        
        public virtual void GenerateSlotsRange(Type slotType, int startIndex, int endIndex, CollectionContext context)
        {
            if (slotType.IsSubclassOf(typeof(TSlotType)) == false && typeof(TSlotType) != slotType)
            {
                throw new ArgumentException($"Given type {slotType.Name} is not a subclass of {typeof(TSlotType).Name}");
            }
            
            if (isReadOnly)
            {
                return;
            }

            int[] changedSlots = new int[endIndex - startIndex + 1];
            for (int i = startIndex; i <= endIndex; i++)
            {
                var inst = (TSlotType) Activator.CreateInstance(slotType, new object[0]);
                inst.collection = this;
                inst.index = i;

                slots[i] = inst;
                changedSlots[i - startIndex] = i;
            }

            if (context.HasFlag(CollectionContext.Events.SlotChanged))
            {
                InvokeOnSlotsChanged(new CollectionSlotsChangedResult(changedSlots));
            }
        }

        public virtual object Clone()
        {
            var copy = (CollectionBase<TSlotType, TElementType>) MemberwiseClone();
            copy.logger = logger;
            copy.slots = new TSlotType[copy.slotCount];
            for (int i = 0; i < copy.slotCount; i++)
            {
                copy.slots[i] = (TSlotType)slots[i].Clone();
            }

            return copy;
        }

        public void SetSimulationEnabled(bool enabled)
        {
            foreach (var slot in slots)
            {
                slot.setCollectionEntry = enabled;
            }
        }

        public IEnumerator<TElementType> GetEnumerator()
        {
            foreach (var slot in slots)
            {
                yield return slot.item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return collectionName;
        }
    }
}
