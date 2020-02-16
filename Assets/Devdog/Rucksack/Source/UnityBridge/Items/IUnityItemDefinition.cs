using System;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    public interface IUnityItemDefinition : IItemDefinition, IEquatable<IUnityItemDefinition>
    {
        /// <summary>
        /// The icon used for UI
        /// </summary>
        Sprite icon { get; set; }
        
        /// <summary>
        /// The model / prefab used when instantiating the item into the world.
        /// </summary>
        GameObject worldModel { get; set; }
    }
}