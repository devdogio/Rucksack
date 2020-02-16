using System;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public class BuyFromVendorResult<T> : EventArgs
        where T : IIdentifiable, IStackable, IEquatable<T>
    {
        public ICustomer<T> customer { get; }
        public IVendorProduct<T> item { get; }
        public int amount { get; }
        
        public CurrencyDecorator<double>[] currencies { get; }

        public BuyFromVendorResult(ICustomer<T> customer, IVendorProduct<T> item, int amount, CurrencyDecorator<double>[] currencies)
        {
            this.customer = customer;
            this.item = item;
            this.amount = amount;
            this.currencies = currencies;
        }
    }
}