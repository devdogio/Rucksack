namespace Devdog.Rucksack.Vendors
{
    public struct ProductAmount<T>
    {
        public readonly T item;
        public readonly int amount;

        public ProductAmount(T item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }
}