using System.Linq;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.Characters
{
    public class InventoryPlayer : MonoBehaviour, IInventoryPlayer
    {
        [SerializeField]
        private string[] _itemCollections = new string[0];
        public string[] itemCollections => _itemCollections;

        [SerializeField]
        private string[] _equipmentCollections = new string[0];
        public string[] equipmentCollections => _equipmentCollections;

        [SerializeField]
        private string[] _currencyCollections = new string[0];
        public string[] currencyCollections => _currencyCollections;

        public CollectionGroup<IItemInstance> itemCollectionGroup { get; protected set; }
        public CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>> equipmentCollectionGroup { get; protected set; }
        public CurrencyCollectionGroup<ICurrency> currencyCollectionGroup { get; protected set; }

        protected ILogger logger;
        public InventoryPlayer()
        {
            logger = new UnityLogger("[Player] ");

            itemCollectionGroup = new CollectionGroup<IItemInstance>();
            equipmentCollectionGroup = new CollectionGroup<IEquippableItemInstance, IEquipmentCollection<IEquippableItemInstance>>();
            currencyCollectionGroup = new CurrencyCollectionGroup<ICurrency>();
        }

        protected virtual void Awake()
        {
            RegisterCollectionListeners();
            SetCollectionGroups();
        }

        protected virtual void Start()
        {

            if (GetComponent<IDropHandler>() == null)
            {
                logger.Warning($"No implementation of {nameof(IDropHandler)} found on player", this);
            }
        }

        protected virtual void OnDestroy()
        {
            UnRegisterCollectionListeners();
        }

        protected virtual void RegisterCollectionListeners()
        {
            CollectionRegistry.byName.OnAddedItem += OnRegistryChanged;
            CollectionRegistry.byName.OnRemovedItem += OnRegistryChanged;
        }

        protected virtual void UnRegisterCollectionListeners()
        {
            CollectionRegistry.byName.OnAddedItem -= OnRegistryChanged;
            CollectionRegistry.byName.OnRemovedItem -= OnRegistryChanged;
        }

        protected void SetCollectionGroups()
        {
            itemCollectionGroup.Set(GetItemCollections());
            equipmentCollectionGroup.Set(GetEquipmentCollections());
            currencyCollectionGroup.Set(GetCurrencyCollections());
        }

        protected virtual void OnRegistryChanged(string key, ICollection value)
        {
            SetCollectionGroups();
            logger.LogVerbose($"Collection {key} changed. Updating player's " +
                $"collection groups.\n{itemCollectionGroup.collectionCount} " +
                $"inventories\n{equipmentCollectionGroup.collectionCount} equipment " +
                $"collections\n{currencyCollectionGroup.collectionCount} currency collections", this);
        }

        protected virtual ICollection<IItemInstance>[] GetItemCollections()
        {
            return CollectionRegistry.byName.Get(itemCollections).Cast<ICollection<IItemInstance>>().ToArray();
        }

        protected virtual IEquipmentCollection<IEquippableItemInstance>[] GetEquipmentCollections()
        {
            return CollectionRegistry.byName.Get(equipmentCollections).Cast<IEquipmentCollection<IEquippableItemInstance>>().ToArray();
        }

        protected virtual ICurrencyCollection<ICurrency, double>[] GetCurrencyCollections()
        {
            return CurrencyCollectionRegistry.byName.Get(currencyCollections).Cast<ICurrencyCollection<ICurrency, double>>().ToArray();
        }
    }
}