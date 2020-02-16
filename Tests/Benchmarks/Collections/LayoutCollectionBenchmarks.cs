using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Tests;
using NUnit.Framework;

namespace Devdog.Rucksack.Benchmarks
{
    public class LayoutCollectionBenchmarks
    {
        [Test]
        public void AddItemToCollectionPerformance()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var c = new LayoutCollection<IItemInstance>(16, 4);

            BenchmarkUtility.Profile("Add item to collection of 4x4", 10000, () =>
            {
                var added = c.Add((IItemInstance)item.Clone(), 1);
            });
        }
        
        [Test]
        public void RemoveItemFromCollectionTest()
        {
            var itemDef = new ItemDefinition(Guid.NewGuid()) { maxStackSize = 999 };
            var item = new ItemInstance(Guid.NewGuid(), itemDef);
            
            var c = new LayoutCollection<IItemInstance>(16, 4);
            c.Add(item, 10000);
            
            BenchmarkUtility.Profile("Remove item to collection of 4x4", 10000, () =>
            {
                var removed = c.Remove(item, 1);
            });
        }
        
        [Test]
        public void CloneCollectionPerformance()
        {
            var c = new LayoutCollection<IItemInstance>(16, 4);

            BenchmarkUtility.Profile("Clone collection of 4x4", 10000, () =>
            {
                var clone = c.Clone();
            });
        }
        
        [Test]
        public void SimulationCollectionTest()
        {
            var c = new LayoutCollection<IItemInstance>(16, 4);

            BenchmarkUtility.Profile("Simulation collection of size 4x4", 10000, () =>
            {
                using (new CollectionSimulation<LayoutCollection<IItemInstance>>(c))
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
            
            var c = new LayoutCollection<IItemInstance>(16, 4);

            BenchmarkUtility.Profile("Simulation collection of size 4x4", 10000, () =>
            {
                var amount = c.GetCanAddAmount(item);
            });
        }
    }
}