using System.Linq;
using System.Runtime.CompilerServices;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using UnityEngine.Networking;

namespace Devdog.Rucksack.CharacterEquipment
{
    public class UNetEquipmentReplicator
    {
        protected readonly ILogger logger;
        protected readonly UNetActionsBridge bridge;

        protected static UNetEquipmentInputValidator inputValidator;
        static UNetEquipmentReplicator()
        {
            inputValidator = new UNetEquipmentInputValidator(new UnityLogger("[UNet][Validation] "));
        }
        
        public UNetEquipmentReplicator(UNetActionsBridge bridge, ILogger logger = null)
        {
            this.bridge = bridge;
            this.logger = logger ?? new UnityLogger("[UNet] ");
        }
        
        public void TargetRpc_AddEquipmentCollection(NetworkConnection target, AddEquipmentCollectionMessage data)
        {
            var collection = UNetActionsBridge.collectionFinder.GetClientCollection(data.collectionGuid);
            if (collection == null)
            {
                var equippableCharacter = bridge.GetComponent<IEquippableCharacter<IEquippableItemInstance>>();
                var restoreItemsToGroup = bridge.GetComponent<IInventoryCollectionOwner>().itemCollectionGroup;
                collection = UNetCollectionUtility.CreateClientEquipmentCollection(data.collectionName, data.collectionGuid, data.owner, bridge, data.slots.Select(o => o.ToSlotInstance(bridge.equipmentTypeDatabase, equippableCharacter)).ToArray(), equippableCharacter);
            }
        }
        
        
        protected void HandleError(Error error, [CallerMemberName] string name = "")
        {
            if (error != null)
            {
                // TODO: Send message back to client about failed action...
                logger.Error($"Player action '{name}' failed: ", error, bridge.player);
            }
        }
    }
}