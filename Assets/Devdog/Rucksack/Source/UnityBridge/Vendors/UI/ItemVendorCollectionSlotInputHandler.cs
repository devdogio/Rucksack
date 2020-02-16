using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemVendorCollectionSlotInputHandler : MonoBehaviour, ICollectionSlotInputHandler<IVendorProduct<IItemInstance>>, IPointerClickHandler
    {
        public PointerEventData.InputButton buyButton = PointerEventData.InputButton.Right;
        public bool consumeEvent = true;
        
        private VendorUIBase<IItemInstance> _vendorUI;
        private CollectionSlotUIBase<IVendorProduct<IItemInstance>> _slot;
        private readonly ILogger _logger;
        private ItemVendorCollectionSlotInputHandler()
        {
            _logger = _logger ?? new UnityLogger("[UI][Collection] ");
        }

        private void Awake()
        {
            _slot = GetComponent<CollectionSlotUIBase<IVendorProduct<IItemInstance>>>();
            Assert.IsNotNull(_slot, "No SlotUI found on input handler!");

            _vendorUI = GetComponentInParent<VendorUIBase<IItemInstance>>();
            Assert.IsNotNull(_vendorUI, "No VendorUI found in parent of ItemVendorCollectionSlotInputHandler");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == buyButton && _slot.current != null)
            {
                var player = PlayerManager.currentPlayer.GetComponent<InventoryPlayer>();
                
                // TODO: Customer ID should be play session ID or playerID on server side. This can be used to give each player a unique set of purchasable items.
                var customer = new Customer<IItemInstance>(System.Guid.NewGuid(), player, player.itemCollectionGroup, player.currencyCollectionGroup);
                var bought = _vendorUI.vendor.BuyFromVendor(customer, _slot.current.item, 1);
                if (bought.error != null)
                {
                    _logger.Error("Item could not be bought", bought.error);
                }
//                else
//                {
//                    _logger.LogVerbose($"Bought item {bought.result.item} x {bought.result.amount}", this);
//                }
                
                if (consumeEvent)
                {
                    eventData.Use();
                }
            }
        }
    }
}