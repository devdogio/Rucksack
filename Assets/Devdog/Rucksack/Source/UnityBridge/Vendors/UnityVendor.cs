using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public class UnityVendor<T> : Vendor<T>
        where T : class, IEquatable<T>, IIdentifiable, IStackable, ICloneable
    {
        public readonly Guid vendorGuid;
        public readonly string vendorCollectionName;
        public readonly Guid vendorCollectionGuid;

        public UnityVendor(Guid vendorGuid, string vendorCollectionName, Guid vendorCollectionGuid, VendorConfig config, ICollection<IVendorProduct<T>> collection, ICurrencyCollection<ICurrency, double> currencies = null)
            : base(config, collection, currencies)
        {
            this.vendorGuid = vendorGuid;
            this.vendorCollectionName = vendorCollectionName;
            this.vendorCollectionGuid = vendorCollectionGuid;
        }
    }
}