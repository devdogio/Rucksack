using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemCollectionSlotUIWorldModel : CollectionSlotUIWorldModelBase<IItemInstance>, ICollectionSlotUICallbackReceiver<IItemInstance>
    {
        public void Repaint(IItemInstance item, int amount)
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