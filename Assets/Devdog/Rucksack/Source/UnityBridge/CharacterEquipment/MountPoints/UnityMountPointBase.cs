using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    public abstract class UnityMountPointBase : MonoBehaviour
    {
        // For custom editors;
    }
    
    public abstract class UnityMountPointBase<T> : UnityMountPointBase, IMountPoint<T>
        where T : class
    {
        public GameObject currentModelInstance { get; protected set; }
        public T mountedItem { get; protected set; }
        public bool hasMountedItem
        {
            get { return mountedItem != null || currentModelInstance != null; }
        }
        
        public int objectLayer = 0;

        protected ILogger logger;

        protected UnityMountPointBase()
        {
            logger = new UnityLogger("[Character][MountPoint] ");
        }
        
        public virtual void Mount(T item)
        {
            mountedItem = item;

            var model = GetModel(item);
            var inst = CreateModelInstance(model);
            inst = InitModelInstance(inst);

            currentModelInstance = inst;
        }
        
        /// <summary>
        /// Get the model used to display on the equippable character.
        /// </summary>
        public abstract GameObject GetModel(T item);

        /// <summary>
        /// Create a new instance of the model to display on the character.
        /// </summary>
        public virtual GameObject CreateModelInstance(GameObject model)
        {
            var obj = Instantiate<GameObject>(model, Vector3.zero, Quaternion.identity, transform);
            obj.transform.localPosition = model.transform.position;
            obj.transform.localRotation = model.transform.rotation;
            obj.layer = objectLayer;

            return obj;
        }

        /// <summary>
        /// Initialize the mounted item; For example, set up rigging, enable physics, enable clothing, etc.
        /// </summary>
        /// <returns>The initialized instance</returns>
        public virtual GameObject InitModelInstance(GameObject instance)
        {
            return instance;
        }

        /// <summary>
        /// Clear the state of this mountpoint.
        /// </summary>
        public virtual void Clear()
        {
            Destroy(currentModelInstance);
            mountedItem = null;
        }
        
        public override string ToString()
        {
            return name;
        }
    }
}