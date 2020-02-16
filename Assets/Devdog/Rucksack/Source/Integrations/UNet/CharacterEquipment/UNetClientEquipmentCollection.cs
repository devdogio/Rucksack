using System;
using Devdog.Rucksack.CharacterEquipment;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections.CharacterEquipment
{
    public class UNetClientEquipmentCollection<TEquippableType> : EquipmentCollection<TEquippableType>, IUNetCollection 
        where TEquippableType : class, IEquatable<TEquippableType>, ICloneable, IEquippable<TEquippableType>, ICollectionSlotEntry, IIdentifiable, IStackable
    {
        public UNetActionsBridge actionBridge { get; set; }
        public Guid ID { get; set; }
        public NetworkIdentity owner { get; }
        
        public UNetClientEquipmentCollection(NetworkIdentity owner, UNetActionsBridge actionBridge, int slotCount, IEquippableCharacter<TEquippableType> character, ILogger logger = null)
            : base(slotCount, character, logger)
        {
            this.actionBridge = actionBridge;
            this.owner = owner;
        }
        
        public void Register()
        {
            CollectionRegistry.byID.Register(ID, this);
            CollectionRegistry.byName.Register(collectionName, this);
        }

        public void Server_Register()
        {
            // Ignored
        }

        public void UnRegister()
        {
            CollectionRegistry.byID.UnRegister(ID);
            CollectionRegistry.byName.UnRegister(collectionName);
        }

        public void Server_UnRegister()
        {
            // Ignored
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

        public override string ToString()
        {
            return collectionName;
        }
    }
}