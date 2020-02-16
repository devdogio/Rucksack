using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Items")]
    public class CreateItemInstance : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(UnityItemDefinition))]
        public FsmObject itemDefinition;

        [RequiredField]
        [ObjectType(typeof(ItemInstanceWrapper))]
        public FsmObject itemInstanceResult;

        public override string ErrorCheck()
        {
            if (itemInstanceResult.IsNone || itemInstanceResult.UseVariable == false)
            {
                return nameof(itemInstanceResult) + " variable is not set.";
            }

            return "";
        }

        public override void OnEnter()
        {
            var def = (UnityItemDefinition) itemDefinition.Value;
            var wrapper = ScriptableObject.CreateInstance<ItemInstanceWrapper>();
            wrapper.item = ItemFactory.CreateInstance(def, System.Guid.NewGuid());
            
            itemInstanceResult.Value = wrapper;
            
            Finish();
        }
    }
}