using System;
using System.Linq;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections.CharacterEquipment;
using Photon.Pun;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    public sealed class PUN2EquipmentCollectionCreator : MonoBehaviourPun
    {
        [SerializeField]
        private string _collectionName;

        public string CollectionName { get { return this._collectionName; } }

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

        public PUN2ServerEquipmentCollection<IEquippableItemInstance> collection { get; set; }

        private readonly ILogger _logger;
        public PUN2EquipmentCollectionCreator()
        {
            _logger = new UnityLogger("[PUN2][Collection] ");
        }

        private void Start()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
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

        private void Server_CreateCollection()
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            _logger.Log($"PUN2EquipmentCollectionCreator - Server_CreateCollection ViewId: {this.photonView.ViewID}, _isPlayerCollection: {_isPlayerCollection}", this);
            if (_isPlayerCollection)
            {
                var bridge = GetComponent<PUN2ActionsBridge>();
                if (bridge == null)
                {
                    _logger.Error($"Trying to sync collection to client, but no {nameof(PUN2ActionsBridge)} found on object!", this);
                    return;
                }

                var guid = System.Guid.NewGuid();

                collection = bridge.Server_AddEquipmentCollectionToServerAndClient(
                    //owner: this.photonView,
                    collectionName: _collectionName,
                    collectionGuid: guid,
                    slots: _slots//.Select(o => string.Join(":", o.equipmentTypes.Select(j => j.ID.ToString() )) ).ToArray()
                );
                
                bridge.Server_SetCollectionPermissionOnServerAndClient(
                    collectionGuid: guid,
                    permission: _permission
                );
            }
            else
            {
                var equippableCharacter = GetComponent<IEquippableCharacter<IEquippableItemInstance>>();                
                collection = PUN2CollectionUtility.CreateServerEquipmentCollection(_collectionName, System.Guid.NewGuid(), this.photonView, _slots.Select(o => o.ToSlotInstance(equippableCharacter)).ToArray(), equippableCharacter);
            }
        }
    }
}
