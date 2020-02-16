using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using Photon.Pun;
using UnityEngine;

namespace Devdog.Rucksack.Vendors
{
    public sealed class PUN2ItemVendorCreator : MonoBehaviourPun
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

        public string VendorCollectionName { get { return this._vendorCollectionName; } }

        [SerializeField]
        private SerializedGuid _vendorCollectionGuid;

        [SerializeField]
        private ushort _slotCount = 10;

        [Header("Item")]
        [SerializeField]
        private UnityItemDefinition[] _itemDefs = new UnityItemDefinition[0];

        // TODO: Add option to specify vendor's items / generate set of items
        public PUN2ServerItemVendorCollection collection { get; private set; }

        private void Start()
        {
            PUN2Vendor<IItemInstance> _vendor = null;

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                collection = PUN2CollectionUtility.CreateServerVendorItemCollection(Math.Max(this._itemDefs.Length, this._slotCount), _vendorCollectionName, _vendorCollectionGuid.guid, this.photonView);
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

                _vendor = new PUN2Vendor<IItemInstance>(_vendorGuid.guid, _vendorCollectionName, _vendorCollectionGuid.guid, _config, this.photonView, collection, new InfiniteCurrencyCollection()); // TODO: Make currency customizable

                //if (PhotonNetwork.LocalPlayer.IsMasterClient)
                ServerVendorRegistry.itemVendors.Register(_vendorGuid.guid, _vendor);
            }

            // Create on new vendor here for "client" purposes. This way, changing one vendor ('client' or 'server') doesn't affect the other.
            _vendor = new PUN2Vendor<IItemInstance>(_vendorGuid.guid, _vendorCollectionName, _vendorCollectionGuid.guid, _config, this.photonView, collection, new InfiniteCurrencyCollection()); // TODO: Make currency customizable
            VendorRegistry.itemVendors.Register(_vendorGuid.guid, _vendor);
        }

        private void OnDestroy()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                ServerVendorRegistry.itemVendors.UnRegister(_vendorGuid.guid);
        }
    }
}