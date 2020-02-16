using UnityEngine;
using Devdog.General2;
using Devdog.Rucksack.Currencies;
using UnityEngine.Assertions;

using Photon.Pun;

namespace Devdog.Rucksack.Items
{
    public class PUN2CurrencyPickup : MonoBehaviourPun, ITriggerCallbacks//, IPUN2Pickup
    {
        public UnityCurrencyDecorator currency;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {typeof(PUN2CurrencyPickup).Name}, but is required.");
        }

        //[ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            var bridge = character.GetComponent<PUN2ActionsBridge>();
            if (character is Player && bridge != null)
            {
                var added = bridge.inventoryPlayer.currencyCollectionGroup.Add(currency.currency, currency.amount);
                if (added.error == null)
                {
                    // Action went succesful, destroy pickup source
                    //NetworkServer.Destroy(gameObject);
                    //PhotonNetwork.Destroy(gameObject);
                    //GameObject.Destroy(gameObject);

                    bridge.photonView.RPC(nameof(bridge.Cmd_RequestUnUseTrigger), PhotonNetwork.MasterClient, this.photonView.ViewID);
                    PhotonNetwork.Destroy(gameObject);
                }
                else
                {
                    // Something went wrong, remove item from registry to avoid cluttering it with un-used items.
                    // TODO: Notify client why pickup failed

                    DevdogLogger.LogError($"[PUN2][Interaction] PUN2CurrencyPickup.OnTriggerUsed: {added.error}", this);
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