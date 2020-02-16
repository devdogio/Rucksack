using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;

namespace Devdog.Rucksack.Integrations.TMP
{
    public sealed class TMP_EquipmentCollectionSlotUI : 
        TMP_ItemCollectionUIBase<IEquippableItemInstance>
    {
        protected override IUnityItemDefinition GetItemDefinition(IEquippableItemInstance item)
        {
            return item.itemDefinition as IUnityItemDefinition;
        }

        protected override string GetStackSizeFormat()
        {
            var collectionSlotUI = slotUI.GetComponent<EquipmentCollectionSlotUI>();

            if (collectionSlotUI != null)
            {
                return collectionSlotUI.stackSizeFormat;
            }

            return null; //TODO: Should an error be reported?
        }
    }
}