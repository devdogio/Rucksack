using System;
using System.Linq;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Tests;
using NUnit.Framework;

namespace Devdog.Rucksack.Benchmarks
{
    public class ItemBenchmarks
    {
        [Test]
        public void ConvertItemDefinitionsToItemBenchmark()
        {
            var def1 = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 3 };
            var def2 = new ItemDefinition(Guid.NewGuid()){ layoutShape = new ComplexShape2D(new bool[,] { {true, true}, {true, false} }) };
            var def3 = new ItemDefinition(Guid.NewGuid()){ maxStackSize = 5};

            var arr = new[] {def1, def2, def3};

            BenchmarkUtility.Profile("Convert 3 items from ItemDefinition into item instances.", 10000, () =>
            {
                var instances = arr.ToItemInstances<ItemInstance>().ToArray();
            });
        }
        
      
    }
}