using System;

using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public class PUN2ClientCollection<T> : PUN2CollectionBase<T>
        where T : class, IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        protected PUN2ActionsBridge actionBridge;

        public PUN2ClientCollection(PhotonView owner, PUN2ActionsBridge actionBridge, int slotCount, ILogger logger = null)
            : base(owner, slotCount, logger)
        {
            this.actionBridge = actionBridge;
        }

        public override Result<bool> SwapOrMerge(int fromIndex, ICollection toCollection, int toIndex, int mergeAmount, CollectionContext context = null)
        {
            var toCol = toCollection as IPUN2Collection;
            if (toCol == null)
            {
                logger.Warning("Given toCollection in swap is not a PUN2 compatible collection; Action can not be synced (action ignored)", this);
                return new Result<bool>(false, Errors.NetworkNoAuthority);
            }

            logger.LogVerbose($"[Client] Requesting swap of item from: {fromIndex} to {toIndex}");

            actionBridge.photonView.RPC(nameof(actionBridge.Cmd_RequestSwapOrMerge), PhotonNetwork.MasterClient, 
                /*collectionGuid:*/ ID.ToByteArray(),
                /*toCollectionGuid:*/ toCol.ID.ToByteArray(),
                /*fromIndex:*/ fromIndex,
                /*toIndex:*/ toIndex,
                /*amount:*/ mergeAmount
            );

            return true;
        }

        public override Result<bool> MoveAuto(int fromIndex, ICollection toCollection, int moveAmount, CollectionContext context = null)
        {
            var toCol = toCollection as IPUN2Collection;
            if (toCol == null)
            {
                logger.Warning("Given toCollection in move is not a PUN2 compatible collection; Action can not be synced (action ignored)", this);
                return new Result<bool>(false, Errors.NetworkNoAuthority);
            }

            logger.LogVerbose($"[Client] Requesting move of item from: {fromIndex} to auto. defined slot(s)");

            actionBridge.photonView.RPC(nameof(actionBridge.Cmd_RequestMoveAuto), PhotonNetwork.MasterClient, 
                /*collectionGuid:*/ ID.ToByteArray(),
                /*toCollectionGuid:*/ toCol.ID.ToByteArray(),
                /*fromIndex:*/ fromIndex,
                /*amount:*/ moveAmount
            );

            return true;
        }
    }
}