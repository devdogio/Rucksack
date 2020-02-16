using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using UnityEngine.Assertions;
using Photon.Pun;

namespace Devdog.Rucksack.Vendors
{
    public class PUN2Vendor<T> : Vendor<T>, INetworkVendor<T>
        where T : class, IEquatable<T>, IIdentifiable, IStackable, ICloneable
    {
        public Guid vendorGuid { get; }
        public string vendorCollectionName { get; }
        public Guid vendorCollectionGuid { get; }
        public PhotonView ownerIdentity { get; }

        public IReadOnlyCollection<IVendorProduct<T>> itemCollection
        {
            get { return vendorCollection; }
        }

        public IReadOnlyCurrencyCollection<ICurrency, double> currencyCollection
        {
            get { return vendorCurrencies; }
        }

        public PUN2Vendor(Guid vendorGuid, string vendorCollectionName, Guid vendorCollectionGuid, VendorConfig config, PhotonView ownerIdentity, ICollection<IVendorProduct<T>> collection, ICurrencyCollection<ICurrency, double> currencies = null)
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
            var bridge = c.GetComponent<PUN2ActionsBridge>();
            Assert.IsNotNull(bridge, "No PUN2ActionsBridge found on customer character");

            //if (!PhotonNetwork.IsMasterClient /*!bridge.photonView.IsServer() /*bridge.isClient*/)
            {
                bridge.photonView.RPC(nameof(bridge.Cmd_RequestBuyItemFromVendor), PhotonNetwork.MasterClient, 
                    /*vendorGuid: */ vendorGuid.ToByteArray(),
                    /*itemGuid: */ item.ID.ToByteArray(),
                    /*amount: */ amount
                );

                return new Result<BuyFromVendorResult<T>>(new BuyFromVendorResult<T>(customer, null, amount, new CurrencyDecorator<double>[0]));
            }
            //else if (bridge.photonView.IsServer() /*bridge.isServer*/)
            //{
            //    return Server_BuyFromVendor(customer, item, amount);
            //}
            //
            //return new Result<BuyFromVendorResult<T>>(null, Errors.NetworkNoAuthority);
        }

        public Result<BuyFromVendorResult<T>> Server_BuyFromVendor(ICustomer<T> customer, T item, int amount = 1)
        {
            return base.BuyFromVendor(customer, item, amount);
        }

        public override Result<SellToVendorResult<T>> SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1)
        {
            var c = customer.character as UnityEngine.Component;
            Assert.IsNotNull(c, "Customer character object is not a UnityEngine.Component type");
            var bridge = c.GetComponent<PUN2ActionsBridge>();
            Assert.IsNotNull(bridge, "No PUN2ActionsBridge found on customer character");

            //if (!bridge.photonView.IsServer() /*bridge.isClient*/)
            {
                var a = vendorCollection as IPUN2Collection;
                Assert.IsNotNull(a, "PUN2 vendor's collection is not a IPUN2Collection");

                bridge.photonView.RPC(nameof(bridge.Cmd_RequestSellItemToVendor), PhotonNetwork.MasterClient,
                    ///*sellFromCollectionGuid: */ a.ID.ToByteArray(),
                    /*vendorGuid: */ vendorGuid.ToByteArray(),
                    /*itemGuid: */ product.ID.ToByteArray(),
                    /*amount: */ amount
                );

                return new Result<SellToVendorResult<T>>(null, null /*new Error(int.MinValue, "No result known yet")*/);
            }
            //else if (bridge.photonView.IsServer() /*bridge.isServer*/)
            //{
            //    return Server_SellToVendor(customer, product, amount);
            //}
            
            //return new Result<SellToVendorResult<T>>(null, Errors.NetworkNoAuthority);
        }

        public Result<SellToVendorResult<T>> Server_SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1)
        {
            return base.SellToVendor(customer, product, amount);
        }
    }
}