using UnityEngine;
using Devdog.General2;
using UnityEngine.Assertions;

using Photon.Pun;

namespace Devdog.Rucksack.Items
{
    public class PUN2ItemInstancePickup : MonoBehaviourPun, ITriggerCallbacks//, IPUN2Pickup
    {
        public IUnityItemInstance itemInstance;
        public int amount = 1;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {nameof(PUN2ItemInstancePickup)}, but is required.");
        }

        //[ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            var bridge = character.GetComponent<PUN2ActionsBridge>();
            if (character is Player && bridge != null)
            {
                var added = bridge.inventoryPlayer.itemCollectionGroup.Add(itemInstance, amount);
                if (added.error != null)
                {
                    // TODO: Notify client why pickup failed
                    DevdogLogger.LogError($"[PUN2][Interaction] PUN2ItemInstancePickup.OnTriggerUsed: {added.error}", this);
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