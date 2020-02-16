using System;

namespace Devdog.Rucksack.Currencies
{
    /// <summary>
    /// This currency collection can never be depleted.
    /// </summary>
    public class InfiniteCurrencyCollection : ICurrencyCollection<ICurrency, double>
    {
        public event EventHandler<CurrencyChangedResult<double>> OnCurrencyChanged
        {
            add
            {
                // Event is never fired...
            }
            remove
            {
                // Event is never fired...
            }
        }
       
        public double GetAmount(ICurrency currency)
        {
            return double.MaxValue;
        }

        public bool Contains(ICurrency currency)
        {
            return true;
        }

        public Result<bool> CanAdd(ICurrency currency, double amount)
        {
            return true;
        }

        public Result<bool> Add(ICurrency currency, double amount)
        {
            return true;
        }

        public Result<bool> CanRemove(ICurrency currency, double amount)
        {
            return true;
        }

        public Result<bool> Remove(ICurrency currency, double amount)
        {
            return true;
        }

        public Result<bool> Set(ICurrency currency, double amount)
        {
            return true;
        }

        public Result<double> GetCanRemoveAmount(ICurrency currency)
        {
            return double.MaxValue;
        }
        
        public double GetCanAddAmount(ICurrency currency)
        {
            return double.MaxValue;
        }

        public void ChangeAmount(ICurrency currency, double changeAmount)
        { }

        public void Clear()
        { }

        public CurrencyDecorator<double>[] ToDecorators()
        {
            return new CurrencyDecorator<double>[0];
        }
    }
}
