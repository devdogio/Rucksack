using Devdog.Rucksack.Currencies;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Currency")]
    public class GetCurrencyCollection : FsmStateAction
    {
        public FsmString collectionName;

        [RequiredField]
        [ObjectType(typeof(CurrencyCollectionWrapper))]
        public FsmObject collectionResult;


        public override string ErrorCheck()
        {
            if (collectionResult.IsNone || collectionResult.UseVariable == false)
            {
                return nameof(collectionResult) + " variable is not set";
            }

            return "";
        }

        public override void OnEnter()
        {
            var col = CurrencyCollectionRegistry.byName.Get(collectionName.Value) as ICurrencyCollection<ICurrency, double>;
            if (col == null)
            {
                LogWarning($"Couldn't find currency collection with name {collectionName.Value}");
                Finish();
                return;
            }
            
            var wrapper = ScriptableObject.CreateInstance<CurrencyCollectionWrapper>();
            wrapper.collection = col;
            
            collectionResult.Value = wrapper;
            Finish();
        }
    }
}