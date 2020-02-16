using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public class UNetVendorInputValidator : NetworkInputValidatorBase
    {
        protected readonly ILogger logger;

        public UNetVendorInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[UNet][Validation] ");
        }

        public Result<bool> ValidateBuyItemFromVendor(NetworkIdentity identity, RequestBuyItemFromVendorMessage data, out INetworkVendor<IItemInstance> outVendor, out INetworkItemInstance outItem)
        {
            outVendor = null;
            outItem = null;
            if (CheckGuidBytes(data.vendorGuid.bytes) == false ||
                CheckGuidBytes(data.itemGuid.bytes) == false)
            {
                return Failed();
            }

            outItem = ServerItemRegistry.Get(data.itemGuid) as INetworkItemInstance;
            outVendor = ServerVendorRegistry.itemVendors.Get(data.vendorGuid);
            if (outItem == null || outVendor == null)
            {
                return Failed();
            }
            
            var tempItem = outItem;
            var vendorItemAmount = outVendor.itemCollection.GetAmount(o => o?.ID == tempItem.ID);
            if (vendorItemAmount < data.amount)
            {
                return Failed();
            }
            
            // Check if player has read permission to vendor collection
            if (CheckClientVendorPermission(identity, outVendor, outItem) == false)
            {
                return Failed();
            }

            return true;
        }

        public Result<bool> ValidateSellItemToVendor(NetworkIdentity identity, RequestSellItemToVendorMessage data, out INetworkVendor<IItemInstance> outVendor, out INetworkItemInstance outItem)
        {
            outVendor = null;
            outItem = null;
            if (CheckGuidBytes(data.vendorGuid.bytes) == false ||
                CheckGuidBytes(data.itemGuid.bytes) == false)
            {
                return Failed();
            }

            outItem = ServerItemRegistry.Get(data.itemGuid) as INetworkItemInstance;
            outVendor = ServerVendorRegistry.itemVendors.Get(data.vendorGuid);
            if (outItem == null || outVendor == null)
            {
                return Failed();
            }
            
            // Item has to be in a collection to be sellable.
            if (outItem.collectionEntry?.collection == null)
            {
                return Failed();
            }

            if (outItem.collectionEntry.amount < data.amount)
            {
                return Failed();
            }

            return true;
        }
        
        protected bool CheckClientVendorPermission(NetworkIdentity identity, INetworkVendor<IItemInstance> vendor, IItemInstance item)
        {
            var unetCol = vendor.itemCollection as IUNetCollection;
            if (unetCol == null)
            {
                return false;
            }

            // TODO: Instead of checking read permission on the vendor's collection use trigger permissions instead (UNetPermissionRegistry.objects)
            
            var itemPermission = UNetPermissionsRegistry.collections.GetPermission(unetCol, identity);
            if (itemPermission.CanRead() == false)
            {
                return false;
            }

            return true;
        }
    }
}