namespace Devdog.Rucksack.Integrations.Morph3D
{
    public static class Morph3DErrors
    {
        [ErrorParameters("{CollectionName}", "{Item}")]
        public static readonly Error CharacterHasNoM3DCharacterManager = new Error(3001, "The character has no M3DCharacterManager component attached.");


    }
}
