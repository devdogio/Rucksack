using Devdog.General2;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Vendors
{
    public sealed class UNetItemVendorEnabler : NetworkBehaviour, ITriggerCallbacks
    {
        [Header("Vendor")]
        [SerializeField]
        private SerializedGuid _vendorGuid;
        public System.Guid vendorGuid
        {
            get { return _vendorGuid.guid; }
        }
        
//        [SerializeField]
//        private SerializedGuid _vendorCollectionGuid;

        [SerializeField]
        private string _vendorCollectionName;
        
        private NetworkIdentity _identity;
        private ItemVendorUI _currentVendorUI;

        private void Awake()
        {
            _identity = GetComponent<NetworkIdentity>();
        }
        
//        [ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData data)
        {
            if (isServer)
            {
                var vendor = ServerVendorRegistry.itemVendors.Get(vendorGuid) as UNetVendor<IItemInstance>;
                var bridge = character.GetComponent<UNetActionsBridge>();
                if (character is Player && bridge != null && vendor != null)
                {
                    // Register collection on client
                    bridge.Server_AddVendorItemCollectionToClient(new AddCollectionMessage()
                    {
                        collectionGuid = vendor.vendorCollectionGuid,
                        collectionName = vendor.vendorCollectionName,
                        owner = _identity,
                        slotCount = 10,
                    });
                
                    // Give the client read permission to the vendor's items.
                    // TODO: Consider putting this in a single call with registration (add collection call)
                    bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                    {
                        collectionGuid = vendor.vendorCollectionGuid,
                        permission = ReadWritePermission.Read
                    });

                    bridge.Server_SetItemVendorCollectionContentsOnClient(vendor.vendorGuid, vendor.vendorCollectionGuid, vendor.vendorCollection);
                }
            }
            
            if (isClient && character == PlayerManager.currentPlayer)
            {
                var vendor = VendorRegistry.itemVendors.Get(vendorGuid) as UNetVendor<IItemInstance>;
                var vendors = FindObjectsOfType<ItemVendorUI>();
                foreach (var vendorUI in vendors)
                {
                    if (vendorUI.collectionName == _vendorCollectionName)
                    {
                        vendorUI.vendor = vendor;
                        vendorUI.collection = vendor?.vendorCollection;
                        vendorUI.window.Show();

                        _currentVendorUI = vendorUI;
                        return;
                    }
                }
                
                new UnityLogger("[Vendor] ").Warning("Couldn't find VendorUI that repaints collection " + vendor?.vendorCollectionName);
            }
        }

//        [ServerCallback]
        public void OnTriggerUnUsed(Character character, TriggerEventData data)
        {
            if (isServer)
            {
                var vendor = ServerVendorRegistry.itemVendors.Get(vendorGuid) as UNetVendor<IItemInstance>;
                var bridge = character.GetComponent<UNetActionsBridge>();
                if (character is Player && bridge != null && vendor != null)
                {
                    // Revoke the client's read access on the vendor's collection
                    bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                    {
                        collectionGuid = vendor.vendorCollectionGuid,
                        permission = ReadWritePermission.None
                    });
                }
            }
            
            if (isClient && character == PlayerManager.currentPlayer)
            {
                _currentVendorUI?.window.Hide();
            }
        }
    }
}