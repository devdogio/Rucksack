using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Photon.Pun;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Database;

namespace Devdog.Rucksack.CharacterEquipment
{
    public sealed class PUN2EquipmentReplicator
    {
        //private readonly ILogger logger;
        private readonly PUN2ActionsBridge bridge;

        //private static PUN2EquipmentInputValidator inputValidator;
        //static PUN2EquipmentReplicator()
        //{
        //    inputValidator = new PUN2EquipmentInputValidator(new UnityLogger("[PUN2][Validation] "));
        //}

        public PUN2EquipmentReplicator(PUN2ActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            //this.logger = logger ?? new UnityLogger("[PUN2] ");
        }

        public void TargetRpc_AddEquipmentCollection(PhotonView owner, string collectionName, Guid collectionGuid, EquipmentCollectionSlot<IEquippableItemInstance>[] slots)
        {
            var collection = PUN2ActionsBridge.collectionFinder.GetClientCollection(collectionGuid);
            if (collection == null)
            {
                var equippableCharacter = bridge.GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
                var restoreItemsToGroup = bridge.GetComponent<IInventoryCollectionOwner>().itemCollectionGroup;
                /*collection = */PUN2CollectionUtility.CreateClientEquipmentCollection(collectionName, collectionGuid, owner, bridge, slots, equippableCharacter);
            }
        }
    }
}
