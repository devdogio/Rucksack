using System;

namespace Devdog.Rucksack
{
    [System.Serializable]
    public class Error : IEquatable<Error>
    {
        public readonly int errorNumber;
        public readonly string message;

        private object[] _params = new object[0];

        public Error(int error, string message)
        {
            this.errorNumber = error;
            this.message = message;
        }

        
        // TODO: Consider chaining methods like .WithParams(abc)
        // TODO: Experimental
        public Error WithParams(params object[] p)
        {
            _params = p;
            return this;
        }


        public static bool operator ==(Error left, Error right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Error left, Error right)
        {
            return !Equals(left, right);
        }

        public bool Equals(Error other)
        {
            if (other == null)
            {
                return false;
            }
            
            return this.errorNumber == other.errorNumber;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Error) obj);
        }

        public override int GetHashCode()
        {
            return errorNumber;
        }

        public override string ToString()
        {
            return errorNumber + ":" + string.Format(message, _params);
        }
    }
}