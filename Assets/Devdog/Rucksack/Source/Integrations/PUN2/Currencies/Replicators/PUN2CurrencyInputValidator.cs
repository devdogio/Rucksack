namespace Devdog.Rucksack.Currencies
{
    public sealed class PUN2CurrencyInputValidator : NetworkInputValidatorBase
    {
        private readonly ILogger logger;

        public PUN2CurrencyInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[PUN2][Validation] ");
        }
    }
}