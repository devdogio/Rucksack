//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Devdog.Rucksack.Database.Mongo;
//using Devdog.Rucksack.Items;
//using MongoDB.Bson.Serialization;
//using NUnit.Framework;
//
//namespace Devdog.Rucksack.Tests
//{
//    public class MongoDatabaseTest
//    {
//        private MongoDatabase<ItemDefinition> _database;
//        private Guid _itemGuid;
//        private const string DatabaseName = "Rucksack";
//        private const string CollectionName = "items";
//
//
//        public MongoDatabaseTest()
//        {
//            _itemGuid = Guid.Parse("1a05017f-3e74-448b-ac8c-c93b250faee1");
//            _database = new MongoDatabase<ItemDefinition>("mongodb://localhost:27017", DatabaseName, CollectionName);
////            _database.database.Client.Settings.ConnectTimeout = TimeSpan.FromMilliseconds(500);
//
//        }
//
//        ~MongoDatabaseTest()
//        {
////            ((MongoDatabase<ItemDefinition>)_database).client.DropDatabase(DatabaseName); // Make sure we don't have left over data between our test runs.
//            _database.Dispose();            
//        }
//        
//        [SetUp]
//        public void Setup()
//        {
//        }
//
//        [TearDown]
//        public void TearDown()
//        {
//        }
//
//        [Test]
//        [Timeout(500)]
//        public async Task GetItemTest()
//        {
//            var result = await _database.GetAsync(new Identifier(_itemGuid));
//            
//            Assert.IsNull(result.error);
//            Assert.AreEqual(_itemGuid, result.result.ID);
//        }
//        
//        [Test]
//        [Timeout(500)]
//        public void GetItemQueryTest()
//        {
//            var result = _database.Get(o => o.ID == _itemGuid);
//            
//            Assert.IsNull(result.error);
//            Assert.AreEqual(_itemGuid, result.result.ID);
//        }
//        
//        [Test]
//        [Timeout(500)]
//        public void GetItemQueryTest2()
//        {
//            var result = _database.Get(o => o.ID == Guid.NewGuid());
//            Assert.AreEqual(Errors.DatabaseItemNotFound, result.error);
//            Assert.IsNull(result.result);
//        }
//        
//        [Test]
//        [Timeout(500)]
//        public void SetItemTest()
//        {
//            _database.Set(new Identifier(_itemGuid), new ItemDefinition(_itemGuid){ layoutShape = new SimpleShape2D(4, 2), maxStackSize = 10 });
//        }
//
//    }
//}