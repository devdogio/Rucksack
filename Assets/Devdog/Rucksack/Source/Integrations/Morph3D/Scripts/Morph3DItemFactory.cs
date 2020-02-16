using UnityEngine;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Integrations.Morph3D
{
    [RequireComponent(typeof(UnityItemFactory))]
    public class Morph3DItemFactory : MonoBehaviour
    {
        public void Awake()
        {
            ItemFactory.Bind<Morph3DHairDefinition, Morph3DHairInstance>();
			ItemFactory.Bind<Morph3DCloththingDefinition, Morph3DClothingInstance>();
			ItemFactory.Bind<Morph3DPropDefinition, Morph3DPropInstance>();
		}
    } 
}
