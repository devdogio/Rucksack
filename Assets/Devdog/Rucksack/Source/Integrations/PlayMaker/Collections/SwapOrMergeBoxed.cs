using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class SwapOrMergeBoxed : BoxedCollectionAction
    {
        public FsmInt fromIndex;
        
        [RequiredField]
        [ObjectType(typeof(BoxedCollectionWrapper))]
        public FsmObject toCollection;
        
        public FsmInt toIndex;
        
        public override void OnEnter()
        {
            var fromCol = (BoxedCollectionWrapper)collection.Value;
            var toCol = (BoxedCollectionWrapper)toCollection.Value;
            
            var result = fromCol.collection.SwapOrMerge(fromIndex.Value, toCol.collection, toIndex.Value, fromCol.collection.GetAmount(fromIndex.Value));
            if (result.error != null)
            {
                LogWarning(result.error.ToString());
            }
            
            Finish();
        }
    }
}