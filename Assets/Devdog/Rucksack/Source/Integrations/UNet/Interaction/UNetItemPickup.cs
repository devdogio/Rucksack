using UnityEngine;
using Devdog.General2;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Items
{
    public class UNetItemPickup : NetworkBehaviour, ITriggerCallbacks, IUNetPickup
    {
        public UnityItemDefinition itemDefinition;
        public int amount = 1;

        private ITrigger _trigger;
        private void Awake()
        {
            _trigger = GetComponent<ITrigger>();
            Assert.IsNotNull(_trigger, $"Couldn't find ITrigger on {typeof(UNetItemPickup).Name}, but is required.");
        }
        
        [ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData eventData)
        {
            var bridge = character.GetComponent<UNetActionsBridge>();
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
                }
                else
                {
                    // Action went succesful, destroy pickup source
                    NetworkServer.Destroy(gameObject);
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