using System;
using System.Linq;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections.CharacterEquipment;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    public sealed class UNetEquipmentCollectionCreator : NetworkBehaviour
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
        private UnitySerializedEquipmentCollectionSlot[] _slots = new UnitySerializedEquipmentCollectionSlot[0];
        public UnitySerializedEquipmentCollectionSlot[] slots
        {
            get { return _slots; }
            set { _slots = value; }
        }

        [SerializeField]
        private ReadWritePermission _permission = ReadWritePermission.Read;
        
        public UNetServerEquipmentCollection<IEquippableItemInstance> collection { get; private set; }

        private NetworkIdentity _identity;
        private readonly ILogger _logger;
        public UNetEquipmentCollectionCreator()
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
                collection = bridge.Server_AddEquipmentCollectionToServerAndClient(new AddEquipmentCollectionMessage()
                {
                    owner = _identity,
                    collectionName = _collectionName,
                    collectionGuid = guid,
                    slots = _slots.Select(o => new EquipmentSlotDataMessage()
                    {
                        equipmentTypeGuids = o.equipmentTypes.Select(j => new GuidMessage(){ bytes = j.ID.ToByteArray() }).ToArray(),
                    }).ToArray()
                });
                
                bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                {
                    collectionGuid = guid,
                    permission = _permission
                });
            }
            else
            {
                var equippableCharacter = GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
//                var restoreItemsToGroup = GetComponent<IInventoryCollectionOwner>().itemCollectionGroup;
                collection = UNetCollectionUtility.CreateServerEquipmentCollection(_collectionName, System.Guid.NewGuid(), _identity, _slots.Select(o => o.ToSlotInstance(equippableCharacter)).ToArray(), equippableCharacter);
            }
        }
    }
}