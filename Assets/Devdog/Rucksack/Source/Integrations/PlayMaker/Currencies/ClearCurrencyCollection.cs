using Devdog.Rucksack.Currencies;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Currency")]
    public class ClearCurrencyCollection : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(CurrencyCollectionWrapper))]
        public FsmObject collection;
        
        
        public override void OnEnter()
        {
            var wrapper = (CurrencyCollectionWrapper) collection.Value;
            wrapper.collection.Clear();
            Finish();
        }
    }
}