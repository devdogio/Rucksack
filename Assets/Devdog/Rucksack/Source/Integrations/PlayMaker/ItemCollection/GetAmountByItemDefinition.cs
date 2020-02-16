using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class GetAmountByItemDefinition : ItemCollectionAction
    {
        [RequiredField]
        [ObjectType(typeof(UnityItemDefinition))]
        public FsmObject itemDefinition;
        
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
            var inst = ItemFactory.CreateInstance(((UnityItemDefinition)itemDefinition.Value), System.Guid.NewGuid());

            amountResult.Value = col.GetAmount(inst);
            Finish();
        }
    }
}