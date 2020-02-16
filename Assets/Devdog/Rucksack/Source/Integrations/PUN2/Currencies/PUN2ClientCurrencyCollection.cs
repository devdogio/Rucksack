using Photon.Pun;

namespace Devdog.Rucksack.Currencies
{
    public class PUN2ClientCurrencyCollection : PUN2CurrencyCollectionBase
    {
        protected PUN2ActionsBridge actionsBridge;

        public PUN2ClientCurrencyCollection(PhotonView owner, PUN2ActionsBridge actionsBridge, ILogger logger = null)
            : base(owner, logger)
        {
            this.actionsBridge = actionsBridge;
        }
    }
}