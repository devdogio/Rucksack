using System;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public sealed class NullCustomer<T> : ICustomer<T>
        where T : IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        public Guid ID { get; }
        public object character { get; set; }
        
        public NullCustomer(Guid id)
        {
            ID = id;
        }

        public Result<bool> CanAddItem(T item, int amount = 1)
        {
            return new Result<bool>(true);
        }

        public Result<bool> AddItem(T item, int amount = 1)
        {
            return new Result<bool>(true);
        }

        public Result<bool> CanRemoveItem(T item, int amount = 1)
        {
            return new Result<bool>(true);
        }

        public Result<bool> RemoveItem(T item, int amount)
        {
            return new Result<bool>(true);
        }

        
        
        public Result<bool> CanAddCurrency(ICurrency currency, double amount)
        {
            return new Result<bool>(true);
        }

        public Result<bool> AddCurrency(ICurrency currency, double amount)
        {
            return new Result<bool>(true);
        }

        public Result<bool> CanRemoveCurrency(ICurrency currency, double amount)
        {
            return new Result<bool>(true);
        }

        public Result<bool> RemoveCurrency(ICurrency currency, double amount)
        {
            return new Result<bool>(true);
        }

    }
}