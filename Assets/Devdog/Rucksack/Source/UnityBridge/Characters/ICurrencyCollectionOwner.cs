using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Characters
{
    public interface ICurrencyCollectionOwner
    {
        CurrencyCollectionGroup<ICurrency> currencyCollectionGroup { get; }
    }
}