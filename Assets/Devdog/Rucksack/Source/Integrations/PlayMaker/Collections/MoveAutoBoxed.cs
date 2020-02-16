using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class MoveAutoBoxed : BoxedCollectionAction
    {
        public FsmInt fromIndex;
        
        [RequiredField]
        [ObjectType(typeof(BoxedCollectionWrapper))]
        public FsmObject toCollection;
        
        public override void OnEnter()
        {
            var fromCol = (BoxedCollectionWrapper)collection.Value;
            var toCol = (BoxedCollectionWrapper)toCollection.Value;
            
            var result = fromCol.collection.MoveAuto(fromIndex.Value, toCol.collection, fromCol.collection.GetAmount(fromIndex.Value));
            if (result.error != null)
            {
                LogWarning(result.error.ToString());
            }
            
            Finish();
        }
    }
}