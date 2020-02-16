using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    /// <summary>
    /// A simple drag and drop utility class for dragging and dropping anything in Unity's UI system.
    /// <remarks>You can swap out the <see cref="IDragAndDropHandler"/> to create your own global drag and drop behavior.</remarks>
    /// </summary>
    public static class DragAndDropUtility
    {
        public class Model
        {
            /// <summary>
            /// The source of the object we're dragging
            /// Note: This can be the same as the dragging object if we're dragging the original item (not cloned)
            /// </summary>
            public readonly RectTransform source;

            /// <summary>
            /// The object we're dragging around.
            /// Note: This will be the same as source if the dragging object is not cloned.
            /// </summary>
            public readonly RectTransform draggingObject;
            
            /// <summary>
            /// The actual data object we're dragging from one place to another. This can be the user's payload.
            /// </summary>
            public readonly object dataObject;

            public Model(RectTransform source, RectTransform draggingObject, object dataObject)
            {
                this.source = source;
                this.draggingObject = draggingObject;
                this.dataObject = dataObject;
            }
        }

        public static event Action<Model, PointerEventData> OnBeginDrag;
        public static event Action<Model, PointerEventData> OnDrag;
        public static event Action<Model, PointerEventData> OnEndDrag;
        
        public static IDragAndDropHandler handler { private get; set; }
        public static Model currentDragModel { get; private set; }

        public static bool isDragging
        {
            get { return currentDragModel != null; }
        }

        static DragAndDropUtility()
        {
            handler = new DragAndDropHandler();
        }
        
        public static Result<bool> BeginDrag(Model model, PointerEventData eventData)
        {
            Clean();
            currentDragModel = model;
            
            var result = handler.BeginDrag(model, eventData);
            OnBeginDrag?.Invoke(model, eventData);
            return result;
        }

        public static void Drag(PointerEventData eventData)
        {
            if (currentDragModel == null)
            {
                throw new ArgumentException("Can not drag, BeginDrag() was never called.");
            }
            
            OnDrag?.Invoke(currentDragModel, eventData);
            handler.Drag(currentDragModel, eventData);
        }

        public static Result<bool> EndDrag(PointerEventData eventData, bool clean = true)
        {
            if (currentDragModel == null)
            {
                throw new ArgumentException("Can not end drag, BeginDrag() was never called.");
            }
            
            var result = handler.EndDrag(currentDragModel, eventData);
            OnEndDrag?.Invoke(currentDragModel, eventData);

            if (clean)
            {
                Clean();
            }
            
            return result;
        }

        public static void Clean()
        {
            handler.Clean(currentDragModel);
            currentDragModel = null;
        }
    }
}