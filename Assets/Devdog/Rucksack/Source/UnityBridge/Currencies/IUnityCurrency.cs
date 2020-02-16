using UnityEngine;

namespace Devdog.Rucksack.Currencies
{
    public interface IUnityCurrency : ICurrency
    {
        Sprite icon { get; }
    }
}