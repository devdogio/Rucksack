using System;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    public sealed class ItemCollectionTypeRestrictionBehaviour : CollectionRestrictionBase<IItemInstance>
    {
        [SerializeField]
        private SerializedType _type;

        [SerializeField]
        private bool _allowSubTypes = true;

        protected override void AssignRestrictionToCollection(Collection<IItemInstance> collection)
        {
            collection.restrictions.Add(new ItemCollectionTypeRestriction(_type.type, _allowSubTypes));
        }
    }
}