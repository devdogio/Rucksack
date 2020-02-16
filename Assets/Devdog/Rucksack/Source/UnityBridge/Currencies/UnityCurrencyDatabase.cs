using Devdog.Rucksack.Database;
using UnityEngine;

namespace Devdog.Rucksack.Currencies
{
    [CreateAssetMenu(menuName = RucksackConstants.AddPath + "Currencies Database")]
    public class UnityCurrencyDatabase : UnityDatabase<UnityCurrency>
    {
        
    }
}