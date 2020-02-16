using Devdog.General2;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public sealed class EquipmentCollectionSlotInputHandler : MonoBehaviour, ICollectionSlotInputHandler<IEquippableItemInstance>, IPointerClickHandler
    {
        public PointerEventData.InputButton unEquipButton = PointerEventData.InputButton.Right;
        public bool consumeEvent = true;

        private CollectionSlotUIBase<IEquippableItemInstance> _slot;
        private readonly ILogger _logger;

        private EquipmentCollectionSlotInputHandler()
        {
            _logger = _logger ?? new UnityLogger("[UI][Collection] ");
        }

        private void Awake()
        {
            _slot = GetComponent<CollectionSlotUIBase<IEquippableItemInstance>>();
            Assert.IsNotNull(_slot, "No SlotUI found on input handler!");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == unEquipButton && _slot.current != null)
            {
                var player = PlayerManager.currentPlayer;
                var used = _slot.current.Use(player, new ItemContext()
                {
                    useAmount = _slot.collection.GetAmount(_slot.collectionIndex)
                });
                
                _logger.Error("", used.error, this);

                if (consumeEvent)
                {
                    eventData.Use();
                }
            }
        }
    }
}