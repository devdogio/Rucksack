using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Items")]
    public class CloneItemInstance : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ItemInstanceWrapper))]
        public FsmObject itemInstance;

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
            var sourceWrapper = (ItemInstanceWrapper) itemInstance.Value;
            
            var cloneWrapper = ScriptableObject.CreateInstance<ItemInstanceWrapper>();
            cloneWrapper.item = (IItemInstance)sourceWrapper.item.Clone();
            
            itemInstanceResult.Value = cloneWrapper;

            Finish();
        }
    }
}