using System;
using Devdog.General2;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using NUnit.Framework;

namespace Devdog.Rucksack.Tests
{
    public class ItemsSerializationTest
    {
        private ItemDefinition _itemDef;
        private ItemDefinition _childItemDef;

        private ICurrency _gold;
        
        [SetUp]
        public void Setup()
        {
            _gold = new Currency(Guid.NewGuid(), "Gold", "GLD", 0, 999);
            _itemDef = new ItemDefinition(Guid.NewGuid())
            {
                name = "Root item",
                description = "Some description of root item",
                layoutShape = new ComplexShape2D(new bool[3,3]
                {
                    {true, false, false},
                    {false, true, false},
                    {false, false, true},
                }),
                maxStackSize = 3,
                buyPrice = new CurrencyDecorator<double>[]
                {
                    new CurrencyDecorator(_gold, 10d), 
                },
                sellPrice = new CurrencyDecorator<double>[]
                {
                    new CurrencyDecorator(_gold, 7d), 
                }
            };
            _childItemDef = new ItemDefinition(Guid.NewGuid(), _itemDef)
            {
                name = "Child item",
                description = "Child description",
                maxStackSize = 10,
            };
        }

        [Test]
        public void SerializeItemDefinitionTest()
        {
            var json = JsonSerializer.Serialize(_itemDef, null);
            var child = JsonSerializer.Serialize(_childItemDef, null);
            
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Length > 0);
        }
        
        
        
    }
}