namespace Devdog.Rucksack.Crafting
{
    public sealed class CraftDetails<T>
        where T : IIdentifiable
    {
        public IBlueprint<T> blueprint;

        public IFuel fuel;
        public float? fuelAvailable;
        
        // Currency?


    }
}