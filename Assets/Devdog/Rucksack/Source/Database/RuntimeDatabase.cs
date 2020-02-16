using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Devdog.Rucksack.Database
{
    public class RuntimeDatabase<T> : IDatabase<T>
    {
        private readonly Dictionary<IIdentifiable, T> _dict;
        
        private readonly ILogger _logger;
        public RuntimeDatabase(ILogger logger = null)
        {
            _dict = new Dictionary<IIdentifiable, T>();
            _logger = logger ?? new Logger("[Database] ");
        }
       
        public void Preload(IEnumerable<IIdentifiable> ids)
        {
            _logger.Warning($"Preloading is not supported on {GetType()}");
        }

        public Result<T> Get(IIdentifiable identifier)
        {
            T outVal;
            if (_dict.TryGetValue(identifier, out outVal))
            {
                return new Result<T>(outVal);
            }

            return new Result<T>(default(T), Errors.DatabaseItemNotFound);
        }

        public Result<T> Get(Expression<Func<T, bool>> selector)
        {
            foreach (var kvp in _dict)
            {
                if (selector.Compile().Invoke(kvp.Value))
                {
                    return new Result<T>(kvp.Value);
                }
            }

            return new Result<T>(default(T), Errors.DatabaseItemNotFound);
        }

        public Task<Result<T>> GetAsync(IIdentifiable identifier, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() => Get(identifier), cancellationToken);
        }

        public Task<Result<T>> GetAsync(Expression<Func<T, bool>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() => Get(selector), cancellationToken);
        }

        public Result<IEnumerable<T>> GetAll()
        {
            return new Result<IEnumerable<T>>(_dict.Values);
        }

        public Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() => GetAll(), cancellationToken);
        }
        
        public Result<bool> Set(IIdentifiable identifier, T item)
        {
            _dict[identifier] = item;
            return true;
        }

        public Task<Result<bool>> SetAsync(IIdentifiable identifier, T item, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() => Set(identifier, item), cancellationToken);
        }

        public Result<bool> Remove(IIdentifiable identifier)
        {
            _dict.Remove(identifier);
            return true;
        }

        public Task<Result<bool>> RemoveAsync(IIdentifiable identifier, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => Remove(identifier), cancellationToken);
        }

        public void Dispose()
        {
            // Nothing to dispose of
        }
    }
}