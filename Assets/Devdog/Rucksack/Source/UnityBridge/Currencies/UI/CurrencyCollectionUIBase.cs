using Devdog.General2.UI;
using Devdog.Rucksack.Currencies;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    [RequireComponent(typeof(UIWindow))]
    public abstract class CurrencyCollectionUIBase<TCurrency> : UIQueuedMonoBehaviour<TCurrency>
        where TCurrency : CurrencyUIBase<TCurrency>
    {
        [SerializeField]
        private string _collectionName;

        // TODO: Consider grabbing all from the database if none are defined...
        [SerializeField]
        private UnityCurrency[] _repaintCurrencies = new UnityCurrency[0];

        private ICurrencyCollection<ICurrency, double> _collection;

        public ICurrencyCollection<ICurrency, double> collection
        {
            get { return _collection; }
            set
            {
                if (_collection != null)
                {
                    _collection.OnCurrencyChanged -= OnCurrencyChanged;
                }

                _collection = value;
                if (_collection != null)
                {
                    // Get rid of the old
                    foreach (var t in repaintableElements)
                    {
                        if (t != null)
                        {
                            Destroy(t.gameObject);
                        }
                    }

                    repaintableElements = new TCurrency[_repaintCurrencies.Length];
                    for (int i = 0; i < repaintableElements.Length; i++)
                    {
                        repaintableElements[i] = CreateUIElement(i);
                        Repaint(i);
                    }

                    _collection.OnCurrencyChanged += OnCurrencyChanged;
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            collection = FindCollection();
            CurrencyCollectionRegistry.byName.OnAddedItem += OnLocalCollectionAdded;
            CurrencyCollectionRegistry.byName.OnRemovedItem += OnLocalCollectionRemoved;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            collection = null; // Unregisters events
            CurrencyCollectionRegistry.byName.OnAddedItem -= OnLocalCollectionAdded;
            CurrencyCollectionRegistry.byName.OnRemovedItem -= OnLocalCollectionRemoved;
        }

        private void OnCurrencyChanged(object sender, CurrencyChangedResult<double> currencyChangedResult)
        {
            for (var i = 0; i < _repaintCurrencies.Length; i++)
            {
                if (_repaintCurrencies[i].Equals(currencyChangedResult.currency))
                {
                    RepaintOrQueue(i);
                    return;
                }
            }
        }

        private void OnLocalCollectionAdded(string s, ICurrencyCollection c)
        {
            if (_collectionName == s)
            {
                collection = FindCollection();

                logger.LogVerbose($"Local collection found with name: {_collectionName} and type {collection.GetType().Name}", this);
            }
        }

        private void OnLocalCollectionRemoved(string s, ICurrencyCollection c)
        {
            if (_collectionName == s)
            {
                collection = FindCollection();
            }
        }

        protected virtual ICurrencyCollection<ICurrency, double> FindCollection()
        {
            var col = CurrencyCollectionRegistry.byName.Get(_collectionName) as ICurrencyCollection<ICurrency, double>;
            if (col != null)
            {
                logger.LogVerbose($"Local collection found with name: {_collectionName} and type {col.GetType().Name}", this);
                return col as ICurrencyCollection<ICurrency, double>;
            }

            logger.Warning($"Collection with name {_collectionName} not found (might be initialized later)", this);
            return null;
        }

        protected override TCurrency CreateUIElement(int index)
        {
            var inst = base.CreateUIElement(index);
            inst.collectionUI = this;
            inst.currency = _repaintCurrencies[index];
            inst.gameObject.name = inst + ":" + index;

            return inst;
        }

        protected override void Repaint(int slot)
        {
            // Check if the slots are still valid; Collection could've been resized.
            if (slot >= 0 && slot < repaintableElements.Length)
            {
                repaintableElements[slot]?.Repaint(collection.GetAmount(_repaintCurrencies[slot]), _repaintCurrencies[slot]);
                logger.LogVerbose($"{_collectionName} element #{slot} repainted");
            }
        }
    }
}