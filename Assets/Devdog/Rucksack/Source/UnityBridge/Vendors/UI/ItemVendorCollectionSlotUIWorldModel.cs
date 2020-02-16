using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemVendorCollectionSlotUIWorldModel : CollectionSlotUIWorldModelBase<IVendorProduct<IItemInstance>>, ICollectionSlotUICallbackReceiver<IVendorProduct<IItemInstance>>
    {
        public void Repaint(IVendorProduct<IItemInstance> product, int amount)
        {
            if (worldModelParent == null)
            {
                return;
            }
            
            var def = product?.item?.itemDefinition as IUnityItemDefinition;
            if (product != null && def != null)
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