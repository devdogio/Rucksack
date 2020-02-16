using System;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;

namespace Devdog.Rucksack.Tests
{
    public class EquippableItemInstance : CollectionItemInstance, IEquippableItemInstance, IEquatable<EquippableItemInstance>
    {
        public IEquipmentCollection<IEquippableItemInstance> equippedTo {
            get { return collectionEntry?.collection as IEquipmentCollection<IEquippableItemInstance>; }
        }
        
        public new IEquippableItemDefinition itemDefinition { get; }
        public IEquipmentType equipmentType
        {
            get { return itemDefinition.equipmentType; }
        }

        public EquippableItemInstance(Guid ID, IEquippableItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        {
            this.itemDefinition = itemDefinition;
        }
        
        public virtual void OnEquipped(int index, IEquipmentCollection<IEquippableItemInstance> collection)
        {
            
        }

        public virtual void OnUnEquipped(int index, IEquipmentCollection<IEquippableItemInstance> collection)
        {
            
        }
                
//        public Result<bool> DropDraggingItem(Vector3 positon, Quaternion rotation, Transform parent = null)
//        {
//            return true;
//        }
        
        public bool Equals(EquippableItemInstance other)
        {
            return base.Equals(other);
        }
        
        public bool Equals(IEquippableItemInstance other)
        {
            return base.Equals(other);
        }

        public override object Clone()
        {
            var clone = (EquippableItemInstance)MemberwiseClone();
            clone.ID = System.Guid.NewGuid();
//            clone.collectionEntry = null;
            return clone;
        }

        public override string ToString()
        {
            return base.ToString() + ":" + equipmentType;
        }
    }
}