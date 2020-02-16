using System;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Collections
{
    public sealed class ItemCollectionTypeRestriction : ICollectionRestriction<IItemInstance>
    {
        public readonly Type type;
        public readonly bool allowSubClasses;

        public ItemCollectionTypeRestriction(Type requiredType, bool allowSubClasses = true)
        {
            this.type = requiredType;
            this.allowSubClasses = allowSubClasses;
        }
        
        public Result<bool> CanAdd(IItemInstance item, CollectionContext context)
        {
            if (item?.GetType() == type) 
            {
                return true;
            } 
            else if (allowSubClasses && item?.GetType().IsSubclassOf(type) == true)
            {
                return true;
            }
            
            return new Result<bool>(false, Errors.CollectionRestrictionPreventedAction);
        }

        public Result<bool> CanRemove(IItemInstance item, CollectionContext context)
        {
            return true;
        }
    }
}