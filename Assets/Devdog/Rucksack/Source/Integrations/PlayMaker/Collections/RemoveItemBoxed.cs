using System;
using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class RemoveItemBoxed : BoxedCollectionAction
    {
        [RequiredField]
        public FsmObject item;
        public FsmInt amount = 1;
        
        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            
            if (item.IsNone || item.UseVariable)
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
            var removed = col.RemoveBoxed(inst, amount.Value);
            if (removed.error != null)
            {
                LogWarning(removed.error.ToString());
            }
            
            Finish();
        }
    }
}