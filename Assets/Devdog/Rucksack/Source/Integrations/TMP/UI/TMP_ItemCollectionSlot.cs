using Devdog.Rucksack.UI;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Integrations.TMP
{
    /// <summary>
    /// Use this class in addition another slot component if you want to use TextMesh Pro
    /// instead of unity Text.
    ///  </summary>
    /// <remarks>Compatible with <see cref="ItemCollectionSlotUI"/>, <see cref="ItemVendorCollectionSlotUI"/> and
    /// <see cref="EquipmentCollectionSlotUI"/>. 
    /// </remarks>
    public sealed class TMP_ItemCollectionSlot : 
        TMP_ItemCollectionUIBase<IItemInstance>
    {
        protected override IUnityItemDefinition GetItemDefinition(IItemInstance item)
        {
            return item.itemDefinition as IUnityItemDefinition;
        }
        
        protected override string GetStackSizeFormat()
        {
            var collectionSlotUI = slotUI.GetComponent<ItemCollectionSlotUI>();

            if (collectionSlotUI != null)
            {
                return collectionSlotUI.stackSizeFormat;
            }

            return null; //TODO: Should an error be reported?
        }
    }
}