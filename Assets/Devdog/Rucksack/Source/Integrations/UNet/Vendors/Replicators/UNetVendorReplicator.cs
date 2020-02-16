using System;
using System.Runtime.CompilerServices;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public class UNetVendorReplicator
    {
        protected readonly ILogger logger;
        protected readonly UNetActionsBridge bridge;

        protected static UNetVendorInputValidator inputValidator;

        static UNetVendorReplicator()
        {
            inputValidator = new UNetVendorInputValidator(new UnityLogger("[UNet][Validation] "));
        }
        
        public UNetVendorReplicator(UNetActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[UNet] ");
        }

        
        #region Server
        
        public void Cmd_RequestBuyItemFromVendor(RequestBuyItemFromVendorMessage data)
        {
            logger.LogVerbose($"[Server] Client requested to buy item {data.itemGuid} {data.amount}x from vendor {data.vendorGuid} with netID {bridge.netId}", bridge);

            INetworkItemInstance item;
            INetworkVendor<IItemInstance> vendor;
            var error = inputValidator.ValidateBuyItemFromVendor(bridge.identity, data, out vendor, out item).error;
            if (error == null)
            {
                error = vendor.Server_BuyFromVendor(new Customer<IItemInstance>(Guid.NewGuid(), bridge.player, bridge.inventoryPlayer.itemCollectionGroup, bridge.inventoryPlayer.currencyCollectionGroup), item, data.amount).error;
            }
            
            HandleError(error);
        }

        public void Cmd_RequestSellItemToVendor(RequestSellItemToVendorMessage data)
        {
            logger.LogVerbose($"[Server] Client requested to sell item {data.itemGuid} {data.amount}x to vendor {data.vendorGuid} with netID {bridge.netId}", bridge);

            INetworkItemInstance item;
            INetworkVendor<IItemInstance> vendor;
            var error = inputValidator.ValidateSellItemToVendor(bridge.identity, data, out vendor, out item).error;
            if (error == null)
            {
                var product = new VendorProduct<IItemInstance>(item, item.itemDefinition.buyPrice, item.itemDefinition.sellPrice);
                error = vendor.Server_SellToVendor(new Customer<IItemInstance>(Guid.NewGuid(), bridge.player, bridge.inventoryPlayer.itemCollectionGroup, bridge.inventoryPlayer.currencyCollectionGroup), product, data.amount).error;
            }
            
            HandleError(error);
        }
        
        #endregion
        
        
        
        #region Client
        
        public void TargetRpc_AddVendorItemCollection(NetworkConnection target, AddCollectionMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid) as UNetClientCollection<IVendorProduct<IItemInstance>>;
            if (collection == null)
            {
                collection = UNetCollectionUtility.CreateClientVendorItemCollection(data.slotCount, data.collectionName, data.collectionGuid, data.owner, bridge);
            }
        }

        public void TargetRpc_SetItemVendorCollectionContents(NetworkConnection target, SetItemVendorCollectionContentsMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid) as UNetClientCollection<IVendorProduct<IItemInstance>>;
            if (collection != null)
            {
                var products = new Tuple<IVendorProduct<IItemInstance>, int>[data.products.Length];
                for (int i = 0; i < data.products.Length; i++)
                {
                    int amount;
                    var product = data.products[i].TryCreateVendorProductInstance(bridge.itemsDatabase, bridge.currencyDatabase, out amount);
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
        
        #endregion
        
        
        protected void HandleError(Error error, [CallerMemberName] string name = "")
        {
            if (error != null)
            {
                // TODO: Send message back to client about failed action...
                logger.Error($"Player action '{name}' failed: ", error, bridge.player);
            }
        }
    }
}