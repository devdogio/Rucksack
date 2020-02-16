using Devdog.General2;
using Devdog.Rucksack.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Currencies
{
    public sealed class UNetCurrencyCollectionCreator : NetworkBehaviour
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
        
        public UNetCurrencyCollectionBase collection { get; private set; }

        private NetworkIdentity _identity;
        private readonly ILogger _logger;
        public UNetCurrencyCollectionCreator()
        {
            _logger = new UnityLogger("[UNet][Collection] ");
        }

        private void Awake()
        {
            _identity = GetComponent<NetworkIdentity>();
        }

        // NOTE: All RPC messages from OnStartServer seem to fail -> Start with [ServerCallback] attribute does work...?
        [ServerCallback]
        private void Start()
        {
            Server_CreateCollection();
        }

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

        [Server]
        private void Server_CreateCollection()
        {
            if (_isPlayerCollection)
            {
                var bridge = GetComponent<UNetActionsBridge>();
                if (bridge == null)
                {
                    _logger.Error($"Trying to sync collection to client, but no {nameof(UNetActionsBridge)} found on object!", this);
                    return;
                }

                var guid = System.Guid.NewGuid();
                bridge.Server_AddCurrencyCollectionToServerAndClient(new AddCurrencyCollectionMessage()
                {
                    owner = _identity,
                    collectionName = _collectionName,
                    collectionGuid = guid,
                });
                
                bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                {
                    collectionGuid = guid,
                    permission = _permission
                });
            }
            else
            {
                collection = UNetCurrencyCollectionUtility.CreateServerCurrencyCollection(_collectionName, System.Guid.NewGuid(), _identity);
            }
        }
    }
}