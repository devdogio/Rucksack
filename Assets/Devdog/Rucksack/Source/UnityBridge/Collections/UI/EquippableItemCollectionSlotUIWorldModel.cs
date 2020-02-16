using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.UI
{
    public sealed class EquippableItemCollectionSlotUIWorldModel : CollectionSlotUIWorldModelBase<IEquippableItemInstance>, ICollectionSlotUICallbackReceiver<IEquippableItemInstance>
    {
        public void Repaint(IEquippableItemInstance item, int amount)
        {
            if (worldModelParent == null)
            {
                return;
            }
            
            var def = item?.itemDefinition as IUnityItemDefinition;
            if (item != null && def != null)
            {
                Create3DModel(worldModelParent, def.worldModel);
            }
            else
            {
                Clean();
            }
        }
    }
}