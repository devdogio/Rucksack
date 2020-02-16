using System;
using System.Collections.Generic;

namespace Devdog.Rucksack.Collections
{
    public class NetworkPermissionsMap<TType, TIdentity>
        where TType : INetworkObject<TIdentity>
    {
        public delegate void OnPermissionChangedDelegate(PermissionChangedResult<TType, TIdentity> result);
        
        private readonly Dictionary<TType, HashSet<TIdentity>> _collectionToIdentity;
        private readonly Dictionary<TIdentity, HashSet<PermissionModel<TType>>> _identityToCollection;
        private readonly Dictionary<TIdentity, List<OnPermissionChangedDelegate>> _eventHandlers;

        private readonly IEqualityComparer<PermissionModel<TType>> _permissionModelComparer;
        public NetworkPermissionsMap(IEqualityComparer<PermissionModel<TType>> permissionModelComparer = null)
        {
            _permissionModelComparer = permissionModelComparer ?? new PermissionModelComparer<TType>();
            
            _collectionToIdentity = new Dictionary<TType, HashSet<TIdentity>>();
            _identityToCollection = new Dictionary<TIdentity, HashSet<PermissionModel<TType>>>();
            _eventHandlers = new Dictionary<TIdentity, List<OnPermissionChangedDelegate>>();
        }

        public void AddEventListener(TIdentity identity, OnPermissionChangedDelegate callback)
        {
            if (_eventHandlers.ContainsKey(identity) == false)
            {
                _eventHandlers.Add(identity, new List<OnPermissionChangedDelegate>());
            }
            
            _eventHandlers[identity].Add(callback);
        }

        public bool RemoveEventListener(TIdentity identity, OnPermissionChangedDelegate callback)
        {
            if (_eventHandlers.ContainsKey(identity))
            {
                return _eventHandlers[identity].Remove(callback);
            }

            return false;
        }

        public void RemoveAllEventListenersForIdentity(TIdentity identity)
        {
            if (_eventHandlers.ContainsKey(identity))
            {
                _eventHandlers[identity].Clear();
            }
        }
        
        /// <summary>
        /// Make sure the hashsets inside each dictionary exist to avoid useless checkups.
        /// </summary>
        private void SetKeys(TType key)
        {
            if (_collectionToIdentity.ContainsKey(key) == false)
            {
                _collectionToIdentity[key] = new HashSet<TIdentity>();
            }
        }

        private void SetKeys(TIdentity identity)
        {
            if (_identityToCollection.ContainsKey(identity) == false)
            {
                _identityToCollection[identity] = new HashSet<PermissionModel<TType>>(_permissionModelComparer);
            }
        }

        private PermissionModel<TType> GetCollectionKey(TType collection)
        {
            return new PermissionModel<TType>(collection, ReadWritePermission.None);
        }

        /// <summary>
        /// Revoke all permissions from all clients, servers, etc. Everything!
        /// </summary>
        public void RevokeAll()
        {
            _collectionToIdentity.Clear();
            _identityToCollection.Clear();
        }
        
        /// <summary>
        /// Revoke permission for all collections for a certain identity.
        /// </summary>
        /// <param name="identity"></param>
        public void RevokeAllForIdentity(TIdentity identity)
        {
            foreach (var col in _collectionToIdentity)
            {
                col.Value.Remove(identity);
            }
            _identityToCollection.Remove(identity);
        }

        /// <summary>
        /// Get the the permission for this collection by a given identity.
        /// </summary>
        /// <returns>The permission the identity has over the given collection</returns>
        public void SetPermission(TType collection, TIdentity identity, ReadWritePermission permission)
        {
            SetKeys(collection);
            SetKeys(identity);

            // Remove the collection from the list, unless we're the owners, in that case, just set it to no permission.
            if (permission == ReadWritePermission.None && identity.Equals(collection.owner) == false)
            {
                _collectionToIdentity[collection].Remove(identity);
                _identityToCollection[identity].Remove(new PermissionModel<TType>(collection, ReadWritePermission.None));
            }
            else
            {
                _collectionToIdentity[collection].Add(identity);

                var c = _identityToCollection[identity];
                var model = new PermissionModel<TType>(collection, permission);
                
                // The equality operator ignores the permission on the PermissionModel object, however,
                // the HashSet<T> doesn't update a value if the equality operator considers the object equal.
                // Because of this the model is never set in the hashset; The only work around I could find was removing and re-adding.
                c.Remove(model);
                c.Add(model);
            }

            // Notify event listeners
            if (_eventHandlers.ContainsKey(identity))
            {
                foreach (var d in _eventHandlers[identity])
                {
                    d(new PermissionChangedResult<TType, TIdentity>(collection, identity, permission));
                }
            }
        }
        
        /// <summary>
        /// Get the the permission for this collection by a given identity.
        /// </summary>
        /// <returns>The permission the identity has over the given collection</returns>
        public ReadWritePermission GetPermission(TType collection, TIdentity identity)
        {
            if (_identityToCollection.ContainsKey(identity))
            {
                if (_identityToCollection[identity] == null)
                {
                    // The identity hashset is empty, no permission set.
                    return ReadWritePermission.None;
                }
                
                foreach (var collectionPermission in _identityToCollection[identity])
                {
                    if (EqualityComparer<TType>.Default.Equals(collectionPermission.obj, collection))
                    {
                        return collectionPermission.permission;
                    }
                }
            }

            // Permission not found; Assume the worst...
            return ReadWritePermission.None;
        }

//        public PermissionModel<TType> Get(TIdentity identity, System.Guid guid)
//        {
//            var cols = GetAllWithPermission(identity);
//            foreach (var col in cols)
//            {
//                if (col.obj.ID == guid)
//                {
//                    return col;
//                }
//            }
//            
//            return new PermissionModel<TType>(default(TType), ReadWritePermission.None);
//        }
        
        /// <summary>
        /// Get all collections this identity has permission to.
        /// </summary>
        public IEnumerable<PermissionModel<TType>> GetAllWithPermission(TIdentity identity)
        {
            if (_identityToCollection.ContainsKey(identity))
            {
                return _identityToCollection[identity];
            }

            return new PermissionModel<TType>[0];
        }

        /// <summary>
        /// Get all relevant identities (connection identifiers) for this collection. The relevant connections are listeners / some other way interested in this collection.
        /// </summary>
        public IEnumerable<TIdentity> GetAllIdentitiesWithPermission(TType collection)
        {
            var key = GetCollectionKey(collection);
            if (_collectionToIdentity.ContainsKey(key.obj))
            {
                return _collectionToIdentity[key.obj];
            }

            return new TIdentity[0];
        }
        
        /// <summary>
        /// Get all collections this identity has permission to.
        /// </summary>
        public IEnumerable<Tuple<PermissionModel<TType>, IEnumerable<TIdentity>>> GetAll()
        {
            foreach (var kvp in _identityToCollection)
            {
                foreach (var model in kvp.Value)
                {
                    yield return new Tuple<PermissionModel<TType>, IEnumerable<TIdentity>>(model, _collectionToIdentity[model.obj]);
                }
            }
        }
    }
}