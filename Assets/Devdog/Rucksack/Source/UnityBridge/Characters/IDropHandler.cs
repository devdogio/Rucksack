using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Characters
{
    public interface IDropHandler
    {
        float pickupDistance { get; }
        float maxDropDistance { get; }
        
        Result<GameObject> Drop(Character character, IUnityItemInstance item, Vector3 worldPosition);
    }
}