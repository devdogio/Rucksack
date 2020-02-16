using System.Collections.Generic;
using Devdog.General2;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public sealed class EquipmentCollectionSlotDragHandler : CollectionSlotDragHandlerBase<IEquippableItemInstance>, IDropAreaSourceOverwriter
    {
        public override Result<bool> CanDropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            return base.CanDropDraggingItem(model, eventData);
        }

        public override void DropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            var beginSlot = model.source.GetComponent<CollectionSlotUIBase>();
            if (beginSlot != null)
            {
                var equippable = beginSlot.collection.GetBoxed(beginSlot.collectionIndex) as IEquippableItemInstance;
                if (equippable != null)
                {
                    equippable.Use(PlayerManager.currentPlayer, new ItemContext()
                    {
                        useAmount = beginSlot.collection.GetAmount(beginSlot.collectionIndex),
                        targetIndex = slot.collectionIndex
                    });
                }
            }
        }

        public Result<bool> CanDropDraggingItemOnTarget(DragAndDropUtility.Model model, List<IDropArea> targetDropAreas, PointerEventData eventData)
        {
            foreach (var dropArea in targetDropAreas)
            {
                if (dropArea.CanDropDraggingItem(model, eventData).result)
                {
                    return true;
                }
            }
            
            return new Result<bool>(false, Errors.UIDragFailedIncompatibleDragObject);
        }

        public void DropDraggingItemOnTarget(DragAndDropUtility.Model model, List<IDropArea> targetDropAreas, PointerEventData eventData)
        {
            var equipmentCol = slot.collection as IEquipmentCollection<IEquippableItemInstance>;
            if (equipmentCol == null)
            {
                logger.Warning("Equipment collection is not compatible with slot drag handler.", this);
                return;
            }

            var beginSlot = model.source.GetComponent<CollectionSlotUIBase>();
            var context = new Collections.CollectionContext() {
                validationFlags = ~Collections.CollectionContext.Validations.SpecificInstance,
                originalIndex = beginSlot.collectionIndex
            };

            foreach (var targetDropArea in targetDropAreas)
            {
                var targetSlot = (targetDropArea as UnityEngine.Component)?.GetComponent<CollectionSlotUIBase>();
                if (targetSlot != null)
                {
                    if (targetSlot.collection == equipmentCol)
                    {
                        if (targetSlot.collection.CanSetBoxed(targetSlot.collectionIndex, equipmentCol[slot.collectionIndex], equipmentCol.GetAmount(slot.collectionIndex), context).result == false)
                        {
                            // We're trying to move to another slot inside the same collection, but can't place in that slot, ignore action.
                            continue;
                        }
                    }

                    var player = PlayerManager.currentPlayer;
                    var dragAmount = equipmentCol.GetAmount(slot.collectionIndex);

                    // If already eqipped, or the item is moving to a non-equipment collection, don't call Use again as it would unequip the item
                    if (equipmentCol[slot.collectionIndex].equippedTo == null || !(targetSlot.collection is IEquipmentCollection<IEquippableItemInstance>))
                    {
                        equipmentCol[slot.collectionIndex].Use(player, new ItemContext()
                        {
                            useAmount = dragAmount,
                            targetIndex = targetSlot.collectionIndex
                        });
                    }
                    else
                        beginSlot.collection.SwapOrMerge(beginSlot.collectionIndex, targetSlot.collection, targetSlot.collectionIndex, dragAmount, context);

                    eventData.Use();
                    break;
                }
            }
        }
    }
}