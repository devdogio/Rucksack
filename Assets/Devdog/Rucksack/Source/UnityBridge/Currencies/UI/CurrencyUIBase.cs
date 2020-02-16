using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.UI
{
    public abstract class CurrencyUIBase<TCurrencyUI> : UIMonoBehaviour
        where TCurrencyUI : CurrencyUIBase<TCurrencyUI>
    {
        public ICurrency currency { get; set; }
        public CurrencyCollectionUIBase<TCurrencyUI> collectionUI { get; set; }

        public override string ToString()
        {
            return currency?.ToString() ?? "<NULL>";
        }

        public abstract void Repaint(double amount, ICurrency currency);
    }
}