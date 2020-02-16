using System.Linq;
using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class IndexOfAll : ItemCollectionAction
    {
        [RequiredField]
        [ObjectType(typeof(UnityItemDefinition))]
        public FsmObject item;
        
        [RequiredField]
        [ObjectType(typeof(int))]
        public FsmArray indexResults;
        
        public override string ErrorCheck()
        {
            var result = base.ErrorCheck();
            
            if (item.IsNone)
            {
                result += nameof(item) + " variable not set" + "\n";
            }

            if (indexResults.IsNone)
            {
                result += nameof(indexResults) + " variable not set" + "\n";
            }

            return result;
        }

        public override void OnEnter()
        {
            var col = ((ItemCollectionWrapper)collection.Value).collection;
            var itemDef = (UnityItemDefinition) item.Value;
            var inst = ItemFactory.CreateInstance(itemDef, System.Guid.NewGuid());
            indexResults.intValues = col.IndexOfAll(inst).ToArray();
            
            Finish();
        }
    }
}