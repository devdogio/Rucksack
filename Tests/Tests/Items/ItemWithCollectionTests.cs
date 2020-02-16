using System;
using System.Collections.Generic;
using Devdog.Rucksack.Items;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Devdog.Rucksack.Tests
{
    internal class ItemWithCollectionTests
    {

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;
        }

        [Test]
        public void TestCreateInstance()
        {
            var instance = new ItemWithCollectionMock(System.Guid.NewGuid(), new ItemDefinition(System.Guid.NewGuid())
            {
                name = "Some Name",
                description = "Some description",
                maxStackSize = 1,
            });

            var inst = new ItemInstance(System.Guid.NewGuid(), new ItemDefinition(System.Guid.NewGuid()));
            var set = instance.collection.Set(0, inst, 1);
            Assert.IsNull(set.error);
            Assert.AreEqual(inst, instance.collection[0]);
        }
    }
}