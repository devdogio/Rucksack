using System.Collections.Generic;

namespace Devdog.Rucksack.Currencies
{
    public sealed class ConversionTable<T, TPrecision>
        where T: class
    {
        public struct Row
        {
            public readonly TPrecision conversionRate;

            public Row(TPrecision conversionRate)
            {
                this.conversionRate = conversionRate;
            }
        }
        
        public readonly Dictionary<T, Row> conversions;
        
        
        public ConversionTable(Dictionary<T, Row> conversions = null)
        {
            this.conversions = conversions ?? new Dictionary<T, Row>();
        }

        public bool CanConvertTo(T to)
        {
            return conversions.ContainsKey(to);
        }

        public TPrecision ConversionRate(T to)
        {
            if (CanConvertTo(to) == false)
            {
                return default(TPrecision);
            }
            
            return conversions[to].conversionRate;
        }
    }
}