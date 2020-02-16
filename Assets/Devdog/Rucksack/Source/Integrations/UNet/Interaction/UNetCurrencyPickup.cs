using UnityEngine;
using Devdog.General2;
using Devdog.Rucksack.Currencies;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Items
{
    public class UNetCurrencyPickup : NetworkBehaviour, ITriggerCallbacks, IUNetPickup
    {
        public UnityCurrencyDecorator currency;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {typeof(UNetCurrencyPickup).Name}, but is required.");
        }
        
        [ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            var bridge = character.GetComponent<UNetActionsBridge>();
            if (character is Player && bridge != null)
            {
                var added = bridge.inventoryPlayer.currencyCollectionGroup.Add(currency.currency, currency.amount);
                if (added.error == null)
                {
                    // Action went succesful, destroy pickup source
                    NetworkServer.Destroy(gameObject);
                }
                else
                {
                    // Something went wrong, remove item from registry to avoid cluttering it with un-used items.
                    // TODO: Notify client why pickup failed
                }
            }
        }

        [ServerCallback]
        public void OnTriggerUnUsed(Character character, TriggerEventData eventData)
        {
            // Item pickup can't be un-used.
        }
    }
}