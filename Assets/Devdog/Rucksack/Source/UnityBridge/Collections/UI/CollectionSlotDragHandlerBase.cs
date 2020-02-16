using System;
using System.Collections;
using Devdog.Rucksack.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public abstract class CollectionSlotDragHandlerBase<T> : MonoBehaviour, ICollectionSlotInputHandler<T>, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IDropArea
        where T : class, IEquatable<T>
    {
        [SerializeField]
        protected PointerEventData.InputButton dragButton = PointerEventData.InputButton.Left;

        [SerializeField]
        protected bool handlePointerClick = false;

        public bool HandlePointerClick { get { return this.handlePointerClick; } }

        [SerializeField]
        protected bool consumeEvent = true;

        protected ILogger logger;
        protected CollectionSlotUIBase<T> slot;
        protected Coroutine activeCoroutine;

        public CollectionSlotDragHandlerBase()
        {
            logger = new UnityLogger("[UI][Collection] ");
        }
        
        protected virtual void Awake()
        {
            slot = GetComponent<CollectionSlotUIBase<T>>();
            Assert.IsNotNull(slot, "Slot InputHandler couldn't find itemGuid..");
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (handlePointerClick)
            {
                if (DragAndDropUtility.isDragging)
                {
                    OnEndDrag(eventData);
                }
                else
                {
                    OnBeginDrag(eventData);
                }
            }
        }
        
        protected virtual RectTransform GetDragObject(PointerEventData eventData, Canvas canvasParent)
        {
            var clone = Instantiate(gameObject, eventData.position, Quaternion.identity, canvasParent.transform).GetComponent<RectTransform>();
            clone.transform.localScale = Vector3.one;
            clone.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            clone.anchorMin = Vector2.one * 0.5f;
            clone.anchorMax = Vector2.one * 0.5f;

            var group = clone.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.ignoreParentGroups = true;
            group.interactable = false;
            
            return clone;
        }
        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == dragButton && slot.current != null)
            {
                var dragClone = GetDragObject(eventData, GetComponentInParent<Canvas>()?.rootCanvas);
                DragAndDropUtility.BeginDrag(new DragAndDropUtility.Model(GetComponent<RectTransform>(), dragClone, slot.current), eventData);
                
                if (consumeEvent)
                {
                    eventData.Use();
                }
                
                if (handlePointerClick)
                {
                    activeCoroutine = StartCoroutine(ManualDragLoop());
                }
            }
        }
        
        private IEnumerator ManualDragLoop()
        {
            while (DragAndDropUtility.isDragging)
            {
                OnDrag(new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition,
                });

                yield return null;
            }
        }
        
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == dragButton && DragAndDropUtility.isDragging)
            {
                DragAndDropUtility.Drag(eventData);
                
                if (consumeEvent)
                {
                    eventData.Use();
                }
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == dragButton && DragAndDropUtility.isDragging)
            {
                DragAndDropUtility.EndDrag(eventData);
                
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                
                if (consumeEvent)
                {
                    eventData.Use();
                }
            }
        }
        
        public virtual Result<bool> CanDropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            var beginSlot = model.source.GetComponent<CollectionSlotUIBase>();
            var canSet = slot.collection.CanSetBoxed(slot.collectionIndex, model.dataObject, beginSlot.collection.GetAmount(beginSlot.collectionIndex), new CollectionContext()
            {
                validationFlags = ~CollectionContext.Validations.SpecificInstance,
                originalIndex = beginSlot.collectionIndex
            });
            
            if (canSet.error != null)
            {
                return new Result<bool>(false, Errors.UIDragFailedIncompatibleDragObject);
            }

            return true;
        }

        public virtual void DropDraggingItem(DragAndDropUtility.Model model, PointerEventData eventData)
        {
            var beginSlot = model.source.GetComponent<CollectionSlotUIBase>();
            var fromCol = beginSlot.collection;
            var fromIndex = beginSlot.collectionIndex;

            if (fromCol != null)
            {
                var result = fromCol.SwapOrMerge(fromIndex, slot.collection, slot.collectionIndex, fromCol.GetAmount(fromIndex));
                logger.Error("", result.error, this);
            }
            else
            {
                logger.Warning("Couldn't drag item to new itemGuid. Source collection is not a compatible type", this);
            }
        }
    }
}