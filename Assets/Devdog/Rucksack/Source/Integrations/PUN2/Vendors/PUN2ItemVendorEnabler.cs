using Devdog.General2;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using UnityEngine;

using Photon.Pun;
using System;

namespace Devdog.Rucksack.Vendors
{
    [RequireComponent(typeof(PUN2ItemVendorCreator))]
    public sealed class PUN2ItemVendorEnabler : MonoBehaviourPun, ITriggerCallbacks
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

        //public string VendorCollectionName { get { return this._vendorCollectionName; } }

        private ItemVendorUI _currentVendorUI;

        void Reset()
        {
            var creator = GetComponent<PUN2ItemVendorCreator>();
            this._vendorGuid = new SerializedGuid() { guid = creator != null ? creator.vendorGuid : Guid.Empty };
            this._vendorCollectionName = creator?.VendorCollectionName;
        }

        //        [ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData data)
        {
            var bridge = character.GetComponent<PUN2ActionsBridge>();

            if (PhotonNetwork.LocalPlayer.IsMasterClient /*isServer*/)
            {
                var vendor = ServerVendorRegistry.itemVendors.Get(vendorGuid) as PUN2Vendor<IItemInstance>;
                
                if (character is Player && bridge != null && vendor != null)
                {
                    // Register collection on client
                    bridge.Server_AddVendorItemCollectionToClient(
                        /*vendorGuid: */ this.vendorGuid,
                        /*collectionName: */ vendor.vendorCollectionName,
                        /*collectionGuid: */ vendor.vendorCollectionGuid,
                        /*slotCount*/ (ushort)vendor.vendorCollection.slotCount
                    );

                    // Give the client read permission to the vendor's items.
                    // TODO: Consider putting this in a single call with registration (add collection call)
                    bridge.Server_SetCollectionPermissionOnServerAndClient(
                        /*collectionGuid: */ vendor.vendorCollectionGuid,
                        /*permission: */ ReadWritePermission.Read
                    );

                    bridge.Server_SetItemVendorCollectionContentsOnClient(vendor.vendorCollectionGuid, vendor.vendorCollection);
                }

                // Send back an RPC to the client that it can show its UI
                bridge.photonView.RPC(nameof(bridge.TargetRPC_ShowItemVendorUI), bridge.photonView.Owner, this.vendorGuid.ToByteArray());
            }
        }

        public void RPC_ShowUI()
        {
            var vendor = VendorRegistry.itemVendors.Get(vendorGuid) as PUN2Vendor<IItemInstance>;
            
            if (vendor == null)
                new UnityLogger("[Vendor] ").Error($"Couldn't find vendor for guid: '{vendorGuid}'", this);
            
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
            
            new UnityLogger("[Vendor] ").Warning("Couldn't find VendorUI that repaints collection " + vendor?.vendorCollectionName, this);
        }

        //        [ServerCallback]
        public void OnTriggerUnUsed(Character character, TriggerEventData data)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient /*isServer*/)
            {
                var vendor = ServerVendorRegistry.itemVendors.Get(vendorGuid) as PUN2Vendor<IItemInstance>;
                var bridge = character.GetComponent<PUN2ActionsBridge>();
                if (character is Player && bridge != null && vendor != null)
                {
                    // Revoke the client's read access on the vendor's collection
                    bridge.Server_SetCollectionPermissionOnServerAndClient(
                        /*collectionGuid: */ vendor.vendorCollectionGuid,
                        /*permission: */ ReadWritePermission.None
                    );
                }
            }

            if (/*!PhotonNetwork.LocalPlayer.IsMasterClient*/ /*isClient*/ character == PlayerManager.currentPlayer)
            {
                _currentVendorUI?.window.Hide();
            }
        }
    }
}