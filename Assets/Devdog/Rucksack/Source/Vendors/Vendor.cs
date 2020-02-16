using System;
using System.Collections.Generic;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public class Vendor<T> : IVendor<T>
        where T : class, IEquatable<T>, IIdentifiable, IStackable, ICloneable
    {
        public event EventHandler<BuyFromVendorResult<T>> OnBoughtFromVendor;
        public event EventHandler<SellToVendorResult<T>> OnSoldToVendor;

        public Collections.ICollection<IVendorProduct<T>> vendorCollection { get; set; }
        public ICurrencyCollection<ICurrency, double> vendorCurrencies { get; set; }

        public VendorConfig config { get; set; }

        public Vendor(VendorConfig config, Collections.ICollection<IVendorProduct<T>> collection, ICurrencyCollection<ICurrency, double> currencies = null)
        {
            this.config = config;
            vendorCollection = collection;
            vendorCurrencies = currencies ?? new InfiniteCurrencyCollection();
        }

        public IEnumerable<ProductAmount<IVendorProduct<T>>> GetAllProducts(ICustomer<T> customer)
        {
            for (int i = 0; i < vendorCollection.slotCount; i++)
            {
                yield return new ProductAmount<IVendorProduct<T>>(vendorCollection[i], vendorCollection.GetAmount(i));
            }
        }

        public void SetAllProducts(ICustomer<T> customer, IEnumerable<ProductAmount<IVendorProduct<T>>> products)
        {
            vendorCollection.Clear();

            int i = 0;
            foreach (var product in products)
            {
                vendorCollection.Set(i++, product.item, product.amount);
            }
        }

        public virtual Result<bool> CanBuyFromVendor(ICustomer<T> customer, T item, int amount = 1)
        {
            //
            // Vendor
            //
            var index = vendorCollection.IndexOf(o => o?.item != null && o.item.Equals(item));
            if (index == -1)
            {
                return new Result<bool>(false, Errors.VendorDoesNotContainItem);
            }

            var product = vendorCollection[index];
            if (product.buyPrice.Length == 0)
            {
                return new Result<bool>(false, Errors.VendorProductHasNoPrice);
            }

            var canRemove = vendorCollection.CanRemove(product, amount);
            if (canRemove.result == false)
            {
                return canRemove;
            }

            foreach (var c in product.buyPrice)
            {
                var vendorCanAddCurrency = vendorCurrencies.CanAdd(c.currency, c.amount * amount * config.buyFromVendorPriceMultiplier);
                if (vendorCanAddCurrency.result == false)
                {
                    return vendorCanAddCurrency;
                }
            }


            //
            // Customer
            //
            var canAdd = customer.CanAddItem(product.item, amount);
            if (canAdd.result == false)
            {
                return canAdd;
            }

            foreach (var c in product.buyPrice)
            {
                var canRemoveCurrency = customer.CanRemoveCurrency(c.currency, c.amount * amount * config.buyFromVendorPriceMultiplier);
                if (canRemoveCurrency.result == false)
                {
                    return canRemoveCurrency;
                }
            }

            return true;
        }

        public virtual Result<BuyFromVendorResult<T>> BuyFromVendor(ICustomer<T> customer, T item, int amount = 1)
        {
            var canBuy = CanBuyFromVendor(customer, item, amount);
            if (canBuy.result == false)
            {
                return new Result<BuyFromVendorResult<T>>(null, canBuy.error);
            }

            // We make a clone of the item to make sure a new unique instance is returned.
            // In the scenario where the user sells part of a stack to the vendor and buys it back he/she would buy back the exact same item instance, causing an error.

            // NOTE: Try to find the specific item instance
            var index = vendorCollection.IndexOf(o => o?.item != null && ReferenceEquals(o.item, item));
            if (index == -1)
            {
                // Couldn't find specific; Try to find an item that's equal to the one we want.
                index = vendorCollection.IndexOf(o => o?.item != null && o.item.Equals(item));
            }

            // No item found; Abort!
            if (index == -1)
            {
                return new Result<BuyFromVendorResult<T>>(null, Errors.VendorDoesNotContainItem);
            }

            var buyProduct = vendorCollection[index];
            if (amount < vendorCollection.GetAmount(index))
            {
                // Not buying entire stack, so make a clone.
                buyProduct = (IVendorProduct<T>)buyProduct.Clone();
            }

            if (config.removeBoughtProductFromVendor)
            {
                vendorCollection.Remove(buyProduct, amount);
            }

            foreach (var c in buyProduct.buyPrice)
            {
                vendorCurrencies.Add(c.currency, c.amount * amount * config.buyFromVendorPriceMultiplier);
            }

            customer.AddItem(buyProduct.item, amount);
            foreach (var c in buyProduct.buyPrice)
            {
                customer.RemoveCurrency(c.currency, c.amount * amount * config.buyFromVendorPriceMultiplier);
            }


            var buyResult = new BuyFromVendorResult<T>(customer, buyProduct, amount, buyProduct.buyPrice);
            OnBoughtFromVendor?.Invoke(this, buyResult);
            return buyResult;
        }

        public virtual Result<bool> CanSellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1)
        {
            if (product.sellPrice.Length == 0)
            {
                return new Result<bool>(false, Errors.VendorProductHasNoPrice);
            }

            //
            // Vendor
            //
            var canAdd = vendorCollection.CanAdd(product, amount);
            if (canAdd.result == false)
            {
                return canAdd;
            }

            // Vendor doesn't have enough currency
            foreach (var c in product.sellPrice)
            {
                var canRemoveCurrency = vendorCurrencies.CanRemove(c.currency, c.amount * amount * config.sellToVendorPriceMultiplier);
                if (canRemoveCurrency.result == false)
                {
                    return canRemoveCurrency;
                }
            }


            //
            // Customer
            //
            var canRemoveCustomer = customer.CanRemoveItem(product.item, amount);
            if (canRemoveCustomer.result == false)
            {
                return canRemoveCustomer;
            }

            foreach (var c in product.sellPrice)
            {
                var canAddCurrency = customer.CanAddCurrency(c.currency, c.amount * amount * config.sellToVendorPriceMultiplier);
                if (canAddCurrency.result == false)
                {
                    return canAddCurrency;
                }
            }

            return true;
        }

        public virtual Result<SellToVendorResult<T>> SellToVendor(ICustomer<T> customer, IVendorProduct<T> product, int amount = 1)
        {
            var canSell = CanSellToVendor(customer, product, amount);
            if (canSell.result == false)
            {
                return new Result<SellToVendorResult<T>>(null, canSell.error);
            }

            var originalAmount = product.collectionEntry?.amount;
            customer.RemoveItem(product.item, amount);

            var soldProduct = product;
            if (config.addSoldProductToVendor)
            {
                // NOTE: If we're removing the entire stack from the customer's collection it's safe the move everything over.
                // NOTE: If we're moving a part of the stack, duplicate it and move the duplicates.
                if (amount < originalAmount)
                {
                    // Not selling the entire stack; Duplicate the item for the next slot.
                    soldProduct = (IVendorProduct<T>)product.Clone();
                }

                vendorCollection.Add(soldProduct, amount);
            }

            foreach (var c in product.sellPrice)
            {
                vendorCurrencies.Remove(c.currency, c.amount * amount * config.sellToVendorPriceMultiplier);
                customer.AddCurrency(c.currency, c.amount * amount * config.sellToVendorPriceMultiplier);
            }

            var sellResult = new SellToVendorResult<T>(customer, soldProduct, amount, soldProduct.sellPrice);
            OnSoldToVendor?.Invoke(this, sellResult);
            return sellResult;
        }
    }
}