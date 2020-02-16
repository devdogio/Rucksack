using System;
using Devdog.Rucksack.Currencies;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class CurrencyCollectionGroupTests
    {

        private CurrencyCollectionGroup<ICurrency> _group;
        private ICurrencyCollection<ICurrency, double> _col1;
        private ICurrencyCollection<ICurrency, double> _col2;
        
        private ICurrency _gold;
        private ICurrency _silver;
        
        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _col1 = new CurrencyCollection();
            _col2 = new CurrencyCollection();
            _group = new CurrencyCollectionGroup<ICurrency>(new CurrencyCollectionGroup<ICurrency>.Slot[]
            {
                new CurrencyCollectionGroup<ICurrency>.Slot(_col1, new CurrencyCollectionPriority<ICurrency>()), 
                new CurrencyCollectionGroup<ICurrency>.Slot(_col2, new CurrencyCollectionPriority<ICurrency>(60, 60, 60)), 
            });
            
            _gold = new Currency(Guid.NewGuid(), "Gold", "GOLD", 2, 999d);
            _silver = new Currency(Guid.NewGuid(), "Silver", "Silver", 2, 999d);
        }

        [Test]
        public void AddCurrencyTest()
        {
            var added = _group.Add(_gold, 100d);
            
            Assert.IsNull(added.error);
            Assert.IsTrue(added.result);
            
            Assert.AreEqual(0d, _col1.GetAmount(_gold));
            Assert.AreEqual(100d, _col2.GetAmount(_gold));
        }
        
        [Test]
        public void AddCurrencyTest2()
        {
            _group.Add(_gold, _gold.maxAmount - 10d);
            var added = _group.Add(_gold, 50d);
            
            Assert.IsNull(added.error);
            Assert.IsTrue(added.result);
            
            Assert.AreEqual(40d, _col1.GetAmount(_gold));
            Assert.AreEqual(_gold.maxAmount, _col2.GetAmount(_gold));
        }
        
        [Test]
        public void RemoveCurrencyTest()
        {
            _group.Add(_gold, 100d);

            var removed = _group.Remove(_gold, 50d);
            
            Assert.IsNull(removed.error);
            Assert.IsTrue(removed.result);
            
            Assert.AreEqual(0d, _col1.GetAmount(_gold));
            Assert.AreEqual(50d, _col2.GetAmount(_gold));
        }
        
        [Test]
        public void RemoveCurrencyTest2()
        {
            _group.Add(_gold, _gold.maxAmount + 100d);

            var removed = _group.Remove(_gold, 150d);
            
            Assert.IsNull(removed.error);
            Assert.IsTrue(removed.result);
            
            Assert.AreEqual(100d, _col1.GetAmount(_gold));
            Assert.AreEqual(_gold.maxAmount - 150d, _col2.GetAmount(_gold));
        }
         
        [Test]
        public void RemoveCurrencyTest3()
        {
            _group.Add(_gold, _gold.maxAmount + 100d);

            var removed = _group.Remove(_gold, _gold.maxAmount + 50d);
            
            Assert.IsNull(removed.error);
            Assert.IsTrue(removed.result);
            
            Assert.AreEqual(50d, _col1.GetAmount(_gold));
            Assert.AreEqual(0d, _col2.GetAmount(_gold));
        }
        
        [Test]
        public void GetAmountTest()
        {
            _group.Add(_gold, _gold.maxAmount + 100d);
            _group.Add(_silver, _silver.maxAmount + 100d);
            
            Assert.AreEqual(_gold.maxAmount + 100d, _group.GetAmount(_gold));
            Assert.AreEqual(_silver.maxAmount + 100d, _group.GetAmount(_silver));
        }
        
        [Test]
        public void GetCanAddAmount()
        {
            _group.Add(_gold, _gold.maxAmount + 100d);
            _group.Add(_silver, _silver.maxAmount + 100d);
            
            Assert.AreEqual(_gold.maxAmount - 100d, _group.GetCanAddAmount(_gold));
            Assert.AreEqual(_silver.maxAmount - 100d, _group.GetCanAddAmount(_silver));
        }
    }
}