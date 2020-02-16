using System;
using System.Linq;

namespace Devdog.Rucksack.Currencies
{
    public sealed class CurrencyCollectionGroup<TCurrency>
        where TCurrency: IIdentifiable, ICurrency
    {
        public sealed class Slot
        {
            public ICurrencyCollection<TCurrency, double> collection;
            public ICurrencyCollectionPriority<TCurrency> priority;

            public Slot(ICurrencyCollection<TCurrency, double> collection)
                :this(collection, new CurrencyCollectionPriority<TCurrency>())
            { }
            
            public Slot(ICurrencyCollection<TCurrency, double> collection, ICurrencyCollectionPriority<TCurrency> priority)
            {
                this.priority = priority;
                this.collection = collection;
            }
        }

        public int collectionCount
        {
            get { return collections.Length; }
        }

        public Slot[] collections { get; private set; }
        private Slot[] _collectionsAddOrdered;
        private Slot[] _collectionsRemoveOrdered;
        public CurrencyCollectionGroup(params ICurrencyCollection<TCurrency, double>[] collections)
            : this(collections.Select(o => new Slot(o)).ToArray())
        { }
        
        public CurrencyCollectionGroup(Slot[] collections)
        {
            this.collections = collections ?? new Slot[0];
            SortArrays();
        }

        private void SortArrays()
        {
            this.collections = this.collections.OrderByDescending(o => o.priority.GetGeneralPriority()).ToArray();
            _collectionsAddOrdered = this.collections.OrderByDescending(o => o.priority.GetAddPriority(default(TCurrency))).ToArray();
            _collectionsRemoveOrdered = this.collections.OrderByDescending(o => o.priority.GetRemovePriority(default(TCurrency))).ToArray();
        }

        public void Set(params ICurrencyCollection<TCurrency, double>[] newCollections)
        {
            Set(newCollections.Select(o => new Slot(o)).ToArray());
        }
        
        public void Set(Slot[] newCollections)
        {
            collections = newCollections;
            SortArrays();
        }
        
        public void AddCollection(Slot collection)
        {
            var l = new System.Collections.Generic.List<Slot>(collections);
            l.Add(collection);

            collections = l.ToArray();
            SortArrays();
        }
        
        public Result<bool> CanAdd(TCurrency currency, double amount)
        {
            if (GetCanAddAmount(currency) < amount)
            {
                return new Result<bool>(false, Errors.CollectionFull);
            }

            return true;
        }

        public Result<bool> Add(TCurrency currency, double amount)
        {
            var canAdd = CanAdd(currency, amount);
            if (canAdd.result == false)
            {
                return canAdd;
            }

            double toAddAmount = amount;
            foreach (var slot in _collectionsAddOrdered)
            {
                var canAddToCol = slot.collection.GetCanAddAmount(currency);
                if (canAddToCol > 0)
                {
                    var addAmount = Math.Min(toAddAmount, canAddToCol);
                    slot.collection.Add(currency, addAmount);

                    toAddAmount -= addAmount;
                    if (toAddAmount <= 0)
                    {
                        break;
                    }
                }
            }
            
            return true;
        }

        public Result<bool> CanRemove(TCurrency currency, double amount)
        {
            double removableAmount = 0.0d;
            foreach (var slot in _collectionsRemoveOrdered)
            {
                var containsAmount = slot.collection.GetAmount(currency);
                var canRemove = slot.collection.CanRemove(currency, containsAmount);
                if (canRemove.error == null)
                {
                    removableAmount += containsAmount;
                }
            }

            if (removableAmount < amount)
            {
                return new Result<bool>(false, Errors.CurrencyCollectionDoesNotContainCurrency);
            }

            return true;
        }

        public Result<bool> Remove(TCurrency currency, double amount)
        {
            var canRemove = CanRemove(currency, amount);
            if (canRemove.result == false)
            {
                return canRemove;
            }

            double removeAmount = amount;
            foreach (var slot in _collectionsRemoveOrdered)
            {
                var canRemoveFromCol = slot.collection.GetAmount(currency);
                if (canRemoveFromCol > 0)
                {
                    var toRemove = Math.Min(removeAmount, canRemoveFromCol);
                    slot.collection.Remove(currency, toRemove);
                    
                    removeAmount -= toRemove;
                    if (removeAmount <= 0)
                    {
                        break;
                    }
                }
            }
            
            return true;
        }

        public double GetAmount(TCurrency currency)
        {
            return collections.Sum(o => o.collection.GetAmount(currency));
        }

        public double GetCanAddAmount(TCurrency currency)
        {
            return collections.Sum(o => o.collection.GetCanAddAmount(currency));
        }
    }
}