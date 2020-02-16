using System.Collections.Generic;

namespace Devdog.Rucksack.Currencies
{
    public class CurrencyDoubleConverter
    {
        public Result<double> Convert<T1, T2>(T1 from, double amount, T2 to)
            where T1 : class, IConvertible<T2, double>
            where T2 : class
        {
            return new DoubleConverter().Convert<T1, T2>(from, amount, to);
        }
        
        public Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row> GetAllConversions(ICurrency start)
        {
            var dict = new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>();
            GetAllConversionsRecursive(start, 1d, dict);
            return dict;
        }
        
        public void GetAllConversionsRecursive(ICurrency start, double startConversionRate, Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row> dict)
        {
            foreach (var kvp in start.conversionTable.conversions)
            {
                var rate = kvp.Value.conversionRate * startConversionRate;

                // Already have this conversion in our table, and it's not better than we one we have.
                if (dict.ContainsKey(kvp.Key))
                {
                    if (rate > dict[kvp.Key].conversionRate)
                    {
                        // Conversion rate is better than we currently have.
                        dict[kvp.Key] = new ConversionTable<ICurrency, double>.Row(rate);
                        GetAllConversionsRecursive(kvp.Key, kvp.Value.conversionRate * startConversionRate, dict);
                    }
                    
                    continue;
                }
                
                dict[kvp.Key] = new ConversionTable<ICurrency, double>.Row(rate);
                GetAllConversionsRecursive(kvp.Key, kvp.Value.conversionRate * startConversionRate, dict);
            }
        }
    }
}