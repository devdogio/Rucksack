using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class ResizeCollection : ItemCollectionAction
    {
        public FsmInt newSize = 10;
        
        public override void OnEnter()
        {
            var col = ((ItemCollectionWrapper)collection.Value).collection;
            col.Resize(newSize.Value);
            
            Finish();
        }
    }
}