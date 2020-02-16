using System;
using Devdog.Rucksack.CharacterEquipment;
using Photon.Pun;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections.CharacterEquipment
{
    public class PUN2ClientEquipmentCollection<TEquippableType> : EquipmentCollection<TEquippableType>, IPUN2Collection
        where TEquippableType : class, IEquatable<TEquippableType>, ICloneable, IEquippable<TEquippableType>, ICollectionSlotEntry, IIdentifiable, IStackable
    {
        public PUN2ActionsBridge actionBridge { get; set; }
        public Guid ID { get; set; }
        public PhotonView owner { get; }

        public PUN2ClientEquipmentCollection(PhotonView owner, PUN2ActionsBridge actionBridge, int slotCount, IEquippableCharacter<TEquippableType> character, ILogger logger = null)
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

        public override Result<CollectionAddResult> Add(TEquippableType item, int amount, CollectionContext context)
        {
            var result = base.Add(item, amount, context);

            logger.Log($"Item {item?.ID} added to PUN2ClientEquipmentCollection with result {result}", this);

            return result;
        }

        public override Result<bool> Set(int index, TEquippableType item, int amount, CollectionContext context)
        {
            var result = base.Set(index, item, amount, context);

            logger.Log($"Item {item?.ID} Set on PUN2ClientEquipmentCollection with result {result}", this);

            return result;
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
                /*collectionGuid: */ ID.ToByteArray(),
                /*toCollectionGuid: */ toCol.ID.ToByteArray(),
                /*fromIndex: */ fromIndex,
                /*toIndex: */ toIndex,
                /*amount: */ mergeAmount
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
                /*collectionGuid: */ ID.ToByteArray(),
                /*toCollectionGuid: */ toCol.ID.ToByteArray(),
                /*fromIndex: */ fromIndex,
                /*amount: */ moveAmount
            );

            return true;
        }

        public override string ToString()
        {
            return collectionName;
        }
    }
}