using UnityEngine;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using UnityEngine.Assertions;

namespace Devdog.Rucksack.Items
{
    public class TriggerItemPickup : MonoBehaviour, ITriggerCallbacks
    {
        public UnityItemDefinition itemDefinition;
        public int amount = 1;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {typeof(TriggerItemPickup).Name}, but is required.");
        }
        
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            var inventoryOwner = character.GetComponent<IInventoryCollectionOwner>();
            if (character is Player && inventoryOwner != null)
            {
                var inst = ItemFactory.CreateInstance(itemDefinition, System.Guid.NewGuid());
                var added = inventoryOwner.itemCollectionGroup.Add(inst, amount);
                if (added.error != null)
                {
                    // TODO: Notify why pickup failed
                    ItemRegistry.UnRegister(inst.ID);
                    
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