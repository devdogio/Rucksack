using Devdog.General2;
using Devdog.Rucksack.Collections;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.Items
{
    public static class PUN2ItemUtility
    {
        private static readonly ILogger _logger;
        static PUN2ItemUtility()
        {
            _logger = new UnityLogger("[PUN2][Item] ");
        }


        public static Result<ItemUsedResult> UseItem(INetworkItemInstance item, Character character, ItemContext useContext)
        {
            var bridge = character.GetComponent<PUN2ActionsBridge>();
            if (bridge != null && bridge.photonView != null)
            {
                if (bridge.photonView.IsMine /*!bridge.photonView.IsServer() /*net.isClient*/)
                {
                    _logger.LogVerbose($"[Client] Trying to use item with guid {item.ID}");

                    bridge.photonView.RPC(nameof(bridge.Cmd_RequestUseItem), PhotonNetwork.MasterClient,
                        /*itemGuid: */ item.ID.ToByteArray(),
                        /*useAmount: */ useContext.useAmount,
                        /*targetIndex: */ useContext.targetIndex
                    );

                    return new Result<ItemUsedResult>(new ItemUsedResult(useContext.useAmount, false, 0f)); // TODO: Set cooldown time.
                }
            }

            _logger.Warning($"UseItem() couldn't find {nameof(PhotonView)} or {nameof(PUN2ActionsBridge)} on player object;", item);
            return new Result<ItemUsedResult>(null, Errors.NetworkNoAuthority);
        }

        public static Result<GameObject> DropItem(INetworkItemInstance item, Character character, Vector3 worldPosition)
        {
            var bridge = character.GetComponent<PUN2ActionsBridge>();
            if (bridge != null && bridge.photonView != null)
            {
                if (bridge.photonView.IsMine /*!bridge.photonView.IsServer() /*net.isClient*/)
                {
                    bridge.photonView.RPC(nameof(bridge.Cmd_RequestDropItem), PhotonNetwork.MasterClient,
                        /*itemGuid: */ item.ID.ToByteArray(),
                        /*worldPosition: */ worldPosition
                    );

                    return new Result<GameObject>(null);
                }
            }

            _logger.Warning($"DropItem() couldn't find {nameof(PhotonView)} or {nameof(PUN2ActionsBridge)} on player object;", item);
            return new Result<GameObject>(null, Errors.NetworkNoAuthority);
        }
    }
}