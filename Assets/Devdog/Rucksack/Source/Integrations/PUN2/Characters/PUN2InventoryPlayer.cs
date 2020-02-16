using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Characters
{
    [RequireComponent(typeof(PhotonView))]
    public class PUN2InventoryPlayer : InventoryPlayer
    {
        protected PhotonView identity;
        public PUN2InventoryPlayer()
        {
            logger = new UnityLogger("[PUN2][player] ");

            itemCollectionGroup = new CollectionGroup<IItemInstance>();
            equipmentCollectionGroup = new CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>>();
            currencyCollectionGroup = new CurrencyCollectionGroup<ICurrency>();
        }

        protected override void Awake()
        {
            identity = GetComponent<PhotonView>();
            PUN2PermissionsRegistry.collections.AddEventListener(this.identity, OnCollectionPermissionChanged);

            base.Awake();
        }

        private void OnCollectionPermissionChanged(PermissionChangedResult<IPUN2Collection, PhotonView> data)
        {
            // Build collection groups when permission has changed. Use the server collections if possible, otherwise use client collections.
            // NOTE: Could use data.permission > None to add/remove directly from the collectionGroup rather than re-creating a completely new collection every time.

            if (itemCollections.Contains(data.obj.collectionName))
            {
                // Collection should be item collection
                var colPermission = PUN2PermissionsRegistry.collections.GetAllWithPermission(identity);
                var clientList = new List<Collections.ICollection<IItemInstance>>();
                var serverList = new List<Collections.ICollection<IItemInstance>>();
                foreach (var permission in colPermission)
                {
                    var col = permission.obj as Collections.ICollection<IItemInstance>;
                    if (col == null)
                    {
                        continue;
                    }

                    if (itemCollections.Contains(permission.obj.collectionName) == false)
                    {
                        continue;
                    }

                    if (CollectionRegistry.byID.Contains(col))
                    {
                        if (clientList.Contains(col) == false)
                        {
                            clientList.Add(col);
                        }
                    }

                    if (ServerCollectionRegistry.byID.Contains(col))
                    {
                        if (serverList.Contains(col) == false)
                        {
                            serverList.Add(col);
                        }
                    }
                }

                itemCollectionGroup.Set(serverList.ToArray());
                
                if (!PhotonNetwork.LocalPlayer.IsMasterClient /*identity.isClient && identity.isServer == false*/)
                {
                    itemCollectionGroup.Set(clientList.ToArray());
                }
            }
            else if (equipmentCollections.Contains(data.obj.collectionName))
            {
                // Collection should be equipment collection
                var colPermission = PUN2PermissionsRegistry.collections.GetAllWithPermission(identity);
                var clientList = new List<IEquipmentCollection<IEquippableItemInstance>>();
                var serverList = new List<IEquipmentCollection<IEquippableItemInstance>>();
                foreach (var permission in colPermission)
                {
                    var col = permission.obj as IEquipmentCollection<IEquippableItemInstance>;
                    if (col == null)
                    {
                        continue;
                    }

                    if (equipmentCollections.Contains(permission.obj.collectionName) == false)
                    {
                        continue;
                    }

                    if (CollectionRegistry.byID.Contains(col))
                    {
                        if (clientList.Contains(col) == false)
                        {
                            clientList.Add(col);
                        }
                    }

                    if (ServerCollectionRegistry.byID.Contains(col))
                    {
                        if (serverList.Contains(col) == false)
                        {
                            serverList.Add(col);
                        }
                    }
                }

                equipmentCollectionGroup.Set(serverList.ToArray());
                if (!PhotonNetwork.LocalPlayer.IsMasterClient /*identity.isClient && identity.isServer == false*/)
                {
                    equipmentCollectionGroup.Set(clientList.ToArray());
                }
            }
            else if (currencyCollections.Contains(data.obj.collectionName))
            {
                // Collection should be currency collection
                // Collection should be equipment collection
                var colPermission = PUN2PermissionsRegistry.collections.GetAllWithPermission(identity);
                var clientList = new List<ICurrencyCollection<ICurrency, double>>();
                var serverList = new List<ICurrencyCollection<ICurrency, double>>();
                foreach (var permission in colPermission)
                {
                    var col = permission.obj as ICurrencyCollection<ICurrency, double>;
                    if (col == null)
                    {
                        continue;
                    }

                    if (currencyCollections.Contains(permission.obj.collectionName) == false)
                    {
                        continue;
                    }

                    if (CurrencyCollectionRegistry.byID.Contains(col))
                    {
                        if (clientList.Contains(col) == false)
                        {
                            clientList.Add(col);
                        }
                    }

                    if (ServerCurrencyCollectionRegistry.byID.Contains(col))
                    {
                        if (serverList.Contains(col) == false)
                        {
                            serverList.Add(col);
                        }
                    }
                }

                currencyCollectionGroup.Set(serverList.ToArray());
                if (!PhotonNetwork.LocalPlayer.IsMasterClient /*identity.isClient && identity.isServer == false*/)
                {
                    currencyCollectionGroup.Set(clientList.ToArray());
                }
            }
        }

        protected override void RegisterCollectionListeners()
        {
            // Ignored
        }

        protected override void UnRegisterCollectionListeners()
        {
            // Ignored
        }
    }
}
