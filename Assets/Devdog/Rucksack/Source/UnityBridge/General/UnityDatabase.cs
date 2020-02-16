using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Devdog.Rucksack.Database
{
    public abstract class UnityDatabase<T> : ScriptableObject, IDatabase<T>
        where T : UnityEngine.Object, IIdentifiable
    {
        // TODO: Consider changing to dict for faster lookups.
        [SerializeField]
        private List<T> _objects = new List<T>();
        private readonly ILogger _logger;

        protected UnityDatabase(ILogger logger = null)
        {
            _logger = logger ?? new UnityLogger($"[{GetType()}] ");
        }
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_objects.Contains(null))
            {
                _logger.Error($"Database contains null record!", this);
            }
        }
#endif
    
        public void Preload(IEnumerable<IIdentifiable> ids)
        {
            _logger.Warning($"Preloading is not supported on {GetType()}");
        }

        public Result<T> Get(IIdentifiable identifier)
        {
            var obj = _objects.FirstOrDefault(o => o.ID == identifier.ID);
            if (obj == null)
            {
                return new Result<T>(default(T), Errors.DatabaseItemNotFound);
            }

            return obj;
        }

        public Result<T> Get(Expression<Func<T, bool>> selector)
        {
            var obj = _objects.AsQueryable().FirstOrDefault(selector);
            if (obj == null)
            {
                return new Result<T>(default(T), Errors.DatabaseItemNotFound);
            }

            return obj;
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
            return _objects;
        }

        public Task<Result<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() => GetAll(), cancellationToken);
        }

        private int IndexOf(IIdentifiable identifier)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i].ID == identifier.ID)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public Result<bool> Set(IIdentifiable identifier, T item)
        {
            var index = IndexOf(identifier);
            if (index != -1)
            {
                _objects[index] = item;
            }
            else
            {
                // Item not in array yet
                _objects.Add(item);
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            return true;
        }

        public Task<Result<bool>> SetAsync(IIdentifiable identifier, T item, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.Factory.StartNew(() => Set(identifier, item), cancellationToken);
        }

        public Result<bool> Remove(IIdentifiable identifier)
        {
            _objects.RemoveAt(IndexOf(identifier));
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