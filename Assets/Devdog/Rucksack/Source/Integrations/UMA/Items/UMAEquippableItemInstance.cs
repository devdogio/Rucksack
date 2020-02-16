using System.Collections.Generic;
using Devdog.General2;
using UMA;
using UnityEngine;
using System.Linq;
using Devdog.Rucksack.Items;
using System;
using UMA.CharacterSystem;

namespace Devdog.Rucksack.Integrations.UMA
{
    public class UMAEquippableItemInstance : UnityEquippableItemInstance
    {
        protected UMAEquippableItemDefinition umaDefinition
        {
            get { return (UMAEquippableItemDefinition) itemDefinition; }
        }

        protected UMAEquippableItemInstance()
        { }

        protected UMAEquippableItemInstance(Guid ID, UMAEquippableItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        { }

        public override Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            var umaData = character.GetComponent<UMAData>();
            if(umaData == null)
            {
                return UMAErrors.CharacterHasNoUMAData;
            }

			var avatar = character.GetComponent<DynamicCharacterAvatar>();
			if (avatar == null)
			{
				return UMAErrors.CharacterHasNoDynamicCharacterAvatar;
			}

			var result = base.DoUse(character, useContext);
            if(result.error != null)
            {
                return result;
            }

            if (equippedTo != null)
            {
                foreach (var umaEquipSlot in umaDefinition.equipSlotsData)
                {
                    var slot = GetUMASlot(umaData, umaEquipSlot.umaEquipSlot.slotName);
                    if (slot == null && umaEquipSlot.umaSlotDataAsset == null)
                    {
                        return UMAErrors.NoSuitableUMASlot; //Should add parameters?
                    }

                    if (umaEquipSlot.umaSlotDataAsset != null)
                    {
                        slot = UMAReplaceSlot(umaData, umaEquipSlot);
                        UMAMarkAllDirty(umaData);
                    }

                    if (slot != null)
                    {
                        

                        var overlayLibrary = avatar.context.overlayLibrary;
                        UMAAddOverlay(umaEquipSlot, slot, overlayLibrary, umaEquipSlot.umaOverlayDataAsset.overlayName);
                    }
                }
			}
            else
            {
                foreach (var umaEquipSlot in umaDefinition.equipSlotsData)
                {
                    var slot = GetUMASlot(umaData, umaEquipSlot.umaEquipSlot.slotName);
                    if (slot == null && umaEquipSlot.umaSlotDataAsset == null)
                    {
                        return UMAErrors.NotVisuallyEquipped;
                    }

                    if (umaEquipSlot.umaPrevReplacedSlot != null)
                    {
                        UMARestoreReplacedSlot(umaData, umaEquipSlot);
                        UMAMarkAllDirty(umaData);
                    }
                    else
                    {
                        if (slot != null)
                        {
                            UMARemoveOverlay(slot, umaEquipSlot.umaOverlayDataAsset.overlayName);
                        }
                    }
                }				
			}

            umaData.isTextureDirty = true;
            umaData.isAtlasDirty = true;
            umaData.Dirty();

			return result;
        }

        private void UMAAddOverlay(UMAEquipSlotData equipSlotData, SlotData slot, OverlayLibraryBase overlayLibrary, string overlayName)
        {
            slot.AddOverlay(overlayLibrary.InstantiateOverlay(overlayName));

            if (equipSlotData.umaOverrideColor)
            {
                slot.SetOverlayColor(equipSlotData.umaOverlayColor, overlayName);
            }
        }

        private void UMARemoveOverlay(SlotData slot, string overlayName)
        {
            slot.RemoveOverlay(overlayName);
        }

        private SlotData UMAReplaceSlot(UMAData umaData, UMAEquipSlotData equipSlotData)
        {
            var l = new List<SlotData>(umaData.umaRecipe.slotDataList);

            // Remove the object we're replacing
            if (equipSlotData.umaReplaceSlot != null)
            {
                equipSlotData.umaReplacedSlot = GetUMASlot(umaData, equipSlotData.umaReplaceSlot.slotName);
                l.RemoveAll(o => o != null && o.asset.slotName == equipSlotData.umaReplaceSlot.slotName);
            }

            // Add new slot
            var slot = new SlotData(equipSlotData.umaSlotDataAsset);
            l.Add(slot);
            equipSlotData.umaPrevReplacedSlot = slot;

            umaData.umaRecipe.slotDataList = l.ToArray();

            return slot;
        }

        private void UMAMarkAllDirty(UMAData umaData)
        {
            umaData.isTextureDirty = true;
            umaData.isAtlasDirty = true;
            umaData.isMeshDirty = true;
            umaData.isShapeDirty = true;
        }

        private void UMARestoreReplacedSlot(UMAData umaData, UMAEquipSlotData equipSlotData)
        {
            var l = new List<SlotData>(umaData.umaRecipe.slotDataList);
            l.Add(equipSlotData.umaReplacedSlot);
            l.Remove(equipSlotData.umaPrevReplacedSlot);
            umaData.umaRecipe.slotDataList = l.ToArray();
        }

        protected virtual SlotData GetUMASlot(UMAData umaData, string slotName)
        {
            return umaData.umaRecipe.slotDataList.FirstOrDefault(o => o != null && o.asset.slotName == slotName);
        }
    }

	public class UMARecipeItemInstance : UnityEquippableItemInstance
	{
		protected UMARecipeItemDefinition umaDefinition
		{
			get { return (UMARecipeItemDefinition)itemDefinition; }
		}

		protected UMARecipeItemInstance()
		{ }

		protected UMARecipeItemInstance(Guid ID, UMARecipeItemDefinition itemDefinition)
			: base(ID, itemDefinition)
		{ }

		public override Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
		{
			var umaData = character.GetComponent<UMAData>();
			if (umaData == null)
			{
				return UMAErrors.CharacterHasNoUMAData;
			}

			var avatar = character.GetComponent<DynamicCharacterAvatar>();
			if (avatar == null)
			{
				return UMAErrors.CharacterHasNoDynamicCharacterAvatar;
			}

			var result = base.DoUse(character, useContext);
			if (result.error != null)
			{
				return result;
			}
			
			if (equippedTo != null)
			{
				avatar.SetSlot(umaDefinition.recipe);
			}
			else
			{
				avatar.ClearSlot(umaDefinition.recipe.wardrobeSlot);
			}

			avatar.BuildCharacter(false);
			avatar.ForceUpdate(true, true, true);

			return result;
		}

		private void UMAAddOverlay(UMAEquipSlotData equipSlotData, SlotData slot, OverlayLibraryBase overlayLibrary, string overlayName)
		{
			slot.AddOverlay(overlayLibrary.InstantiateOverlay(overlayName));

			if (equipSlotData.umaOverrideColor)
			{
				slot.SetOverlayColor(equipSlotData.umaOverlayColor, overlayName);
			}
		}

		private void UMARemoveOverlay(SlotData slot, string overlayName)
		{
			slot.RemoveOverlay(overlayName);
		}

		private SlotData UMAReplaceSlot(UMAData umaData, UMAEquipSlotData equipSlotData)
		{
			var l = new List<SlotData>(umaData.umaRecipe.slotDataList);

			// Remove the object we're replacing
			if (equipSlotData.umaReplaceSlot != null)
			{
				equipSlotData.umaReplacedSlot = GetUMASlot(umaData, equipSlotData.umaReplaceSlot.slotName);
				l.RemoveAll(o => o != null && o.asset.slotName == equipSlotData.umaReplaceSlot.slotName);
			}

			// Add new slot
			var slot = new SlotData(equipSlotData.umaSlotDataAsset);
			l.Add(slot);
			equipSlotData.umaPrevReplacedSlot = slot;

			umaData.umaRecipe.slotDataList = l.ToArray();

			return slot;
		}

		private void UMAMarkAllDirty(UMAData umaData)
		{
			umaData.isTextureDirty = true;
			umaData.isAtlasDirty = true;
			umaData.isMeshDirty = true;
			umaData.isShapeDirty = true;
		}

		private void UMARestoreReplacedSlot(UMAData umaData, UMAEquipSlotData equipSlotData)
		{
			var l = new List<SlotData>(umaData.umaRecipe.slotDataList);
			l.Add(equipSlotData.umaReplacedSlot);
			l.Remove(equipSlotData.umaPrevReplacedSlot);
			umaData.umaRecipe.slotDataList = l.ToArray();
		}

		protected virtual SlotData GetUMASlot(UMAData umaData, string slotName)
		{
			return umaData.umaRecipe.slotDataList.FirstOrDefault(o => o != null && o.asset.slotName == slotName);
		}
	}
}
