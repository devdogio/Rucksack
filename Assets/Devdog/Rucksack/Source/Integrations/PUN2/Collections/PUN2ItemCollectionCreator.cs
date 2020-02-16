using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public sealed class PUN2ItemCollectionCreator : MonoBehaviourPun
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

        public IPUN2Collection collection { get; private set; }

        private readonly ILogger _logger;
        public PUN2ItemCollectionCreator()
        {
            _logger = new UnityLogger("[PUN2][Collection] ");
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

        private void Start()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                Server_CreateCollection();
            }
        }

        private void Server_CreateCollection()
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            _logger.Log($"UNetItemCollectionCreator - Server_CreateCollection viewId: {this.photonView.ViewID}, _isPlayerCollection: {_isPlayerCollection}", this);

            if (_isPlayerCollection)
            {
                var bridge = GetComponent<PUN2ActionsBridge>();
                if (bridge == null)
                {
                    _logger.Error($"Trying to sync collection to client, but no {nameof(PUN2ActionsBridge)} found on object!", this);
                    return;
                }

                var guid = System.Guid.NewGuid();

                collection = bridge.Server_AddCollectionToServerAndClient(collectionName: _collectionName, collectionGuid: guid, slotCount: slotCount);
                bridge.Server_SetCollectionPermissionOnServerAndClient(collectionGuid: guid, permission: _permission);
            }
            else
            {
                collection = PUN2CollectionUtility.CreateServerItemCollection(_slotCount, _collectionName, System.Guid.NewGuid(), this.photonView);
            }
        }
    }
}
