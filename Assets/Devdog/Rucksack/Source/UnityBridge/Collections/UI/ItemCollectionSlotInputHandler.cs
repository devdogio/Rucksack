using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemCollectionSlotInputHandler : MonoBehaviour, ICollectionSlotInputHandler<IItemInstance>, IPointerClickHandler
    {
        public PointerEventData.InputButton useButton = PointerEventData.InputButton.Right;
        public bool consumeEvent = true;
        
        private CollectionSlotUIBase<IItemInstance> _slot;
        private readonly ILogger _logger;
        private ItemCollectionSlotInputHandler()
        {
            _logger = _logger ?? new UnityLogger("[UI][Collection] ");
        }

        private void Awake()
        {
            _slot = GetComponent<CollectionSlotUIBase<IItemInstance>>();
            Assert.IsNotNull(_slot, "No SlotUI found on input handler!");
        }
                
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == useButton && _slot.current != null)
            {
                var used = _slot.current.Use(PlayerManager.currentPlayer, new ItemContext());
                _logger.Error("", used.error);
                
                if (consumeEvent)
                {
                    eventData.Use();
                }
            }
        }
    }
}