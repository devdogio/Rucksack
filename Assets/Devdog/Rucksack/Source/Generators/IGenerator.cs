using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Generators
{
    public interface IGenerator<T>
        where T : IEquatable<T>
    {
        void SetSource(IEnumerable<T> source);
        IEnumerable<T> Generate();
    }
}