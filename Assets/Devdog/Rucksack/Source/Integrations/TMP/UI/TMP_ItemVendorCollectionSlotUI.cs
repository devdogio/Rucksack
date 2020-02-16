using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using Devdog.Rucksack.Vendors;

namespace Devdog.Rucksack.Integrations.TMP
{
    public sealed class TMP_ItemVendorCollectionSlotUI : 
        TMP_ItemCollectionUIBase<IVendorProduct<IItemInstance>>
    {
        protected override IUnityItemDefinition GetItemDefinition(IVendorProduct<IItemInstance> item)
        {
            return item.item.itemDefinition as IUnityItemDefinition;
        }
        
        protected override string GetStackSizeFormat()
        {
            var collectionSlotUI = slotUI.GetComponent<ItemVendorCollectionSlotUI>();

            if (collectionSlotUI != null)
            {
                return collectionSlotUI.stackSizeFormat;
            }

            return null; //TODO: Should an error be reported?
        }
    }
}