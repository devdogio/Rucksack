using System;

namespace Devdog.Rucksack.Currencies
{
    public sealed class CurrencyChangedResult<TPrecision> : EventArgs
    {
        public ICurrency currency { get; }
        public TPrecision amountBefore { get; }
        public TPrecision amountAfter { get; }

        public CurrencyChangedResult(ICurrency currency, TPrecision amountBefore, TPrecision amountAfter)
        {
            this.currency = currency;
            this.amountBefore = amountBefore;
            this.amountAfter = amountAfter;
        }
    }
}