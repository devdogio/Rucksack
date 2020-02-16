using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using Photon.Pun;
using System;

namespace Devdog.Rucksack.Currencies
{
    public sealed class PUN2VendorInputValidator : NetworkInputValidatorBase
    {
        private readonly ILogger logger;

        public PUN2VendorInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[PUN2][Validation] ");
        }

        public Result<bool> ValidateBuyItemFromVendor(PhotonView identity, byte[] vendorGuidBytes, Guid vendorGuid, byte[] itemGuidBytes, Guid itemGuid, ushort amount, out INetworkVendor<IItemInstance> outVendor, out INetworkItemInstance outItem)
        {
            outVendor = null;
            outItem = null;
            if (CheckGuidBytes(vendorGuidBytes) == false ||
                CheckGuidBytes(itemGuidBytes) == false)
            {
                return Failed();
            }

            outItem = ServerItemRegistry.Get(itemGuid) as INetworkItemInstance;
            outVendor = ServerVendorRegistry.itemVendors.Get(vendorGuid);
            if (outItem == null || outVendor == null)
            {
                return Failed();
            }

            var tempItem = outItem;
            var vendorItemAmount = outVendor.itemCollection.GetAmount(o => o?.ID == tempItem.ID);
            if (vendorItemAmount < amount)
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

        public Result<bool> ValidateSellItemToVendor(PhotonView identity, byte[] vendorGuidBytes, Guid vendorGuid, byte[] itemGuidBytes, Guid itemGuid, ushort amount, out INetworkVendor<IItemInstance> outVendor, out INetworkItemInstance outItem)
        {
            outVendor = null;
            outItem = null;
            if (CheckGuidBytes(vendorGuidBytes) == false ||
                CheckGuidBytes(itemGuidBytes) == false)
            {
                return Failed();
            }

            outItem = ServerItemRegistry.Get(itemGuid) as INetworkItemInstance;
            outVendor = ServerVendorRegistry.itemVendors.Get(vendorGuid);
            if (outItem == null || outVendor == null)
            {
                return Failed();
            }

            // Item has to be in a collection to be sellable.
            if (outItem.collectionEntry?.collection == null)
            {
                return Failed();
            }

            if (outItem.collectionEntry.amount < amount)
            {
                return Failed();
            }

            return true;
        }

        private bool CheckClientVendorPermission(PhotonView identity, INetworkVendor<IItemInstance> vendor, IItemInstance item)
        {
            var unetCol = vendor.itemCollection as IPUN2Collection;
            if (unetCol == null)
            {
                return false;
            }

            // TODO: Instead of checking read permission on the vendor's collection use trigger permissions instead (UNetPermissionRegistry.objects)

            var itemPermission = PUN2PermissionsRegistry.collections.GetPermission(unetCol, identity);
            if (itemPermission.CanRead() == false)
            {
                return false;
            }

            return true;
        }
    }
}
