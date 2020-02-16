using System;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemCollectionSellToVendorSlotInputHandler : MonoBehaviour, ICollectionSlotInputHandler<IItemInstance>, IPointerClickHandler
    {
        public PointerEventData.InputButton useButton = PointerEventData.InputButton.Right;
        
        private CollectionSlotUIBase<IItemInstance> _slot;
        private readonly ILogger _logger;
        private ItemCollectionSellToVendorSlotInputHandler()
        {
            _logger = _logger ?? new UnityLogger("[UI][Collection] ");
        }

        private void Awake()
        {
            _slot = GetComponent<CollectionSlotUIBase<IItemInstance>>();
            Assert.IsNotNull(_slot, "No SlotUI found on input handler!");
        }
                
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == useButton && _slot.current != null)
            {
                var uis = FindObjectsOfType<ItemVendorUI>();
                foreach (var ui in uis)
                {
                    if (ui.window.isVisible && ui.vendor != null)
                    {
                        // TODO: Add option for dialogs (buy / sell, are you sure?)
                        var player = PlayerManager.currentPlayer.GetComponent<InventoryPlayer>();
                        ui.vendor.SellToVendor(new Customer<IItemInstance>(Guid.Empty, player, player.itemCollectionGroup, player.currencyCollectionGroup), 
                            new VendorProduct<IItemInstance>(_slot.current, _slot.current.itemDefinition.buyPrice, _slot.current.itemDefinition.sellPrice),
                            _slot.collection.GetAmount(_slot.collectionIndex));

                        eventData.Use();
                        return;
                    }
                }
            }
            
//            _logger.Warning("Tried to sell item to vendor, but no vendor windows found, ignoring");
        }
    }
}