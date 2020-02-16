using UnityEngine.Networking;

namespace Devdog.Rucksack.Vendors
{
    public class SetItemVendorCollectionContentsMessage : MessageBase
    {

        public GuidMessage collectionGuid;
        public ItemVendorProductMessage[] products;

    }
}