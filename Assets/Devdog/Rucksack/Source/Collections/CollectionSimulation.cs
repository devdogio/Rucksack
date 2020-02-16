using System;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionSimulation<TCollectionType> : IDisposable
        where TCollectionType : ICollection, ISimulatable, ICloneable
    {
        public TCollectionType collection { get; }
        
        public CollectionSimulation(TCollectionType collection)
        {
            this.collection = (TCollectionType)collection.Clone();
            this.collection.SetSimulationEnabled(true);
        }
        
        public void Dispose()
        {
            
        }
    }
}