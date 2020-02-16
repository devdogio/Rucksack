namespace Devdog.Rucksack.Collections
{
    public class CollectionPriority<T> : ICollectionPriority<T>
        where T : IIdentifiable
    {
        private const int DefaultPriority = 50;
        
        public int generalPriority { get; }
        public int addPriority { get; }
        public int removePriority { get; }
        
        public CollectionPriority(int generalPriority = DefaultPriority, int addPriority = DefaultPriority, int removePriority = DefaultPriority)
        {
            this.generalPriority = generalPriority;
            this.addPriority = addPriority;
            this.removePriority = removePriority;
        }

        public int GetGeneralPriority()
        {
            return generalPriority;
        }
        
        public int GetAddPriority(T item)
        {
            return addPriority;
        }

        public int GetRemovePriority(T item)
        {
            return removePriority;
        }
    }
}