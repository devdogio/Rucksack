using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    public class ItemCollectionAction : FsmStateAction
    {
        [ObjectType(typeof(ItemCollectionWrapper))]
        public FsmObject collection;
        
        public override string ErrorCheck()
        {
            if (collection.IsNone)
            {
                return nameof(collection) + " variable not set" + "\n";
            }

            return "";
        }
    }
}