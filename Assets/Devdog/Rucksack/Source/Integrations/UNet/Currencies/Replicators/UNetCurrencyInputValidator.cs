
namespace Devdog.Rucksack.Currencies
{
    public class UNetCurrencyInputValidator : NetworkInputValidatorBase
    {
        protected readonly ILogger logger;

        public UNetCurrencyInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[UNet][Validation] ");
        }

        
        
    }
}