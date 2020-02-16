using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/ItemCollection")]
    public class ClearCollection : ItemCollectionAction
    {
        public override void OnEnter()
        {
            var col = (ItemCollectionWrapper)collection.Value;
            col.collection.Clear();
            
            Finish();
        }
    }
}