using System;
using System.Collections;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Photon.Pun;
using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    [RequireComponent(typeof(PUN2ActionsBridge))]
    public class PUN2EquippableCharacter : UnityEquippableCharacter
    {
        protected PUN2ActionsBridge bridge;

        protected override void Awake()
        {
            base.Awake();

            bridge = GetComponent<PUN2ActionsBridge>();

            // Register to collection events. First is for when we are a client and the later one is for the master.
            if (PhotonNetwork.IsMasterClient)
                ServerCollectionRegistry.byName.OnAddedItem += CollectionRegistry_OnItemAdded;
            else
                CollectionRegistry.byName.OnAddedItem += CollectionRegistry_OnItemAdded;
            
        }

        private void CollectionRegistry_OnItemAdded(string key, Collections.ICollection value)
        {
            if (key == GetComponent<PUN2EquipmentCollectionCreator>()?.CollectionName)
            {
                CollectionRegistry.byName.OnAddedItem -= CollectionRegistry_OnItemAdded;
                ServerCollectionRegistry.byName.OnAddedItem -= CollectionRegistry_OnItemAdded;

                DoInit();
            }
        }

        protected override EquipmentCollection<IEquippableItemInstance> FindEquipmentCollection()
        {
            if (PhotonNetwork.IsMasterClient)
                return ServerCollectionRegistry.byName.Get(collectionName) as EquipmentCollection<IEquippableItemInstance>;

            return base.FindEquipmentCollection();
        }

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            CollectionRegistry.byName.OnAddedItem -= CollectionRegistry_OnItemAdded;
            ServerCollectionRegistry.byName.OnAddedItem -= CollectionRegistry_OnItemAdded;
        }

        protected void SendTargetRpc_NotifyItemEquipped(Guid itemDefinitionID, string mountPointName)
        {
            if (bridge == null)
            {
                logger.Warning($"[Server] No {nameof(PUN2ActionsBridge)} found on {nameof(PUN2EquippableCharacter)} - Can't replicate to clients.");
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                //logger.Warning("[Client] Tried to replicate equipment action. Ignored.");
                return;
            }

            bridge.photonView.RPC(nameof(bridge.TargetRpc_NotifyItemEquipped), RpcTarget.Others,
                /*itemDefinitionID: */ itemDefinitionID.ToByteArray(),
                /*mountPoint: */ mountPointName
            );
        }

        protected void SendTargetRpc_NotifyItemUnEquipped(string mountPointName)
        {
            if (bridge == null)
            {
                logger.Warning($"[Server] No {nameof(PUN2ActionsBridge)} found on {nameof(PUN2EquippableCharacter)} - Can't replicate to clients.");
                return;
            }

            if (!Photon.Pun.PhotonNetwork.IsMasterClient)
            {
                //logger.Warning("[Client] Tried to replicate unEquipment action. Ignored.");
                return;
            }

            bridge.photonView.RPC(nameof(bridge.TargetRpc_NotifyItemEquipped), RpcTarget.Others,
                /*itemDefinitionID: */ System.Guid.Empty.ToByteArray(),
                /*mountPoint: */ mountPointName
            );
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
    }
}