using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemCollectionSlotUINewItem : MonoBehaviour, ICollectionSlotUICallbackReceiver<IItemInstance>, IPointerEnterHandler, ISelectHandler
    {
        [Required]
        [SerializeField]
        private Image _newItemIcon;

        private bool _isNew = false;
        
        public void Repaint(IItemInstance item, int amount)
        {
            _isNew = true; // TODO: Set this properly
            if (item != null)
            {
                _newItemIcon.gameObject.SetActive(true);
                if (_isNew)
                {
                    _newItemIcon.CrossFadeAlpha(1f, 0.3f, false);
//                    _newItemIcon.color = Color.white;
                }
            }
            else
            {
                _newItemIcon.gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetIsNew(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetIsNew(false);
        }

        private void SetIsNew(bool isNew)
        {
            _isNew = isNew;
            if (_isNew == false)
            {
                _newItemIcon.CrossFadeAlpha(0f, 0.3f, false);
            }
        }
    }
}