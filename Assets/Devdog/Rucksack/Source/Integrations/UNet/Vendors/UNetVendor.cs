using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Vendors
{
    public class UNetVendor<T> : Vendor<T>, INetworkVendor<T>
        where T : class, IEquatable<T>, IIdentifiable, IStackable, ICloneable
    {
        public Guid vendorGuid { get; }
        public string vendorCollectionName { get; }
        public Guid vendorCollectionGuid { get; }
        public NetworkIdentity ownerIdentity { get; }

        public IReadOnlyCollection<IVendorProduct<T>> itemCollection
        {
            get { return vendorCollection; }
        }

        public IReadOnlyCurrencyCollection<ICurrency, double> currencyCollection
        {
            get { return vendorCurrencies; }
        }

        public UNetVendor(Guid vendorGuid, string vendorCollectionName, Guid vendorCollectionGuid, VendorConfig config, NetworkIdentity ownerIdentity, ICollection<IVendorProduct<T>> collection, ICurrencyCollection<ICurrency, double> currencies = null)
            : base(config, collection, currencies)
        {
            this.vendorGuid = vendorGuid;
            this.vendorCollectionName = vendorCollectionName;
            this.vendorCollectionGuid = vendorCollectionGuid;
            this.ownerIdentity = ownerIdentity;
        }

        public override Result<BuyFromVendorResult<T>> BuyFromVendor(ICustomer<T> customer, T item, int amount = 1)
        {
            var c = customer.character as UnityEngine.Component;
            Assert.IsNotNull(c, "Customer character object is not a UnityEngine.Component type");
            var bridge = c.GetComponent<UNetActionsBridge>();
            Assert.IsNotNull(bridge, "No UNetActionsBridge found on customer character");
            
            if (bridge.isClient)
            {
                bridge.Cmd_RequestBuyItemFromVendor(new RequestBuyItemFromVendorMessage()
                {
                    vendorGuid = vendorGuid,
                    itemGuid = item.ID,
                    amount = (ushort) amount
                });
                
                return new Result<BuyFromVendorResult<T>>(new BuyFromVendorResult<T>(customer, null, amount, new CurrencyDecorator<double>[0]));
            }
            else if(bridge.isServer)
            {
                return Server_BuyFromVendor(customer, item, amount);
            }

            return new Result<BuyFromVendorResult<T>>(null, Errors.NetworkNoAuthority);
        }

        public Result<BuyFromVendorResult<T>> Server_BuyFromVendor(ICustomer<T> customer, T item, int amount = 1)
        {
            return base.BuyFromVendor(customer, item, amount);
        }

        public override Result<SellToVendorResult<T>> SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1)
        {
            var c = customer.character as UnityEngine.Component;
            Assert.IsNotNull(c, "Customer character object is not a UnityEngine.Component type");
            var bridge = c.GetComponent<UNetActionsBridge>();
            Assert.IsNotNull(bridge, "No UNetActionsBridge found on customer character");

            if (bridge.isClient)
            {
                var a = vendorCollection as IUNetCollection;
                Assert.IsNotNull(a, "UNet vendor's collection is not a IUNetCollection");
                
                bridge.Cmd_RequestSellItemToVendor(new RequestSellItemToVendorMessage()
                {
                    sellFromCollectionGuid = a.ID,
                    vendorGuid = vendorGuid,
                    itemGuid = product.ID,
                    amount = (ushort) amount
                });
            }
            else if(bridge.isServer)
            {
                return Server_SellToVendor(customer, product, amount);
            }
            
            return new Result<SellToVendorResult<T>>(null, Errors.NetworkNoAuthority);
        }

        public Result<SellToVendorResult<T>> Server_SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1)
        {
            return base.SellToVendor(customer, product, amount);
        }
    }
}