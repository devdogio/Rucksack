using Devdog.Rucksack.Items;
using Devdog.Rucksack.Collections;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    public class BoxedCollectionWrapper : ScriptableObject
    {
        public ICollection collection;
        
//        public static implicit operator ItemCollectionWrapper(BoxedCollectionWrapper self)
//        {
//            var inst = ScriptableObject.CreateInstance<ItemCollectionWrapper>();
//            inst.collection = self.collection as ICollection<IItemInstance>;
//            return inst;
//        }
    }
}