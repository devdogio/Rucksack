using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class GetAmountByItemInstance : ItemCollectionAction
    {
        [RequiredField]
        [ObjectType(typeof(ItemInstanceWrapper))]
        public FsmObject item;
        
        public FsmInt amountResult;
        
        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            if (amountResult.IsNone || amountResult.UseVariable == false)
            {
                result += nameof(amountResult) + " variable not set" + "\n";
            }

            return result;
        }

        public override void OnEnter()
        {
            var col = ((ItemCollectionWrapper)collection.Value).collection;
            amountResult.Value = col.GetAmount(((ItemInstanceWrapper)item.Value).item);
            Finish();
        }
    }
}