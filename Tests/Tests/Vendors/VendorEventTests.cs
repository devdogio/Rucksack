using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class VendorEventTests
    {
        private IVendor<IItemInstance> _vendor;
        private Collection<IVendorProduct<IItemInstance>> _vendorCollection;
        private CurrencyCollection _vendorCurrencies;
        private Customer<IItemInstance> _customer;
        private Collection<IItemInstance> _customerCollection;
        private CurrencyCollection _customerCurrencies;

        private IItemInstance _item1;
        private IItemInstance _item2;

        private IVendorProduct<IItemInstance> _product1;
        private IVendorProduct<IItemInstance> _product2;

        private Currency _gold;
        
        [SetUp]
        public void Setup()
        {
            _vendorCollection = new Collection<IVendorProduct<IItemInstance>>(10);
            _vendorCurrencies = new CurrencyCollection();

            _gold = new Currency(Guid.NewGuid(), "Gold", "GOLD", 2, 999f);
            
            _vendor = new Vendor<IItemInstance>(new VendorConfig(), _vendorCollection, _vendorCurrencies);
            _customerCollection = new Collection<IItemInstance>(10);
            _customerCurrencies = new CurrencyCollection();
            _customer = new Customer<IItemInstance>(Guid.NewGuid(), null, new CollectionGroup<IItemInstance>(new []
            {
                new CollectionGroup<IItemInstance>.Slot(_customerCollection), 
            }), new CurrencyCollectionGroup<ICurrency>(_customerCurrencies));
            
            _item1 = new ItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5, buyPrice = new[]{ new CurrencyDecorator<double>(_gold, 1d) }, sellPrice = new[] { new CurrencyDecorator<double>(_gold, 0.6d) } });
            _item2 = new ItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5, buyPrice = new[]{ new CurrencyDecorator<double>(_gold, 2d) }, sellPrice = new[] { new CurrencyDecorator<double>(_gold, 1.6d) } });
            
            _product1 = new VendorProduct<IItemInstance>(_item1, _item1.itemDefinition.buyPrice, _item1.itemDefinition.sellPrice);
            _product2 = new VendorProduct<IItemInstance>(_item2, _item2.itemDefinition.buyPrice, _item2.itemDefinition.sellPrice);
        }


        [Test]
        public void OnSellItemToVendorEventTest()
        {
            // Arrange
            SellToVendorResult<IItemInstance> sellResult = null;
            int eventCount = 0;
            _vendor.OnSoldToVendor += (sender, result) =>
            {
                sellResult = result;
                eventCount++;
            };

            _customerCollection.Add(_item1, 10);
            _vendorCurrencies.Add(_gold, 100);
            
            // Act
            var sold = _vendor.SellToVendor(_customer, _product1, 2);

            // Assert
            Assert.IsNull(sold.error);
            Assert.AreEqual(1, eventCount);
            
            Assert.AreEqual(sellResult.amount, 2);
            Assert.AreEqual(sellResult.item, _product1);
            Assert.AreEqual(sellResult.currencies[0].currency, _gold);
        }

        [Test]
        public void OnBuyItemFromVendorEventTest()
        {
            // Arrange
            BuyFromVendorResult<IItemInstance> boughtResult = null;
            int eventCount = 0;
            _vendor.OnBoughtFromVendor += (sender, result) =>
            {
                boughtResult = result;
                eventCount++;
            };

            _vendorCollection.Add(_product1, 10);
            _customerCurrencies.Add(_gold, 100);
            
            // Act
            var bought = _vendor.BuyFromVendor(_customer, _item1, 2);

            // Assert
            Assert.IsNull(bought.error);
            Assert.AreEqual(1, eventCount);
            
            Assert.AreEqual(boughtResult.amount, 2);
            Assert.AreEqual(boughtResult.item, _product1);
            Assert.AreEqual(boughtResult.currencies[0].currency, _gold);
        }
        
    }
}