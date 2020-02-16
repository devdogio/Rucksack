using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public sealed class ItemCollectionDropAreaUI : MonoBehaviour, IDropArea, IPointerClickHandler
    {
        [SerializeField]
        private LayerMask _raycastLayerMask = -1;
        
        private readonly ILogger _logger;
        public ItemCollectionDropAreaUI()
        {
            _logger = new UnityLogger("[UI] ");
        }
        
        public Result<bool> CanDropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            eventData.Use();
            if (model.dataObject is IUnityItemInstance)
            {
                return true;
            }

            return false;
        }

        public void DropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            eventData.Use();

            var obj = (IUnityItemInstance)model.dataObject;
            var worldPosition = CalcDropPosition();
            var dropped = obj.Drop(PlayerManager.currentPlayer, worldPosition);
            _logger.Error("", dropped.error);
        }

        private Vector3 CalcDropPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, _raycastLayerMask))
            {
                return hit.point + (Vector3.up * 0.5f);
            }

            var player = PlayerManager.currentPlayer;
            return player.transform.position + (Vector3.right * 2f) + (Vector3.up * 0.5f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!DragAndDropUtility.isDragging || DragAndDropUtility.currentDragModel == null)
                return;

            var dragHandler = DragAndDropUtility.currentDragModel.source.GetComponent<ItemCollectionSlotDragHandler>();
            if (dragHandler == null)
                return;

            if (dragHandler.HandlePointerClick)
            {
                DragAndDropUtility.EndDrag(eventData);
                eventData.Use();
            }
        }
    }
}