using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    /// <summary>
    /// A simple class that indicates which equipment types can be equipped on this object / itemGuid.
    /// </summary>
    public sealed class UnityEquipmentTypes : MonoBehaviour
    {
        [SerializeField]
        private UnityEquipmentType[] _equipmentTypes = new UnityEquipmentType[0];
        public UnityEquipmentType[] equipmentTypes
        {
            get { return _equipmentTypes; }
        }

        [SerializeField]
        private string _mountPointName = "";
        public string mountPointName
        {
            get { return _mountPointName; }
        }
    }
}