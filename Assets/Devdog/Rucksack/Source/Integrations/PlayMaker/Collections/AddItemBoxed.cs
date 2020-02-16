using System;
using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class AddItemBoxed : BoxedCollectionAction
    {
        [RequiredField]
        [ObjectType(typeof(UnityItemDefinition))]
        public FsmObject item;
        public FsmInt amount = 1;
        
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
            var added = col.AddBoxed(inst, amount.Value);
            if (added.error != null)
            {
                LogWarning(added.error.ToString());
            }
            
            Finish();
        }
    }
}