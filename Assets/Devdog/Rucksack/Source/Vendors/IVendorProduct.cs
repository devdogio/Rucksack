using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{
    public interface IVendorProduct<T> : IIdentifiable, IStackable, ICloneable, IEquatable<IVendorProduct<T>>, ICollectionSlotEntry
    {
        T item { get; }
        CurrencyDecorator<double>[] buyPrice { get; }
        CurrencyDecorator<double>[] sellPrice { get; }
    }
}