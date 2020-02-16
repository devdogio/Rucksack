using System;
using Devdog.General2;
using Devdog.General2.UI;
using Devdog.Rucksack.Collections;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    [RequireComponent(typeof(UIWindow))]
    public class CollectionConditionalShowUI : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private string _collectionName;

        [Required]
        [SerializeField]
        private UIWindow _window;

        private void Start()
        {
            CollectionRegistry.byName.OnAddedItem += OnAdded;
            CollectionRegistry.byName.OnRemovedItem += OnRemoved;
        }

        private void OnDestroy()
        {
            CollectionRegistry.byName.OnAddedItem -= OnAdded;
            CollectionRegistry.byName.OnRemovedItem -= OnRemoved;
        }
        private void OnAdded(string key, ICollection value)
        {
            if (key == _collectionName)
            {
                _window.Show();
            }
        }
        
        private void OnRemoved(string key, ICollection value)
        {
            if (key == _collectionName)
            {
                _window.Hide();
            }
        }
    }
}