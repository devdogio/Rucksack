using Devdog.General2;
using Devdog.Rucksack.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Items
{
    public static class UNetItemUtility
    {
        private static readonly ILogger _logger;
        static UNetItemUtility()
        {
            _logger = new UnityLogger("[UNet][Item] ");
        }
        
        
        public static Result<ItemUsedResult> UseItem(INetworkItemInstance item, Character character, ItemContext useContext)
        {
            var net = character.GetComponent<NetworkIdentity>();
            var bridge = character.GetComponent<UNetActionsBridge>();
            if (net != null && bridge != null)
            {
                if (net.isClient)
                {
                    _logger.LogVerbose($"[Client] Trying to use item with guid {item.ID}");
                    
                    bridge.Cmd_RequestUseItem(new RequestUseItemMessage()
                    {
                        itemGuid = item.ID,
                        useAmount = (ushort)useContext.useAmount,
                        targetIndex = (short)useContext.targetIndex
                    });
                    
                    return new Result<ItemUsedResult>(new ItemUsedResult(useContext.useAmount, false, 0f)); // TODO: Set cooldown time.
                }
            }

            _logger.Warning($"UseItem() couldn't find {nameof(NetworkIdentity)} or {nameof(UNetActionsBridge)} on player object;", item);
            return new Result<ItemUsedResult>(null, Errors.NetworkNoAuthority);
        }
        
        public static Result<GameObject> DropItem(INetworkItemInstance item, Character character, Vector3 worldPosition)
        {
            var net = character.GetComponent<NetworkIdentity>();
            var bridge = character.GetComponent<UNetActionsBridge>();
            if (net != null && bridge != null)
            {
                if (net.isClient)
                {
                    bridge.Cmd_RequestDropItem(new RequestDropItemMessage()
                    {
                        itemGuid = item.ID,
                        worldPosition = worldPosition,
                    });

                    return new Result<GameObject>(null);
                }
            }

            _logger.Warning($"DropItem() couldn't find {nameof(NetworkIdentity)} or {nameof(UNetActionsBridge)} on player object;", item);
            return new Result<GameObject>(null, Errors.NetworkNoAuthority);
        }
    }
}