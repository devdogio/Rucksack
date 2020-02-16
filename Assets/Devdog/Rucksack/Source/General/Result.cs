
namespace Devdog.Rucksack
{
    public struct Result<T>
    {
        public T result { get; }
        public Error error { get; }

        public Result(T result) 
            : this(result, null)
        { }
        
        public Result(T result, Error error)
        {
            this.result = result;
            this.error = error;
        }

        public static implicit operator Result<T>(T val)
        {
            return new Result<T>(val);
        }
        
        public static implicit operator Result<T>(Error error)
        {
            return new Result<T>(default(T), error);
        }
        
        public override string ToString()
        {
            if (error != null)
            {
                return error.ToString();
            }

            return "" + result;
        }
    }
}