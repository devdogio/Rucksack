using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public interface ICustomer<in TItem> : IIdentifiable
    {
        object character { get; }
        
        Result<bool> CanAddItem(TItem item, int amount = 1);
        Result<bool> AddItem(TItem item, int amount = 1);

        Result<bool> CanRemoveItem(TItem item, int amount = 1);
        Result<bool> RemoveItem(TItem item, int amount);

        
        Result<bool> CanAddCurrency(ICurrency currency, double amount);
        Result<bool> AddCurrency(ICurrency currency, double amount);

        Result<bool> CanRemoveCurrency(ICurrency currency, double amount);
        Result<bool> RemoveCurrency(ICurrency currency, double amount);
    }
}