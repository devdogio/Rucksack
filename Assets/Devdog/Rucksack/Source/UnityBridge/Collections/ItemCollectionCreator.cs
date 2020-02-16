using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Collections
{
    /// <summary>
    /// Creates a local item collection on Awake and registers it in the CollectionRegistry
    /// </summary>
    public sealed class ItemCollectionCreator : BaseCollectionCreator<Collection<IItemInstance>>
    {
        [Header("Layout Settings"), SerializeField]
        private int _slotCount = 1;

        /// <summary>
        /// The number of slots in collection to be created.
        /// </summary>
        public int slotCount {
            get { return _slotCount; }
            set { _slotCount = value; }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slotCount <= 0)
            {
                _slotCount = 1;
            }
        }
#endif

        protected override Collection<IItemInstance> CreateCollection()
        {
            var builder = new CollectionBuilder<IItemInstance>();
            return builder.SetLogger(_logger)
                .SetSize(slotCount)
                .SetSlotType<CollectionSlot<IItemInstance>>()
                .SetName(collectionName)
                .Build();
        }

        protected override void RegisterByName(Collection<IItemInstance> col)
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
                    _logger.Warning($"Item collection with name {collectionName} already exists in CollectionRegistry and will be overridden by this one. " +
                        $"Use \"ignoreDuplicates = false\" to avoid collection override.", this);
                }
            }

            CollectionRegistry.byName.Register(collectionName, col);
        }

        protected override void RegiterByID(Collection<IItemInstance> col)
        {
            CollectionRegistry.byID.Register(collectionID, col);
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