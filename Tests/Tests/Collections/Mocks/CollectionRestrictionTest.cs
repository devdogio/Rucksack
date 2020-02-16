using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    public class FakeCollectionRestriction : ICollectionRestriction<IItemInstance>
    {
        public ICollection<IItemInstance> collection { get; private set; }
        
        public FakeCollectionRestriction()
        {
            
        }

        public FakeCollectionRestriction(ICollection<IItemInstance> collection)
        {
            this.collection = collection;
        }
        
        public Result<bool> CanAdd(IItemInstance item, CollectionContext context)
        {
            return new Result<bool>(false, Errors.CollectionRestrictionPreventedAction);
        }

        public Result<bool> CanRemove(IItemInstance item, CollectionContext context)
        {
            return new Result<bool>(false, Errors.CollectionRestrictionPreventedAction);
        }
    }
}
