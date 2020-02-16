using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class IndexOf : ItemCollectionAction
    {
        [RequiredField]
        [ObjectType(typeof(UnityItemDefinition))]
        public FsmObject item;
        public FsmInt indexResult;
        
        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            if (item.IsNone || item.UseVariable == false)
            {
                result += nameof(item) + " variable not set" + "\n";
            }

            if (indexResult.IsNone || indexResult.UseVariable == false)
            {
                result += nameof(indexResult) + " variable not set" + "\n";
            }

            return result;
        }

        public override void OnEnter()
        {
            var col = ((ItemCollectionWrapper)collection.Value).collection;
            var itemDef = (UnityItemDefinition) item.Value;
            var inst = ItemFactory.CreateInstance(itemDef, System.Guid.NewGuid());
            indexResult.Value = col.IndexOf(inst);
            
            Finish();
        }
    }
}