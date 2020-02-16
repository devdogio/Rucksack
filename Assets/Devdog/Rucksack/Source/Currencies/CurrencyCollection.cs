using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Currencies
{
    public class CurrencyCollection : ICurrencyCollection<ICurrency, double>
    {
        public event EventHandler<CurrencyChangedResult<double>> OnCurrencyChanged;
        private Dictionary<ICurrency, double> _currencies = new Dictionary<ICurrency, double>();
        
        public string collectionName { get; set; }
        public bool isReadOnly { get; set; }
        public CurrencyCollection()
        {
            
        }

        public double GetAmount(ICurrency currency)
        {
            double amount;
            _currencies.TryGetValue(currency, out amount);
            return amount;
        }

        public bool Contains(ICurrency currency)
        {
            return _currencies.ContainsKey(currency);
        }

        public Result<bool> CanAdd(ICurrency currency, double amount)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }
            
            if (GetCanAddAmount(currency) < amount)
            {
                return new Result<bool>(false, Errors.CollectionFull);
            }
                
            return new Result<bool>(true);
        }

        public Result<bool> Add(ICurrency currency, double amount)
        {
            var canAdd = CanAdd(currency, amount);
            if (canAdd.result == false)
            {
                return canAdd;
            }

            double before = 0f;
            if (_currencies.ContainsKey(currency) == false)
            {
                _currencies[currency] = amount;
            }
            else
            {
                before = _currencies[currency];
                _currencies[currency] += amount;
            }
            
//            logger.LogVerbose($"Added {Math.Round(amount, currency.decimals)} {currency}");
            OnCurrencyChanged?.Invoke(this, new CurrencyChangedResult<double>(currency, before, GetAmount(currency)));
            return new Result<bool>(true);
        }

        public Result<bool> Add(CurrencyDecorator decorator)
        {
            return Add(decorator.currency, decorator.amount);
        }

        public Result<bool> CanRemove(ICurrency currency, double amount)
        {
            if (isReadOnly)
            {
                return new Result<bool>(false, Errors.CollectionIsReadOnly);
            }

            if (amount <= 0d)
            {
                return true;
            }

            if (_currencies.ContainsKey(currency) == false)
            {
                return new Result<bool>(false, Errors.CurrencyCollectionDoesNotContainCurrency);
            }
            
            if (GetAmount(currency) < amount)
            {
                return new Result<bool>(false, Errors.CurrencyCollectionDoesNotContainCurrency);
            }
            
            return new Result<bool>(true);
        }

        public Result<bool> Remove(ICurrency currency, double amount)
        {
            var canRemove = CanRemove(currency, amount);
            if (canRemove.result == false)
            {
                return canRemove;
            }
            
            if (amount <= 0d)
            {
                return true;
            }

            var before = _currencies[currency];
            _currencies[currency] -= amount;
            if (_currencies[currency] <= 0f)
            {
                _currencies.Remove(currency);
            }
            
//            logger.LogVerbose($"Removed {Math.Round(amount, currency.decimals)} {currency}");
            OnCurrencyChanged?.Invoke(this, new CurrencyChangedResult<double>(currency, before, GetAmount(currency)));
            return new Result<bool>(true);
        }

        public Result<bool> Remove(CurrencyDecorator<double> decorator)
        {
            return Remove(decorator.currency, decorator.amount);
        }

        public Result<bool> Set(ICurrency currency, double amount)
        {
            var current = GetAmount(currency);
            if (amount > current)
            {
                return Add(currency, amount - current);
            }
            
            if (amount < current)
            {
                return Remove(currency, current - amount);
            }
            
            return new Result<bool>(true);
        }

        public double GetCanAddAmount(ICurrency currency)
        {
            if (isReadOnly)
            {
                return 0d;
            }

            return currency.maxAmount - GetAmount(currency);
        }

        public void Clear()
        {
            if (isReadOnly)
            {
                return;
            }

            foreach (var currency in _currencies)
            {
                OnCurrencyChanged?.Invoke(this, new CurrencyChangedResult<double>(currency.Key, currency.Value, 0f));
            }
            
            _currencies.Clear();
        }

        public CurrencyDecorator<double>[] ToDecorators()
        {
            var decs = new CurrencyDecorator<double>[_currencies.Count];
            int i = 0;
            foreach (var currency in _currencies)
            {
                decs[i++] = new CurrencyDecorator<double>(currency.Key, currency.Value);
            }

            return decs;
        }
    }
}
