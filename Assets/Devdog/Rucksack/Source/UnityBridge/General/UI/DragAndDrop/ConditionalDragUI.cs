using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.Rucksack.UI
{
    public class ConditionalDragUI : MonoBehaviour
    {
        private enum ShowOption
        {
            Always,
            OnlyWhenAccepted
        }

        [SerializeField]
        private ShowOption _showOnStartDrag = ShowOption.Always;
        
        [SerializeField]
        private bool _hideOnEndDrag = true;
        
        [SerializeField]
        private Transform[] _transforms = new Transform[0];
        
        [SerializeField]
        private Behaviour[] _components = new Behaviour[0];
        
        private void OnEnable()
        {
            DragAndDropUtility.OnBeginDrag += OnBeginDrag;
            DragAndDropUtility.OnEndDrag += OnEndDrag;
            
            SetTransformsActive(_transforms, false);
            SetComponentsActive(_components, false);
        }

        private void OnDisable()
        {
            DragAndDropUtility.OnBeginDrag -= OnBeginDrag;
            DragAndDropUtility.OnEndDrag -= OnEndDrag;
        }

        private bool IsAcceptedByDropArea(DragAndDropUtility.Model model, PointerEventData pointerEventData)
        {
            foreach (var area in GetComponents<IDropArea>())
            {
                if (area.CanDropDraggingItem(model, pointerEventData).result)
                {
                    return true;
                }
            }

            return false;
        }
        
        private void OnBeginDrag(DragAndDropUtility.Model model, PointerEventData pointerEventData)
        {
            switch (_showOnStartDrag)
            {
                case ShowOption.Always:
                {
                    SetTransformsActive(_transforms, true);
                    SetComponentsActive(_components, true);
                    break;
                }
                case ShowOption.OnlyWhenAccepted:
                {
                    if (IsAcceptedByDropArea(model, pointerEventData))
                    {
                        SetTransformsActive(_transforms, true);
                        SetComponentsActive(_components, true);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnEndDrag(DragAndDropUtility.Model arg1, PointerEventData arg2)
        {
            if (_hideOnEndDrag)
            {
                SetTransformsActive(_transforms, false);
                SetComponentsActive(_components, false);
            }
        }

        private void SetTransformsActive(IEnumerable<Transform> transforms, bool b)
        {
            foreach (var t in transforms)
            {
                t.gameObject.SetActive(b);
            }
        }
        
        private void SetComponentsActive(IEnumerable<Behaviour> components, bool b)
        {
            foreach (var c in components)
            {
                c.enabled = b;
            }
        }
    }
}