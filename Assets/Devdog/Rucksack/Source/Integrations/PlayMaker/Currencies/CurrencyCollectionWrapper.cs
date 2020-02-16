using Devdog.Rucksack.Currencies;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    public class CurrencyCollectionWrapper : ScriptableObject
    {
        public ICurrencyCollection<ICurrency, double> collection;
    }
}