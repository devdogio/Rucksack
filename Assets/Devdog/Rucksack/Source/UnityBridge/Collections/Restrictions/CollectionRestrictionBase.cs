using System;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    public abstract class CollectionRestrictionBase<T> : MonoBehaviour 
        where T : class, IEquatable<T>, IItemInstance
    {
        [SerializeField]
        protected string _collectionName;

        protected virtual void Start()
        {
//            foreach (var collectionCreator in GetComponents<UNetItemCollectionCreator>())
//            {
//                if (collectionCreator.collectionName == _collectionName)
//                {
//                    AssignRestrictionToCollection(collectionCreator.collection as Collection<T>);
//                    return;
//                }
//            }

            foreach (var collectionCreator in GetComponents<ItemCollectionCreator>())
            {
                if (collectionCreator.collectionName == _collectionName)
                {
                    AssignRestrictionToCollection(collectionCreator.collection as Collection<T>);
                    return;
                }
            }
        }

        protected abstract void AssignRestrictionToCollection(Collection<T> collection);
    }
}