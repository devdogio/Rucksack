using System;
using UnityEngine;
using Devdog.General2;
using Devdog.Rucksack.Database;
using Devdog.Rucksack.Items;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RegisterItemInstanceMessage : MessageBase
    {
        public GuidMessage itemGuid;
        public GuidMessage itemDefinitionGuid;
        public string serializedData;

        public RegisterItemInstanceMessage()
            : this(null)
        {

        }

        public RegisterItemInstanceMessage(IItemInstance itemInstance)
        {
            if (itemInstance != null)
            {
                itemGuid = itemInstance.ID;
                itemDefinitionGuid = itemInstance.itemDefinition.ID;
                serializedData = ""; // TODO: Set Serializable data
            }
            else
            {
                itemGuid = Guid.Empty;
                itemDefinitionGuid = Guid.Empty;
                serializedData = "";
            }
        }
        
        public IItemInstance TryCreateItemInstance(IDatabase<UnityItemDefinition> database, ILogger logger = null)
        {
            logger = logger ?? new NullLogger();

            logger.LogVerbose($"[Client] Server sent a new item instance with guid:\n {itemGuid.guid}\n and item definition guid:\n {itemDefinitionGuid}");
            var itemDef = database.Get(new Identifier(itemDefinitionGuid.guid));
            if (itemDef.error != null)
            {
                logger.Log($"ItemDefinition with guid {itemDefinitionGuid} not found on local client!", itemDef.error);
                return null;
            }

            var instance = ItemFactory.CreateInstance(itemDef.result, itemGuid.guid);
            if (string.IsNullOrEmpty(serializedData) == false)
            {
                JsonUtility.FromJsonOverwrite(serializedData, instance);
            }
            
            return instance;
        }
        
        public override string ToString()
        {
            return $"RegisterItemInstanceMessage - {serializedData}";
        }
    }
}