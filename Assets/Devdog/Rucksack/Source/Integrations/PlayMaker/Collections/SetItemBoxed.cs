using System;
using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class SetItemBoxed : BoxedCollectionAction
    {
        [RequiredField]
        public FsmObject item;
        public FsmInt index;
        public FsmInt amount = 1;

        public FsmBool force = false;
        
        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            
            if (item.IsNone || item.UseVariable == false)
            {
                result += nameof(item) + " variable not set" + "\n";
            }

            if (amount.Value < 1)
            {
                result += "Amount has to be larger than 0" + "\n";
            }

            return result;
        }

        public override void OnEnter()
        {
            var col = ((BoxedCollectionWrapper)collection.Value).collection;
            var itemDef = (UnityItemDefinition) item.Value;

            var inst = ItemFactory.CreateInstance(itemDef, Guid.NewGuid());
            if (force.Value)
            {
                col.ForceSetBoxed(index.Value, inst, amount.Value);
            }
            else
            {
                var error = col.SetBoxed(index.Value, inst, amount.Value).error;
                LogWarning(error.ToString());
            }
            
            Finish();
        }
    }
}