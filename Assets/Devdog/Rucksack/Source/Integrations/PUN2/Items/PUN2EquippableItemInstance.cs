using System;
using Devdog.General2;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    public class PUN2EquippableItemInstance : UnityEquippableItemInstance, INetworkItemInstance
    {
        // Used for (de) serialization
        public PUN2EquippableItemInstance()
        { }

        public PUN2EquippableItemInstance(Guid ID, IUnityEquippableItemDefinition itemDefinition)
            : base(ID, itemDefinition)
        {
            logger = new UnityLogger("[PUN2][Item] ");
        }

        public override Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            return PUN2ItemUtility.UseItem(this, character, useContext);
        }

        public Result<ItemUsedResult> Server_Use(Character character, ItemContext useContext)
        {
            var useResult = base.DoUse(character, useContext);
            if (useResult.error == null)
            {
                if (collectionEntry != null)
                {
                    if (useResult.result.reduceStackSize)
                    {
                        var success = collectionEntry.SetAmountAndUpdateCollection(collectionEntry.amount - useResult.result.usedAmount);
                        if (success.result == false)
                        {
                            return new Result<ItemUsedResult>(null, success.error);
                        }
                    }
                }
            }

            return useResult;
        }

        public void Client_NotifyUsed(Character character, ItemContext useContext)
        { }

        public override Result<GameObject> Drop(Character character, Vector3 worldPosition)
        {
            return PUN2ItemUtility.DropItem(this, character, worldPosition);
        }

        public virtual Result<GameObject> Server_Drop(Character character, Vector3 worldPosition)
        {
            return base.Drop(character, worldPosition);
        }

        public override object Clone()
        {
            var clone = (IItemInstance)base.Clone();
            ServerItemRegistry.Register(clone.ID, clone);
            return clone;
        }
    }
}