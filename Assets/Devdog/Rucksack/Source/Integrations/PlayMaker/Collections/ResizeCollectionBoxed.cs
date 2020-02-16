using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class ResizeCollectionBoxed : BoxedCollectionAction
    {
        public FsmInt newSize = 10;
        
        public override void OnEnter()
        {
            var col = ((BoxedCollectionWrapper)collection.Value).collection;
            col.Resize(newSize.Value);
            
            Finish();
        }
    }
}