using UnityEngine;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Currencies;
using UnityEngine.Assertions;

namespace Devdog.Rucksack.Items
{
    public class TriggerCurrencyPickup : MonoBehaviour, ITriggerCallbacks
    {
        public UnityCurrencyDecorator currency;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {typeof(TriggerCurrencyPickup).Name}, but is required.");
        }
        
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            var currencyOwner = character.GetComponent<ICurrencyCollectionOwner>();
            if (character is Player && currencyOwner != null)
            {
                var added = currencyOwner.currencyCollectionGroup.Add(currency.currency, currency.amount);
                if (added.error != null)
                {
                    
                }
                else
                {
                    // Action went succesful, destroy pickup source
                    Destroy(gameObject);
                }
            }
        }

        public void OnTriggerUnUsed(Character character, TriggerEventData eventData)
        {
            // Item pickup can't be un-used.
        }
    }
}