using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    public class ItemCollectionWrapper : ScriptableObject
    {
        public ICollection<IItemInstance> collection;
        
        public static implicit operator BoxedCollectionWrapper(ItemCollectionWrapper self)
        {
            var inst = ScriptableObject.CreateInstance<BoxedCollectionWrapper>();
            inst.collection = self.collection;
            return inst;
        }
    }
}