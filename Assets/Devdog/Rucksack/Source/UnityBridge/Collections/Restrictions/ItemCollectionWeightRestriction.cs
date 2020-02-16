using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    public sealed class ItemCollectionWeightRestriction : ICollectionRestriction<IItemInstance>
    {
        public readonly float maxWeight = 100f;
        public readonly ICollection<IItemInstance> collection;

        public ItemCollectionWeightRestriction(ICollection<IItemInstance> collection, float maxWeight)
        {
            this.collection = collection;
            this.maxWeight = maxWeight;
        }
        
        private float CalcCollectionWeight(ICollection<IItemInstance> collection)
        {
            float totalWeight = 0f;
            for (int i = 0; i < collection.slotCount; i++)
            {
                var item = collection[i];
                totalWeight += (item?.itemDefinition as IWeight)?.weight ?? 0f * collection.GetAmount(i);
            }
            
            return totalWeight;
        }

        public Result<bool> CanAdd(IItemInstance item, CollectionContext context)
        {
            var amount = (item as ICollectionSlotEntry)?.collectionEntry?.amount;
            var weight = (item.itemDefinition as IWeight)?.weight;

            if (weight * amount + CalcCollectionWeight(collection) > maxWeight)
            {
                return new Result<bool>(false, new Error(Errors.CollectionRestrictionPreventedAction.errorNumber, "Exceeding max collection weight."));
            }
            
            return true;
        }

        public Result<bool> CanRemove(IItemInstance item, CollectionContext context)
        {
            return true;
        }
    }
}