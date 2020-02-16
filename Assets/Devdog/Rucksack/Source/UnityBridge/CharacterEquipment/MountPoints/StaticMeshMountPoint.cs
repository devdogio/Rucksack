using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    /// <summary>
    /// A simple static mesh mount point used to equip static meshes.
    /// <seealso cref="SkinnedMeshMountPoint"/>
    /// <seealso cref="ClothMeshMountPoint"/>
    /// </summary>
    public sealed class StaticMeshMountPoint : UnityMountPointBase<IEquippableItemInstance>
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
    }
}