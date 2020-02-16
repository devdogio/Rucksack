using Devdog.Rucksack.Currencies;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Currency")]
    public class SetCurrency : FsmStateAction
    {
        public FsmFloat amount;

        [RequiredField]
        [ObjectType(typeof(UnityCurrency))]
        public FsmObject currencyType;
        
        [RequiredField]
        [ObjectType(typeof(CurrencyCollectionWrapper))]
        public FsmObject collection;
        
        
        public override void OnEnter()
        {
            var wrapper = (CurrencyCollectionWrapper) collection.Value;
            var result = wrapper.collection.Set((ICurrency)currencyType.Value, amount.Value);
            if (result.error != null)
            {
                LogWarning(result.error.ToString());
            }

            Finish();
        }
    }
}