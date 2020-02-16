using System;

namespace Devdog.Rucksack.Crafting
{
    public class Blueprint<T> : IBlueprint<T>, IIdentifiable
        where T: IEquatable<T>, IIdentifiable
    {
        public Guid ID { get; }
        

        public Blueprint(Guid id)
        {
            ID = id;
        }
        
        public static bool operator ==(Blueprint<T> left, Blueprint<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Blueprint<T> left, Blueprint<T> right)
        {
            return !Equals(left, right);
        }
        
        
        

//        public virtual Result<bool> CanCreate()
//        {
//            throw new NotImplementedException();
//        }
//
//        public virtual Result<CraftResult<T>> Create()
//        {
//            throw new NotImplementedException();
//        }
//        
        
        
        public bool Equals(IBlueprint<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Blueprint<T>) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}