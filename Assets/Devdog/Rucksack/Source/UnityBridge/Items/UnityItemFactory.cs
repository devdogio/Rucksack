using UnityEngine;

namespace Devdog.Rucksack.Items
{
    [ExecuteInEditMode]
    public sealed class UnityItemFactory : MonoBehaviour
    {
        private void Awake()
        {
            ItemFactory.binder = new DefaultItemFactoryBinder(new UnityLogger("[Item][Factory] "));
            ItemFactory.Bind<UnityItemDefinition, UnityItemInstance>();
            ItemFactory.Bind<UnityEquippableItemDefinition, UnityEquippableItemInstance>();
        }
    }
}