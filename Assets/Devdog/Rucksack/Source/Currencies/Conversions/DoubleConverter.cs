
namespace Devdog.Rucksack.Currencies
{
    public sealed class DoubleConverter
    {
        
        
        public Result<double> Convert<T1, T2>(T1 from, double amount, T2 to)
            where T1 : class, IConvertible<T2, double>
            where T2 : class
        {
            if (amount < 0)
            {
                return new Result<double>(0, Errors.CurrencyCanNotConvertToTarget);
            }
            
            var convertingToSelf = ReferenceEquals(from, to);
            var canConvert = from.conversionTable.CanConvertTo(to) || convertingToSelf;
            if (canConvert == false)
            {
                return new Result<double>(0.0d, Errors.CurrencyCanNotConvertToTarget);
            }

            var conversionRate = convertingToSelf ? 1d : from.conversionTable.ConversionRate(to);
            return new Result<double>(amount * conversionRate);
        }
        
    }
}