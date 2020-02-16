using System;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using TMPro;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.TMP
{
    public abstract class TMP_ItemCollectionUIBase<TItemInstance>: 
        UIMonoBehaviour,
        ICollectionSlotUICallbackReceiver<TItemInstance>
        where TItemInstance : IEquatable<TItemInstance>
    {
         #region Inspector Fields
        [Header("Item info")]
        [SerializeField]
        private TMP_Text _itemName;
        public TMP_Text itemName
        {
            get { return _itemName; }
        }

        [SerializeField]
        private TMP_Text _itemDescription;
        public TMP_Text itemDescription
        {
            get { return _itemDescription; }
        }

        [SerializeField]
        private TMP_Text _stackSize;
        public TMP_Text stackSize
        {
            get { return _stackSize; }
        }
        #endregion

        private CollectionSlotUIBase<TItemInstance> _slotUIInstance;
        private readonly Devdog.Rucksack.ILogger _logger = new UnityLogger("[UI][Collection] ");

        protected CollectionSlotUIBase<TItemInstance> slotUI
        {
            get
            {
                if (_slotUIInstance == null)
                {
                    _slotUIInstance = (CollectionSlotUIBase<TItemInstance>) GetComponent<CollectionSlotUIBase>();
                }

                return _slotUIInstance;
            }
        }

        protected void Set(string name, string description, string stackSize)
        {
            this.Set(_itemName, name);
            this.Set(_itemDescription, description);
            this.Set(_stackSize, stackSize);
        }

        protected string GetStackSize(int amount, int maxStackSize)
        {
            return string.Format(
                GetStackSizeFormat(),
                amount,
                maxStackSize);
        }

        protected abstract IUnityItemDefinition GetItemDefinition(TItemInstance item);
        
        public void Repaint(TItemInstance item, int amount)
        {
            if (item != null)
            {
                var def = GetItemDefinition(item);
                
                Repaint(def, amount);
            }
            else
            {
                Set(string.Empty, string.Empty, string.Empty);
            }
        }
        
        protected void Repaint(IUnityItemDefinition def, int amount)
        {
            if (def == null)
            {
                _logger.Warning($"Trying to add a item definition to collection that doesn't implement {nameof(IUnityItemDefinition)} - Can't add item to collection UI", this);
                return;
            }

            Set(def.name, def.description, GetStackSize(amount, def.maxStackSize));
        }
        
        //TODO: This methods shows a common interface is necessary for slots UI.
        protected abstract string GetStackSizeFormat();
       /* {
            var itemCollectionSlotUI = slotUI.GetComponent<ItemCollectionSlotUI>();

            if(itemCollectionSlotUI != null)
            {
                return itemCollectionSlotUI.stackSizeFormat;
            }

            var equipmentCollectionSlotUI = slotUI.GetComponent<EquipmentCollectionSlotUI>();

            if (equipmentCollectionSlotUI != null)
            {
                return equipmentCollectionSlotUI.stackSizeFormat;
            }

            var itemVendorCollectionSlotUI = slotUI.GetComponent<ItemVendorCollectionSlotUI>();

            if (itemVendorCollectionSlotUI != null)
            {
                return itemVendorCollectionSlotUI.stackSizeFormat;
            }

            return null; //TODO: Should an error be reported?
        }*/
    }
}