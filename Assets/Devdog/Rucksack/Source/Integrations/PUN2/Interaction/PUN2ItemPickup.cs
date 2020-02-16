using UnityEngine;
using Devdog.General2;
using UnityEngine.Assertions;

using Photon.Pun;

namespace Devdog.Rucksack.Items
{
    public class PUN2ItemPickup : MonoBehaviourPun, ITriggerCallbacks//, IPUN2Pickup
    {
        public UnityItemDefinition itemDefinition;
        public int amount = 1;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {typeof(PUN2ItemPickup).Name}, but is required.");
        }

        //[ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            var bridge = character.GetComponent<PUN2ActionsBridge>();
            if (character is Player && bridge != null)
            {
                var inst = ItemFactory.CreateInstance(itemDefinition, System.Guid.NewGuid());
                var added = bridge.inventoryPlayer.itemCollectionGroup.Add(inst, amount);
                if (added.error != null)
                {
                    // Something went wrong, remove item from registry to avoid cluttering it with un-used items.
                    ServerItemRegistry.UnRegister(inst.ID);
                    ItemRegistry.UnRegister(inst.ID);

                    // TODO: Notify client why pickup failed

                    DevdogLogger.LogError($"[PUN2][Interaction] PUN2ItemPickup.OnTriggerUsed: {added.error}", this);
                }
                else
                {
                    // Action went succesful, destroy pickup source
                    //NetworkServer.Destroy(gameObject);
                    //PhotonNetwork.Destroy(gameObject);
                    //GameObject.Destroy(gameObject);

                    bridge.photonView.RPC(nameof(bridge.Cmd_RequestUnUseTrigger), PhotonNetwork.MasterClient, this.photonView.ViewID);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        //[ServerCallback]
        public void OnTriggerUnUsed(Character character, TriggerEventData eventData)
        {
            // Item pickup can't be un-used.
        }
    }
}