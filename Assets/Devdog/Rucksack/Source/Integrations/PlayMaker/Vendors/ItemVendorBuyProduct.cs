using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Vendor")]
    public class ItemVendorBuyProduct : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(ItemVendorWrapper))]
        public FsmObject vendor;

        [RequiredField]
        [ObjectType(typeof(ItemVendorCustomerWrapper))]
        public FsmObject customer;

        [RequiredField]
        [ObjectType(typeof(ItemInstanceWrapper))]
        public FsmObject itemInstance;
        
        public FsmInt amount = 1;

        public override void OnEnter()
        {
            var vendorWrapper = (ItemVendorWrapper) vendor.Value;
            var customerWrapper = (ItemVendorCustomerWrapper) customer.Value;

            var bought = vendorWrapper.vendor.BuyFromVendor(customerWrapper.customer, ((ItemInstanceWrapper)itemInstance.Value).item, amount.Value);
            if (bought.error != null)
            {
                LogWarning(bought.error.ToString());
            }
            
            Finish();
        }
    }
}