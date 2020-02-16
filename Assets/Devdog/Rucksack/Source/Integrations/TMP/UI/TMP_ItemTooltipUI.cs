using Devdog.General2.UI;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.Rucksack.Integrations.TMP
{
    [RequireComponent(typeof(UIWindow))]
    public class TMP_ItemTooltipUI : UIMonoBehaviour
    {
        [SerializeField]
        protected TMP_Text itemName;
        [SerializeField]
        protected TMP_Text itemDescription;
        [SerializeField]
        protected Image icon;

        private UIWindow _window;

        private void Awake()
        {
            _window = GetComponent<UIWindow>();
        }

        public virtual void Repaint(IUnityItemInstance item, int amount, PointerEventData eventData)
        {
            if (item != null)
            {
                this.Set(itemName, item.itemDefinition.name);
                this.Set(itemDescription, item.itemDefinition.description);
                Set(icon, item.itemDefinition.icon);

                _window.Show();
            }
            else
            {
                _window.Hide();
            }
        }

        protected virtual void Update()
        {
            if (_window.isVisible)
            {
                // Update position to mouse
                UIUtility.PositionRectTransformAtPosition(this, transform, Input.mousePosition);
            }
        }
    }
}