using Devdog.General2.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public class DragAndDropHandler : IDragAndDropHandler
    {
        protected readonly ILogger logger;
        protected readonly List<IDropAreaSourceOverwriter> _overwritersCache = new List<IDropAreaSourceOverwriter>();
        public DragAndDropHandler()
        {
            logger = new UnityLogger("[UI][DragAndDrop] ");
        }

        public virtual Result<bool> CanDrag(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            return true;
        }

        public virtual Result<bool> CanEndDrag(DragAndDropUtility.Model model, List<IDropArea> dropAreas, List<IDropAreaSourceOverwriter> overwriters, PointerEventData eventData)
        {
            foreach (var overwriter in overwriters)
            {
                var canOverwrite = overwriter.CanDropDraggingItemOnTarget(model, dropAreas, eventData);
                if (canOverwrite.result)
                {
                    return canOverwrite;
                }
            }

            foreach (var dropArea in dropAreas)
            {
                var canDrop = dropArea.CanDropDraggingItem(model, eventData);
                if (canDrop.result)
                {
                    return canDrop;
                }
            }

            return new Result<bool>(false, Errors.UIDragFailedNoReceiver);
        }
        
        public virtual Result<bool> BeginDrag(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            var canDrag = CanDrag(model, eventData);
            if (canDrag.result == false)
            {
                return canDrag;
            }
            
            if (model.dataObject != null)
            {
                logger.LogVerbose("Start drag of item " + model.dataObject, model.source);
                return true;
            }

            return new Result<bool>(false, Errors.UIDragFailed);
        }

        public virtual void Drag(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            UIUtility.PositionRectTransformAtPosition(model.draggingObject, model.draggingObject, eventData.position);
        }

        public virtual Result<bool> EndDrag(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            if (model.draggingObject == null)
            {
                return new Result<bool>(false, Errors.UIDragFailed);
            }
            
            var dropAreas = GetHoveringDropAreas(eventData);
            if (dropAreas.Contains(model.source.GetComponent<IDropArea>()))
            {
                // Ended on self, don't do a thing...
                return true;
            }

            var dropOverwriters = GetHoveringDropOverwriterAreas(model, eventData);
            var canEndDrag = CanEndDrag(model, dropAreas, dropOverwriters, eventData);
            if (canEndDrag.result == false)
            {
                return canEndDrag;
            }

            foreach (var overwriter in dropOverwriters)
            {
                var canOverwrite = overwriter.CanDropDraggingItemOnTarget(model, dropAreas, eventData);
                if (canOverwrite.result)
                {
                    overwriter.DropDraggingItemOnTarget(model, dropAreas, eventData);
                    return true;
                }
            }

            foreach (var dropArea in dropAreas)
            {
                var canDrop = dropArea.CanDropDraggingItem(model, eventData);
                if (canDrop.result)
                {
                    dropArea.DropDraggingItem(model, eventData);
                    return true;
                }
            }   

            return new Result<bool>(false, Errors.UIDragFailedNoReceiver);
        }

        protected virtual List<IDropArea> GetHoveringDropAreas(PointerEventData eventData)
        {
            var l = new List<IDropArea>();
            
            // Unity "sometimes" returns the hovered list from back to front sometimes from front to back...
            var hovered = eventData.hovered;
            foreach (var h in hovered)
            {
                l.AddRange(h.GetComponents<IDropArea>());
            }

            return l;
        }
        
        protected virtual List<IDropAreaSourceOverwriter> GetHoveringDropOverwriterAreas(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            model.source.GetComponents<IDropAreaSourceOverwriter>(_overwritersCache);
            return _overwritersCache;
        }
        
        public virtual void Clean(DragAndDropUtility.Model model)
        {
            if(model != null && model.draggingObject != null)
            {
                UnityEngine.Object.Destroy(model.draggingObject.gameObject);
            }
        }
    }
}