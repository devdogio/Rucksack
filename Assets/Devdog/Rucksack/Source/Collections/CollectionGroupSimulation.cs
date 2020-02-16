using System;

namespace Devdog.Rucksack.Collections
{

    public sealed class CollectionGroupSimulation<TElementType> : CollectionGroupSimulation<TElementType, ICollection<TElementType>>
        where TElementType : class, IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable
    {
        public CollectionGroupSimulation(CollectionGroup<TElementType, ICollection<TElementType>> group)
            : base(group)
        {
            
        }
    }

    public class CollectionGroupSimulation<TElementType, TCollectionType> : IDisposable
        where TElementType : IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable
        where TCollectionType : ICloneable, ICollection<TElementType>
    {
        public CollectionGroup<TElementType, TCollectionType> group { get; }
        
        public CollectionGroupSimulation(CollectionGroup<TElementType, TCollectionType> group)
        {
            this.group = (CollectionGroup<TElementType, TCollectionType>)group.Clone();
            foreach (var collection in this.group.collections)
            {
                var c = collection.collection as ISimulatable;
                if (c == null)
                {
                    throw new ArgumentException("Collection in group is not a simulation collection; Can't simulate actions! - " + collection.collection);
                }

                c.SetSimulationEnabled(true);
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}