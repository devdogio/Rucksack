namespace Devdog.Rucksack.Integrations.UMA
{
    public static class UMAErrors
    {
        [ErrorParameters("{CollectionName}", "{Item}")]
        public static readonly Error NoSuitableUMASlot = new Error(2001, "No suitable UMA slot found");

        [ErrorParameters("{CollectionName}", "{Item}")]
        public static readonly Error NotVisuallyEquipped = new Error(2002, "Not visually equiped");

        [ErrorParameters("{CollectionName}", "{Item}")]
        public static readonly Error CharacterHasNoUMAData = new Error(2003, "The character has no UMAData component attached.");

        [ErrorParameters("{CollectionName}", "{Item}")]
        public static readonly Error CharacterHasNoDynamicCharacterAvatar = new Error(2004, "The character has no DynamicCharacterAvatar component attached.");

    }
}
