using System.Collections.Generic;
using System.Linq;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Characters
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class UNetInventoryPlayer : InventoryPlayer
    {
        protected NetworkIdentity identity;

//        public CollectionGroup<IItemInstance> serverItemCollectionGroup { get; set; }
//        public CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>> serverEquipmentCollectionGroup { get; set; }
//        public CurrencyCollectionGroup<ICurrency> serverCurrencyCollectionGroup { get; set; }
        
        public UNetInventoryPlayer()
        {
            logger = new UnityLogger("[UNet][player] ");
            
            itemCollectionGroup = new CollectionGroup<IItemInstance>();
            equipmentCollectionGroup = new CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>>();
            currencyCollectionGroup = new CurrencyCollectionGroup<ICurrency>();
        }
        
        protected override void Awake()
        {
            identity = GetComponent<NetworkIdentity>();
            UNetPermissionsRegistry.collections.AddEventListener(identity, OnCollectionPermissionChanged);
//            UNetPermissionsRegistry.collections.AddEventListener(identity, OnCollectionPermissionChanged);
            
            base.Awake();
        }
        
        protected virtual void OnCollectionPermissionChanged(PermissionChangedResult<IUNetCollection, NetworkIdentity> data)
        {
            // Build collection groups when permission has changed. Use the server collections if possible, otherwise use client collections.
            // NOTE: Could use data.permission > None to add/remove directly from the collectionGroup rather than re-creating a completely new collection every time.
            
            if (itemCollections.Contains(data.obj.collectionName))
            {
                // Collection should be item collection
                var colPermission = UNetPermissionsRegistry.collections.GetAllWithPermission(identity);
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
                        if(clientList.Contains(col) == false)
                        {
                            clientList.Add(col);
                        }
                    }

                    if (ServerCollectionRegistry.byID.Contains(col))
                    {
                        if(serverList.Contains(col) == false)
                        {
                            serverList.Add(col);
                        }
                    }                    
                }
                
                itemCollectionGroup.Set(serverList.ToArray());
                if (identity.isClient && identity.isServer == false)
                {
                    itemCollectionGroup.Set(clientList.ToArray());
                }
            }
            else if (equipmentCollections.Contains(data.obj.collectionName))
            {
                // Collection should be equipment collection
                var colPermission = UNetPermissionsRegistry.collections.GetAllWithPermission(identity);
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
                        if(clientList.Contains(col) == false)
                        {
                            clientList.Add(col);
                        }
                    }

                    if (ServerCollectionRegistry.byID.Contains(col))
                    {
                        if(serverList.Contains(col) == false)
                        {
                            serverList.Add(col);
                        }
                    }                    
                }
                
                equipmentCollectionGroup.Set(serverList.ToArray());
                if (identity.isClient && identity.isServer == false)
                {
                    equipmentCollectionGroup.Set(clientList.ToArray());
                }
            }
            else if (currencyCollections.Contains(data.obj.collectionName))
            {
                // Collection should be currency collection
                // Collection should be equipment collection
                var colPermission = UNetPermissionsRegistry.collections.GetAllWithPermission(identity);
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
                        if(clientList.Contains(col) == false)
                        {
                            clientList.Add(col);
                        }
                    }

                    if (ServerCurrencyCollectionRegistry.byID.Contains(col))
                    {
                        if(serverList.Contains(col) == false)
                        {
                            serverList.Add(col);
                        }
                    }                    
                }
                
                currencyCollectionGroup.Set(serverList.ToArray());
                if (identity.isClient && identity.isServer == false)
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