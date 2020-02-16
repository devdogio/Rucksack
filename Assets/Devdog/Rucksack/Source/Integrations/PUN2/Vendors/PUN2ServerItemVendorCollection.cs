using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public class PUN2ServerItemVendorCollection : PUN2CollectionBase<IVendorProduct<IItemInstance>>
    {
        public PUN2ServerItemVendorCollection(PhotonView owner, int slotCount, ILogger logger = null)
            : base(owner, slotCount, logger)
        {
            //            OnSlotsChanged += NotifyOnSlotsChanged;
            //            OnSizeChanged += NotifyOnSizeChanged;
        }

        //        protected void NotifyOnSlotsChanged(object sender, CollectionSlotsChangedResult data)
        //        {
        //            if (owner.isServer)
        //            {
        //                var clients = PUN2PermissionsRegistry.collections.GetAllIdentitiesWithPermission(this);
        //                foreach (var client in clients)
        //                {
        //                    var actionBridge = client.GetComponent<PUN2ActionsBridge>();
        //                    if (actionBridge != null)
        //                    {
        //                        logger.LogVerbose($"[Server] Notify client with NetID: {actionBridge.identity.netId} of changed itemGuid: {data.affectedSlots.ToSimpleString()} on collection: {collectionName}", this);
        //                
        //                        // TODO: Combine all affected slots in single call to client!
        //                        foreach (var itemGuid in data.affectedSlots)
        //                        {
        ////                            if (this[itemGuid] != null)
        ////                            {
        ////                                // TODO: Remove this line - Client should request it by itself, or the server should check if the client already knows the item instance...
        ////                                actionBridge.Server_TellClientToRegisterItemInstance(this[itemGuid].item);
        ////                            }
        //                        
        //                            // TODO: Update single itemGuid when changed in vendor collection...
        //                            
        ////                            actionBridge.Server_SetVendorProducts(new ItemVendorProductMessage()
        ////                            {
        ////                                products = this.Select(o => new ItemVendorProduct(o, o.collectionEntry.amount)).ToArray(),
        ////                                vendorCollectionGuidBytes = collectionGuid.ToByteArray()
        ////                            });
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        protected void NotifyOnSizeChanged(object sender, CollectionSizeChanged data)
        //        {
        //            // Ignore for now; Vendor collections shouldn't be resized over the network anyway...?
        //        }
    }
}