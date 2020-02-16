using System;
using Devdog.General2;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Tests
{
    [System.Serializable]
    public class ItemWithCollectionMock : ItemInstance
    {
        public ICollection<IItemInstance> collection { get; }
        
        public ItemWithCollectionMock(Guid ID, IItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        {
            this.collection = new Collection<IItemInstance>(5);
        }
        
        
    }
}