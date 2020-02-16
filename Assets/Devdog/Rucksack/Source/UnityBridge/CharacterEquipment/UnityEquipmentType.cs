using System;
using Devdog.General2.Localization;
using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    [CreateAssetMenu(menuName = RucksackConstants.AddPath + "Equipment type")]
    public sealed class UnityEquipmentType : ScriptableObject, IEquipmentType, ICloneable
    {
        [SerializeField]
        private SerializedGuid _guid;
        public Guid ID
        {
            get { return _guid.guid; }
        }

        [SerializeField]
        private LocalizedString _name = new LocalizedString();
        public new string name
        {
            get { return _name.message; }
            set { _name.message = value; }
        }
        
        public UnityEquipmentType()
        {
            
        }
        
        public static bool operator ==(UnityEquipmentType left, IEquipmentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnityEquipmentType left, IEquipmentType right)
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

        public object Clone()
        {
            var clone = (UnityEquipmentType) MemberwiseClone();
            clone._guid = new SerializedGuid()
            {
                guid = System.Guid.NewGuid()
            };
            
            return clone;
        }

        public void ResetID(System.Guid guid)
        {
            _guid = new SerializedGuid()
            {
                guid = guid
            };
        }

        public override string ToString()
        {
            return name;
        }
    }
}