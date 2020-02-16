using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestSellItemToVendorMessage : MessageBase
    {
        /// <summary>
        /// Sell from what collection?
        /// </summary>
        public GuidMessage sellFromCollectionGuid;
        
        /// <summary>
        /// The Unique GUID of the item we want to sell
        /// </summary>
        public GuidMessage itemGuid;
        
        /// <summary>
        /// The amount of items to sell
        /// </summary>
        public ushort amount;
        
        /// <summary>
        /// The vendor GUID to whom we're trying to sell
        /// </summary>
        public GuidMessage vendorGuid;
        
        // TODO: Add transaction ID?
    }
}