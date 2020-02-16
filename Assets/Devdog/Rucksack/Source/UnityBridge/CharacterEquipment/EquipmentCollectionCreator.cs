using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    public sealed class EquipmentCollectionCreator : BaseCollectionCreator<IEquipmentCollection<IEquippableItemInstance>>
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponent<IEquippableCharacter<IEquippableItemInstance>>() == null)
            {
                _logger.Warning($"{typeof(EquipmentCollectionCreator).Name} can only be added on a IEquippableCharacter component", this);
            }
        }
#endif

        protected override void RegisterByName(IEquipmentCollection<IEquippableItemInstance> col)
        {
            if (CollectionRegistry.byName.Contains(collectionName))
            {
                if (!ignoreDuplicates)
                {
                    _logger.Error($"Collection with name {collectionName} already exists in CollectionRegistry", this);
                    return;
                }
                else
                {
                    _logger.Warning($"Equipment collection with name {collectionName} already exists in CollectionRegistry and will be overridden by this one. " +
                        $"Use \"ignoreDuplicates = false\" to avoid collection override.", this);
                }
            }

            CollectionRegistry.byName.Register(collectionName, col);
        }

        protected override void RegiterByID(IEquipmentCollection<IEquippableItemInstance> col)
        {
            CollectionRegistry.byID.Register(collectionID, col);
        }

        protected override IEquipmentCollection<IEquippableItemInstance> CreateCollection()
        {
            return new EquipmentCollection<IEquippableItemInstance>(0, GetComponent<IEquippableCharacter<IEquippableItemInstance>>(), _logger)
            {
                collectionName = collectionName
            };
        }

        protected override void UnRegister()
        {
            if (CollectionRegistry.byName != null)
                CollectionRegistry.byName.UnRegister(collectionName);

            if (CollectionRegistry.byID != null)
                CollectionRegistry.byID.UnRegister(collectionID);
        }
    }
}