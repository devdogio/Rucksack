using System;
using System.Collections.Generic;
using Devdog.General2;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.CharacterEquipment
{
    /// <summary>
    /// A skinned mesh mount point used to equip skinned meshes and sync up their bones + animations with the root character.
    /// <seealso cref="StaticMeshMountPoint"/>
    /// <seealso cref="ClothMeshMountPoint"/>
    /// </summary>
    public sealed class SkinnedMeshMountPoint : UnityMountPointBase<IEquippableItemInstance>
    {
        public enum Mode
        {
            Replace,
            ForceSetAll,
        }
        
//        [Required]
        public Transform rootBone;

        [Required]
        public SkinnedMeshRenderer meshRenderer;

        /// <summary>
        /// Forcefully replace all bones? When set to false only matching bones (by name) will be replaced.
        /// </summary>
        public Mode replacementMode = Mode.Replace;
        
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
            var skinnedMeshes = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(var skinnedMesh in skinnedMeshes)
            {
                HandleSkinnedMesh(skinnedMesh);
            }

            if(skinnedMeshes.Length == 0)
            {
                logger.Log("Tried to equip skinned mesh onto player, but object is not a skinned mesh.", instance);
            }

            return base.InitModelInstance(instance);
        }
        
        private void HandleSkinnedMesh(SkinnedMeshRenderer skinnedMesh)
        {
            skinnedMesh.rootBone = rootBone;

            switch (replacementMode)
            {
                case Mode.Replace:
                {
                    ReplaceBonesOnTarget(meshRenderer, skinnedMesh);
                    break;
                }
                case Mode.ForceSetAll:
                {
                    skinnedMesh.bones = meshRenderer.bones;
                    logger.LogVerbose("Force copied " + skinnedMesh.bones.Length + " bones to new skinned mesh", skinnedMesh);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// The bones on the writeTo target will be replaced.
        /// Any bones that could not be found on the source will be ignored.
        /// </summary>
        /// <param name="source">Used to copy the bones from and replace onto the writeTo target</param>
        /// <param name="writeTo">Bones are replaced on this target</param>
        private void ReplaceBonesOnTarget(SkinnedMeshRenderer source, SkinnedMeshRenderer writeTo)
        {
            var boneCountBefore = writeTo.bones.Length;
            var newBones = new List<Transform>();

            // Search the writeTo.bones (current ones) in the source and copy over those bones.
            foreach (var t in writeTo.bones)
            {
                var bone = FindRecursive(source.rootBone, t.name);
                if (bone != null)
                {
                    newBones.Add(bone);
                }
            }

            writeTo.bones = newBones.ToArray();
            DevdogLogger.LogVerbose($"Merged {newBones.Count}/{boneCountBefore} bones from {writeTo.gameObject.name} to {source.gameObject.name} (source has {source.bones.Length} bones)", source);
        }

        private Transform FindRecursive(Transform t, string findName)
        {
            if (t.name == findName)
            {
                return t;
            }

            foreach (Transform child in t)
            {
                var val = FindRecursive(child, findName);
                if (val != null)
                {
                    return val;
                }
            }

            return null;
        }
    }
}