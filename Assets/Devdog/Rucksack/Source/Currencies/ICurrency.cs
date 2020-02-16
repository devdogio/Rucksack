using System;

namespace Devdog.Rucksack.Currencies
{
    public interface ICurrency : IIdentifiable, IEquatable<ICurrency>, IConvertible<ICurrency, double>
    {
        string name { get; }
        string tokenName { get; }
        
        int decimals { get; }
        double maxAmount { get; }
    }
}