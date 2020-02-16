using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public interface IDropArea
    {
        Result<bool> CanDropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData);
        void DropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData);
    }
}