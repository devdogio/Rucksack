using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Devdog.Rucksack.Database
{
    public interface IReadOnlyDatabase<T> : IDisposable
    {
        void Preload(IEnumerable<IIdentifiable> ids);
        
        Result<T> Get(IIdentifiable identifier);
        Result<T> Get(Expression<Func<T, bool>> selector);
        Task<Result<T>> GetAsync(IIdentifiable identifier, CancellationToken cancellationToken = default(CancellationToken));
        Task<Result<T>> GetAsync(Expression<Func<T, bool>> selector, CancellationToken cancellationToken = default(CancellationToken));
        
        Result<IEnumerable<T>> GetAll();
        Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}