using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.Integrations.TMP
{
	/// <summary>
	/// Use this component instead of ItemCollectionTooltipHandler if you want to use
	/// TextMesh Pro instead of Unity text.
	/// </summary>
	public class TMP_ItemVendorCollectionTooltipHandler : TMP_ItemCollectionTooltipHandlerBase<IVendorProduct<IItemInstance>>
	{
		override public void OnPointerEnter(PointerEventData eventData)
		{
			tooltip?.Repaint(_slot.current.item as IUnityItemInstance, _slot.collection.GetAmount(_slot.collectionIndex), eventData);
		}
	}
}