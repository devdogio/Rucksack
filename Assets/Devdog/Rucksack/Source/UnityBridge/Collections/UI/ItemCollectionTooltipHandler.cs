using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
	public sealed class ItemCollectionTooltipHandler : MonoBehaviour, 
		ICollectionSlotInputHandler<IItemInstance>, 
		IPointerEnterHandler, 
		IPointerExitHandler
    {
        private ItemTooltipUI _tooltip;
        private CollectionSlotUIBase<IItemInstance> _slot;
        private readonly ILogger _logger;

		private ItemTooltipUI tooltip
		{
			get
			{
				if (_tooltip == null)
				{
					_tooltip = FindObjectOfType<ItemTooltipUI>();
				}

				return _tooltip;
			}
		}

		private ItemCollectionTooltipHandler()
        {
            _logger = _logger ?? new UnityLogger("[UI][Collection] ");
        }

        private void Awake()
        {
            _slot = GetComponent<CollectionSlotUIBase<IItemInstance>>();
            Assert.IsNotNull(_slot, "No SlotUI found on input handler!");
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip?.Repaint(_slot.current as IUnityItemInstance, _slot.collection.GetAmount(_slot.collectionIndex), eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {            
            tooltip?.Repaint(null, 0, eventData);
        }

		public void OnDisable()
		{
			tooltip?.Repaint(null, 0, null);
		}
	}
}