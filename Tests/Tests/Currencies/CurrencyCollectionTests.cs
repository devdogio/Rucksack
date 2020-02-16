using System;
using Devdog.Rucksack.Currencies;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class CurrencyCollectionTests
    {

        private CurrencyCollection _currencies;
        private Currency _gold;
        private Currency _silver;
        private Currency _copper;

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _currencies = new CurrencyCollection();
            _gold = new Currency(Guid.NewGuid(), "Gold", "GOLD", 2, 999f);
            _silver = new Currency(Guid.NewGuid(), "Silver", "SILVER", 2, 999f);
            _copper = new Currency(Guid.NewGuid(), "Copper", "COPPER", 2, 999f);
        }

        [Test]
        public void AddToCurrencyTest()
        {
            var result = _currencies.Add(_gold, 2);
            
            Assert.IsNull(result.error);
            Assert.IsTrue(result.result);
        }
        
        [Test]
        public void AddToCurrencyMaxAmountTest()
        {
            var result1 = _currencies.Set(_gold, 999f);
            var result = _currencies.Add(_gold, 10f);

            Assert.IsNull(result1.error);
            Assert.AreEqual(Errors.CollectionFull, result.error);
            Assert.IsFalse(result.result);
        }
        
        
        [Test]
        public void RemoveFromCurrencyTest()
        {
            _currencies.Set(_gold, 10f);

            var result = _currencies.Remove(_gold, 1f);
            
            Assert.IsNull(result.error);
            Assert.IsTrue(result.result);
            Assert.AreEqual(9f, _currencies.GetAmount(_gold));
        }
        
        [Test]
        public void SetCurrencyTest()
        {
            _currencies.Set(_gold, 2f);
            
            Assert.AreEqual(2f, _currencies.GetAmount(_gold));
        }

        [Test]
        public void SetCurrencyTest2()
        {
            _currencies.Add(_gold, 2f);
            _currencies.Set(_gold, 1f);
            
            Assert.AreEqual(1f, _currencies.GetAmount(_gold));
        }
        
        [Test]
        public void SetCurrencyTest3()
        {
            _currencies.Add(_gold, 2f);
            _currencies.Set(_gold, 0f);
            
            Assert.AreEqual(0f, _currencies.GetAmount(_gold));
        }
        
        [Test]
        public void SetCurrencyTest4()
        {
            _currencies.Add(_gold, 2f);
            _currencies.Set(_gold, 0f);
            
            Assert.IsFalse(_currencies.Contains(_gold));
        }

        [Test]
        public void SetCurrencyTest5()
        {
            _currencies.Set(_gold, 0f);
            Assert.IsFalse(_currencies.Contains(_gold));
        }
        
        [Test]
        public void SetCurrencyExceedingMaxTest()
        {
            var result = _currencies.Set(_gold, 1000f);
            Assert.AreEqual(Errors.CollectionFull, result.error);
        }
        
        [Test]
        public void AddCurrencyExceedingMaxTest()
        {
            var result = _currencies.Add(_gold, 1000f);
            Assert.AreEqual(Errors.CollectionFull, result.error);
        }
                
        [Test]
        public void RemoveMoreThanInCollectionTest()
        {
            var result = _currencies.Add(_gold, 50d);
            var removed = _currencies.Remove(_gold, 100d);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, removed.error);
        }

        [Test]
        public void Remove0Test()
        {
            var removed = _currencies.Remove(_gold, 0d);
            
            Assert.AreEqual(null, removed.error);
            Assert.AreEqual(true, removed.result);
        }
        
        [Test]
        public void Remove10Test()
        {
            var removed = _currencies.Remove(_gold, 10d);
            
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, removed.error);
            Assert.AreEqual(false, removed.result);
        }
        
        [Test]
        public void Add0Test()
        {
            var removed = _currencies.Add(_gold, 0d);
            
            Assert.IsNull(removed.error);
            Assert.AreEqual(true, removed.result);
        }
        
        
        [Test]
        public void ReadOnlyTest()
        {
            // Arrange
            _currencies.Add(_gold, 10f);
            _currencies.isReadOnly = true;

            // Act
            var canAdd = _currencies.CanAdd(_gold, 1f);
            var add = _currencies.Add(_gold, 2);
            var canRemove = _currencies.CanRemove(_gold, 1f);
            var remove = _currencies.Remove(_gold, 2);
            
            _currencies.Clear();
            _currencies.Set(_gold, 1f);
            
            // Assert
            Assert.AreEqual(Errors.CollectionIsReadOnly, canAdd.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, add.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, canRemove.error);
            Assert.AreEqual(Errors.CollectionIsReadOnly, remove.error);
            
            Assert.IsFalse(canAdd.result);
            Assert.IsFalse(add.result);
            Assert.IsFalse(canRemove.result);
            Assert.IsFalse(remove.result);
            
            Assert.AreEqual(10f, _currencies.GetAmount(_gold));
        }

        [Test]
        public void EventChangedAddTest()
        {
            // Arrange
            _currencies.Add(_gold, 10f);
            int callCount = 0;
            CurrencyChangedResult<double> eventResult = null;
                
            _currencies.OnCurrencyChanged += (sender, result) =>
            {
                eventResult = result;
                callCount++;
            };
            
            // Act
            var add = _currencies.Add(_gold, 2);
            
            // Assert
            Assert.IsNull(add.error);
            Assert.IsTrue(add.result);
            
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(_gold, eventResult.currency);
            Assert.AreEqual(10f, eventResult.amountBefore);
            Assert.AreEqual(12f, eventResult.amountAfter);
        }
        
        [Test]
        public void EventChangedRemoveTest()
        {
            // Arrange
            _currencies.Add(_gold, 10f);
            int callCount = 0;
            CurrencyChangedResult<double> eventResult = null;
                
            _currencies.OnCurrencyChanged += (sender, result) =>
            {
                eventResult = result;
                callCount++;
            };
            
            // Act
            var remove = _currencies.Remove(_gold, 2);
            
            // Assert
            Assert.IsNull(remove.error);
            Assert.IsTrue(remove.result);
            
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(_gold, eventResult.currency);
            Assert.AreEqual(10f, eventResult.amountBefore);
            Assert.AreEqual(8f, eventResult.amountAfter);
        }
        
        [Test]
        public void EventChangedSetTest()
        {
            // Arrange
            _currencies.Add(_gold, 10f);
            int callCount = 0;
            CurrencyChangedResult<double> eventResult = null;
                
            _currencies.OnCurrencyChanged += (sender, result) =>
            {
                eventResult = result;
                callCount++;
            };
            
            // Act
            _currencies.Set(_gold, 2f);
            
            // Assert
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(_gold, eventResult.currency);
            Assert.AreEqual(10f, eventResult.amountBefore);
            Assert.AreEqual(2f, eventResult.amountAfter);
        }
        
        [Test]
        public void EventClearTest()
        {
            // Arrange
            _currencies.Set(_gold, 10f);
            _currencies.Set(_silver, 5f);
            
            int callCount = 0;
            CurrencyChangedResult<double> eventResult1 = null;
            CurrencyChangedResult<double> eventResult2 = null;
                
            _currencies.OnCurrencyChanged += (sender, result) =>
            {
                if (eventResult1 == null)
                {
                    eventResult1 = result;
                }
                else
                {
                    eventResult2 = result;
                }
                
                callCount++;
            };
            
            // Act
            _currencies.Clear();
            
            // Assert
            Assert.AreEqual(2, callCount);
            Assert.AreEqual(_gold, eventResult1.currency);
            Assert.AreEqual(10f, eventResult1.amountBefore);
            Assert.AreEqual(0f, eventResult1.amountAfter);
            
            Assert.AreEqual(_silver, eventResult2.currency);
            Assert.AreEqual(5f, eventResult2.amountBefore);
            Assert.AreEqual(0f, eventResult2.amountAfter);
        }

        [Test]
        public void ToDecorators()
        {
            _currencies.Set(_gold, 1f);
            _currencies.Set(_copper, 10f);

            var decs = _currencies.ToDecorators();

            Assert.AreEqual(_gold, decs[0].currency);
            Assert.AreEqual(1f, decs[0].amount);
            
            Assert.AreEqual(_copper, decs[1].currency);
            Assert.AreEqual(10f, decs[1].amount);
        }

        [Test]
        public void CanRemoveZero()
        {
            var can = _currencies.CanRemove(_gold, 0f);
            var removed = _currencies.Remove(_gold, 0f);            
            
            Assert.IsNull(can.error);
            Assert.AreEqual(true, can.result);
            
            Assert.IsNull(removed.error);
            Assert.AreEqual(true, removed.result);
        }

        [Test]
        public void RemovingZeroDoesNotFireEvents()
        {
            var eventCount = 0;
            _currencies.OnCurrencyChanged += (sender, result) => { eventCount++; };
            
            var removed = _currencies.Remove(_gold, 0f);

            _currencies.Set(_gold, 10f);
            
            var removed2 = _currencies.Remove(_gold, 0f);
            
            Assert.IsNull(removed.error);
            Assert.IsNull(removed2.error);
            Assert.AreEqual(1, eventCount);
        }

    }
}