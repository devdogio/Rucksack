using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    /// <summary>
    /// A collection itemGuid for IUnityItemInstance collections.
    /// </summary>
    [AddComponentMenu(RucksackConstants.AddPath + nameof(ItemDefinitionCollectionSlotUI), 0)]
    public class ItemDefinitionCollectionSlotUI : CollectionSlotUIBase<IUnityItemDefinition>
    {
        [Header("Item info")]
        [SerializeField]
        private Text _itemName;
        public Text itemName
        {
            get { return _itemName; }
        }

        [SerializeField]
        private Text _itemDescription;
        public Text itemDescription
        {
            get { return _itemDescription; }
        }

        [SerializeField]
        private Image _itemIcon;
        public Image itemIcon
        {
            get { return _itemIcon; }
        }
        
        public sealed override void Repaint(IUnityItemDefinition item, int amount)
        {
            if (current != null)
            {
                Set(itemName, item.name);
                Set(itemDescription, item.description);
                Set(itemIcon, item.icon);
            }
            else
            {
                Set(itemName, string.Empty);
                Set(itemDescription, string.Empty);
                Set(itemIcon, null);
            }

            base.Repaint(item, amount);
        }
    }
}