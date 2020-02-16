using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Vendors
{
    public sealed class UNetItemVendorCreator : NetworkBehaviour
    {
        [Header("Vendor")]
        [SerializeField]
        private SerializedGuid _vendorGuid;
        public System.Guid vendorGuid
        {
            get { return _vendorGuid.guid; }
        }

        [SerializeField]
        private VendorConfig _config;
        
        [Header("Vendor collection")]
//        [SerializeField]
//        private int _slotCount;

        [SerializeField]
        private string _vendorCollectionName;

        [SerializeField]
        private SerializedGuid _vendorCollectionGuid;

        [Header("Item")]
        [SerializeField]
        private UnityItemDefinition[] _itemDefs = new UnityItemDefinition[0];
        
        // TODO: Add option to specify vendor's items / generate set of items
        public UNetServerItemVendorCollection collection { get; private set; }
        
        private UNetVendor<IItemInstance> _vendor;
        private NetworkIdentity _identity;

        private void Awake()
        {
            _identity = GetComponent<NetworkIdentity>();
        }
        
//        [ServerCallback]
        private void Start()
        {
            if (isServer)
            {
                collection = UNetCollectionUtility.CreateServerVendorItemCollection(10, _vendorCollectionName, _vendorCollectionGuid.guid, _identity);
                foreach (var itemDef in _itemDefs)
                {
                    if (itemDef == null)
                    {
                        continue;
                    }

                    var inst = ItemFactory.CreateInstance(itemDef, Guid.NewGuid());
                    var prod = new VendorProduct<IItemInstance>(inst, itemDef.buyPrice, itemDef.sellPrice);
                    collection.Add(prod);
                }
            }
            
            _vendor = new UNetVendor<IItemInstance>(_vendorGuid.guid, _vendorCollectionName, _vendorCollectionGuid.guid, _config, _identity, collection, new InfiniteCurrencyCollection()); // TODO: Make currency customizable

            if (isServer)
            {
                ServerVendorRegistry.itemVendors.Register(_vendorGuid.guid, _vendor);
            }

            if (isClient)
            {
                VendorRegistry.itemVendors.Register(_vendorGuid.guid, _vendor);
            }
        }

        [ServerCallback]
        private void OnDestroy()
        {
            ServerVendorRegistry.itemVendors.UnRegister(_vendorGuid.guid);
        }
    }
}