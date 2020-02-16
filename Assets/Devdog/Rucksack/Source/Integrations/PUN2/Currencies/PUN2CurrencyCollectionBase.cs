using Devdog.Rucksack.Collections;
using Photon.Pun;

namespace Devdog.Rucksack.Currencies
{
    public abstract class PUN2CurrencyCollectionBase : CurrencyCollection, IPUN2Collection
    {
        public System.Guid ID { get; set; }

        /// <summary>
        /// The owner of this collection
        /// </summary>
        public PhotonView owner { get; protected set; }
        protected readonly ILogger logger;

        public PUN2CurrencyCollectionBase(PhotonView owner, ILogger logger = null)
        {
            this.logger = logger ?? new UnityLogger("[PUN2][Collection] ");
            if (owner == null)
            {
                this.logger.Error("PUN2CurrencyCollectionBase ownerIdentity is null - PUN2ItemCollection needs an owner identity");
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
        }

        public void UnRegister()
        {
            CurrencyCollectionRegistry.byID.UnRegister(ID);
            CurrencyCollectionRegistry.byName.UnRegister(collectionName);
        }

        public void Server_UnRegister()
        {
            ServerCurrencyCollectionRegistry.byID.UnRegister(ID);
        }
    }
}
