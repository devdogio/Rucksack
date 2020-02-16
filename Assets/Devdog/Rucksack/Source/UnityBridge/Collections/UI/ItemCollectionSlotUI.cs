using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    /// <summary>
    /// A collection itemGuid for IUnityItemInstance collections.
    /// </summary>
    [AddComponentMenu(RucksackConstants.AddPath + nameof(ItemCollectionSlotUI), 0)]
    public class ItemCollectionSlotUI : CollectionSlotUIBase<IItemInstance>
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

        // TODO: Use complex object to set progress
        [Header("Item Properties")]
        [SerializeField]
        private Image _cooldown;
        public Image cooldown
        {
            get { return _cooldown; }
        }

        [SerializeField]
        private Text _stackSize;
        public Text stackSize
        {
            get { return _stackSize; }
        }

        /// <summary>
        /// The stack size format used for string.Format().
        /// {0} = Current stack size
        /// {1} = Max stack size
        /// </summary>
        [SerializeField]
        private string _stackSizeFormat = "{0}/{1}";
        public string stackSizeFormat
        {
            get { return _stackSizeFormat; }
        }
        
        public sealed override void Repaint(IItemInstance item, int amount)
        {
            if (item != null)
            {
                var inst = item as IUnityItemInstance;
                if (inst == null)
                {
                    logger.Warning($"Trying to add an item instance to collection that doesn't implement {nameof(IUnityItemInstance)} - Can't add item to collection UI. Type is {item.GetType().FullName}", this);
                    return;
                }
            
                Set(_itemName, inst.itemDefinition.name);
                Set(_itemDescription, inst.itemDefinition.description);
                Set(_itemIcon, inst.itemDefinition.icon);
                SetActive(_cooldown, true);
                Set(_stackSize, string.Format(_stackSizeFormat, amount, inst.itemDefinition.maxStackSize));
            }
            else
            {
                Set(_itemName, string.Empty);
                Set(_itemDescription, string.Empty);
                Set(_itemIcon, null);
                SetActive(_cooldown, false);
                Set(_stackSize, string.Empty);
            }

            base.Repaint(item, amount);
        }
    }
}