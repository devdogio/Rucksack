namespace Devdog.Rucksack.Items
{
    public partial class ItemUsedResult
    {
        public readonly int usedAmount;
        public readonly bool reduceStackSize;
        public readonly float cooldownTime;

        public ItemUsedResult(int usedAmount, bool reduceStackSize, float cooldownTime = 0f)
        {
            this.usedAmount = usedAmount;
            this.reduceStackSize = reduceStackSize;
            this.cooldownTime = cooldownTime;
        }
    }
}