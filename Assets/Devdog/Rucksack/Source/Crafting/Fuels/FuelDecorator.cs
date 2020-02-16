namespace Devdog.Rucksack.Crafting
{
    public sealed class FuelDecorator<T>
        where T : IFuel
    {
        public T fuel { get; }
        public int amount { get; }

        public FuelDecorator(T fuel, int amount)
        {
            this.fuel = fuel;
            this.amount = amount;
        }
    }
}