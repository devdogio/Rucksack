using Devdog.General2;
using Devdog.Rucksack.Collections;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.Currencies
{
    public sealed class PUN2CurrencyCollectionCreator : MonoBehaviourPun
    {
        [SerializeField]
        private string _collectionName;

        /// <summary>
        /// Should this collection be synced to newly connected players?
        /// NOTE: Only use this if you want the player to have initial collections or always relevant collections...
        /// </summary>
        [SerializeField]
        private bool _isPlayerCollection = false;

        [SerializeField]
        private ReadWritePermission _permission = ReadWritePermission.Read;

        public PUN2CurrencyCollectionBase collection { get; private set; }

        private readonly ILogger _logger;
        public PUN2CurrencyCollectionCreator()
        {
            _logger = new UnityLogger("[PUN2][Collection] ");
        }

        private void Start()
        {
            Server_CreateCollection();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_isPlayerCollection)
            {
                if (GetComponent<Player>() == null)
                {
                    _logger.Error($"{nameof(_isPlayerCollection)} can only be used on player objects.", this);
                    _isPlayerCollection = false;
                }
            }
        }
#endif

        private void Server_CreateCollection()
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            if (_isPlayerCollection)
            {
                var bridge = GetComponent<PUN2ActionsBridge>();
                if (bridge == null)
                {
                    _logger.Error($"Trying to sync collection to client, but no {nameof(PUN2ActionsBridge)} found on object!", this);
                    return;
                }

                var guid = System.Guid.NewGuid();
                bridge.Server_AddCurrencyCollectionToServerAndClient(
                    //owner: this.photonView,
                    collectionName: _collectionName,
                    collectionGuid: guid
                );

                bridge.Server_SetCollectionPermissionOnServerAndClient(
                    collectionGuid: guid,
                    permission: _permission
                );
            }
            else
            {
                collection = PUN2CurrencyCollectionUtility.CreateServerCurrencyCollection(_collectionName, System.Guid.NewGuid(), this.photonView);
            }
        }
    }
}