
namespace Devdog.Rucksack.CharacterEquipment
{
    public class UNetEquipmentInputValidator : NetworkInputValidatorBase
    {
        protected readonly ILogger logger;

        public UNetEquipmentInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[UNet][Validation] ");
        }


    }
}