using System.Collections.Generic;
using Devdog.General2;
using Devdog.Rucksack.Items;
using System;
using MORPH3D;
using MORPH3D.COSTUMING;

namespace Devdog.Rucksack.Integrations.Morph3D
{
	public abstract class Morph3DEquippableItemInstance<TDefinition> : UnityEquippableItemInstance
		where TDefinition : Morph3DEquippableItemDefinition
    {
        protected TDefinition morph3DDefinition
        {
            get { return (TDefinition) itemDefinition; }
        }

        protected Morph3DEquippableItemInstance()
        { }

        protected Morph3DEquippableItemInstance(Guid ID, Morph3DEquippableItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        { }

        public override Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            var characterManager = character.GetComponent<M3DCharacterManager>();
            if(characterManager == null)
            {
                return Morph3DErrors.CharacterHasNoM3DCharacterManager;
            }

            var result = base.DoUse(character, useContext);
            if(result.error != null)
            {
                return result;
            }

            if (equippedTo != null)
            {
				Attach(characterManager);
            }
            else
            {
				Dettach(characterManager);
			}

            return result;
        }

		abstract protected void Attach(M3DCharacterManager characterManager);
		abstract protected void Dettach(M3DCharacterManager characterManager);
        
    }
}
