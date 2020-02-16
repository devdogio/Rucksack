using Devdog.Rucksack.UI;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.Integrations.TMP
{
	/// <summary>
	/// Use this component instead of ItemCollectionTooltipHandler if you want to use
	/// TextMesh Pro instead of Unity text.
	/// </summary>
	public abstract class TMP_ItemCollectionTooltipHandlerBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		protected readonly ILogger _logger;
		private TMP_ItemTooltipUI _tooltip;

		protected TMP_ItemCollectionTooltipHandlerBase()
		{
			_logger = _logger ?? new UnityLogger("[UI][Collection] ");
		}

		protected TMP_ItemTooltipUI tooltip
		{
			get
			{
				if (_tooltip == null)
				{
					_tooltip = FindObjectOfType<TMP_ItemTooltipUI>();
				}

				return _tooltip;
			}
		}

		public abstract void OnPointerEnter(PointerEventData eventData);

		public abstract void OnPointerExit(PointerEventData eventData);
	}

	/// <summary>
	/// Use this component instead of ItemCollectionTooltipHandler if you want to use
	/// TextMesh Pro instead of Unity text.
	/// </summary>
	abstract public class TMP_ItemCollectionTooltipHandlerBase<T> : TMP_ItemCollectionTooltipHandlerBase, ICollectionSlotInputHandler<T>
		where T : IEquatable<T>
	{

		protected CollectionSlotUIBase<T> _slot;

		private void Awake()
		{
			_slot = GetComponent<CollectionSlotUIBase<T>>();
			Assert.IsNotNull(_slot, "No SlotUI found on input handler!");
		}



		override public void OnPointerExit(PointerEventData eventData)
		{
			tooltip?.Repaint(null, 0, eventData);
		}

		public void OnDisable()
		{
			tooltip?.Repaint(null, 0, null);
		}
	}
}