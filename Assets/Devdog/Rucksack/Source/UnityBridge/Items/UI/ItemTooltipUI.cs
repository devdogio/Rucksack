using Devdog.General2.UI;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    [RequireComponent(typeof(UIWindow))]
    public class ItemTooltipUI : UIMonoBehaviour
    {
        [SerializeField]
        protected Text itemName;
        [SerializeField]
        protected Text itemDescription;
        [SerializeField]
        protected Image icon;


        [SerializeField]
        protected Vector3 offset;

        private UIWindow _window;

        private void Awake()
        {
            _window = GetComponent<UIWindow>();
        }
        
        public virtual void Repaint(IUnityItemInstance item, int amount, PointerEventData eventData)
        {
            if (item != null)
            {
                Set(itemName, item.itemDefinition.name); 
                Set(itemDescription, item.itemDefinition.description); 
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
                UIUtility.PositionRectTransformAtPosition(this, transform, Input.mousePosition + offset);
            }
        }
    }
}