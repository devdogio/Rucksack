using Devdog.Rucksack.Collections;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class GetCollectionBoxed : FsmStateAction
    {
        public FsmString collectionName;

        [RequiredField]
        [ObjectType(typeof(BoxedCollectionWrapper))]
        public FsmObject collectionResult;
        
        public override void Reset()
        {
            
        }

        public override void OnEnter()
        {
            var col = CollectionRegistry.byName.Get(collectionName.Value);
            if (col == null)
            {
                LogWarning($"Couldn't find collection with name {collectionName.Value}");
                Finish();
                return;
            }
            
            var wrapper = ScriptableObject.CreateInstance<BoxedCollectionWrapper>();
            wrapper.collection = col;
            
            collectionResult.Value = wrapper;
            Finish();
        }
    }
}