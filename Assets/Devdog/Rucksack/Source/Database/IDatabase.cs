using System.Threading;
using System.Threading.Tasks;

namespace Devdog.Rucksack.Database
{
    public interface IDatabase<T> : IReadOnlyDatabase<T>
    {
        
        Result<bool> Set(IIdentifiable identifier, T item);
        Task<Result<bool>> SetAsync(IIdentifiable identifier, T item, CancellationToken cancellationToken = default(CancellationToken));

        Result<bool> Remove(IIdentifiable identifier);
        Task<Result<bool>> RemoveAsync(IIdentifiable identifier, CancellationToken cancellationToken = default(CancellationToken));
        
    }
}