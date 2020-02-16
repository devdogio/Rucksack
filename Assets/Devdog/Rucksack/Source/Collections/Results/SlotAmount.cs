namespace Devdog.Rucksack.Collections
{
    public struct SlotAmount
    {
        public int slot;
        public int amount;
            
        public SlotAmount(int slot, int amount)
        {
            this.slot = slot;
            this.amount = amount;
        }
    }
    
    public struct SlotAmountItem<T>
    {
        public int slot;
        public int amount;
        public T item;
            
        public SlotAmountItem(int slot, int amount, T item)
        {
            this.slot = slot;
            this.amount = amount;
            this.item = item;
        }
    }
}