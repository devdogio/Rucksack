using System;

namespace Devdog.Rucksack
{
    public struct Identifier : IIdentifiable, IEquatable<IIdentifiable>, IEquatable<Identifier>
    {
        public Guid ID { get; }

        public Identifier(Guid guid)
        {
            ID = guid;
        }
        
        public bool Equals(IIdentifiable other)
        {
            if (ReferenceEquals(other, null)) return false;
            return ID.Equals(other.ID);
        }

        public bool Equals(Identifier other)
        {
            return Equals((IIdentifiable) other);
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }
}