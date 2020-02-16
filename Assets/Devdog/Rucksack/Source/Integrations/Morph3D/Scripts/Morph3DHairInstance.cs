using System.Linq;
using System;
using MORPH3D;

namespace Devdog.Rucksack.Integrations.Morph3D
{
	public class Morph3DHairInstance : Morph3DEquippableItemInstance<Morph3DHairDefinition>
	{
		protected Morph3DHairInstance()
		{ }

		protected Morph3DHairInstance(Guid ID, Morph3DEquippableItemDefinition itemDefinition)
			: base(ID, itemDefinition)
		{ }

		protected override void Attach(M3DCharacterManager characterManager)
		{
			characterManager.AttachCIHair(morph3DDefinition.item, true);
			characterManager.DetectAttachedHair();
		}

		protected override void Dettach(M3DCharacterManager characterManager)
		{
			var itemsToRemove = characterManager
				.GetAllHair()
				.Where(item => item.name == morph3DDefinition.item.name);
			

			foreach(var item in itemsToRemove)
			{
				UnityEngine.Object.DestroyObject(item.gameObject);
			}

			characterManager.DetectAttachedHair();
		}
	}
}
