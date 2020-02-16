using Devdog.General2;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    public abstract class CollectionSlotUIWorldModelBase<T> : MonoBehaviour
    {
        [Required]
        [SerializeField]
        protected Transform worldModelParent;

        [SerializeField]
        protected float modelScale = 50f;

        // UI Layer
        protected const int ObjectLayerNumber = 5;
        
        protected GameObject currentInstantiatedObject;
        protected GameObject currentPrefab;

        protected virtual void Create3DModel(Transform parent, GameObject worldModel)
        {
            if (currentPrefab == worldModel)
            {
                return;
            }
            
            // Clean up previous object...
            Clean();
            
            currentInstantiatedObject = Instantiate<GameObject>(worldModel, Vector3.zero, Quaternion.identity, parent);
            currentInstantiatedObject.transform.localPosition = -Vector3.forward * 50f;
            currentInstantiatedObject.transform.localRotation = Quaternion.identity;
            SetLayerRecursively(currentInstantiatedObject.transform, ObjectLayerNumber);

            HandleScale(currentInstantiatedObject);
            AddExtras(currentInstantiatedObject);           

            currentPrefab = worldModel;
        }

        protected void SetLayerRecursively(Transform start, int layer)
        {
            start.gameObject.layer = layer;
            foreach (Transform child in start)
            {
                child.gameObject.layer = layer;
                SetLayerRecursively(child, layer);
            }
        }

        protected virtual void HandleScale(GameObject instantiatedObject)
        {
            // TODO: Handle scaling better; See bounding box of object and scale to fit in UI...?
            instantiatedObject.transform.localScale *= modelScale;
        }
        
        public virtual void AddExtras(GameObject o)
        {
            var spin = currentInstantiatedObject.AddComponent<Spin>();
            spin.angles = new Vector3(10f, 10f, 0f);
        }
        
        public virtual void Clean()
        {
            Destroy(currentInstantiatedObject);
            currentPrefab = null;
        }
    }
}