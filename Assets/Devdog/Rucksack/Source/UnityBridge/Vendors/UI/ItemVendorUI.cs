using Devdog.General2;
using Devdog.General2.UI;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.Rucksack.UI
{
    public class ItemVendorUI : VendorUIBase<IItemInstance>
    {
        private CollectionUIBase<IVendorProduct<IItemInstance>> _itemCollectionUI;

        public CollectionUIBase<IVendorProduct<IItemInstance>> itemCollectionUI
        {
            get { return _itemCollectionUI; }
        }

        public string collectionName
        {
            get { return _itemCollectionUI.collectionName; }
        }

        public ICollection<IVendorProduct<IItemInstance>> collection
        {
            get { return _itemCollectionUI.collection; }
            set { _itemCollectionUI.collection = value; }
        }

        public override IVendor<IItemInstance> vendor { get; set; }

        public UIWindow window { get; protected set; }

        protected virtual void Awake()
        {
            _itemCollectionUI = GetComponent<CollectionUIBase<IVendorProduct<IItemInstance>>>();
            Assert.IsNotNull(_itemCollectionUI, "No itemCollectionUI");

            window = GetComponent<UIWindow>();
        }

        protected virtual void Start()
        {
        }

        void Reset()
        {
            _itemCollectionUI = GetComponent<CollectionUIBase<IVendorProduct<IItemInstance>>>();
        }
    }
}