using System.Linq;
using System;
using MORPH3D;

namespace Devdog.Rucksack.Integrations.Morph3D
{
	public class Morph3DPropInstance : Morph3DEquippableItemInstance<Morph3DPropDefinition>
	{
		protected Morph3DPropInstance()
		{ }

		protected Morph3DPropInstance(Guid ID, Morph3DEquippableItemDefinition itemDefinition)
			: base(ID, itemDefinition)
		{ }

		protected override void Attach(M3DCharacterManager characterManager)
		{
			characterManager.AttachCIProp(morph3DDefinition.item, true);
			characterManager.DetectAttachedProps();
		}

		protected override void Dettach(M3DCharacterManager characterManager)
		{
			characterManager.DetachAndUnloadProp(morph3DDefinition.item);

			var itemsToRemove = characterManager
				.GetAllAttachedProps()
				.Where(item => item.name == morph3DDefinition.item.name);


			foreach (var item in itemsToRemove)
			{
				UnityEngine.Object.DestroyObject(item.gameObject);
			}

			characterManager.DetectAttachedProps();
		}
	}
}
