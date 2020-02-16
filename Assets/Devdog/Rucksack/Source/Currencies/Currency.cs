using System;

namespace Devdog.Rucksack.Currencies
{
    public class Currency : ICurrency
    {
        public Guid ID { get; }
        public string name { get; set; }
        public string tokenName { get; set; }
        public int decimals { get; }
        public double maxAmount { get; }
        public ConversionTable<ICurrency, double> conversionTable { get; set; }

        // For serializers
        public Currency()
        { }
        
        public Currency(Guid id, string name, string tokenName, int decimals, double maxAmount, ConversionTable<ICurrency, double> conversionTable = null)
        {
            ID = id;
            this.name = name;
            this.tokenName = tokenName;
            this.decimals = decimals;
            this.maxAmount = maxAmount;
            
            this.conversionTable = conversionTable ?? new ConversionTable<ICurrency, double>();
        }

        public static bool operator ==(Currency left, Currency right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Currency left, Currency right)
        {
            return !Equals(left, right);
        }
        

        public bool Equals(ICurrency other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Currency) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }
    }
}