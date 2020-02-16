using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Devdog.Rucksack.Currencies
{
    public sealed class PUN2VendorReplicator
    {
        private readonly ILogger logger;
        private readonly PUN2ActionsBridge bridge;

        private static PUN2VendorInputValidator inputValidator;

        static PUN2VendorReplicator()
        {
            inputValidator = new PUN2VendorInputValidator(new UnityLogger("[PUN2][Validation] "));
        }

        public PUN2VendorReplicator(PUN2ActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[PUN2] ");
        }

        public void Cmd_RequestBuyItemFromVendor(byte[] vendorGuidBytes, Guid vendorGuid, byte[] itemGuidBytes, Guid itemGuid, ushort amount)
        {
            logger.LogVerbose($"[Server] Client requested to buy item {itemGuid} {amount}x from vendor {vendorGuid} with ViewID {bridge.photonView.ViewID}", bridge);

            INetworkItemInstance item;
            INetworkVendor<IItemInstance> vendor;
            var error = inputValidator.ValidateBuyItemFromVendor(bridge.photonView, vendorGuidBytes, vendorGuid, itemGuidBytes, itemGuid, amount, out vendor, out item).error;
            if (error == null)
            {
                error = vendor.Server_BuyFromVendor(new Customer<IItemInstance>(Guid.NewGuid(), bridge.player, bridge.inventoryPlayer.itemCollectionGroup, bridge.inventoryPlayer.currencyCollectionGroup), item, amount).error;
            }

            HandleError(error);
        }

        public void Cmd_RequestSellItemToVendor(byte[] vendorGuidBytes, Guid vendorGuid, byte[] itemGuidBytes, Guid itemGuid, ushort amount)
        {
            logger.LogVerbose($"[Server] Client requested to sell item {itemGuid} {amount}x to vendor {vendorGuid} with ViewID {bridge.photonView.ViewID}", bridge);

            INetworkItemInstance item;
            INetworkVendor<IItemInstance> vendor;
            var error = inputValidator.ValidateSellItemToVendor(bridge.photonView, vendorGuidBytes, vendorGuid, itemGuidBytes, itemGuid, amount, out vendor, out item).error;
            if (error == null)
            {
                var product = new VendorProduct<IItemInstance>(item, item.itemDefinition.buyPrice, item.itemDefinition.sellPrice);
                error = vendor.Server_SellToVendor(new Customer<IItemInstance>(Guid.NewGuid(), bridge.player, bridge.inventoryPlayer.itemCollectionGroup, bridge.inventoryPlayer.currencyCollectionGroup), product, amount).error;
            }

            HandleError(error);
        }

        public PUN2ClientCollection<IVendorProduct<IItemInstance>> TargetRpc_AddVendorItemCollection(PhotonView owner, string collectionName, Guid collectionGuid, ushort slotCount)
        {
            logger.LogVerbose($"[Client] Server requested to add vendor item collection '{collectionName}' with Guid '{collectionGuid}' and slot count {slotCount}. ViewID: {bridge.photonView.ViewID}", bridge);

            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid) as PUN2ClientCollection<IVendorProduct<IItemInstance>>;
            if (collection == null)
            {
                collection = PUN2CollectionUtility.CreateClientVendorItemCollection(slotCount, collectionName, collectionGuid, owner, bridge);
            }

            return collection;
        }

        public void TargetRpc_SetItemVendorCollectionContents(PhotonView target, Guid collectionGuid, ItemVendorProductMessage[] productMessages)
        {
            logger.LogVerbose($"[Client] Server requested to set item vendor collection {collectionGuid} to contents {productMessages.Length}. ViewID: {bridge.photonView.ViewID}", bridge);

            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid) as PUN2ClientCollection<IVendorProduct<IItemInstance>>;
            if (collection != null)
            {
                var products = new Tuple<IVendorProduct<IItemInstance>, int>[productMessages.Length];
                for (int i = 0; i < products.Length; i++)
                {
                    int amount;
                    var product = productMessages[i].TryCreateVendorProductInstance(bridge.itemsDatabase, bridge.currencyDatabase, out amount);
                    if (product.item != null)
                    {
                        products[i] = new Tuple<IVendorProduct<IItemInstance>, int>(product, amount);
                    }
                    else
                    {
                        products[i] = new Tuple<IVendorProduct<IItemInstance>, int>(null, 0);
                    }
                }

                collection.ForceSet(products);
            }
        }

        private void HandleError(Error error, [CallerMemberName] string name = "")
        {
            if (error != null)
            {
                // TODO: Send message back to client about failed action...
                logger.Error($"Player action '{name}' failed: ", error, bridge.player);
            }
        }
    }
}
