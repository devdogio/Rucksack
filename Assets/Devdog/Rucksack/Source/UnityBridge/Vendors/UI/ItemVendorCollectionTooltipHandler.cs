using System.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
	public sealed class ItemVendorCollectionTooltipHandler : 
		MonoBehaviour, 
		ICollectionSlotInputHandler<IVendorProduct<IItemInstance>>,
		IPointerEnterHandler, 
		IPointerExitHandler
    {
        private ItemTooltipUI _tooltip;
        private CollectionSlotUIBase<IVendorProduct<IItemInstance>> _slot;

        private readonly ILogger _logger;
        private Coroutine _checkIfItemIsGoneCoroutine;

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

		private ItemVendorCollectionTooltipHandler()
        {
            _logger = _logger ?? new UnityLogger("[UI][Collection] ");
        }

        private void Awake()
        {
            _slot = GetComponent<CollectionSlotUIBase<IVendorProduct<IItemInstance>>>();
            Assert.IsNotNull(_slot, "No SlotUI found on input handler!");
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
			if (_slot.current != null && tooltip != null)
            {
                if (_checkIfItemIsGoneCoroutine != null)
                {
                    StopCoroutine(_checkIfItemIsGoneCoroutine);
                }

                _checkIfItemIsGoneCoroutine = StartCoroutine(CheckIfItemIsGone());
                tooltip.Repaint(_slot.current.item as IUnityItemInstance, _slot.collection.GetAmount(_slot.collectionIndex), eventData);
            }
        }

        private IEnumerator CheckIfItemIsGone()
        {
            while (true)
            {
                yield return null;
                
                if (_slot.current == null)
                {
                    OnPointerExit(new PointerEventData(EventSystem.current)
                    {
                        position = Input.mousePosition,
                    });
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_checkIfItemIsGoneCoroutine != null)
            {
                StopCoroutine(_checkIfItemIsGoneCoroutine);
            }
            
            tooltip?.Repaint(null, 0, eventData);
        }

		public void OnDisable()
		{
			tooltip?.Repaint(null, 0, null);
		}
	}
}