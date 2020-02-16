using Devdog.General2;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using UnityEngine;

namespace Devdog.Rucksack.Vendors
{
    public sealed class ItemVendorEnabler : MonoBehaviour, ITriggerCallbacks
    {
        [Header("Vendor")]
        [SerializeField]
        private SerializedGuid _vendorGuid;
        
        private ItemVendorUI _activeUI;
        public void OnTriggerUsed(Character character, TriggerEventData data)
        {
            var vendor = VendorRegistry.itemVendors.Get(_vendorGuid.guid) as UnityVendor<IItemInstance>;
            if (vendor != null)
            {
                var vendors = FindObjectsOfType<ItemVendorUI>();
                foreach (var vendorUI in vendors)
                {
                    if (vendorUI.collectionName == vendor.vendorCollectionName)
                    {
                        vendorUI.vendor = vendor;
                        vendorUI.collection = vendor.vendorCollection;
                        vendorUI.window.Show();

                        _activeUI = vendorUI;
                        
                        return;
                    }
                }
                
                new UnityLogger("[Vendor] ").Warning("Couldn't find VendorUI that repaints collection for vendor: " + vendor.vendorCollectionName);
                return;
            }
            
            new UnityLogger("[Vendor] ").Warning("Couldn't find VendorUI that repaints for vendor: " + _vendorGuid);
        }

        public void OnTriggerUnUsed(Character character, TriggerEventData data)
        {
            if (_activeUI != null)
            {
                _activeUI.window.Hide();
            }
        }
    }
}