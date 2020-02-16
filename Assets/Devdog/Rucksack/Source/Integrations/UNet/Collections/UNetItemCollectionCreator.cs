using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    public sealed class UNetItemCollectionCreator : NetworkBehaviour
    {
        [SerializeField]
        private string _collectionName;
        public string collectionName
        {
            get { return _collectionName; }
        }

        [SerializeField]
        private int _slotCount;
        public int slotCount
        {
            get { return _slotCount; }
        }

        /// <summary>
        /// Should this collection be synced to newly connected players?
        /// NOTE: Only use this if you want the player to have initial collections or always relevant collections...
        /// </summary>
        [SerializeField]
        private bool _isPlayerCollection = false;
        
        [SerializeField]
        private ReadWritePermission _permission = ReadWritePermission.Read;
        
        public IUNetCollection collection { get; private set; }


        private NetworkIdentity _identity;
        private readonly ILogger _logger;
        public UNetItemCollectionCreator()
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
        
//        public override void OnStartServer()
//        {
//            TargetRpc_SomeMessage(connectionToClient);
//            CreateCollection();
//        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_isPlayerCollection)
            {
                if (GetComponent<Devdog.General2.Player>() == null)
                {
                    _logger.Error($"{nameof(_isPlayerCollection)} can only be used on player objects.", this);
                    _isPlayerCollection = false;
                }
            }
        }
#endif

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
                collection = bridge.Server_AddCollectionToServerAndClient(new AddCollectionMessage()
                {
                    owner = _identity,
                    collectionName = _collectionName,
                    collectionGuid = guid,
                    slotCount = (ushort)slotCount,
                });
                
                bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                {
                    collectionGuid = guid,
                    permission = _permission
                });
            }
            else
            {
                collection = UNetCollectionUtility.CreateServerItemCollection(_slotCount, _collectionName, System.Guid.NewGuid(), _identity);
            }
        }
    }
}