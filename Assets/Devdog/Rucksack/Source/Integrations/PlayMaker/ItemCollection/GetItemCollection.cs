using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class GetItemCollection : FsmStateAction
    {
        [RequiredField]
        public FsmString collectionName;

        [RequiredField]
        [ObjectType(typeof(ItemCollectionWrapper))]
        public FsmObject collectionResult;

        public override string ErrorCheck()
        {
            if (collectionResult.IsNone || collectionResult.UseVariable == false)
            {
                return nameof(collectionResult) + " variable is not set";
            }

            return "";
        }

        public override void OnEnter()
        {
            var col = CollectionRegistry.byName.Get(collectionName.Value) as ICollection<IItemInstance>;
            if (col == null)
            {
                LogWarning($"Couldn't find collection with name {collectionName.Value}");
                Finish();
                return;
            }
            
            var wrapper = ScriptableObject.CreateInstance<ItemCollectionWrapper>();
            wrapper.collection = col;
            
            collectionResult.Value = wrapper;
            Finish();
        }
    }
}