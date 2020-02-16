using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Currencies
{
    public sealed class CurrencyDecorator : CurrencyDecorator<double>, IEquatable<CurrencyDecorator>
    {
        // For serializers
        public CurrencyDecorator()
        { }
        
        public CurrencyDecorator(ICurrency definition, double amount)
            : base(definition, amount)
        { }
        
        public static bool operator ==(CurrencyDecorator left, CurrencyDecorator right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CurrencyDecorator left, CurrencyDecorator right)
        {
            return !Equals(left, right);
        }
        
        
        public override string ToString()
        {
            return $"{Math.Round(amount, currency.decimals)} {currency}";
        }
        
        public bool Equals(CurrencyDecorator other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    
    public class CurrencyDecorator<TPrecision> : IEquatable<CurrencyDecorator<TPrecision>>
    {
        public ICurrency currency;
        public TPrecision amount;

        // For serializers
        public CurrencyDecorator()
        { }
        
        public CurrencyDecorator(ICurrency currency, TPrecision amount)
        {
            this.currency = currency;
            this.amount = amount;
        }
        
        public static bool operator ==(CurrencyDecorator<TPrecision> left, CurrencyDecorator<TPrecision> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CurrencyDecorator<TPrecision> left, CurrencyDecorator<TPrecision> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{amount} {currency}";
        }

        public bool Equals(CurrencyDecorator<TPrecision> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(currency, other.currency) && EqualityComparer<TPrecision>.Default.Equals(amount, other.amount);
        }

        public override bool Equals(object obj)
        {
            return Equals((CurrencyDecorator<TPrecision>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((currency != null ? currency.GetHashCode() : 0) * 397) ^ EqualityComparer<TPrecision>.Default.GetHashCode(amount);
            }
        }
    }
}