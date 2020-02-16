using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Characters
{
    public interface IInventoryCollectionOwner
    {
        CollectionGroup<IItemInstance> itemCollectionGroup { get; }
    }
}