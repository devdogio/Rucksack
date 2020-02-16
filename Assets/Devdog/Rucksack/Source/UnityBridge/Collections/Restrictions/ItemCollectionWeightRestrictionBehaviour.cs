using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    public sealed class ItemCollectionWeightRestrictionBehaviour : CollectionRestrictionBase<IItemInstance>
    {
        [SerializeField]
        private float _maxWeight = 100f;
        
        protected override void AssignRestrictionToCollection(Collection<IItemInstance> collection)
        {
            collection.restrictions.Add(new ItemCollectionWeightRestriction(collection, _maxWeight));
        }
    }
}