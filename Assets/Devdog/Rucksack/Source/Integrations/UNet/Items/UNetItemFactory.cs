using UnityEngine;

namespace Devdog.Rucksack.Items
{
    [ExecuteInEditMode]
    public sealed class UNetItemFactory : MonoBehaviour
    {
        private void Awake()
        {
            ItemFactory.binder = new UNetItemFactoryBinder(new UnityLogger("[UNet][Item][Factory] "));
            ItemFactory.Bind<UnityItemDefinition, UNetItemInstance>();
            ItemFactory.Bind<UnityEquippableItemDefinition, UNetEquippableItemInstance>();
        }
    }
}