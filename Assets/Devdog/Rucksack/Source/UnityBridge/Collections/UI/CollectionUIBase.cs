using System;
using Devdog.General2.UI;
using Devdog.Rucksack.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    /// <summary>
    /// 
    /// 
    /// <remarks>Using <see cref="CollectionSlotUIBase"/> (non generic) for the base class, because Unity can't serialize the generic...</remarks>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireComponent(typeof(UIWindow))]
    public abstract class CollectionUIBase<T> : UIQueuedMonoBehaviour<CollectionSlotUIBase>
        where T : IEquatable<T>
    {
        [SerializeField]
        private string _collectionName;
        public string collectionName
        {
            get { return _collectionName; }
        }

        protected CollectionRegistryFinder<ICollection<T>> finder;
        public ICollection<T> collection
        {
            get { return finder?.collection; }
            set
            {
                if (finder == null)
                {
                    RegisterFinder();
                }
                
                finder.collection = value;
            }
        }

        protected override void Start()
        {
            base.Start();
            
#if UNITY_EDITOR
            OnValidate();
#endif
        }

        protected virtual void OnEnable()
        {
            RegisterFinder();
        }

        protected virtual void OnDisable()
        {
            UnRegisterFinder();
        }

        protected virtual void RegisterFinder()
        {
            if (finder != null)
            {
                UnRegisterFinder();
            }
            
            finder = new CollectionRegistryFinder<ICollection<T>>(collectionName);
            finder.OnCollectionChanged += OnCollectionChanged;

            // Initial
            if (finder.collection != null)
            {
                OnCollectionChanged(null, finder.collection);
            }
        }
        
        protected virtual void UnRegisterFinder()
        {
            finder.OnCollectionChanged -= OnCollectionChanged;
        }
        
        protected virtual void OnCollectionChanged(ICollection<T> before, ICollection<T> after)
        {
            if (before != null)
            {
                before.OnSlotsChanged -= OnCollectionSlotsChanged;
                before.OnSizeChanged -= OnCollectionSizeChanged;
            }

            // Completely new collection; Remove all slots and re-generate.
            if (after != null)
            {
                // Get rid of the old
                foreach (var t in repaintableElements)
                {
                    if (t != null)
                    {
                        Destroy(t.gameObject);
                    }
                }
                    
                repaintableElements = new CollectionSlotUIBase[after.slotCount];
                for (int i = 0; i < after.slotCount; i++)
                {
                    repaintableElements[i] = CreateUIElement(i);
                    Repaint(i);
                }
                    
                after.OnSlotsChanged += OnCollectionSlotsChanged;
                after.OnSizeChanged += OnCollectionSizeChanged;
            }
        }
        
        
#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (prefab == null || prefab.GetComponent<CollectionSlotUIBase<T>>() == null)
            {
                logger.Error($"Slot prefab {prefab} is not a valid type and does not implement {typeof(T).Name}", this);
            }
        }

#endif

        protected override void OnWindowHide()
        {
            base.OnWindowHide();

            if (DragAndDropUtility.isDragging && IsDecendantOf(DragAndDropUtility.currentDragModel.source, uiContainer))
            {
                // Forcefully stop and don't handle drop event if the window is closed mid-drag
                DragAndDropUtility.EndDrag(new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition,
                });
            }
        }

        protected bool IsDecendantOf(Transform descendant, Transform ancestor)
        {
            var r = descendant;
            while (r != null)
            {
                if (r == ancestor)
                {
                    return true;
                }
                
                r = r.parent;
            }

            return false;
        }
        
        protected override CollectionSlotUIBase CreateUIElement(int index)
        {
            var inst = (CollectionSlotUIBase<T>)base.CreateUIElement(index);
            inst.collectionUI = this;
            inst.collectionIndex = index;
            inst.name = inst + ":" + index;
            
            return inst;
        }

        protected override void Repaint(int slot)
        {
            // Check if the slots are still valid; Collection could've been resized.
            if (slot >= 0 && slot < collection.slotCount)
            {
                var element = (CollectionSlotUIBase<T>) repaintableElements[slot];
                if (element != null)
                {                
                    element.Repaint(collection[slot], collection.GetAmount(slot));
                }
                
                logger.LogVerbose($"{collectionName} itemGuid #{slot} repainted");
            }
        }
        
        protected virtual void OnCollectionSlotsChanged(object sender, CollectionSlotsChangedResult e)
        {
            logger.LogVerbose($"Collection {_collectionName} slots changed: {e.affectedSlots.Length} affected slots. - {e.affectedSlots.ToSimpleString()}", this);
            foreach (var slot in e.affectedSlots)
            {
                RepaintOrQueue(slot);
            }
        }

        protected virtual void OnCollectionSizeChanged(object sender, CollectionSizeChangedResult e)
        {
            logger.LogVerbose($"Collection {_collectionName} changed size from {e.sizeBefore} to {e.sizeAfter}. Resizing UI elements array...", this);
            Array.Resize(ref repaintableElements, e.sizeAfter);
            
            if (e.sizeAfter > e.sizeBefore)
            {
                // Generate new UI slots
                for (int i = e.sizeBefore; i < e.sizeAfter; i++)
                {
                    repaintableElements[i] = CreateUIElement(i);
                    RepaintOrQueue(i);
                }
            } 
            else if (e.sizeBefore > e.sizeAfter)
            {
                // Remove some ui slots
                for (int i = e.sizeAfter; i < e.sizeBefore; i++)
                {
                    DestroyUISlot(i);
                }
            }
        }
    }
}