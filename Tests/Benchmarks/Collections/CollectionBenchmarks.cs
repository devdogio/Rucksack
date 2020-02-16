using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Tests;
using NUnit.Framework;

namespace Devdog.Rucksack.Benchmarks
{
    public class CollectionBenchmarks
    {
        [Test]
        public void AddItemToCollectionPerformance()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            var c = new Collection<IItemInstance>(16, new NullLogger());

            var items = new IItemInstance[10001];
            for (int j = 0; j < items.Length; j++)
            {
                items[j] = (IItemInstance)item.Clone();
            }
            
            int i = 0;
            BenchmarkUtility.Profile("Add item to collection of 16", 10000, () =>
            {
                var added = c.Add(items[i++], 1);
            });
        }
        
        [Test]
        public void RemoveItemFromCollectionTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var c = new Collection<IItemInstance>(16);
            c.Add(item, 10000);
            
            BenchmarkUtility.Profile("Remove item to collection of 16", 10000, () =>
            {
                var removed = c.Remove(item, 1);
            });
        }
        
        [Test]
        public void CloneCollectionPerformance()
        {
            var c = new Collection<IItemInstance>(16);

            BenchmarkUtility.Profile("Clone collection of 16", 10000, () =>
            {
                var clone = c.Clone();
            });
        }
        
        [Test]
        public void SimulationCollectionTest()
        {
            var c = new Collection<IItemInstance>(16);

            BenchmarkUtility.Profile("Simulation collection of size 16", 10000, () =>
            {
                using (new CollectionSimulation<Collection<IItemInstance>>(c))
                {
                    // Do something...
                    
                }
            });
        }
        
        [Test]
        public void GetCanAddAmountTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 50 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var c = new Collection<IItemInstance>(16);

            BenchmarkUtility.Profile("Simulation collection of size 16", 10000, () =>
            {
                var amount = c.GetCanAddAmount(item);
            });
        }
    }
}