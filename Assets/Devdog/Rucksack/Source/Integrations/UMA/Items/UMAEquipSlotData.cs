using Devdog.General2;
using UMA;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.UMA
{
    [System.Serializable]
    public class UMAEquipSlotData
    {
        [Tooltip("The UMA slot name of the character you want to equip it to. \nExample: MaleTorso")]
        [Required]
        public SlotDataAsset umaEquipSlot;

        public bool umaOverrideColor = false;
        public Color umaOverlayColor = Color.white;

        [Tooltip("The overlay object to equip. Use the slot to define it's equip location.")]
        [Required]
        public OverlayDataAsset umaOverlayDataAsset;

        public SlotDataAsset umaSlotDataAsset;
        public SlotDataAsset umaReplaceSlot;

        public SlotData umaReplacedSlot;
        public SlotData umaPrevReplacedSlot;
    }
}
