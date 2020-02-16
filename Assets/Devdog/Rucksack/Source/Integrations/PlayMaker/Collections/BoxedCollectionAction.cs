using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Collections/Boxed")]
    public class BoxedCollectionAction : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(BoxedCollectionWrapper))]
        public FsmObject collection;
        
        public override string ErrorCheck()
        {
            if (collection.IsNone || collection.UseVariable == false)
            {
                return nameof(collection) + " variable not set" + "\n";
            }

            return "";
        }
    }
}