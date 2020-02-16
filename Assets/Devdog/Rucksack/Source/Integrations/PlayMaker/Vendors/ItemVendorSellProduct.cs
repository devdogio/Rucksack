using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using HutongGames.PlayMaker;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Vendor")]
    public class ItemVendorSellProduct : FsmStateAction
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
            var item = ((ItemInstanceWrapper) itemInstance.Value).item;
            var product = new VendorProduct<IItemInstance>(item, item.itemDefinition.buyPrice,  item.itemDefinition.sellPrice);
            
            var sold = vendorWrapper.vendor.SellToVendor(customerWrapper.customer, product, amount.Value);
            if (sold.error != null)
            {
                LogWarning(sold.error.ToString());
            }
            
            Finish();
        }
    }
}