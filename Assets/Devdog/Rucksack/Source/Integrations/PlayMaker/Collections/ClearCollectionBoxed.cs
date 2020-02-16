using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class ClearCollectionBoxed : BoxedCollectionAction
    {
        public override void OnEnter()
        {
            var col = (BoxedCollectionWrapper)collection.Value;
            col.collection.Clear();
            
            Finish();
        }
    }
}