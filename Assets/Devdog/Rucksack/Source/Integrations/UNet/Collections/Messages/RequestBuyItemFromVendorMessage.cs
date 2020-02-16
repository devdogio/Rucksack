using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestBuyItemFromVendorMessage : MessageBase
    {
        /// <summary>
        /// The GUID of the vendor from whom we're trying to buy
        /// </summary>
        public GuidMessage vendorGuid;
        
        /// <summary>
        /// The GUID of the item we're trying to buy
        /// </summary>
        public GuidMessage itemGuid;
        
        /// <summary>
        /// The amount of items we're trying to buy
        /// </summary>
        public ushort amount;
        
        // TODO: Add transaction GUID?
    }
}