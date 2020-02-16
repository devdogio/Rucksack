using System;
using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public abstract class PUN2CollectionBase<T> : Collection<T>, IPUN2Collection
        where T : class, IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        public System.Guid ID { get; set; }

        /// <summary>
        /// The owner of this collection
        /// </summary>
        public PhotonView owner { get; protected set; }

        public PUN2CollectionBase(PhotonView owner, int slotCount, ILogger logger = null)
            : base(slotCount, logger)
        {
            if (owner == null)
            {
                this.logger.Error("PUN2ItemCollection ownerIdentity is null - PUN2ItemCollection needs an owner identity");
            }

            this.owner = owner;
        }

        public override string ToString()
        {
            return $"{collectionName}({this.GetType().Name})";
        }

        public void Register()
        {
            CollectionRegistry.byID.Register(ID, this);
            CollectionRegistry.byName.Register(collectionName, this);
        }

        public void Server_Register()
        {
            ServerCollectionRegistry.byID.Register(ID, this);
        }

        public void UnRegister()
        {
            CollectionRegistry.byID.UnRegister(ID);
            CollectionRegistry.byName.UnRegister(collectionName);
        }

        public void Server_UnRegister()
        {
            ServerCollectionRegistry.byID.UnRegister(ID);
        }
    }
}