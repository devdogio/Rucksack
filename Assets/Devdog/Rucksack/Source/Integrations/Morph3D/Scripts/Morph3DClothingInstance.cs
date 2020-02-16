using System.Linq;
using System;
using MORPH3D;

namespace Devdog.Rucksack.Integrations.Morph3D
{
	public class Morph3DClothingInstance : Morph3DEquippableItemInstance<Morph3DCloththingDefinition>
	{
		protected Morph3DClothingInstance()
		{ }

		protected Morph3DClothingInstance(Guid ID, Morph3DEquippableItemDefinition itemDefinition)
			: base(ID, itemDefinition)
		{ }

		protected override void Attach(M3DCharacterManager characterManager)
		{
			characterManager.AttachCIClothing(morph3DDefinition.item, true);
			characterManager.DetectAttachedClothing();
		}

		protected override void Dettach(M3DCharacterManager characterManager)
		{
			var itemsToRemove = characterManager
				.GetAllClothing()
				.Where(item => item.name == morph3DDefinition.item.name);


			foreach (var item in itemsToRemove)
			{
				UnityEngine.Object.DestroyObject(item.gameObject);
			}

			characterManager.DetectAttachedClothing();
		}
	}
}
