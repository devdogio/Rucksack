using Devdog.Rucksack.Collections;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public abstract class UNetCurrencyCollectionBase : CurrencyCollection, IUNetCollection
    {
        public System.Guid ID { get; set; }

        /// <summary>
        /// The owner of this collection
        /// </summary>
        public NetworkIdentity owner { get; protected set; }
        protected readonly ILogger logger;

        public UNetCurrencyCollectionBase(NetworkIdentity owner, ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[UNet][Collection] ");
            if (owner == null)
            {
                this.logger.Error("UNetCurrencyCollectionBase ownerIdentity is null - UNetItemCollection needs an owner identity");
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
            CurrencyCollectionRegistry.byID.Register(ID, this);
            CurrencyCollectionRegistry.byName.Register(collectionName, this);
        }

        public void Server_Register()
        {
            ServerCurrencyCollectionRegistry.byID.Register(ID, this);
//            ServerCurrencyCollectionRegistry.byName.Register(collectionName, this);
        }

        public void UnRegister()
        {
            CurrencyCollectionRegistry.byID.UnRegister(ID);
            CurrencyCollectionRegistry.byName.UnRegister(collectionName);
        }

        public void Server_UnRegister()
        {
            ServerCurrencyCollectionRegistry.byID.UnRegister(ID);
//            ServerCurrencyCollectionRegistry.byName.UnRegister(collectionName);
        }
    }
}