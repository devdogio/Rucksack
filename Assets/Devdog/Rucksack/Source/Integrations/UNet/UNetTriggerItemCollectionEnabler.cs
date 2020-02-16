using System.Linq;
using Devdog.General2;
using Devdog.Rucksack.Items;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Enable read/write permission on a collection when a trigger is used and remove permission when the trigger is un-used.
    /// </summary>
    public sealed class UNetTriggerItemCollectionEnabler : NetworkBehaviour, ITriggerCallbacks
    {
        [SerializeField]
        private string _collectionName;

        [SerializeField]
        private ReadWritePermission _permissionOnUse = ReadWritePermission.ReadWrite;

        [SerializeField]
        private ReadWritePermission _permissionOnUnUse = ReadWritePermission.None;

        private NetworkIdentity _identity;
//        private readonly ILogger _logger;
        public UNetTriggerItemCollectionEnabler()
        {
//            _logger = new UnityLogger("[UNet][Collection] ");
        }

        private void Awake()
        {
            _identity = GetComponent<NetworkIdentity>();
        }

        [ServerCallback]
        public void OnTriggerUsed(Character character, TriggerEventData data)
        {
            var bridge = character.GetComponent<UNetActionsBridge>();
            if (character is Player && bridge != null)
            {
                var col = GetCollection(bridge);
                if (col != null)
                {
                    // NOTE: If the player isn't the owner of the collection we need to make sure it gets registered on the client.
                    // NOTE: If the player IS the owner of the collection it should always be managed by the player, and should be registered on the client at all times.
                    if (bridge.identity != col.owner)
                    {
                        // Make sure the collection exists both on the server and client.
                        bridge.Server_AddCollectionToServerAndClient(new AddCollectionMessage()
                        {
                            collectionName = _collectionName,
                            collectionGuid = col.ID,
                            owner = col.owner,
                            slotCount = (ushort)col.slotCount,
                        });
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
                    
                    bridge.Server_SetCollectionContentsOnClient(new SetCollectionContentsMessage()
                    {
                        collectionName = _collectionName,
                        collectionGuid = col.ID,
                        items = itemsArray,
                    });
                    
                    bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                    {
                        collectionGuid = col.ID,
                        permission = _permissionOnUse
                    });
                }
            }
        }

        [ServerCallback]
        public void OnTriggerUnUsed(Character character, TriggerEventData data)
        {
            var bridge = character.GetComponent<UNetActionsBridge>();
            if (character is Player && bridge != null)
            {
                var col = GetCollection(bridge);
                Assert.IsNotNull(col, "Collection not found on object or player!");
                bridge.Server_SetCollectionPermissionOnServerAndClient(new SetCollectionPermissionMessage()
                {
                    collectionGuid = col.ID,
                    permission = _permissionOnUnUse
                });
            }
        }

        private UNetCollectionBase<IItemInstance> GetCollection(UNetActionsBridge bridge)
        {
            // The collection belongs to this identity
            var cols = UNetPermissionsRegistry.collections.GetAllWithPermission(_identity);
            var triggerCollection = cols.FirstOrDefault(o => o.obj.collectionName == _collectionName);
            if (triggerCollection.obj != null)
            {
                // NOTE: The collection belongs to this object's network identity.
                // NOTE: We're handling the lifetime of the object and handle registration of the collection on clients.
                return triggerCollection.obj as UNetCollectionBase<IItemInstance>;
            }

            // The collection belongs to the player using this trigger
            var cols2 = UNetPermissionsRegistry.collections.GetAllWithPermission(bridge.identity);
            var playerCollection = cols2.FirstOrDefault(o => o.obj.collectionName == _collectionName);
            return playerCollection.obj as UNetCollectionBase<IItemInstance>;
        }
    }
}