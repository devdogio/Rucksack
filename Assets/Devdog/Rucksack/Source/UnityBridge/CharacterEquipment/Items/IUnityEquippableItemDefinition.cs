using Devdog.Rucksack.CharacterEquipment.Items;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    public interface IUnityEquippableItemDefinition : IUnityItemDefinition, IWeight, IEquippableItemDefinition
    {
        GameObject equipmentModel { get; }
    }
}