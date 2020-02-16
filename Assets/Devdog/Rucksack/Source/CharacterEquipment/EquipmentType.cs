using System;

namespace Devdog.Rucksack.CharacterEquipment
{
    public sealed class EquipmentType : IEquipmentType
    {
        public Guid ID { get; }
        public string name { get; set; }

        public EquipmentType(Guid ID, params IEquipmentType[] incompatible)
        {
            this.ID = ID;
        }
        
        
        
        public static bool operator ==(EquipmentType left, IEquipmentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EquipmentType left, IEquipmentType right)
        {
            return !Equals(left, right);
        }
        
        public bool IsCompatible(IEquipmentType type)
        {
            return true;
        }
        
        
        public bool Equals(IEquipmentType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IEquipmentType) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }
    }
}