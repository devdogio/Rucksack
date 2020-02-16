using Devdog.General2;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;

namespace Devdog.Rucksack.Items
{
    public partial class UnityEquippableItemInstance : UnityItemInstance, IEquippableItemInstance
    {
        public IEquipmentCollection<IEquippableItemInstance> equippedTo {
            get { return collectionEntry?.collection as IEquipmentCollection<IEquippableItemInstance>; }
        }

        public new IEquippableItemDefinition itemDefinition { get; }
        public IEquipmentType equipmentType
        {
            get { return itemDefinition.equipmentType; }
        }

        // Constructor for (de)serialization
        protected UnityEquippableItemInstance()
        { }

        protected UnityEquippableItemInstance(System.Guid ID, IUnityEquippableItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        {
            this.itemDefinition = itemDefinition;
        }

        public override Result<bool> CanUse(Character character, ItemContext useContext)
        {
            var canUse = base.CanUse(character, useContext);
            if (canUse.result == false)
            {
                return canUse;
            }
            
            var equippable = character.GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
            if (equippable == null)
            {
                return new Result<bool>(false, Errors.CharacterInvalid);
            }

            return true;
        }

        public override Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            var equippableCharacter = character.GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
            if (equippedTo == null)
            {
                if (useContext.targetIndex < 0)
                {
                    var equippedResult = equippableCharacter.Equip(this, useContext.useAmount);
                    if (equippedResult.error != null)
                    {
                        return new Result<ItemUsedResult>(null, equippedResult.error);
                    }
                }
                else
                {
                    var equippedResult = equippableCharacter.EquipAt(useContext.targetIndex, this, useContext.useAmount);
                    if (equippedResult.error != null)
                    {
                        return new Result<ItemUsedResult>(null, equippedResult.error);
                    }
                }
            }
            else
            {
                Result<UnEquipmentResult> unEquippedResult;
                if (useContext.targetIndex < 0)
                {
                    unEquippedResult = equippableCharacter.UnEquip(this, useContext.useAmount);
                }
                else
                {
                    // TODO: Unequip current item to a target slot (in case dragged)
                    unEquippedResult = equippableCharacter.UnEquip(this, useContext.useAmount);
                }
             
                if (unEquippedResult.error != null)
                {
                    return new Result<ItemUsedResult>(null, unEquippedResult.error);
                }
            }
            
            return new Result<ItemUsedResult>(new ItemUsedResult(useContext.useAmount, false, 0f));
        }

        public virtual void OnEquipped(int index, IEquipmentCollection<IEquippableItemInstance> collection)
        {
            logger.LogVerbose("Item equipped! :: " + ToString(), this);
        }

        public virtual void OnUnEquipped(int index, IEquipmentCollection<IEquippableItemInstance> collection)
        {
            logger.LogVerbose("Item un-equipped! :: " + ToString(), this);
        }
        
        public bool Equals(UnityEquippableItemInstance other)
        {
            return base.Equals(other);
        }
        
        public bool Equals(IEquippableItemInstance other)
        {
            return base.Equals(other);
        }
        
        public override string ToString()
        {
            return base.ToString() + ":" + equipmentType;
        }

    }
}