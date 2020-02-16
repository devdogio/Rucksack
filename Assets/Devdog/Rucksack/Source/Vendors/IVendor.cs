using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Vendors
{
    public interface IVendor
    {
        
    }
    
    public interface IVendor<T> : IVendor
        where T : IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        event EventHandler<BuyFromVendorResult<T>> OnBoughtFromVendor;
        event EventHandler<SellToVendorResult<T>> OnSoldToVendor;
        
        IEnumerable<ProductAmount<IVendorProduct<T>>> GetAllProducts(ICustomer<T> customer);
        void SetAllProducts(ICustomer<T> customer, IEnumerable<ProductAmount<IVendorProduct<T>>> products);
//        void Set(ICustomer<T> customer, int index, IVendorProduct<T> product, int amount = 1);
//        bool HasItemForSale(IIdentifiable identifier, int amount = 1);

        Result<bool> CanBuyFromVendor(ICustomer<T> customer, T item, int amount = 1);
        Result<bool> CanSellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1);
        Result<BuyFromVendorResult<T>> BuyFromVendor(ICustomer<T> customer, T product, int amount = 1);
        Result<SellToVendorResult<T>> SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1);
    }
}