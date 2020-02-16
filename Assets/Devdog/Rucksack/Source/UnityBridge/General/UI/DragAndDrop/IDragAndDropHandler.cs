using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public interface IDragAndDropHandler
    {
        Result<bool> BeginDrag(DragAndDropUtility.Model model, PointerEventData eventData);
        void Drag(DragAndDropUtility.Model model, PointerEventData eventData);
        Result<bool> EndDrag(DragAndDropUtility.Model model, PointerEventData eventData);
        void Clean(DragAndDropUtility.Model model);
    }
}