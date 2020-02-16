using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Devdog.Rucksack.UI
{
    public abstract class CollectionSlotUIBase : UIMonoBehaviour
    {
        // This class is only used for GetComponent<T> method calls, as interfaces don't do well with destroyed objects.
//        public abstract Collections.ICollection collection { get; }
//        public abstract int collectionIndex { get; set; }

        public abstract Collections.ICollection collection { get;}
        public int collectionIndex { get; set; }
    }

    public abstract class CollectionSlotUIBase<T> : CollectionSlotUIBase
        where T : IEquatable<T>
    {
        public T current
        {
            get { return collectionUI.collection[collectionIndex]; }
        }

        public CollectionUIBase<T> collectionUI { get; set; }
        public override Collections.ICollection collection
        {
            get { return collectionUI.collection; }
        }

        protected readonly ILogger logger = new UnityLogger("[UI][Collection] ");
        protected static List<ICollectionSlotUICallbackReceiver<T>> callbackReceiversCache = new List<ICollectionSlotUICallbackReceiver<T>>();

        protected virtual void Awake()
        {
            var inputHandler = gameObject.GetComponent<ICollectionSlotInputHandler<T>>();
            if (inputHandler == null)
            {
                logger.Warning("No input handler found on " + typeof(T).Name, this);
            }
        }
        
        public virtual void Repaint(T item, int amount)
        {
            GetComponents<ICollectionSlotUICallbackReceiver<T>>(callbackReceiversCache);
            foreach (var receiver in callbackReceiversCache)
            {
                receiver.Repaint(item, amount);
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}