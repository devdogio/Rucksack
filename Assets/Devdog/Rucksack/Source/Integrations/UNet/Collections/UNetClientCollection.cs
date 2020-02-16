using System;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public class UNetClientCollection<T> : UNetCollectionBase<T>
        where T : class, IEquatable<T>, IStackable, IIdentifiable, ICloneable
    {
        protected UNetActionsBridge actionBridge;
        
        public UNetClientCollection(NetworkIdentity owner, UNetActionsBridge actionBridge, int slotCount, ILogger logger = null)
            : base(owner, slotCount, logger)
        {
            this.actionBridge = actionBridge;
        }

        public override Result<bool> SwapOrMerge(int fromIndex, ICollection toCollection, int toIndex, int mergeAmount, CollectionContext context = null)
        {
            var toCol = toCollection as IUNetCollection;
            if (toCol == null)
            {
                logger.Warning("Given toCollection in swap is not a UNet compatible collection; Action can not be synced (action ignored)", this);
                return new Result<bool>(false, Errors.NetworkNoAuthority);
            }
            
            logger.LogVerbose($"[Client] Requesting swap of item from: {fromIndex} to {toIndex}");
            actionBridge.Cmd_RequestSwapOrMerge(new RequestSwapOrMergeMessage()
            {
                collectionGuid = ID,
                toCollectionGuid = toCol.ID,
                fromIndex = (ushort)fromIndex,
                toIndex = (ushort)toIndex,
                amount = (short)mergeAmount
            });
            
            return true;
        }
        
        public override Result<bool> MoveAuto(int fromIndex, ICollection toCollection, int moveAmount, CollectionContext context = null)
        {
            var toCol = toCollection as IUNetCollection;
            if (toCol == null)
            {
                logger.Warning("Given toCollection in move is not a UNet compatible collection; Action can not be synced (action ignored)", this);
                return new Result<bool>(false, Errors.NetworkNoAuthority);
            }
            
            logger.LogVerbose($"[Client] Requesting move of item from: {fromIndex} to auto. defined slot(s)");
            actionBridge.Cmd_RequestMoveAuto(new RequestMoveAutoMessage()
            {
                collectionGuid = ID,
                toCollectionGuid = toCol.ID,
                fromIndex = (ushort)fromIndex,
                amount = (ushort)moveAmount
            });
            
            return true;
        }
    }
}