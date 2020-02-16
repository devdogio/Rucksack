using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Vendors
{
    public class ItemVendorCreator : BaseCollectionCreator<ICollection<IVendorProduct<IItemInstance>>>
    {
        [Header("Vendor"), SerializeField]
        private SerializedGuid _vendorGuid;
        [SerializeField]
        private bool _generateVendorIDOnStart = false;
        [SerializeField]
        private VendorConfig _config;

        [Header("Layout Settings"), SerializeField]
        private int _slotCount = 1;

        [Header("Item"), SerializeField]
        private bool _generateItemsOnStart = true;
        [SerializeField]
        private bool _randomItems = false;
        [SerializeField]
        private UnityItemDefinition[] _itemDefs = new UnityItemDefinition[0];

        /// <summary>
        /// Vendor
        /// </summary>
        public Vendor<IItemInstance> vendor { get; protected set; }

        /// <summary>
        /// Vendor unique ID
        /// </summary>
        public System.Guid vendorGuid {
            get { return _vendorGuid.guid; }
            set { _vendorGuid.guid = value; }
        }

        /// <summary>
        /// The number of slots in collection to be created.
        /// </summary>
        public int slotCount
        {
            get { return _slotCount; }
            set { _slotCount = value; }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slotCount <= 0)
            {
                _slotCount = 1;
            }
        }
#endif

        public override void Initialize()
        {
            base.Initialize();

            if (_generateVendorIDOnStart)
            {
                _vendorGuid.guid = Guid.NewGuid();
            }

            vendor = CreateVendor();
            RegisterVendor(vendor);

            if (_generateItemsOnStart)
            {
                GenerateItems(_itemDefs);
            }
        }

        public virtual void GenerateItems(UnityItemDefinition[] items)
        {
            // TODO: Make better vendor item generator (item amounts, randomization, etc)
            foreach (var itemDef in items)
            {
                if (itemDef == null)
                {
                    continue;
                }

                var inst = ItemFactory.CreateInstance(itemDef, System.Guid.NewGuid());
                collection.Add(new VendorProduct<IItemInstance>(inst, itemDef.buyPrice, itemDef.sellPrice));
            }
        }

        protected virtual void RegisterVendor(Vendor<IItemInstance> vendor)
        {
            VendorRegistry.itemVendors.Register(vendorGuid, vendor);
        }

        protected virtual Vendor<IItemInstance> CreateVendor()
        {
            return new UnityVendor<IItemInstance>(vendorGuid, collectionName, collectionID, _config, collection, new InfiniteCurrencyCollection()); // TODO: Make currency customizable
        }

        protected override ICollection<IVendorProduct<IItemInstance>> CreateCollection()
        {
            var coll = new Collection<IVendorProduct<IItemInstance>>(Mathf.Max(slotCount, _itemDefs.Length));
            coll.collectionName = this.collectionName;

            return coll;
        }

        protected override void RegisterByName(ICollection<IVendorProduct<IItemInstance>> col)
        {
            CollectionRegistry.byName.Register(collectionName, col);
        }

        protected override void RegiterByID(ICollection<IVendorProduct<IItemInstance>> col)
        {
            CollectionRegistry.byID.Register(collectionID, col);
        }

        protected override void UnRegister()
        {
            if (CollectionRegistry.byName != null)
                CollectionRegistry.byName.UnRegister(collectionName);

            if (CollectionRegistry.byID != null)
                CollectionRegistry.byID.UnRegister(collectionID);
        }
    }
}