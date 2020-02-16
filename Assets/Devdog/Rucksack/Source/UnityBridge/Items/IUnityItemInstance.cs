using System;
using Devdog.General2;
using Devdog.Rucksack.Collections;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    public interface IUnityItemInstance : IItemInstance, IEquatable<IUnityItemInstance>, ICollectionSlotEntry
    {       
        new IUnityItemDefinition itemDefinition { get; }
        
        /// <summary>
        /// DropDraggingItem the item into the world.
        /// </summary>
        Result<GameObject> Drop(Character character, Vector3 worldPosition);

        
        // NOTE: Good idea?? - Completely destroys the item and removes it from all collections it's in.
//        Result<bool> Destroy();
    }
}