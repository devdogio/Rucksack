using System;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public abstract class UNetCollectionBase<T> : Collection<T>, IUNetCollection
        where T : class, IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        public System.Guid ID { get; set; }

        /// <summary>
        /// The owner of this collection
        /// </summary>
        public NetworkIdentity owner { get; protected set; }

        public UNetCollectionBase(NetworkIdentity owner, int slotCount, ILogger logger = null)
            : base(slotCount, logger)
        {
            if (owner == null)
            {
                this.logger.Error("UNetItemCollection ownerIdentity is null - UNetItemCollection needs an owner identity");
//                throw new ArgumentException("UNetItemCollection ownerIdentity is null - UNetItemCollection needs an owner identity");
            }

            this.owner = owner;
        }

        public override string ToString()
        {
            return collectionName;
        }
        
        public void Register()
        {
            CollectionRegistry.byID.Register(ID, this);
            CollectionRegistry.byName.Register(collectionName, this);
        }

        public void Server_Register()
        {
            ServerCollectionRegistry.byID.Register(ID, this);
//            ServerCollectionRegistry.byName.Register(collectionName, this);
        }

        public void UnRegister()
        {
            CollectionRegistry.byID.UnRegister(ID);
            CollectionRegistry.byName.UnRegister(collectionName);
        }

        public void Server_UnRegister()
        {
            ServerCollectionRegistry.byID.UnRegister(ID);
//            ServerCollectionRegistry.byName.UnRegister(collectionName);
        }
    }
}