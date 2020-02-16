using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public class Customer<T> : ICustomer<T>
        where T : IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        public Guid ID { get; }
        public object character { get; }
        
        protected readonly CollectionGroup<T> inventories;
        protected readonly CurrencyCollectionGroup<ICurrency> currencies;

        public Customer(Guid id, object character, CollectionGroup<T> inventories, CurrencyCollectionGroup<ICurrency> currencies)
        {
            ID = id;
            this.character = character;
            this.inventories = inventories;
            this.currencies = currencies;
        }

        public Result<bool> CanAddItem(T item, int amount = 1)
        {
            return inventories.CanAdd(item, amount);
        }

        public Result<bool> AddItem(T item, int amount = 1)
        {
            var added = inventories.Add(item, amount);
            if (added.error != null)
            {
                return new Result<bool>(false, added.error);
            }
            
            return new Result<bool>(true);
        }

        public Result<bool> CanRemoveItem(T item, int amount = 1)
        {
            return inventories.CanRemove(item, amount);
        }

        public Result<bool> RemoveItem(T item, int amount)
        {
            var removed = inventories.Remove(item, amount);
            if (removed.error != null)
            {
                return new Result<bool>(false, removed.error);
            }
            
            return new Result<bool>(true);
        }

        
        
        public Result<bool> CanAddCurrency(ICurrency currency, double amount)
        {
            return currencies.CanAdd(currency, amount);
        }

        public Result<bool> AddCurrency(ICurrency currency, double amount)
        {
            return currencies.Add(currency, amount);
        }

        public Result<bool> CanRemoveCurrency(ICurrency currency, double amount)
        {
            return currencies.CanRemove(currency, amount);
        }

        public Result<bool> RemoveCurrency(ICurrency currency, double amount)
        {
            return currencies.Remove(currency, amount);
        }
    }
}