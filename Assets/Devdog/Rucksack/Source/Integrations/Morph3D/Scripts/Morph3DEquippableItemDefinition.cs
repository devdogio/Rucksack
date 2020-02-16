using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Integrations.Morph3D
{
	public class Morph3DEquippableItemDefinition : UnityEquippableItemDefinition
    {
		
    }

	public class Morph3DEquippableItemDefinition<TItemType> : Morph3DEquippableItemDefinition
	{
		public TItemType item;
	}
}
