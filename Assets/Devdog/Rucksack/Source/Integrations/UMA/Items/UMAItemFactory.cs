using UnityEngine;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Integrations.UMA
{
    [RequireComponent(typeof(UnityItemFactory))]
    public class UMAItemFactory : MonoBehaviour
    {
        public void Awake()
        {
            ItemFactory.Bind<UMAEquippableItemDefinition, UMAEquippableItemInstance>();
			ItemFactory.Bind<UMARecipeItemDefinition, UMARecipeItemInstance>();
        }
    } 
}
