using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;
using Devdog.General2;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Database;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Collections.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.General2.Serialization;
using Devdog.Rucksack.Vendors;

namespace Devdog.Rucksack
{
    public sealed class PUN2ActionsBridge : MonoBehaviourPun
    {
        public bool isHost
        {
            get { return this.photonView.Owner.IsMasterClient;/* PhotonNetwork.LocalPlayer.IsMasterClient;*/ }
        }

        public Player player { get; private set; }

        [SerializeField]
        private UnityCurrencyDatabase _currencyDatabase;
        public IDatabase<UnityCurrency> currencyDatabase
        {
            get { return _currencyDatabase; }
        }

        [SerializeField]
        private UnityItemDefinitionDatabase _itemsDatabase;
        public IDatabase<UnityItemDefinition> itemsDatabase
        {
            get { return _itemsDatabase; }
        }

        [SerializeField]
        private UnityEquipmentTypeDatabase _equipmentTypeDatabase;
        public IDatabase<UnityEquipmentType> equipmentTypeDatabase
        {
            get { return _equipmentTypeDatabase; }
        }



        [Required]
        [SerializeField]
        private PUN2InventoryPlayer _inventoryPlayer;
        public PUN2InventoryPlayer inventoryPlayer
        {
            get { return _inventoryPlayer; }
        }

        private PUN2CollectionReplicator collectionReplicator;
        private PUN2EquipmentReplicator equipmentReplicator;
        private PUN2CurrencyReplicator currencyReplicator;
        private PUN2VendorReplicator vendorReplicator;


        public static PUN2CollectionFinder collectionFinder { get; }
        private static readonly ILogger logger;

        static PUN2ActionsBridge()
        {
            logger = new UnityLogger("[PUN2] ");
            collectionFinder = new PUN2CollectionFinder();
        }

        public PUN2ActionsBridge()
        {
            collectionReplicator = new PUN2CollectionReplicator(this, logger);
            equipmentReplicator = new PUN2EquipmentReplicator(this, logger);
            currencyReplicator = new PUN2CurrencyReplicator(this, logger);
            vendorReplicator = new PUN2VendorReplicator(this, logger);
        }

        void Awake()
        {
            player = GetComponent<Player>();
        }

        void OnDestroy()
        {
            PUN2PermissionsRegistry.collections.RevokeAllForIdentity(base.photonView);
            PUN2PermissionsRegistry.objects.RevokeAllForIdentity(base.photonView);

            PUN2PermissionsRegistry.collections.RemoveAllEventListenersForIdentity(base.photonView);
            PUN2PermissionsRegistry.objects.RemoveAllEventListenersForIdentity(base.photonView);
        }

        public void Server_SetCurrencyOnClient(Guid collectionGuid, Guid currencyGuid, double amount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetCurrencyOnClient)}(collectionGuid: {collectionGuid}, currencyGuid: {currencyGuid}, amount: {amount})", this);

            this.photonView.RPC(nameof(TargetRpc_SetCurrency), this.photonView.Owner, collectionGuid.ToByteArray(), currencyGuid.ToByteArray(), amount);
        }

        [PunRPC]
        private void TargetRpc_SetCurrency(byte[] collectionGuidBytes, byte[] currencyGuidBytes, double amount)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);
            Guid currencyGuid = new Guid(currencyGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_SetCurrency)}(collectionGuid: {collectionGuid}, currencyGuid: {currencyGuid}, amount: {amount})", this);

            currencyReplicator.TargetRpc_SetCurrency(this.photonView, collectionGuid, currencyGuid, amount);
        }

        public void Server_SetCollectionPermissionOnServerAndClient(Guid collectionGuid, ReadWritePermission permission)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetCollectionPermissionOnServerAndClient)}(collectionGuid: {collectionGuid}, permission: {permission})", this);

            Server_SetCollectionPermissionOnServer(collectionGuid, permission);

            this.photonView.RPC(nameof(TargetRpc_SetCollectionPermission), this.photonView.Owner, collectionGuid.ToByteArray(), permission);
        }

        [PunRPC]
        private void TargetRpc_SetCollectionPermission(byte[] collectionGuidBytes, ReadWritePermission permission)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_SetCollectionPermission)}(collectionGuid: {collectionGuid}, permission: {permission})", this);

            collectionReplicator.TargetRpc_SetCollectionPermission(collectionGuid, permission);
        }

        public void Server_SetCollectionPermissionOnServer(Guid collectionGuid, ReadWritePermission permission)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetCollectionPermissionOnServer)}(collectionGuid: {collectionGuid}, permission: {permission})", this);

            var col = collectionFinder.GetServerCollection(collectionGuid);
            if (col != null)
            {
                Server_SetCollectionPermissionOnServer(col, permission);
            }
        }

        public void Server_SetCollectionPermissionOnServer(IPUN2Collection collection, ReadWritePermission permission)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetCollectionPermissionOnServer)}(collection: ({collection.collectionName}, {collection.ID}), permission: {permission})", this);

            PUN2PermissionsRegistry.collections.SetPermission(collection, this.photonView, permission);
        }

        public PUN2ServerCollection<IItemInstance> Server_AddCollectionToServerAndClient(string collectionName, Guid collectionGuid, int slotCount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return null;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddCollectionToServerAndClient)}(collectionGuid: {collectionName}, collectionGuid: {collectionGuid}, slotCount: {slotCount})", this);

            var col = Server_AddCollectionToServer(collectionName, collectionGuid, slotCount);

            this.photonView.RPC(nameof(TargetRpc_AddCollection), this.photonView.Owner, collectionName, collectionGuid.ToByteArray(), slotCount);

            return col;
        }

        public PUN2ServerCollection<IItemInstance> Server_AddCollectionToServer(string collectionName, Guid collectionGuid, int slotCount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return null;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddCollectionToServer)}(collectionGuid: {collectionName}, collectionGuid: {collectionGuid}, slotCount: {slotCount})", this);

            // NOTE: The collection might already exist server side on the target objects (for example, a trigger)

            var col = collectionFinder.GetServerCollection(collectionGuid) as PUN2ServerCollection<IItemInstance>;
            if (col == null)
            {
                col = PUN2CollectionUtility.CreateServerItemCollection(slotCount, collectionName, collectionGuid, this.photonView /*data.owner*/);
            }

            return col;
        }

        [PunRPC]
        private void TargetRpc_AddCollection(string collectionName, byte[] collectionGuidBytes, int slotCount)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_AddCollection)}(collectionGuid: {collectionName}, collectionGuid: {collectionGuid}, slotCount: {slotCount})", this);

            try
            {
                collectionReplicator.TargetRpc_AddCollection(this.photonView, collectionName, collectionGuid, slotCount);
            }
            catch (Exception ex)
            {
                logger.Error(ex, this);
            }

        }
        
        public void Server_TellClientToRegisterItemInstance(IItemInstance itemInstance)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            if (itemInstance == null)
            {
                logger.Error("Given item instance is null!", this);
                return;
            }

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_TellClientToRegisterItemInstance)}(itemInstance.ID: {itemInstance.ID}, itemInstance.itemDefinition.ID: {itemInstance.itemDefinition.ID}, serializedData: {string.Empty})", this);

            this.photonView.RPC(nameof(TargetRpc_RegisterItemInstance), this.photonView.Owner, itemInstance.ID.ToByteArray(), itemInstance.itemDefinition.ID.ToByteArray(), string.Empty);
        }

        // TODO: use better serialization / deserialization. Item instance needs a reference to the item definition it belongs to; 
        // TODO: For simplicity ItemDefinitions have to be persistent on disk
        [PunRPC]
        private void TargetRpc_RegisterItemInstance(byte[] itemGuidBytes, byte[] itemDefinitionGuidBytes, string serializedData)
        {
            Guid itemGuid = new Guid(itemGuidBytes);
            Guid itemDefinitionGuid = new Guid(itemDefinitionGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_RegisterItemInstance)}(itemGuid: {itemGuid}, itemDefinitionGuid: {itemDefinitionGuid}, serializedData: {serializedData})", this);

            var itemDef = _itemsDatabase.Get(new Identifier(itemDefinitionGuid));
            if (itemDef.error != null)
            {
                logger.Log($"ItemDefinition with guid {itemDefinitionGuid} not found on local client!", itemDef.error);
                return;
            }

            var instance = ItemFactory.CreateInstance(itemDef.result, itemGuid);
            if (string.IsNullOrEmpty(serializedData) == false)
            {
                JsonUtility.FromJsonOverwrite(serializedData, instance);
            }
        }

        public void Server_SetSlotOnClient(Guid collectionGuid, Guid itemInstanceGuid, int index, int amount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetSlotOnClient)}(collectionGuid: {collectionGuid}, itemInstanceGuid: {itemInstanceGuid}, index: {index}, amount: {amount})", this);

            this.photonView.RPC(nameof(TargetRpc_SetSlot), this.photonView.Owner, collectionGuid.ToByteArray(), itemInstanceGuid.ToByteArray(), index, amount);
        }
        
        [PunRPC]
        private void TargetRpc_SetSlot(byte[] collectionGuidBytes, byte[] itemInstanceGuidBytes, int index, int amount)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);
            Guid itemInstanceGuid = new Guid(itemInstanceGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_SetSlot)}({nameof(collectionGuid)}: {collectionGuid}, itemInstanceGuid: {itemInstanceGuid}, index: {index}, amount: {amount})", this);

            collectionReplicator.TargetRpc_SetSlot(collectionGuid, itemInstanceGuid, (ushort)index, (ushort)amount);
        }

        [PunRPC]
        public void Cmd_RequestSwapOrMerge(byte[] collectionGuidBytes, byte[] toCollectionGuidBytes, int fromIndex, int toIndex, int amount)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);
            Guid toCollectionGuid = new Guid(toCollectionGuidBytes);

            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestSwapOrMerge)}(collectionGuid: {collectionGuid}, toCollectionGuid: {toCollectionGuid}, fromIndex: {fromIndex}, toIndex: {toIndex}, amount: {amount})", this);

            collectionReplicator.Cmd_RequestSwapOrMerge(collectionGuidBytes, collectionGuid, toCollectionGuidBytes, toCollectionGuid, (ushort)fromIndex, (ushort)toIndex, (short)amount);
        }

        [PunRPC]
        public void Cmd_RequestMoveAuto(byte[] collectionGuidBytes, byte[] toCollectionGuidBytes, int fromIndex, int amount)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);
            Guid toCollectionGuid = new Guid(toCollectionGuidBytes);

            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestMoveAuto)}(collectionGuid: {collectionGuid}, toCollectionGuid: {toCollectionGuid}, fromIndex: {fromIndex}, amount: {amount})", this);

            collectionReplicator.Cmd_RequestMoveAuto(collectionGuidBytes, collectionGuid, toCollectionGuidBytes, toCollectionGuid, (ushort)fromIndex, (short)amount);
        }

        public PUN2ServerEquipmentCollection<IEquippableItemInstance> Server_AddEquipmentCollectionToServerAndClient(string collectionName, Guid collectionGuid, UnitySerializedEquipmentCollectionSlot[] slots)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return null;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddEquipmentCollectionToServerAndClient)}(collectionName: {collectionName}, collectionGuid: {collectionGuid}, slots: {slots.Length}, owner: {this.photonView.ViewID})", this);

            var col = Server_AddEquipmentCollectionToServer(collectionName, collectionGuid, slots);

            // convert slots to string[]
            //string[] encoded = slots.Select(o => string.Join(":", o.equipmentTypes.Select(j => j.ID.ToString()))).ToArray();
            var selected = slots.Select(o => o.equipmentTypes.Select(j => j.ID).ToArray()).ToArray();

            byte[] encoded = SerializationUtility.SerializeValue(selected, DataFormat.Binary);

            this.photonView.RPC(nameof(TargetRpc_AddEquipmentCollection), this.photonView.Owner, collectionName, collectionGuid.ToByteArray(), encoded);

            return col;
        }

        [PunRPC]
        private void TargetRpc_AddEquipmentCollection(string collectionName, byte[] collectionGuidBytes, byte[] slots)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);

            Guid[][] slotGuids = SerializationUtility.DeserializeValue<Guid[][]>(slots, DataFormat.Binary);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_AddEquipmentCollection)}(collectionName: {collectionName}, collectionGuid: {collectionGuid}, slots: {slotGuids.Length}, owner: {this.photonView.ViewID})", this);

            EquipmentCollectionSlot<IEquippableItemInstance>[] parsedSlots = new EquipmentCollectionSlot<IEquippableItemInstance>[slotGuids.Length];

            for (int i = 0; i < slotGuids.Length; ++i)
            {
                parsedSlots[i] = new EquipmentCollectionSlot<IEquippableItemInstance>();

                parsedSlots[i].equipmentTypes = slotGuids[i].Select(g => _equipmentTypeDatabase.Get(new Identifier(g)).result).Cast<UnityEquipmentType>().ToArray();
            }

            equipmentReplicator.TargetRpc_AddEquipmentCollection(this.photonView, collectionName, collectionGuid, parsedSlots);
        }

        public PUN2ServerEquipmentCollection<IEquippableItemInstance> Server_AddEquipmentCollectionToServer(string collectionName, Guid collectionGuid, UnitySerializedEquipmentCollectionSlot[] slots)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return null;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddEquipmentCollectionToServer)}(collectionName: {collectionName}, collectionGuid: {collectionGuid}, slots: {slots.Length}, owner: {this.photonView.ViewID})", this);

            var col = collectionFinder.GetServerCollection(collectionGuid) as PUN2ServerEquipmentCollection<IEquippableItemInstance>;
            if (col == null)
            {
                var equippableCharacter = GetComponent<IEquippableCharacter<IEquippableItemInstance>>();

                EquipmentCollectionSlot<IEquippableItemInstance>[] parsedSlots = new EquipmentCollectionSlot<IEquippableItemInstance>[slots.Length];
                for (int i = 0; i < slots.Length; ++i)
                {

                    parsedSlots[i] = new EquipmentCollectionSlot<IEquippableItemInstance>();
                    parsedSlots[i].equipmentTypes = slots[i].equipmentTypes.Select(g => g.ID).Select(g => _equipmentTypeDatabase.Get(new Identifier(g)).result).Cast<UnityEquipmentType>().ToArray();
                }

                col = PUN2CollectionUtility.CreateServerEquipmentCollection(collectionName, collectionGuid, this.photonView, parsedSlots, equippableCharacter);
            }

            return col;
        }

        public void Server_AddCurrencyCollectionToServerAndClient(string collectionName, Guid collectionGuid)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddCurrencyCollectionToServerAndClient)}(owner: {this.photonView.ViewID}, collectionName: {collectionName}, collectionGuid: {collectionGuid})", this);

            Server_AddCurrencyCollectionToServer(collectionName, collectionGuid);

            this.photonView.RPC(nameof(TargetRpc_AddCurrencyCollection), this.photonView.Owner, collectionName, collectionGuid.ToByteArray());
        }

        public PUN2ServerCurrencyCollection Server_AddCurrencyCollectionToServer(string collectionName, Guid collectionGuid)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return null;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddCurrencyCollectionToServer)}(owner: {this.photonView.ViewID}, collectionName: {collectionName}, collectionGuid: {collectionGuid})", this);

            var col = collectionFinder.GetServerCurrencyCollection(collectionGuid) as PUN2ServerCurrencyCollection;
            if (col == null)
            {
                col = PUN2CurrencyCollectionUtility.CreateServerCurrencyCollection(collectionName, collectionGuid, this.photonView);
            }

            return col;
        }

        [PunRPC]
        private void TargetRpc_AddCurrencyCollection(string collectionName, byte[] collectionGuidBytes)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_AddCurrencyCollection)}(owner: {this.photonView.ViewID}, collectionName: {collectionName}, collectionGuid: {collectionGuid})", this);

            currencyReplicator.TargetRpc_AddCurrencyCollection(this.photonView, collectionName, collectionGuid);
        }

        [PunRPC]
        public void TargetRpc_NotifyItemEquipped(byte[] itemDefinitionIDBytes, string mountPointName)
        {
            Guid itemDefinitionID = new Guid(itemDefinitionIDBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_NotifyItemEquipped)}(itemDefinitionID: {itemDefinitionID}, mountPointName: {mountPointName})", this);

            var character = GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
            if (character != null)
            {
                var mountPoint = character.mountPoints.FirstOrDefault(o => o.name == mountPointName);
                if (mountPoint != null)
                {
                    if (itemDefinitionID == System.Guid.Empty)
                    {
                        mountPoint.Clear();
                    }
                    else
                    {
                        var item = itemsDatabase.Get(new Identifier(itemDefinitionID));
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
                            logger.Warning($"[Client] Server told us to equip item on this character; But item with ID {itemDefinitionID} was not found.", this);
                        }
                    }
                }
                else
                {
                    logger.Warning($"[Client] Tried to visually equip item, but mountpoint with name {mountPoint} not found", this);
                }
            }
        }

        [PunRPC]
        public void Cmd_RequestUseTrigger(int triggerIdentityId)
        {
            var triggerIdentity = PhotonNetwork.GetPhotonView(triggerIdentityId);
            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestUseTrigger)}(triggerIdentity: {triggerIdentity})", triggerIdentity);

            if (triggerIdentity == null)
            {
                return;
            }

            var trigger = triggerIdentity.GetComponent<ITrigger>();
            if (trigger != null)
            {
                var canUse = trigger.CanUse(player);
                if (canUse)
                {
                    trigger.Server_Use(player);

                    // NOTE: isHost check to avoid doing the same thing twice, which is a bit heavy and spammy to the console...
                    if (isHost == false)
                    {
                        this.photonView.RPC(nameof(TargetRpc_UseTrigger), triggerIdentity.Owner, triggerIdentityId);
                    }
                }
            }
            else
                logger.Warning($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestUseTrigger)}(triggerIdentity: {triggerIdentity}) - No trigger found!", this);
        }

        [PunRPC]
        private void TargetRpc_UseTrigger(int triggerIdentityId)
        {
            var triggerIdentity = PhotonNetwork.GetPhotonView(triggerIdentityId);
            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_UseTrigger)}(triggerIdentity: {triggerIdentity})", triggerIdentity);

            var trigger = triggerIdentity.GetComponent<PUN2Trigger>();
            if (trigger != null)
            {
                trigger.Server_Use(player);
            }
            else
                logger.Warning($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_UseTrigger)}(triggerIdentity: {triggerIdentity}) - No trigger found!", triggerIdentity);
        }

        [PunRPC]
        public void Cmd_RequestUnUseTrigger(int triggerIdentityId)
        {
            var triggerIdentity = PhotonNetwork.GetPhotonView(triggerIdentityId);
            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestUnUseTrigger)}(triggerIdentity: {triggerIdentity})", triggerIdentity);

            var trigger = triggerIdentity.GetComponent<ITrigger>();
            if (trigger != null)
            {
                var canUnUse = trigger.CanUnUse(player);
                if (canUnUse)
                {
                    trigger.Server_UnUse(player);

                    // NOTE: isHost check to avoid doing the same thing twice, which is a bit heavy and spammy to the console...
                    if (isHost == false)
                    {
                        this.photonView.RPC(nameof(TargetRpc_UnUseTrigger), triggerIdentity.Owner, triggerIdentityId);
                    }
                }
            }
        }

        [PunRPC]
        private void TargetRpc_UnUseTrigger(int triggerIdentityId)
        {
            var triggerIdentity = PhotonNetwork.GetPhotonView(triggerIdentityId);
            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_UnUseTrigger)}(triggerIdentity: {triggerIdentity})", triggerIdentity);

            var trigger = triggerIdentity.GetComponent<PUN2Trigger>();
            if (trigger != null)
            {
                trigger.Server_UnUse(player);
            }
            else
                logger.Warning($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_UnUseTrigger)}(triggerIdentity: {triggerIdentity}) - No trigger found!", triggerIdentity);
        }

        [PunRPC]
        public void Cmd_RequestUseItem(byte[] itemGuidBytes, int useAmount, int targetIndex)
        {
            Guid itemGuid = new Guid(itemGuidBytes);

            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestUseItem)}(itemGuid: {itemGuid}, useAmount: {useAmount}, targetIndex: {targetIndex})", this);

            collectionReplicator.Cmd_RequestUseItem(itemGuid, (ushort)useAmount, (short)targetIndex);
        }

        [PunRPC]
        public void Cmd_RequestDropItem(byte[] itemGuidBytes, Vector3 worldPosition)
        {
            Guid itemGuid = new Guid(itemGuidBytes);

            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestDropItem)}(itemGuid: {itemGuid}, worldPosition: {worldPosition})", this);

            collectionReplicator.Cmd_RequestDropItem(itemGuid, worldPosition);
        }

        [PunRPC]
        public void TargetRpc_NotifyItemUsed(byte[] itemGuidBytes, int useAmount, int targetIndex)
        {
            Guid itemGuid = new Guid(itemGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_NotifyItemUsed)}(itemGuid: {itemGuid}, useAmount: {useAmount}, targetIndex: {targetIndex})", this);

            var item = ItemRegistry.Get(itemGuid) as INetworkItemInstance;
            if (item != null)
            {
                item.Client_NotifyUsed(player, new ItemContext()
                {
                    useAmount = useAmount,
                    targetIndex = targetIndex
                });
            }
            else
            {
                logger.Warning("[Client] Server notified of used item, but item not found! :: " + itemGuid);
            }
        }

        public void Server_SetCollectionContentsOnClient(string collectionName, Guid collectionGuid, ItemAmountMessage[] items)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetCollectionContentsOnClient)}(collectionName: {collectionName}, collectionGuid: {collectionGuid}, items: {items.Length})", this);

            var col = collectionFinder.GetServerCollection(collectionGuid);
            if (col != null)
            {
                // TODO: Sync the size of the collection and type as well
                //                col.collectionName = data.collectionName;
                //                col.collectionCount = (ushort)data.items.Length;
                logger.LogVerbose($"[Server] Set client's collection contents ({items.Length} items) with name {collectionName} and guid {col.ID} for client {this.photonView.ViewID}", this);

                this.photonView.RPC(nameof(TargetRpc_SetCollectionContents), this.photonView.Owner, collectionName, collectionGuid.ToByteArray(), SerializationUtility.SerializeValue(items, DataFormat.Binary));
            }
            else
            {
                logger.Warning($"Collection with guid {collectionGuid} is not known to server. Make sure to add the collection before setting state!", this);
            }
        }

        [PunRPC]
        private void TargetRpc_SetCollectionContents(string collectionName, byte[] collectionGuidBytes, byte[] itemBytes)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);
            ItemAmountMessage[] items = SerializationUtility.DeserializeValue<ItemAmountMessage[]>(itemBytes, DataFormat.Binary);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_SetCollectionContents)}(collectionName: {collectionName}, collectionGuid: {collectionGuid}, items: {items.Length})", this);

            collectionReplicator.TargetRpc_SetCollectionContents(this.photonView, collectionName, collectionGuid, items);
        }

        [PunRPC]
        public void Cmd_RequestBuyItemFromVendor(byte[] vendorGuidBytes, byte[] itemGuidBytes, int amount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            Guid vendorGuid = new Guid(vendorGuidBytes);
            Guid itemGuid = new Guid(itemGuidBytes);

            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestBuyItemFromVendor)}(vendorGuid: {vendorGuid}, itemGuid: {itemGuid}, amount: {amount})", this);

            vendorReplicator.Cmd_RequestBuyItemFromVendor(vendorGuidBytes, vendorGuid, itemGuidBytes, itemGuid, (ushort)amount);

            // Update the entire collection of vendor products when the user buys something.
            var vendor = ServerVendorRegistry.itemVendors.Get(vendorGuid) as PUN2Vendor<IItemInstance>;
            if (vendor != null)
            {
                Server_SetItemVendorCollectionContentsOnClient(vendor.vendorCollectionGuid, vendor.vendorCollection);
            }
        }

        [PunRPC]
        public void Cmd_RequestSellItemToVendor(byte[] vendorGuidBytes, byte[] itemGuidBytes, int amount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            Guid vendorGuid = new Guid(vendorGuidBytes);
            Guid itemGuid = new Guid(itemGuidBytes);

            logger.Log($"[Command][ViewId: {this.photonView.ViewID}] {nameof(Cmd_RequestSellItemToVendor)}(vendorGuid: {vendorGuid}, itemGuid: {itemGuid}, amount: {amount})", this);

            vendorReplicator.Cmd_RequestSellItemToVendor(vendorGuidBytes, vendorGuid, itemGuidBytes, itemGuid, (ushort)amount);

            // Update the entire collection of vendor products when the user sells something.
            var vendor = ServerVendorRegistry.itemVendors.Get(vendorGuid) as PUN2Vendor<IItemInstance>;
            if (vendor != null)
            {
                Server_SetItemVendorCollectionContentsOnClient(vendor.vendorCollectionGuid, vendor.vendorCollection);
            }
        }

        public void Server_SetItemVendorCollectionContentsOnClient(Guid vendorCollectionGuid, Devdog.Rucksack.Collections.IReadOnlyCollection<IVendorProduct<IItemInstance>> col)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_SetItemVendorCollectionContentsOnClient)}(vendorCollectionGuid: {vendorCollectionGuid}, col: {col.slotCount})", this);

            ItemVendorProductMessage[] products = new ItemVendorProductMessage[col.slotCount];
            int idx = 0;
            foreach (var product in col)
            {
                if (product != null)
                {
                    products[idx++] = new ItemVendorProductMessage(product, product.collectionEntry.amount);
                }
                else
                {
                    products[idx++] = new ItemVendorProductMessage();
                }
            }

            this.photonView.RPC(nameof(TargetRpc_SetItemVendorCollectionContents), this.photonView.Owner,
                /*collectionGuidBytes: */ vendorCollectionGuid.ToByteArray(),
                /*products: */ SerializationUtility.SerializeValue(products, DataFormat.Binary)
            );
        }

        [PunRPC]
        public void TargetRpc_SetItemVendorCollectionContents(byte[] collectionGuidBytes, byte[] productsBytes)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);
            ItemVendorProductMessage[] products = SerializationUtility.DeserializeValue<ItemVendorProductMessage[]>(productsBytes, DataFormat.Binary);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_SetItemVendorCollectionContents)}(collectionGuid: {collectionGuid}, products: {products.Length})", this);

            vendorReplicator.TargetRpc_SetItemVendorCollectionContents(this.photonView, collectionGuid, products);
        }

        [PunRPC]
        public void TargetRPC_ShowItemVendorUI(byte[] vendorGuidBytes)
        {
            Guid vendorGuid = new Guid(vendorGuidBytes);

            var itemVendors = FindObjectsOfType<PUN2ItemVendorEnabler>();
            foreach(var itemVendor in itemVendors)
            {
                if (itemVendor.vendorGuid == vendorGuid)
                {
                    itemVendor.RPC_ShowUI();
                    return;
                }
            }

        }

        public void Server_AddVendorItemCollectionToClient(Guid vendorGuid, string collectionName, Guid collectionGuid, ushort slotCount)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            logger.Log($"[Server][ViewId: {this.photonView.ViewID}] {nameof(Server_AddVendorItemCollectionToClient)}(vendorGuid: {vendorGuid}, collectionName: {collectionName}, collectionGuid: {collectionGuid}, slotCount: {slotCount})", this);

            this.photonView.RPC(nameof(TargetRpc_AddVendorItemCollection), this.photonView.Owner, vendorGuid.ToByteArray(), collectionName, collectionGuid.ToByteArray(), (int)slotCount);
        }

        [PunRPC]
        private void TargetRpc_AddVendorItemCollection(byte[] vendorGuidBytes, string collectionName, byte[] collectionGuidBytes, int slotCount)
        {
            Guid vendorGuid = new Guid(vendorGuidBytes);
            Guid collectionGuid = new Guid(collectionGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRpc_AddVendorItemCollection)}(collectionName: {collectionName}, collectionGuid: {collectionGuid}, slotCount: {slotCount})", this);

            var collection = vendorReplicator.TargetRpc_AddVendorItemCollection(this.photonView, collectionName, collectionGuid, (ushort)slotCount);

            // The Vendor might be already created on this client, but without a vendorCollection.
            var vendor = VendorRegistry.itemVendors.Get(vendorGuid) as PUN2Vendor<IItemInstance>;
            if (vendor != null && vendor.vendorCollection == null)
                vendor.vendorCollection = collection;
        }

        [PunRPC]
        public void TargetRPC_ShowItemCollectionUI(string collectionName, byte[] collectionGuidBytes)
        {
            Guid collectionGuid = new Guid(collectionGuidBytes);

            logger.Log($"[TargetRpc][ViewId: {this.photonView.ViewID}] {nameof(TargetRPC_ShowItemCollectionUI)}(collectionGuid: {collectionGuid})", this);

            var enablers = FindObjectsOfType<PUN2TriggerItemCollectionEnabler>();
            foreach (var enabler in enablers)
            {
                if (enabler.CollectionName == collectionName)
                {
                    enabler.RPC_ShowUI(collectionGuid);
                    return;
                }
            }
        }
    }
}
