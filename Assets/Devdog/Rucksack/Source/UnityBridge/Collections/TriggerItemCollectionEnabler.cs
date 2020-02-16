using Devdog.General2;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Enable read/write permission on a collection when a trigger is used and remove permission when the trigger is un-used.
    /// </summary>
    public sealed class TriggerItemCollectionEnabler : MonoBehaviour, ITriggerCallbacks
    {
        [SerializeField]
        private SerializedGuid _guid;

        private ItemCollectionUI _activeUI;
        public void OnTriggerUsed(Character character, TriggerEventData data)
        {
            var uis = FindObjectsOfType<ItemCollectionUI>();
            var col = CollectionRegistry.byID.Get(_guid.guid) as ICollection<IItemInstance>;
            if (col != null)
            {
                foreach (var ui in uis)
                {
                    if (ui.collectionName == col.ToString())
                    {
                        _activeUI = ui;
                        
                        _activeUI.collection = col;
                        _activeUI.window.Show();
                        return;
                    }
                }
                
                new UnityLogger("[Collection] ").Warning("Couldn't find ItemCollectionUI that repaints collection for collection with name: " + col.ToString());
            }
            
            new UnityLogger("[Collection] ").Warning("Couldn't find ItemCollectionUI that repaints collection for collection: " + _guid.guid);
        }

        public void OnTriggerUnUsed(Character character, TriggerEventData data)
        {
            if (_activeUI != null)
            {
                _activeUI.window.Hide();
            }
        }
    }
}