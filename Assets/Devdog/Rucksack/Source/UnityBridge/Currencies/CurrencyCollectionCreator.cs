using Devdog.Rucksack.Collections;
using UnityEngine;

namespace Devdog.Rucksack.Currencies
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    public sealed class CurrencyCollectionCreator : BaseCollectionCreator<ICurrencyCollection<ICurrency, double>>
    {
        protected override ICurrencyCollection<ICurrency, double> CreateCollection()
        {
            return new CurrencyCollection()
            {
                collectionName = collectionName
            };
        }

        protected override void RegisterByName(ICurrencyCollection<ICurrency, double> col)
        {
            if (CurrencyCollectionRegistry.byName.Contains(collectionName))
            {
                if (!ignoreDuplicates)
                {
                    _logger.Error($"Currency collection with name {collectionName} already exists in CurrencyCollectionRegistry", this);
                    return;
                }
                else
                {
                    _logger.Warning($"Currency collection with name {collectionName} already exists in CurrencyCollectionRegistry and will be overridden by this one. " +
                        $"Use \"ignoreDuplicates = false\" to avoid collection override.", this);
                }
            }

            CurrencyCollectionRegistry.byName.Register(collectionName, col);
        }

        protected override void RegiterByID(ICurrencyCollection<ICurrency, double> col)
        {
            CurrencyCollectionRegistry.byID.Register(collectionID, col);
        }

        protected override void UnRegister()
        {
            if (CurrencyCollectionRegistry.byName != null)
                CurrencyCollectionRegistry.byName.UnRegister(collectionName);

            if (CurrencyCollectionRegistry.byID != null)
                CurrencyCollectionRegistry.byID.UnRegister(collectionID);
        }
    }
}