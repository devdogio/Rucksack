using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public interface INetworkVendor<T> : IVendor<T>
        where T : IIdentifiable, IStackable, IEquatable<T>, ICloneable
    {
        System.Guid vendorGuid { get; }
        IReadOnlyCollection<IVendorProduct<T>> itemCollection { get; }
        IReadOnlyCurrencyCollection<ICurrency, double> currencyCollection { get; }
        
        Result<BuyFromVendorResult<T>> Server_BuyFromVendor(ICustomer<T> customer, T item, int amount = 1);
        Result<SellToVendorResult<T>> Server_SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1);
    }
}