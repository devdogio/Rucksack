using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    public class EquipmentCollectionSlotUI : CollectionSlotUIBase<IEquippableItemInstance>
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
        
        public override void Repaint(IEquippableItemInstance item, int amount)
        {
            if (current != null)
            {
                var def = current.itemDefinition as IUnityItemDefinition;
                if (def == null)
                {
                    logger.Warning($"Trying to add a item definition to collection that doesn't implement {nameof(IUnityItemDefinition)} - Can't add item to collection UI", this);
                    return;
                }
            
                Set(_itemName, def.name);
                Set(_itemDescription, def.description);
                Set(_itemIcon, def.icon);
                SetActive(_cooldown, true);
                Set(_stackSize, string.Format(_stackSizeFormat, amount, current.itemDefinition.maxStackSize));
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