using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class GetCanAddAmount : ItemCollectionAction
    {
        [RequiredField]
        [ObjectType(typeof(UnityItemDefinition))]
        public FsmObject item;
        public FsmInt canAddResult;

        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            if (canAddResult.IsNone || canAddResult.UseVariable == false)
            {
                result += nameof(canAddResult) + " variable is empty\n";
            }
            
            if (item.IsNone || item.UseVariable == false)
            {
                result += nameof(item) + " variable not set" + "\n";
            }

            return result;
        }

        public override void OnEnter()
        {
            var fromCol = (ItemCollectionWrapper)collection.Value;
            var itemDef = (UnityItemDefinition) item.Value;
            var inst = ItemFactory.CreateInstance(itemDef, System.Guid.NewGuid());

            var result = fromCol.collection.GetCanAddAmount(inst);
            canAddResult.Value = result.result;

            if (result.error != null)
            {
                LogWarning(result.error.ToString());
            }

            Finish();
        }
    }
}