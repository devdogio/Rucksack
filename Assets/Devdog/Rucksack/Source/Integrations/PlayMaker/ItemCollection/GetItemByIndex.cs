using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class GetItemByIndex : ItemCollectionAction
    {
        public FsmInt index;

        [RequiredField]
        [ObjectType(typeof(ItemInstanceWrapper))]
        public FsmObject itemResult;
        
        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            if (itemResult.IsNone || itemResult.UseVariable == false)
            {
                result += nameof(itemResult) + " variable not set" + "\n";
            }

            return result;
        }

        public override void OnEnter()
        {
            var col = ((ItemCollectionWrapper)collection.Value).collection;
            var wrapper = ScriptableObject.CreateInstance<ItemInstanceWrapper>();
            wrapper.item = col[index.Value];

            itemResult.Value = wrapper;
            Finish();
        }
    }
}