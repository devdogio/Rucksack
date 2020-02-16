using System;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public class ItemVendorSellDropArea : MonoBehaviour, IDropArea, IPointerClickHandler
    {
        [Required]
        [SerializeField]
        protected ItemVendorUI vendorUI;
        
        protected ILogger logger;
        public ItemVendorSellDropArea()
        {
            logger = new UnityLogger("[UI] ");
        }
        
        public virtual Result<bool> CanDropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            var item = model.dataObject as IItemInstance;
            var beginSlot = model.source.GetComponent<CollectionSlotUIBase>();
            var fromCol = beginSlot.collection;
            var fromIndex = beginSlot.collectionIndex;
            var slotEntry = item as ICollectionSlotEntry;
            if (item == null)
            {
                return new Result<bool>(false, Errors.UIDragFailedIncompatibleDragObject);
            }
            
            // Check if the item can be set to null in it's collection
            if (slotEntry?.collectionEntry?.collection == null)
            {
                // Can't drop item here, it's not in a collection (can only sell items to vendor if that item is already in a collection!)
                return new Result<bool>(false, Errors.UIDragFailed);
            }

            var canSet = slotEntry.collectionEntry.collection.CanSetBoxed(fromIndex, item, 0);
            if (canSet.error != null)
            {
                return canSet;
            }
            
            return true;
        }

        public virtual void DropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            var item = (IItemInstance)model.dataObject;
            var beginSlot = model.source.GetComponent<CollectionSlotUIBase>();
            var fromCol = beginSlot.collection;
            var fromIndex = beginSlot.collectionIndex;

            var player = PlayerManager.currentPlayer;
            var inventoryPlayer = player.GetComponent<InventoryPlayer>();
            var customer = new Customer<IItemInstance>(Guid.NewGuid(), player, inventoryPlayer.itemCollectionGroup, inventoryPlayer.currencyCollectionGroup);
            
            // TODO: Add option for confirmation dialog with amount selector.
            // TODO: Always selling entire stack right now...
            var sold = vendorUI.vendor.SellToVendor(customer, new VendorProduct<IItemInstance>(item, item.itemDefinition.buyPrice, item.itemDefinition.sellPrice), fromCol.GetAmount(fromIndex));
            if (sold.error == null)
            {
                fromCol.SetBoxed(fromIndex, item, 0);
            }
            else
            {
                logger.Warning("Couldn't sell item to vendor. Error: " + sold.error, this);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!DragAndDropUtility.isDragging || DragAndDropUtility.currentDragModel == null)
                return;

            var dragHandler = DragAndDropUtility.currentDragModel.source.GetComponent<ItemCollectionSlotDragHandler>();
            if (dragHandler == null)
                return;

            if (dragHandler.HandlePointerClick)
            {
                DragAndDropUtility.EndDrag(eventData);
                eventData.Use();
            }
        }
    }
}