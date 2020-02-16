using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.Vendors
{
    public static partial class VendorRegistry
    {
        private static CollectionRegistry.Helper<System.Guid, IVendor> _itemVendors = new CollectionRegistry.Helper<System.Guid, IVendor>();
        public static CollectionRegistry.Helper<System.Guid, IVendor> itemVendors
        {
            get { return _itemVendors; }
        }
    }
}