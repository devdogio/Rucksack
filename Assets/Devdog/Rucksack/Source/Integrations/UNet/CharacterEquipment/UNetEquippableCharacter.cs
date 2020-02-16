using System;
using System.Collections;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class UNetEquippableCharacter : UnityEquippableCharacter
    {
        protected UNetActionsBridge bridge;

        protected virtual IEnumerator Start()
        {
            yield return null;
            DoInit();
        }
        
        protected override void OnEnable()
        {
            bridge = GetComponent<UNetActionsBridge>();
            // base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        
        protected override EquipmentCollection<IEquippableItemInstance> FindEquipmentCollection()
        {
            var c = GetComponent<UNetEquipmentCollectionCreator>();
            if (c != null)
            {
                return c.collection;
            }

            return null;
        }

        protected bool IsValidConnection(NetworkConnection conn)
        {
            return conn.hostId != -1 && conn.connectionId != -1 && conn.address != "localClient";
        }
        
        protected void SendTargetRpc_NotifyItemEquipped(Guid itemDefinitionID, string mountPointName)
        {
            if (bridge == null)
            {
                logger.Warning($"[Server] No {nameof(UNetActionsBridge)} found on {nameof(UNetEquippableCharacter)} - Can't replicate to clients.");
                return;
            }
            
            if (bridge.identity.isServer == false)
            {
                logger.Warning("[Client] Tried to replicate equipment action. Ignored.");
                return;
            }
            
            foreach (var conn in NetworkServer.connections)
            {
                if (IsValidConnection(conn))
                {
                    bridge.TargetRpc_NotifyItemEquipped(conn, new ItemEquippedMessage()
                    {
                        itemDefinitionID = new GuidMessage(){ bytes = itemDefinitionID.ToByteArray() },
                        mountPoint = mountPointName,
                    });
                }
            }
        }
        
        protected void SendTargetRpc_NotifyItemUnEquipped(string mountPointName)
        {
            if (bridge == null)
            {
                logger.Warning($"[Server] No {nameof(UNetActionsBridge)} found on {nameof(UNetEquippableCharacter)} - Can't replicate to clients.");
                return;
            }

            if (bridge.identity.isServer == false)
            {
                logger.Warning("[Client] Tried to replicate unEquipment action. Ignored.");
                return;
            }
            
            foreach (var conn in NetworkServer.connections)
            {
                if (IsValidConnection(conn))
                {
                    bridge.TargetRpc_NotifyItemEquipped(conn, new ItemEquippedMessage()
                    {
                        itemDefinitionID = new GuidMessage(){ bytes = System.Guid.Empty.ToByteArray() },
                        mountPoint = mountPointName,
                    });
                }
            }
        }
        
        public override Result<EquipmentResult<IEquippableItemInstance>[]> Equip(IEquippableItemInstance item, int amount = 1)
        {
            var equipped = base.Equip(item, amount);
            if (equipped.error == null)
            {
                foreach (var result in equipped.result)
                {
                    SendTargetRpc_NotifyItemEquipped(item.itemDefinition.ID, result.mountPoint);
                }
            }
            
            return equipped;
        }

        public override Result<EquipmentResult<IEquippableItemInstance>> EquipAt(int index, IEquippableItemInstance item, int amount = 1)
        {
            var equipped = base.EquipAt(index, item, amount);
            if (equipped.error == null)
            {
                SendTargetRpc_NotifyItemEquipped(item.itemDefinition.ID, equipped.result.mountPoint);
            }

            return equipped;
        }

        public override Result<UnEquipmentResult> UnEquip(IEquippableItemInstance item, int amount = 1)
        {
            var unEquipped = base.UnEquip(item, amount);
            if (unEquipped.error == null)
            {
                if (item.collectionEntry != null)
                {
                    SendTargetRpc_NotifyItemUnEquipped(unEquipped.result.mountPoint);
                }
            }
            
            return unEquipped;
        }

        public override Result<UnEquipmentResult> UnEquipAt(int index, int amount = 1)
        {
            var unEquipped = base.UnEquipAt(index, amount);
            if (unEquipped.error == null)
            {
                SendTargetRpc_NotifyItemUnEquipped(unEquipped.result.mountPoint);
            }

            return unEquipped;
        }

        public virtual void Server_UpdateClient(NetworkConnection conn, IMountPoint<IEquippableItemInstance> mountPoint, IEquippableItemInstance item)
        {
            logger.LogVerbose($"[Server] Updating client {conn} of of changed mountPoint {mountPoint}");
        }
    }
}