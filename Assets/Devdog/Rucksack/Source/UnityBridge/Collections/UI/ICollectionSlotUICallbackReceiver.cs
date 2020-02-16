namespace Devdog.Rucksack.UI
{
    public interface ICollectionSlotUICallbackReceiver<in T>
    {
        void Repaint(T item, int amount);
    }
}