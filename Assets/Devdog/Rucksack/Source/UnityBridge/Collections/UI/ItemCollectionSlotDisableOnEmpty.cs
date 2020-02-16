using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    public class ItemCollectionSlotDisableOnEmpty : MonoBehaviour, ICollectionSlotUICallbackReceiver<IItemInstance>
    {
        public void Repaint(IItemInstance item, int amount)
        {
            gameObject.SetActive(item != null);
        }
    }
}