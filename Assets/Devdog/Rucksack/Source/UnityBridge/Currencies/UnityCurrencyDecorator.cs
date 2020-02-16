using Devdog.General2;

namespace Devdog.Rucksack.Currencies
{
    [System.Serializable]
    public class UnityCurrencyDecorator
    {
        [Required]
        public UnityCurrency currency;
        public double amount;

        public UnityCurrencyDecorator()
        {
            
        }
        
        public UnityCurrencyDecorator(UnityCurrency currency, double amount)
        {
            this.currency = currency;
            this.amount = amount;
        }

        public CurrencyDecorator<double> ToNativeDecorator()
        {
            return new CurrencyDecorator(currency, amount);
        }
    }
}