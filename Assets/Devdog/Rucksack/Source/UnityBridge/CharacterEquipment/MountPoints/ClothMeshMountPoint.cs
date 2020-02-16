using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    /// <summary>
    /// A skinned mesh mount point used to equip skinned meshes and sync up their bones + animations with the root character.
    /// <seealso cref="StaticMeshMountPoint"/>
    /// <seealso cref="SkinnedMeshMountPoint"/>
    /// </summary>
    public sealed class ClothMeshMountPoint : UnityMountPointBase<IEquippableItemInstance>
    {
        public override GameObject GetModel(IEquippableItemInstance item)
        {
            var unityDef = item.itemDefinition as IUnityEquippableItemDefinition;
            if (unityDef != null)
            {
                return unityDef.equipmentModel;
            }

            return null;
        }

        public override GameObject InitModelInstance(GameObject instance)
        {
            var cloth = instance.GetComponent<Cloth>();
            if (cloth != null)
            {
                cloth.ClearTransformMotion();
            }
            else
            {
                logger.Log("Given item instance does not contain a cloth component", this);
            }
            
            return base.InitModelInstance(instance);
        }
    }
}