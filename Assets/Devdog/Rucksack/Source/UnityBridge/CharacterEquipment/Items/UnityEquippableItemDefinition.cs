﻿using System;
using Devdog.General2;
using Devdog.Rucksack.CharacterEquipment;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    [CreateAssetMenu(menuName = RucksackConstants.AddPath + "Unity Equippable Item Definition")]
    public partial class UnityEquippableItemDefinition : UnityItemDefinition, IUnityEquippableItemDefinition
    {
        [Required]
        [SerializeField]
        private UnityEquipmentType _equipmentType;
        public IEquipmentType equipmentType
        {
            get { return this.GetUnityObjectValue(o => o._equipmentType); }
        }

        [SerializeField]
        private string _mountPoint;
        public string mountPoint
        {
            get { return this.GetValue(o => o._mountPoint, ""); }
            set { _mountPoint = value; }
        }

        [SerializeField]
        private GameObject _equipmentModel;

        /// <summary>
        /// The model used for equipping the item to your character.
        /// <remarks>Uses `base.worldModel` if no _equipmentModel is set.</remarks>
        /// </summary>
        public virtual GameObject equipmentModel
        {
            get { return this.GetUnityObjectValue(o => o._equipmentModel) ?? worldModel; }
        }
    }
}