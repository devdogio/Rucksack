using Devdog.General2;
using Devdog.Rucksack.Items;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Items")]
    public class UseItem : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ItemInstanceWrapper))]
        public FsmObject itemInstance;

        [RequiredField]
        [ObjectType(typeof(Character))]
        public FsmObject characterUser;

        public FsmInt useAmount = 1;
        
        public override void OnEnter()
        {
            var wrapper = (ItemInstanceWrapper) itemInstance.Value;
            wrapper.item.Use((Character)characterUser.Value, new ItemContext()
            {
                useAmount = useAmount.Value
            });
            
            Finish();
        }
    }
}