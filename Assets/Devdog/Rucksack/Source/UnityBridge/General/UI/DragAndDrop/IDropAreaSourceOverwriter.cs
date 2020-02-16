using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    /// <summary>
    /// This interface can be used to add an extra validation step when dragging and dropping items.
    /// By default the target <see cref="IDropArea"/> implementer decides the if the object can be dropped at the given location.
    /// Using this interface you can overwrite the drop behavior, so that the source of the drag and drop action gets to handle the drop.
    /// </summary>
    public interface IDropAreaSourceOverwriter
    {
        Result<bool> CanDropDraggingItemOnTarget(DragAndDropUtility.Model model, List<IDropArea> targetDropArea, PointerEventData eventData);
        void DropDraggingItemOnTarget(DragAndDropUtility.Model model, List<IDropArea> targetDropArea, PointerEventData eventData);
    }
}