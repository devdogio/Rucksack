using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class VendorTests
    {
        private Vendor<IItemInstance> _vendor;
        private Collection<IVendorProduct<IItemInstance>> _vendorCollection;
        private CurrencyCollection _vendorCurrencies;
        private Customer<IItemInstance> _customer;
        private Collection<IItemInstance> _customerCollection;
        private CurrencyCollection _customerCurrencies;

        private CollectionItemInstance _item1;
        private CollectionItemInstance _item2;

        private VendorProduct<IItemInstance> _product1;
        private VendorProduct<IItemInstance> _product1_1;
        private VendorProduct<IItemInstance> _product2;

        private Currency _gold;
        private Currency _silver;
        
        [SetUp]
        public void Setup()
        {
            _vendorCollection = new Collection<IVendorProduct<IItemInstance>>(10);
            _vendorCurrencies = new CurrencyCollection();

            _gold = new Currency(Guid.NewGuid(), "Gold", "GOLD", 2, 999f);
            _silver = new Currency(Guid.NewGuid(), "Silver", "SILVER", 2, 999f);
            
            _vendor = new Vendor<IItemInstance>(new VendorConfig(), _vendorCollection, _vendorCurrencies);
            _customerCollection = new Collection<IItemInstance>(10);
            _customerCurrencies = new CurrencyCollection();
            _customer = new Customer<IItemInstance>(Guid.NewGuid(), null, new CollectionGroup<IItemInstance>(new []
            {
                new CollectionGroup<IItemInstance>.Slot(_customerCollection), 
            }), new CurrencyCollectionGroup<ICurrency>(_customerCurrencies));
            
            _item1 = new CollectionItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5, buyPrice = new [] { new CurrencyDecorator(_gold, 1d) }, sellPrice = new [] { new CurrencyDecorator(_gold, 0.6d) } });
            _item2 = new CollectionItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5, buyPrice = new [] { new CurrencyDecorator(_gold, 2d) }, sellPrice = new [] { new CurrencyDecorator(_gold, 1.6d) } });
            
            _product1 = ToProduct(_item1);
            _product1_1 = ToProduct(_item1);
            _product2 = ToProduct(_item2);
        }

        private static VendorProduct<IItemInstance> ToProduct(CollectionItemInstance item)
        {
            return new VendorProduct<IItemInstance>(item, item.itemDefinition.buyPrice, item.itemDefinition.sellPrice);
        }
        
        [Test]
        public void BuyItemWithoutPriceTest()
        {
            var itemWithoutBuyPrice = new CollectionItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5, sellPrice = new [] { new CurrencyDecorator(_gold, 10f) } });
            _vendorCollection.Add(ToProduct(itemWithoutBuyPrice), 3);

            var canBuy = _vendor.CanBuyFromVendor(_customer, itemWithoutBuyPrice, 1);
            
            Assert.AreEqual(Errors.VendorProductHasNoPrice, canBuy.error);
            Assert.IsFalse(canBuy.result);
        }
                
        [Test]
        public void SellItemWithoutPriceTest()
        {
            var itemWithoutBuyPrice = new CollectionItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid()) { maxStackSize = 5, buyPrice = new [] { new CurrencyDecorator(_gold, 10f) } });
            _vendorCollection.Add(ToProduct(itemWithoutBuyPrice), 3);

            var canBuy = _vendor.CanSellToVendor(_customer, new VendorProduct<IItemInstance>(itemWithoutBuyPrice, itemWithoutBuyPrice.itemDefinition.buyPrice, itemWithoutBuyPrice.itemDefinition.sellPrice), 1);
            
            Assert.AreEqual(Errors.VendorProductHasNoPrice, canBuy.error);
            Assert.IsFalse(canBuy.result);
        }

        [Test]
        public void CanBuyFromVendorTest()
        {
            _vendorCollection.Add(_product1, 10);
            _customerCurrencies.Set(_gold, 100f);
            
            var result = _vendor.CanBuyFromVendor(_customer, _item1);
            
            Assert.IsNull(result.error);
            Assert.IsTrue(result.result);
        }

        [Test]
        public void RemoveBoughtProductFromVendorListTest()
        {
            _vendor.config.removeBoughtProductFromVendor = false;
            _customerCurrencies.Add(_gold, 100);
            _customerCurrencies.Add(_silver, 100);

            _vendorCollection.Add(_product1, 3);

            var bought1 = _vendor.BuyFromVendor(_customer, _product1.item, 1);
            var bought2 = _vendor.BuyFromVendor(_customer, _product1.item, 1);
            
            Assert.IsNull(bought1.error);
            Assert.IsNull(bought2.error);
            
            Assert.AreEqual(3, _vendorCollection.GetAmount(_product1));
            Assert.AreEqual(2, _customerCollection.GetAmount(_product1.item));
        }
        
        [Test]
        public void CanBuyFromVendorCollectionFullTest()
        {
            _customer.AddItem(_item1, 50);
            _vendorCollection.Add(_product2, 10);
            
            var result = _vendor.CanBuyFromVendor(_customer, _item2);
            
            Assert.IsFalse(result.result);
            Assert.AreEqual(Errors.CollectionFull, result.error);
        }
        
        [Test]
        public void CanSellToVendorCollectionFullTest()
        {
            _vendorCurrencies.Set(_gold, 100f);

            _customer.AddItem(_item1, 10);
            _vendorCollection.Add(_product2, 50);
            
            var result = _vendor.CanSellToVendor(_customer, _product1);
            
            Assert.AreEqual(Errors.CollectionFull, result.error);
            Assert.IsFalse(result.result);
        }
        
        [Test]
        public void BuyFromVendorDoesNotHaveCurrencyTest()
        {
            _vendorCollection.Add(_product1, 3);
            _vendorCollection.Add(_product2, 3);

            var result = _vendor.BuyFromVendor(_customer, _item1, 2);

            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, result.error);
        }
        
        [Test]
        public void BuyFromVendorDoesHaveCurrencyTest()
        {
            _vendorCollection.Add(_product1, 3);
            _vendorCollection.Add(_product2, 3);

            _customerCurrencies.Add(_gold, 100);

            var result = _vendor.BuyFromVendor(_customer, _item1, 2);

            Assert.IsNull(result.error);
            Assert.AreEqual(1, _vendorCollection.GetAmount(_product1));
        }

        [Test]
        public void BuyNonExistingItemFromVendorTest()
        {
            _vendorCollection.Add(_product1, 3);

            _customerCurrencies.Add(_gold, 100);

            var result = _vendor.BuyFromVendor(_customer, _item2, 1);

            Assert.AreEqual(Errors.VendorDoesNotContainItem, result.error);
            Assert.AreEqual(3, _vendorCollection.GetAmount(_product1));
            Assert.AreEqual(0, _vendorCollection.GetAmount(_product2));

            Assert.AreEqual(0, _customerCollection.GetAmount(_item2));
        }

        [Test]
        public void BuyFromVendorCurrencyCountTest()
        {
            _customerCurrencies.Set(_gold, 100f);
            _vendorCurrencies.Set(_gold, 100f);

            _vendorCollection.Add(_product1, 5);
            _vendorCollection.Add(_product2, 5);
            

            var result = _vendor.BuyFromVendor(_customer, _item1, 5);

            Assert.IsNull(result.error);
            Assert.AreEqual(_product1, result.result.item);
            Assert.AreEqual(5, result.result.amount);
            Assert.AreEqual(_gold, result.result.currencies[0].currency);
            
            Assert.AreEqual(100f + _product1.buyPrice[0].amount * 5, _vendorCurrencies.GetAmount(_gold));
            Assert.AreEqual(100f - _product1.buyPrice[0].amount * 5, _customerCurrencies.GetAmount(_gold));
        }
        
        [Test]
        public void SellToVendorCurrencyCountTest()
        {
            _customerCurrencies.Set(_gold, 100f);
            _vendorCurrencies.Set(_gold, 100f);
            
            _customerCollection.Add(_item1, 10);
            
            var result = _vendor.SellToVendor(_customer, _product1, 5);

            Assert.IsNull(result.error);
            Assert.AreEqual(_product1, result.result.item);
            Assert.AreEqual(5, result.result.amount);
            Assert.AreEqual(_gold, result.result.currencies[0].currency);
            
            Assert.AreEqual(100f - _product1.sellPrice[0].amount * 5, _vendorCurrencies.GetAmount(_gold));
            Assert.AreEqual(100f + _product1.sellPrice[0].amount * 5, _customerCurrencies.GetAmount(_gold));
        }
        
        [Test]
        public void CanSellToVendorTest()
        {
            _vendorCurrencies.Set(_gold, 100f);
            var result = _vendor.CanSellToVendor(_customer, _product1);
            
            Assert.AreEqual(Errors.CollectionDoesNotContainItem, result.error);
        }
        
        [Test]
        public void CanSellToVendorTest2()
        {
            _vendorCurrencies.Set(_gold, 100f);
            _customer.AddItem(_item1, 10);
            var result = _vendor.CanSellToVendor(_customer, _product1);
            
            Assert.IsNull(result.error);
            Assert.IsTrue(result.result);
        }
        
        [Test]
        public void SellToVendorTest()
        {
            _vendorCurrencies.Set(_gold, 100f);
            _vendorCollection.Add(_product1, 1);
            
            _customerCollection.Add(_item2, 20);

            var result = _vendor.SellToVendor(_customer, _product2, 6);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(_product2, result.result.item);
            Assert.AreEqual(6, result.result.amount);
            Assert.AreEqual(_product2.sellPrice[0].currency, result.result.currencies[0].currency);
            
            Assert.AreEqual(1, _vendorCollection.slots[0].amount);
            Assert.AreEqual(5, _vendorCollection.slots[1].amount);
            Assert.AreEqual(1, _vendorCollection.slots[2].amount);
            
            Assert.AreEqual(100d - (_product2.sellPrice[0].amount * 6),_vendorCurrencies.GetAmount(_gold));
            Assert.AreEqual(_product2.sellPrice[0].amount * 6, _customerCurrencies.GetAmount(_gold));
        }
        
        [Test]
        public void SellItemToVendorWithDontAddSoldProductToVendor()
        {
            _vendor.config.addSoldProductToVendor = false;
            
            _vendorCurrencies.Set(_gold, 100f);
            _vendorCollection.Add(_product1, 1);
            
            _customerCollection.Add(_item2, 20);

            var result = _vendor.SellToVendor(_customer, _product2, 6);
            var result2 = _vendor.SellToVendor(_customer, _product2, 2);
            
            Assert.IsNull(result.error);
            Assert.IsNull(result2.error);
            Assert.AreEqual(_product2, result.result.item);
            Assert.AreEqual(6, result.result.amount);
            Assert.AreEqual(_product2.sellPrice[0].currency, result.result.currencies[0].currency);
            
            Assert.AreEqual(1, _vendorCollection.slots[0].amount);
            Assert.AreEqual(0, _vendorCollection.slots[1].amount);
            Assert.AreEqual(0, _vendorCollection.slots[2].amount);
            
            Assert.AreEqual(1, _vendorCollection.GetAmount(_product1));
            Assert.AreEqual(0, _vendorCollection.GetAmount(_product2));
            
            Assert.AreEqual(100d - (_product2.sellPrice[0].amount * 8),_vendorCurrencies.GetAmount(_gold));
            Assert.AreEqual(_product2.sellPrice[0].amount * 8, _customerCurrencies.GetAmount(_gold));
        }

        [Test]
        public void CanBuyFromVendorPriceMultiplierTest()
        {
            _vendor.config.buyFromVendorPriceMultiplier = 9.0f;
            
            _customerCurrencies.Set(_gold, 10f);
            
            _vendorCollection.Add(_product1, 5);

            var result = _vendor.CanBuyFromVendor(_customer, _item1, 1);
            var result2 = _vendor.CanBuyFromVendor(_customer, _item1, 2);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, result2.error);
        }

        [Test]
        public void CanSellToVendorPriceMultiplierTest()
        {
            _vendor.config.sellToVendorPriceMultiplier = 9.0f;
            
            _vendorCurrencies.Set(_gold, 10f);
            _customerCollection.Add(_item1, 5);

            var result = _vendor.CanSellToVendor(_customer, ToProduct(_item1), 1);
            var result2 = _vendor.CanSellToVendor(_customer, ToProduct(_item1), 2);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, result2.error);
        }
        
        [Test]
        public void SellToVendorPriceMultiplierTest()
        {
            _vendor.config.sellToVendorPriceMultiplier = 2.0f;
            
            _vendorCurrencies.Set(_gold, 100f);
            _vendorCollection.Add(_product1, 1);
            
            _customerCollection.Add(_item2, 20);

            var result = _vendor.SellToVendor(_customer, ToProduct(_item2), 6);
            var result2 = _vendor.SellToVendor(_customer, ToProduct(_item2), 2);
            
            Assert.IsNull(result.error);
            Assert.IsNull(result2.error);
            Assert.AreEqual(_product2.sellPrice[0].currency, result.result.currencies[0].currency);
            
            Assert.AreEqual(100d - (_product2.sellPrice[0].amount * 8 * _vendor.config.sellToVendorPriceMultiplier), _vendorCurrencies.GetAmount(_gold), 0.001d);
            Assert.AreEqual(_product2.sellPrice[0].amount * 8 * _vendor.config.sellToVendorPriceMultiplier, _customerCurrencies.GetAmount(_gold), 0.001d);
        }
                
        [Test]
        public void BuyFromVendorPriceMultiplierTest()
        {
            _vendor.config.buyFromVendorPriceMultiplier = 2.0f;
            
            _vendorCollection.Add(_product1, 1);
            _customerCurrencies.Add(_gold, 100);

            var result = _vendor.BuyFromVendor(_customer, _item1, 1);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(_product1.buyPrice[0].currency, result.result.currencies[0].currency);
            
            Assert.AreEqual(_product1.buyPrice[0].amount * _vendor.config.buyFromVendorPriceMultiplier, _vendorCurrencies.GetAmount(_gold), 0.001d);
            Assert.AreEqual(100d - (_product1.buyPrice[0].amount * _vendor.config.buyFromVendorPriceMultiplier), _customerCurrencies.GetAmount(_gold), 0.001d);
        }
        
        [Test]
        public void SellToVendorNotEnoughCurrencyTest()
        {
            _vendorCurrencies.Clear();
            _vendorCollection.Add(_product1, 1);

            var canSell = _vendor.CanSellToVendor(_customer, _product2, 1);
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, canSell.error);
            Assert.IsFalse(canSell.result);
        }
        
        [Test]
        public void BuyFromVendorNotEnoughCurrencyTest()
        {
            _customerCurrencies.Clear();
            _vendorCollection.Add(_product2, 5);
            
            var canBuy = _vendor.CanBuyFromVendor(_customer, _item2, 1);
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, canBuy.error);
            Assert.IsFalse(canBuy.result);
        }
        
        [Test]
        public void VendorFiniteCurrenciesTest()
        {
            var v = ((Vendor<IItemInstance>) _vendor);
            _customerCollection.Add(_item1, 10);

            var soldResult = v.SellToVendor(_customer, _product1, 3);
            
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, soldResult.error);
            Assert.AreEqual(0, v.vendorCurrencies.GetAmount(_gold));
        }

        [Test]
        public void VendorInfiniteCurrenciesTest()
        {
            var v = ((Vendor<IItemInstance>) _vendor);
            v.vendorCurrencies = new InfiniteCurrencyCollection();
            _customerCollection.Add(_item1, 10);

            var soldResult = v.SellToVendor(_customer, _product1, 3);
            
            Assert.IsNull(soldResult.error);
        }

        [Test]
        public void VendorSetItemTest()
        {
            _vendor.SetAllProducts(_customer, new List<ProductAmount<IVendorProduct<IItemInstance>>>()
            {
                new ProductAmount<IVendorProduct<IItemInstance>>(_product1, 3),
                new ProductAmount<IVendorProduct<IItemInstance>>(_product2, 1),
            });

            var allItems = _vendor.GetAllProducts(_customer).ToArray();
            Assert.AreEqual(_product1, allItems[0].item);
            Assert.AreEqual(3, allItems[0].amount);
            
            Assert.AreEqual(_product2, allItems[1].item);
            Assert.AreEqual(1, allItems[1].amount);
        }
        
        [Test]
        public void VendorCollectionAddItemTest()
        {
            _vendor.SetAllProducts(_customer, new List<ProductAmount<IVendorProduct<IItemInstance>>>()
            {
                new ProductAmount<IVendorProduct<IItemInstance>>(_product1, 1)
            });

            var allItems = _vendor.GetAllProducts(_customer).ToArray();
            Assert.AreEqual(_product1, allItems[0].item);
            Assert.AreEqual(1, allItems[0].amount);
            
            Assert.AreEqual(_item1.itemDefinition.buyPrice, _vendorCollection[0].buyPrice);
            Assert.AreEqual(_item1.itemDefinition.sellPrice, _vendorCollection[0].sellPrice);
        }
        
        [Test]
        public void VendorProductEqualityTest()
        {
            var clone = (IVendorProduct<IItemInstance>)_product1.Clone();

            Assert.AreEqual(_product1, _product1_1);
            Assert.AreEqual(_product1, clone);
            Assert.AreEqual(_product1_1, clone);
        }
        
        [Test]
        public void VendorProductEqualityTest2()
        {
            var clone = new VendorProduct<IItemInstance>(_item1, new []{ new CurrencyDecorator(_gold, 1d), }, new []{ new CurrencyDecorator(_gold, 0.6d), });
            var clone2 = new VendorProduct<IItemInstance>(_item1, _product1.buyPrice, _product1.sellPrice);
            
            Assert.AreEqual(_product1, clone);
            Assert.AreEqual(_product1_1, clone);
            
            Assert.AreEqual(_product1, clone2);
            Assert.AreEqual(_product1_1, clone2);
        }
        
        [Test]
        public void VendorProductEqualityNegativeTest()
        {
            var clone = new VendorProduct<IItemInstance>(_item1, new []{ new CurrencyDecorator(_gold, 1.1d), }, new []{ new CurrencyDecorator(_gold, 0.6d), });
            var clone2 = new VendorProduct<IItemInstance>(_item1, new []{ new CurrencyDecorator(_gold, 1d), }, new []{ new CurrencyDecorator(_gold, 0.65d), });
            
            Assert.AreNotEqual(_product1, clone);
            Assert.AreNotEqual(_product1_1, clone);
            
            Assert.AreNotEqual(_product1, clone2);
            Assert.AreNotEqual(_product1_1, clone2);
        }

        [Test]
        public void SellItemWithMultipleCurrenciesTest()
        {
            var item = new CollectionItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid())
            {
                maxStackSize = 5, 
                buyPrice = new [] { new CurrencyDecorator(_gold, 1d), new CurrencyDecorator(_silver, 40d),  }, 
                sellPrice = new [] { new CurrencyDecorator(_gold, 1d), new CurrencyDecorator(_silver, 10d),  }
            });
            
            var product = ToProduct(item);
            
            _customerCollection.Add(item, 3);

            _vendorCurrencies.Add(_gold, 100);
            _vendorCurrencies.Add(_silver, 100);

            
            var soldToVendor = _vendor.SellToVendor(_customer, product, 2);

            
            Assert.IsNull(soldToVendor.error);
            Assert.AreEqual(98d, _vendorCurrencies.GetAmount(_gold));
            Assert.AreEqual(80d, _vendorCurrencies.GetAmount(_silver));
            
            Assert.AreEqual(2d, _customerCurrencies.GetAmount(_gold));
            Assert.AreEqual(20d, _customerCurrencies.GetAmount(_silver));
            
            Assert.AreEqual(1, _customerCollection.GetAmount(item));
            Assert.AreEqual(2, _vendorCollection.GetAmount(product));
        }
        
        [Test]
        public void BuyItemFromVendorMultipleCurrenciesTest()
        {
            var item = new CollectionItemInstance(Guid.NewGuid(), new ItemDefinition(Guid.NewGuid())
            {
                maxStackSize = 5, 
                buyPrice = new [] { new CurrencyDecorator(_gold, 1d), new CurrencyDecorator(_silver, 40d),  }, 
                sellPrice = new [] { new CurrencyDecorator(_gold, 1d), new CurrencyDecorator(_silver, 10d),  }
            });
            
            var product = ToProduct(item);
            
            _vendorCollection.Add(product, 3);

            _customerCurrencies.Add(_gold, 100);
            _customerCurrencies.Add(_silver, 100);

            
            var soldToVendor = _vendor.BuyFromVendor(_customer, product.item, 2);

            
            Assert.IsNull(soldToVendor.error);
            Assert.AreEqual(2d, _vendorCurrencies.GetAmount(_gold));
            Assert.AreEqual(80d, _vendorCurrencies.GetAmount(_silver));
            
            Assert.AreEqual(98d, _customerCurrencies.GetAmount(_gold));
            Assert.AreEqual(20d, _customerCurrencies.GetAmount(_silver));
            
            Assert.AreEqual(2, _customerCollection.GetAmount(item));
            Assert.AreEqual(1, _vendorCollection.GetAmount(product));
        }

        [Test]
        public void VendorWithInifiteCurrencyTest()
        {
            _customerCollection.Add(_item1, 3);
            _vendor.vendorCurrencies = new InfiniteCurrencyCollection();

            var canSellToVendor = _vendor.CanSellToVendor(_customer, _product1);
            
            Assert.IsNull(canSellToVendor.error);
            Assert.AreEqual(true, canSellToVendor.result);
        }
        
        [Test]
        public void VendorWithoutInifiteCurrencyTest()
        {
            _customerCollection.Add(_item1, 3);

            var canSellToVendor = _vendor.CanSellToVendor(_customer, _product1);
            
            Assert.AreEqual(Errors.CurrencyCollectionDoesNotContainCurrency, canSellToVendor.error);
            Assert.AreEqual(false, canSellToVendor.result);
        }

        [Test]
        public void SellItemToVendorCollectionEntryTest()
        {
            _customerCurrencies.Add(_gold, 100);
            _customerCollection.Add(_item1, 3);

            _vendorCurrencies.Add(_gold, 100);
            Assert.AreEqual(_customerCollection, _item1.collectionEntry.collection);
            
            var sold = _vendor.SellToVendor(_customer, _product1, 2);
            Assert.IsNull(sold.error);
            // NOTE: _item1 should've gotten duplicated and added to the vendor's collection. The 1 remaining item stays in the player's inventory (our reference).
            Assert.AreEqual(_customerCollection, _item1.collectionEntry.collection);
            Assert.AreEqual(_item1, sold.result.item.item);
            Assert.AreNotSame(_item1, sold.result.item);
            Assert.AreEqual(_vendorCollection, sold.result.item.collectionEntry.collection);
            
            var bought = _vendor.BuyFromVendor(_customer, _item1, 1);
            Assert.IsNull(bought.error);
            Assert.AreEqual(_customerCollection, bought.result.item.collectionEntry.collection);
            Assert.AreEqual(_customerCollection, _item1.collectionEntry.collection);

            var amount = _customerCollection.GetAmount(_item1);
            var removed = _customerCollection.Remove(_item1, 1);
            Assert.AreEqual(2, amount);
            Assert.IsNull(removed.error);
            Assert.AreEqual(_customerCollection, _item1.collectionEntry.collection);

            Assert.AreEqual(1, _vendorCollection.GetAmount(_product1));
            Assert.AreEqual(1, _customerCollection.GetAmount(_item1));
        }

    }
}