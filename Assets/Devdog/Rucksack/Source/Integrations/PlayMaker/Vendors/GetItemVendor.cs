using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Vendor")]
    public class GetItemVendor : FsmStateAction
    {
        [RequiredField]
        public FsmString vendorGuid;

        [RequiredField]
        [ObjectType(typeof(BoxedCollectionWrapper))]
        public FsmObject vendorResult;
        
        public override void OnEnter()
        {
            var vendor = VendorRegistry.itemVendors.Get(System.Guid.Parse(vendorGuid.Value)) as IVendor<IItemInstance>;
            var wrapper = ScriptableObject.CreateInstance<ItemVendorWrapper>();
            wrapper.vendor = vendor;
            
            vendorResult.Value = wrapper;
            Finish();
        }
    }
}