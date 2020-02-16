
using System;

namespace Devdog.Rucksack.Currencies
{
    public interface ICurrencyCollection<in TCurrencyType> : IReadOnlyCurrencyCollection<TCurrencyType, double>
    {
        
    }
    
    public interface ICurrencyCollection<in TCurrencyType, TPrecision> : IReadOnlyCurrencyCollection<TCurrencyType, TPrecision>
    {
        event EventHandler<CurrencyChangedResult<TPrecision>> OnCurrencyChanged;
        
        
        Result<bool> CanAdd(TCurrencyType currency, TPrecision amount);
        Result<bool> Add(TCurrencyType currency, TPrecision amount);

        Result<bool> CanRemove(TCurrencyType currency, TPrecision amount);
        Result<bool> Remove(TCurrencyType currency, TPrecision amount);

        Result<bool> Set(TCurrencyType currency, TPrecision amount);

        TPrecision GetCanAddAmount(ICurrency currency);
        void Clear();
    }
}