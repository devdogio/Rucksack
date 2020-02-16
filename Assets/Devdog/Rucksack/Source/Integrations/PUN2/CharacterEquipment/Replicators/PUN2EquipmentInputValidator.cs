
namespace Devdog.Rucksack.CharacterEquipment
{
    public class PUN2EquipmentInputValidator : NetworkInputValidatorBase
    {
        protected readonly ILogger logger;

        public PUN2EquipmentInputValidator(ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[PUN2][Validation] ");
        }


    }
}