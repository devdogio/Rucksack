using System;
using System.Linq;
using Devdog.General2;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Collections.CharacterEquipment;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Database;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack
{
    public class UNetActionsBridge : UNetGeneralActionsBridge //, ICollectionBridge
    {
        [SerializeField]
        private UnityCurrencyDatabase _currencyDatabase;
        public virtual IDatabase<UnityCurrency> currencyDatabase
        {
            get { return _currencyDatabase; }
        }

        [SerializeField]
        private UnityItemDefinitionDatabase _itemsDatabase;
        public virtual IDatabase<UnityItemDefinition> itemsDatabase
        {
            get { return _itemsDatabase; }
        }

        [SerializeField]
        private UnityEquipmentTypeDatabase _equipmentTypeDatabase;
        public virtual IDatabase<UnityEquipmentType> equipmentTypeDatabase
        {
            get { return _equipmentTypeDatabase; }
        }



        [Required]
        [SerializeField]
        private UNetInventoryPlayer _inventoryPlayer;
        public UNetInventoryPlayer inventoryPlayer
        {
            get { return _inventoryPlayer; }
        }

        protected UNetCollectionReplicator collectionReplicator;
        protected UNetEquipmentReplicator equipmentReplicator;
        protected UNetCurrencyReplicator currencyReplicator;
        protected UNetVendorReplicator vendorReplicator;
        
        
        public static UNetCollectionFinder collectionFinder { get; }
        protected static readonly ILogger logger;

        static UNetActionsBridge()
        {
            logger = new UnityLogger("[UNet] ");
            collectionFinder = new UNetCollectionFinder();
        }
        
        public UNetActionsBridge()
        {
            collectionReplicator = new UNetCollectionReplicator(this, logger);
            equipmentReplicator = new UNetEquipmentReplicator(this, logger);
            currencyReplicator = new UNetCurrencyReplicator(this, logger);
            vendorReplicator = new UNetVendorReplicator(this, logger);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UNetPermissionsRegistry.collections.RevokeAllForIdentity(identity);
            UNetPermissionsRegistry.objects.RevokeAllForIdentity(identity);
            
            UNetPermissionsRegistry.collections.RemoveAllEventListenersForIdentity(identity);
            UNetPermissionsRegistry.objects.RemoveAllEventListenersForIdentity(identity);
        }

        /// <summary>
        /// When items are created and stacked they are combined into a single item with a stack size.
        /// The original item will become dangling, but due to the registry won't be cleaned by the GC.
        /// Using this method all dangling items in the registry will be cleaned up...
        /// 
        /// The system can't clean up a 1 of the 2 items when merging, as it might've still been used somewhere else at that point...
        /// 
        /// TODO: Make this run every few seconds maybe? Could become slow if the item count is large... (background thread..?)
        /// </summary>
        [Server]
        public void Server_CleanDanglingItemReferences()
        {
            var all = ServerItemRegistry.GetAll();
            foreach (var item in all)
            {
                var entry = item as ICollectionSlotEntry;
                if (entry != null && entry.collectionEntry != null)
                {
                    if (entry.collectionEntry.amount == 0)
                    {
                        // Item is depleted, yet still exists and has a reference to a collection (itemGuid could be filled by other item now).
                        ServerItemRegistry.UnRegister(item.ID);
                        ItemRegistry.UnRegister(item.ID);
                        logger.Log($"[Server] Cleaned dangling item {item.ID} from item registry", this);
                    }
                }
            }
        }
        
        [Server]
        public void Server_TellClientToRegisterItemInstance(IItemInstance itemInstance)
        {
            if (itemInstance == null)
            {
                logger.Error("Given item instance is null!", this);
                return;
            }

            TargetRpc_RegisterItemInstance(connectionToClient, new RegisterItemInstanceMessage(itemInstance)
            {
                // TODO: Properly serialize the models here...
//                serializedData = JsonSerializer.Serialize(itemInstance, null)
            });
        }
        
        
        
//        [Client]
//        public void Client_RequestItemInstanceFromServer(IIdentifiable identifier, NetworkConnection connection)
//        {
//            if (identifier == null)
//            {
//                logger.Error("Given identifier is null!", this);
//                return;
//            }
//
//            Cmd_RequestItemInstance(new RequestItemInstanceMessage()
//            {
//                guidBytes = identifier.ID.ToByteArray()
//            });
//        }

//        [Command]
//        public void Cmd_RequestItemInstance(RequestItemInstanceMessage request)
//        {
//            var guid = new System.Guid(request.guidBytes);
//            var instance = ServerItemRegistry.Get(guid);
//            if (instance == null)
//            {
//                logger.Log("[Server] Couldn't find item instance with GUID: " + guid);
//                return;
//            }
//            
//            // TODO: Note that the current system doesn't validate anything and gives the requester the information.
//
//            logger.LogVerbose($"[Server] Client requested item with guid: {guid} - Sending...");
//            Server_TellClientToRegisterItemInstance(instance);
//        }
        
        

        // TODO: use better serialization / deserialization. Item instance needs a reference to the item definition it belongs to; 
        // TODO: For simplicity ItemDefinitions have to be persistent on disk
        [TargetRpc]
        private void TargetRpc_RegisterItemInstance(NetworkConnection target, RegisterItemInstanceMessage item)
        {
//            itemReplicator.TargetRpc_RegisterItemInstance(target, item);
            item.TryCreateItemInstance(_itemsDatabase, logger);
        }

        [Server]
        private IUNetCollection GetServerCurrencyCollection(System.Guid guid)
        {
            return collectionFinder.GetServerCurrencyCollection(guid);
        }

        [Client]
        private IUNetCollection GetClientCurrencyCollection(System.Guid guid)
        {
            return collectionFinder.GetClientCurrencyCollection(guid);
        }

        [Server]
        private IUNetCollection GetServerCollection(System.Guid guid)
        {
            return collectionFinder.GetServerCollection(guid);
        }

        [Client]
        private IUNetCollection GetClientCollection(System.Guid guid)
        {
            return collectionFinder.GetClientCollection(guid);
        }
        
        
        
        [Command]
        public void Cmd_RequestSwapOrMerge(RequestSwapOrMergeMessage data)
        {
            collectionReplicator.Cmd_RequestSwapOrMerge(data);
        }
        
        [Command]
        public void Cmd_RequestMoveAuto(RequestMoveAutoMessage data)
        {
            collectionReplicator.Cmd_RequestMoveAuto(data);
        }
        
        [Command]
        public void Cmd_RequestUseItem(RequestUseItemMessage data)
        {
            collectionReplicator.Cmd_RequestUseItem(data);
        }

        [Command]
        public void Cmd_RequestDropItem(RequestDropItemMessage data)
        {
            collectionReplicator.Cmd_RequestDropItem(data);
        }
        
        [TargetRpc]
        public void TargetRpc_NotifyItemUsed(NetworkConnection target, ItemUsedMessage data)
        {
            var item = ItemRegistry.Get(data.itemID) as INetworkItemInstance;
            if (item != null)
            {
                item.Client_NotifyUsed(player, new ItemContext()
                {
                    useAmount = data.amountUsed,
                    targetIndex = data.targetIndex
                });
            }
            else
            {
                logger.Warning("[Client] Server notified of used item, but item not found! :: " + data.itemID);
            }
        }

        [TargetRpc]
        public void TargetRpc_NotifyItemEquipped(NetworkConnection target, ItemEquippedMessage data)
        {
            logger.LogVerbose($"[Client] Server told us to equip item with ID {data.itemDefinitionID} to mountpoint {data.mountPoint}", this);

            var character = GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
            if (character != null)
            {
                var mountPoint = character.mountPoints.FirstOrDefault(o => o.name == data.mountPoint);
                if (mountPoint != null)
                {
                    if (data.itemDefinitionID.guid == System.Guid.Empty)
                    {
                        mountPoint.Clear();
                    }
                    else
                    {
                        var item = itemsDatabase.Get(new Identifier(data.itemDefinitionID.guid));
                        if (item.error == null)
                        {
                            var equippable = item.result as IUnityEquippableItemDefinition;
                            if (equippable != null)
                            {
                                var inst = ItemFactory.CreateInstance(equippable, System.Guid.Empty) as IEquippableItemInstance;

                                mountPoint.Clear();
                                mountPoint.Mount(inst);
                            }
                        }
                        else
                        {
                            logger.Warning($"[Client] Server told us to equip item on this character; But item with ID {data.itemDefinitionID.guid} was not found.", this);
                        }
                    }
                }
                else
                {
                    logger.Warning($"[Client] Tried to visually equip item, but mountpoint with name {data.mountPoint} not found", this);
                }
            }
        }
        
        
        
        
        [Server]
        public void Server_AddVendorItemCollectionToServerAndClient(AddCollectionMessage data)
        {
            Server_AddVendorItemCollectionToServer(data);
            Server_AddVendorItemCollectionToClient(data);
        }

        [Server]
        public void Server_AddVendorItemCollectionToClient(AddCollectionMessage data)
        {
            TargetRpc_AddVendorItemCollection(connectionToClient, data);
        }
        
        [Server]
        public IUNetCollection Server_AddVendorItemCollectionToServer(AddCollectionMessage data)
        {
            // NOTE: The collection might already exist server side on the target objects (for example, a trigger)

            var col = GetServerCollection(data.collectionGuid);
            if (col == null)
            {
                col = UNetCollectionUtility.CreateServerVendorItemCollection(data.slotCount, data.collectionName, data.collectionGuid, data.owner);
            }
            
            return col;
        }
        
        [TargetRpc]
        protected void TargetRpc_AddVendorItemCollection(NetworkConnection target, AddCollectionMessage data)
        {
            vendorReplicator.TargetRpc_AddVendorItemCollection(target, data);
        }

        [Server]
        public void Server_SetItemVendorCollectionContentsOnClient(Guid vendorGuid, Guid vendorCollectionGuid, IReadOnlyCollection<IVendorProduct<IItemInstance>> col)
        {
            var products = new System.Collections.Generic.List<ItemVendorProductMessage>(col.slotCount);
            foreach (var prod in col)
            {
                if (prod != null)
                {
                    products.Add(new ItemVendorProductMessage(prod, prod.collectionEntry.amount));
                }
                else
                {
                    products.Add(new ItemVendorProductMessage());
                }
            }
            
            TargetRpc_SetItemVendorCollectionContents(connectionToClient, new SetItemVendorCollectionContentsMessage()
            {
                collectionGuid = vendorCollectionGuid,
                products = products.ToArray()
            });
        }
        
//        [Server]
//        public void Server_SetItemVendorCollectionContentsOnClient(SetItemVendorCollectionContentsMessage data)
//        {
//            TargetRpc_SetItemVendorCollectionContents(connectionToClient, data);
//            logger.LogVerbose($"[Server] Set item vendor collection contents on client with NetID: {netId}", this);
//        }

        [TargetRpc]
        public void TargetRpc_SetItemVendorCollectionContents(NetworkConnection target, SetItemVendorCollectionContentsMessage data)
        {
            vendorReplicator.TargetRpc_SetItemVendorCollectionContents(target, data);
        }
        
        [Command]
        public void Cmd_RequestSellItemToVendor(RequestSellItemToVendorMessage data)
        {
            vendorReplicator.Cmd_RequestSellItemToVendor(data);

            // Update the entire collection of vendor products when the user sells something.
            var vendor = ServerVendorRegistry.itemVendors.Get(data.vendorGuid) as UNetVendor<IItemInstance>;
            if (vendor != null)
            {
                Server_SetItemVendorCollectionContentsOnClient(vendor.vendorGuid, vendor.vendorCollectionGuid, vendor.vendorCollection);
            }
        }

        [Command]
        public void Cmd_RequestBuyItemFromVendor(RequestBuyItemFromVendorMessage data)
        {
            vendorReplicator.Cmd_RequestBuyItemFromVendor(data);
            
            // Update the entire collection of vendor products when the user buys something.
            var vendor = ServerVendorRegistry.itemVendors.Get(data.vendorGuid) as UNetVendor<IItemInstance>;
            if (vendor != null)
            {
                Server_SetItemVendorCollectionContentsOnClient(vendor.vendorGuid, vendor.vendorCollectionGuid, vendor.vendorCollection);
            }
        }

        

        #region Currencies
        
        [Server]
        public UNetServerCurrencyCollection Server_AddCurrencyCollectionToServer(AddCurrencyCollectionMessage data)
        {
            var col = GetServerCurrencyCollection(data.collectionGuid) as UNetServerCurrencyCollection;
            if (col == null)
            {
                col = UNetCurrencyCollectionUtility.CreateServerCurrencyCollection(data.collectionName, data.collectionGuid, data.owner);
//                unetPlayer.Server_AddCurrencyCollection(col, new CurrencyCollectionPriority<ICurrency>());
            }
            
            return col;
        }

        [Server]
        public void Server_AddCurrencyCollectionToServerAndClient(AddCurrencyCollectionMessage data)
        {
            Server_AddCurrencyCollectionToServer(data);
            TargetRpc_AddCurrencyCollection(connectionToClient, data);
        }

        [TargetRpc]
        protected void TargetRpc_AddCurrencyCollection(NetworkConnection target, AddCurrencyCollectionMessage data)
        {
            currencyReplicator.TargetRpc_AddCurrencyCollection(target, data);
        }

//        [Server]
//        public void Server_SetCurrencyCollectionPermissionOnServer(SetCollectionPermissionMessage data)
//        {
//            var col = GetServerCurrencyCollection(data.collectionGuid);
//            if (col != null)
//            {
//                UNetPermissionsRegistry.collections.SetPermission(col, identity, data.permission);
//            }
//            else
//            {
//                logger.Warning($"Currency collection with guid {data.collectionGuid} is not known to server. Make sure to add the collection before setting state!", this);
//            }
//        }
//        
//        [Server]
//        public void Server_SetCurrencyCollectionPermissionOnServerAndClient(SetCollectionPermissionMessage data)
//        {
//            Server_SetCurrencyCollectionPermissionOnServer(data);
//            TargetRpc_SetCurrencyCollectionPermission(connectionToClient, data);
//        }
//
//        [TargetRpc]
//        protected void TargetRpc_SetCurrencyCollectionPermission(NetworkConnection target, SetCollectionPermissionMessage data)
//        {
//            currencyReplicator.TargetRpc_SetCurrencyCollectionPermission(target, data);
//        }

        [Server]
        public void Server_SetCurrencyOnClient(CurrencyAmountMessage data)
        {
            TargetRpc_SetCurrency(connectionToClient, data);
        }

        [TargetRpc]
        protected void TargetRpc_SetCurrency(NetworkConnection target, CurrencyAmountMessage data)
        {
            currencyReplicator.TargetRpc_SetCurrency(target, data);
        }
        
        #endregion        
        

        
        
        
        [Server]
        public UNetServerCollection<IItemInstance> Server_AddCollectionToServerAndClient(AddCollectionMessage data)
        {
            var col = Server_AddCollectionToServer(data);
            TargetRpc_AddCollection(connectionToClient, data);

            return col;
        }
        
        [Server]
        public UNetServerCollection<IItemInstance> Server_AddCollectionToServer(AddCollectionMessage data)
        {
            // NOTE: The collection might already exist server side on the target objects (for example, a trigger)

            var col = GetServerCollection(data.collectionGuid) as UNetServerCollection<IItemInstance>;
            if (col == null)
            {
                col = UNetCollectionUtility.CreateServerItemCollection(data.slotCount, data.collectionName, data.collectionGuid, data.owner);
//                unetPlayer.Server_AddCollection(col, new CollectionPriority<IItemInstance>());
            }
            
            return col;
        }
        
        
        [TargetRpc]
        protected void TargetRpc_AddEquipmentCollection(NetworkConnection target, AddEquipmentCollectionMessage data)
        {
            equipmentReplicator.TargetRpc_AddEquipmentCollection(target, data);
        }

        [Server]
        public UNetServerEquipmentCollection<IEquippableItemInstance> Server_AddEquipmentCollectionToServerAndClient(AddEquipmentCollectionMessage data)
        {
            var col = Server_AddEquipmentCollectionToServer(data);
            TargetRpc_AddEquipmentCollection(connectionToClient, data);
            return col;
        }

        [Server]
        public UNetServerEquipmentCollection<IEquippableItemInstance> Server_AddEquipmentCollectionToServer(AddEquipmentCollectionMessage data)
        {
            var col = GetServerCollection(data.collectionGuid) as UNetServerEquipmentCollection<IEquippableItemInstance>;
            if (col == null)
            {
                var equippableCharacter = GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
//                var restoreItemsToGroup = GetComponent<IInventoryCollectionOwner>().itemCollectionGroup;
                col = UNetCollectionUtility.CreateServerEquipmentCollection(data.collectionName, data.collectionGuid, data.owner, data.slots.Select(o => o.ToSlotInstance(_equipmentTypeDatabase, equippableCharacter)).ToArray(), equippableCharacter);
            }

            return col;
        }
        
        
        [TargetRpc]
        protected void TargetRpc_AddCollection(NetworkConnection target, AddCollectionMessage data)
        {
            collectionReplicator.TargetRpc_AddCollection(target, data);
        }
        
        [Server]
        public void Server_SetCollectionContentsOnClient(SetCollectionContentsMessage data)
        {
            var col = GetServerCollection(data.collectionGuid);
            if (col != null)
            {
                // TODO: Sync the size of the collection and type as well
//                col.collectionName = data.collectionName;
//                col.collectionCount = (ushort)data.items.Length;
                logger.LogVerbose($"[Server] Set client's collection contents ({data.items.Length} items) with name {data.collectionName} and guid {col.ID} for client {netId}", this);
                TargetRpc_SetCollectionContents(connectionToClient, data);
            }
            else
            {
                logger.Warning($"Collection with guid {data.collectionGuid} is not known to server. Make sure to add the collection before setting state!", this);
            }
        }

        [TargetRpc]
        protected void TargetRpc_SetCollectionContents(NetworkConnection target, SetCollectionContentsMessage data)
        {
            collectionReplicator.TargetRpc_SetCollectionContents(target, data);
        }
        
        [Server]
        public void Server_SetCollectionPermissionOnServer(SetCollectionPermissionMessage data)
        {
            var col = GetServerCollection(data.collectionGuid);
            if (col != null)
            {
                Server_SetCollectionPermissionOnServer(col, data.permission);
            }
        }
        
        [Server]
        public void Server_SetCollectionPermissionOnServer(IUNetCollection collection, ReadWritePermission permission)
        {
            UNetPermissionsRegistry.collections.SetPermission(collection, identity, permission);
            logger.Log($"[Server] Set client's permission to {permission} for collection {collection.collectionName} and guid {collection.ID} on netID: {netId}", this);
        }
        
        [Server]
        public void Server_SetCollectionPermissionOnServerAndClient(SetCollectionPermissionMessage data)
        {
            Server_SetCollectionPermissionOnServer(data);
            TargetRpc_SetCollectionPermission(connectionToClient, data);
        }
        
        [TargetRpc]
        private void TargetRpc_SetCollectionPermission(NetworkConnection target, SetCollectionPermissionMessage data)
        {
            collectionReplicator.TargetRpc_SetCollectionPermission(target, data);
        }

        [Server]
        public void Server_SetSlotOnClient(SlotDataMessage data)
        {
            TargetRpc_SetSlot(connectionToClient, data);
        }
        
        [TargetRpc]
        private void TargetRpc_SetSlot(NetworkConnection target, SlotDataMessage data)
        {
            collectionReplicator.TargetRpc_SetSlot(target, data);
        }
        
        
        
        // TODO: Could do errors through network messages instead so they'll always arrive at the same location (player)
        [Server]
        public void Server_SendActionFailedToClient(int errorID)
        {
            TargetRpc_ActionFailed(connectionToClient, errorID);
        }
        
        [TargetRpc]
        private void TargetRpc_ActionFailed(NetworkConnection target, int errorID)
        {
            logger.Warning($"[Client] Server action failed with error number: {errorID}", this);
        }
    }
}