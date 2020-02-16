using Devdog.General2;
using Devdog.Rucksack.Items;
using Photon.Pun;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Devdog.General2.Serialization;
using UnityEngine.Assertions;
using Devdog.Rucksack.UI;
using System;

namespace Devdog.Rucksack.Collections
{
    public class PUN2TriggerItemCollectionEnabler : MonoBehaviourPun, ITriggerCallbacks
    {
        [SerializeField]
        private string _collectionName;

        public string CollectionName { get { return this._collectionName; } }

        [SerializeField]
        private ReadWritePermission _permissionOnUse = ReadWritePermission.ReadWrite;

        [SerializeField]
        private ReadWritePermission _permissionOnUnUse = ReadWritePermission.None;

        private ItemCollectionUI _currentItemCollectionUI;

        public void OnTriggerUsed(Character character, TriggerEventData data)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            var bridge = character.GetComponent<PUN2ActionsBridge>();
            if (character is Player && bridge != null)
            {
                var col = GetCollection(bridge);
                if (col != null)
                {
                    // NOTE: If the player isn't the owner of the collection we need to make sure it gets registered on the client.
                    // NOTE: If the player IS the owner of the collection it should always be managed by the player, and should be registered on the client at all times.
                    if (bridge.photonView != col.owner)
                    {
                        // Make sure the collection exists both on the server and client.
                        bridge.Server_AddCollectionToServerAndClient(
                            collectionName: _collectionName,
                            collectionGuid: col.ID,
                            slotCount: col.slotCount
                        );
                    }

                    var itemsArray = new ItemAmountMessage[col.slotCount];
                    for (int i = 0; i < itemsArray.Length; i++)
                    {
                        itemsArray[i] = new ItemAmountMessage()
                        {
                            itemInstance = new RegisterItemInstanceMessage(col[i]),
                            amount = (ushort)col.GetAmount(i)
                        };
                    }
                    
                    bridge.Server_SetCollectionContentsOnClient(
                        collectionName: _collectionName,
                        collectionGuid: col.ID,
                        items: itemsArray
                    );
                    
                    bridge.Server_SetCollectionPermissionOnServerAndClient(
                        collectionGuid: col.ID,
                        permission: _permissionOnUse
                    );

                    bridge.photonView.RPC(nameof(bridge.TargetRPC_ShowItemCollectionUI), bridge.photonView.Owner, this._collectionName, col.ID.ToByteArray());
                }
            }
        }

        public void RPC_ShowUI(Guid collectionGuid)
        {
            var col = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid);

            if (col != null)
            {
                var uis = FindObjectsOfType<UI.ItemCollectionUI>();
                foreach (var ui in uis)
                {
                    if (ui.collectionName == col.collectionName)
                    {
                        _currentItemCollectionUI = ui;

                        _currentItemCollectionUI.collection = (ICollection<IItemInstance>)col;
                        _currentItemCollectionUI.window.Show();
                        return;
                    }
                }

                new UnityLogger("[Collection] ").Warning("Couldn't find ItemCollectionUI that repaints collection for collection with name: " + col.ToString());
            }
        }

        public void OnTriggerUnUsed(Character character, TriggerEventData data)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var bridge = character.GetComponent<PUN2ActionsBridge>();
                if (character is Player && bridge != null)
                {
                    var col = GetCollection(bridge);

                    Assert.IsNotNull(col, "Collection not found on object or player!");

                    bridge.Server_SetCollectionPermissionOnServerAndClient(
                        collectionGuid: col.ID,
                        permission: _permissionOnUnUse
                    );
                }
            }
            else if (character == PlayerManager.currentPlayer)
            {
                _currentItemCollectionUI?.window.Hide();
            }
        }

        private PUN2CollectionBase<IItemInstance> GetCollection(PUN2ActionsBridge bridge)
        {
            // The collection belongs to this identity
            var cols = PUN2PermissionsRegistry.collections.GetAllWithPermission(this.photonView);
            var triggerCollection = cols.FirstOrDefault(o => o.obj.collectionName == _collectionName);
            if (triggerCollection.obj != null)
            {
                // NOTE: The collection belongs to this object's network identity.
                // NOTE: We're handling the lifetime of the object and handle registration of the collection on clients.
                return triggerCollection.obj as PUN2CollectionBase<IItemInstance>;
            }
        
            // The collection belongs to the player using this trigger
            var cols2 = PUN2PermissionsRegistry.collections.GetAllWithPermission(bridge.photonView);
            var playerCollection = cols2.FirstOrDefault(o => o.obj.collectionName == _collectionName);
            return playerCollection.obj as PUN2CollectionBase<IItemInstance>;
        }
    }
}