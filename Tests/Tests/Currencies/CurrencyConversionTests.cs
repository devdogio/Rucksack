using System;
using System.Collections.Generic;
using Devdog.Rucksack.Currencies;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class CurrencyConversionTests
    {
        private CurrencyDoubleConverter _converter;

        private CurrencyCollection _currencyCollection;
        private ICurrency _gold;
        private ICurrency _silver;
        private ICurrency _copper;
        private ICurrency _diamonds;
        private ICurrency _guildCredits;
        
        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _converter = new CurrencyDoubleConverter();
            
            _currencyCollection = new CurrencyCollection();
            _gold = new Currency(Guid.NewGuid(), "Gold", "GOLD", 2, double.MaxValue);
            _silver = new Currency(Guid.NewGuid(), "Silver", "SILVER", 5, double.MaxValue);
            _copper = new Currency(Guid.NewGuid(), "Copper", "COPPER", 5, double.MaxValue);
            _diamonds = new Currency(Guid.NewGuid(), "Diamonds", "DIAMONDS", 0, double.MaxValue);
            _guildCredits = new Currency(Guid.NewGuid(), "Guild Credits", "GUILD", 0, double.MaxValue);

            _gold.conversionTable = new ConversionTable<ICurrency, double>(new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>()
            {
                {_diamonds, new ConversionTable<ICurrency, double>.Row(0.01d)},
                {_silver, new ConversionTable<ICurrency, double>.Row(100d)},
            });

            _silver.conversionTable = new ConversionTable<ICurrency, double>(new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>()
            {
                {_gold, new ConversionTable<ICurrency, double>.Row(0.01d)},
                {_copper, new ConversionTable<ICurrency, double>.Row(100d)},
            });
            
            _copper.conversionTable = new ConversionTable<ICurrency, double>(new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>()
            {
                {_silver, new ConversionTable<ICurrency, double>.Row(0.01d)},
                {_guildCredits, new ConversionTable<ICurrency, double>.Row(2d)},
            });
            
            _currencyCollection.Add(_gold, 10);
            _currencyCollection.Add(_silver, 10);
        }

        [Test]
        public void ConvertGoldToSilverTest()
        {
            var result = _converter.Convert(_gold, 3, _silver);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(300, result.result);
        }

        [Test]
        public void ConvertSilverToGoldTest()
        {
            var result = _converter.Convert(_silver, 3, _gold);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(0.03d, result.result);
        }
        
        [Test]
        public void ConvertNegativeTest()
        {
            var result = _converter.Convert(_silver, -3, _gold);
            
            Assert.AreEqual(Errors.CurrencyCanNotConvertToTarget, result.error);
            Assert.AreEqual(0, result.result);
        }
        
        [Test]
        public void ConvertToSelfTest()
        {
            var result = _converter.Convert(_silver, 3, _silver);
            
            Assert.IsNull(result.error);
            Assert.AreEqual(3d, result.result);
        }

        [Test]
        public void GetAllConversionsTest()
        {
            var dict = _converter.GetAllConversions(_gold);
            
            Assert.AreEqual(5, dict.Count);
            Assert.AreEqual(0.01d, dict[_diamonds].conversionRate);
            Assert.AreEqual(1.0d, dict[_gold].conversionRate);
            Assert.AreEqual(100d, dict[_silver].conversionRate);
            Assert.AreEqual(10000d, dict[_copper].conversionRate);
            Assert.AreEqual(20000d, dict[_guildCredits].conversionRate);
        }
        
        [Test]
        public void GetAllConversionsMultipleStepsForOptimalConversionTest()
        {
            _gold.conversionTable = new ConversionTable<ICurrency, double>(new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>()
            {
                {_diamonds, new ConversionTable<ICurrency, double>.Row(0.01d)},
                {_silver, new ConversionTable<ICurrency, double>.Row(100d)},
            });

            _silver.conversionTable = new ConversionTable<ICurrency, double>(new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>()
            {
                {_gold, new ConversionTable<ICurrency, double>.Row(0.01d)},
                {_diamonds, new ConversionTable<ICurrency, double>.Row(0.012) }, // 1 gold = 100 silver. 1 silver = 0.012 diamonds = 1 gold = 0.012*100 = 1.2d diamonds.
                {_copper, new ConversionTable<ICurrency, double>.Row(100d)},
            });
            
            _copper.conversionTable = new ConversionTable<ICurrency, double>(new Dictionary<ICurrency, ConversionTable<ICurrency, double>.Row>()
            {
                {_silver, new ConversionTable<ICurrency, double>.Row(0.01d)},
                {_diamonds, new ConversionTable<ICurrency, double>.Row(0.008) }, // 1 gold = 100 silver. 1 Silver = 100 copper. copper silver = 0.008 diamonds = 1 gold = 0.008*100000 = 80d diamonds.
                {_guildCredits, new ConversionTable<ICurrency, double>.Row(2d)},
            });
            
            
            var dict = _converter.GetAllConversions(_gold);
            
            Assert.AreEqual(5, dict.Count);
            Assert.AreEqual(80d, dict[_diamonds].conversionRate);
            Assert.AreEqual(1.0d, dict[_gold].conversionRate);
            Assert.AreEqual(100d, dict[_silver].conversionRate);
            Assert.AreEqual(10000d, dict[_copper].conversionRate);
            Assert.AreEqual(20000d, dict[_guildCredits].conversionRate);
        }
    }
}