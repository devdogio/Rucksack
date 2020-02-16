using UnityEngine;

namespace Devdog.Rucksack.Items
{
    [ExecuteInEditMode]
    public sealed class PUN2ItemFactory : MonoBehaviour
    {
        private void Awake()
        {
            ItemFactory.binder = new PUN2ItemFactoryBinder(new UnityLogger("[PUN2][Item][Factory] "));
            ItemFactory.Bind<UnityItemDefinition, PUN2ItemInstance>();
            ItemFactory.Bind<UnityEquippableItemDefinition, PUN2EquippableItemInstance>();
        }
    }
}