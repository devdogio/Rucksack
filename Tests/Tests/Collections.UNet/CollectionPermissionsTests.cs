using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.Rucksack.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Devdog.Rucksack.Tests
{
    public class CollectionPermissionsTests
    {
        public class MockCollection : INetworkCollection<Identity>
        {
            public Guid ID { get; set; }
            public string collectionName { get; set; }
            public Identity owner { get; set; }
        }

        public class Identity : INetworkObject<int>
        {
            public Guid ID { get; set; }
            public int owner { get; set; }
        }
        
        private MockCollection _collection1;
        private MockCollection _collection2;
        private MockCollection _collection3;

        private Identity _identity1;
        private Identity _identity2;
        private Identity _identity3;
        
        private NetworkPermissionsMap<MockCollection, Identity> _permissions;

        [SetUp]
        public void Setup()
        {
            UnityEngine.Assertions.Assert.raiseExceptions = true;

            _identity1 = new Identity(){ owner = 1 };
            _identity2 = new Identity() { owner = 2 };
            _identity3 =  new Identity(){ owner = 3 };

            _collection1 = new MockCollection() { ID = System.Guid.NewGuid(), collectionName = "Collection1", owner = _identity1};
            _collection2 = new MockCollection() { ID = System.Guid.NewGuid(), collectionName = "Collection2", owner = _identity1 };
            _collection3 = new MockCollection() { ID = System.Guid.NewGuid(), collectionName = "Collection3", owner = _identity1 };
            
            _permissions = new NetworkPermissionsMap<MockCollection, Identity>();
        }

        [TearDown]
        public void Teardown()
        {
            _permissions.RevokeAll();
        }

        [Test]
        public void EqualityTest2()
        {
            var comparer = EqualityComparer<Identity>.Default;
            Assert.IsTrue(comparer.Equals(_identity1, _identity1));
            Assert.IsFalse(comparer.Equals(_identity1, _identity2));
            Assert.IsFalse(comparer.Equals(_identity1, _identity3));

        }
        
        [Test]
        public void HashSetEqualityComparerShouldIgnorePermissionFieldTest()
        {
            var set = new HashSet<PermissionModel<MockCollection>>(new PermissionModelComparer<MockCollection>());
            var added = set.Add(new PermissionModel<MockCollection>(_collection1, ReadWritePermission.ReadWrite));

            // The equality operator ignores the permission on the PermissionModel object, however,
            // the HashSet<T> doesn't update a value if the equality operator considers the object equal.
            // Because of this the model is never set in the hashset; The only work around I could find was removing and re-adding.
            set.Remove(new PermissionModel<MockCollection>(_collection1, ReadWritePermission.None));
                
            var added2 = set.Add(new PermissionModel<MockCollection>(_collection1, ReadWritePermission.None));
            
            Assert.IsTrue(added);
            Assert.IsTrue(added2);
            
            Assert.AreEqual(1, set.Count);
        }
        
        [Test]
        public void SetCollectionPermissionTest()
        {
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            
            var permission1 = _permissions.GetPermission(_collection2, _identity1);
            var permission2 = _permissions.GetPermission(_collection2, _identity2);
            var permission3 = _permissions.GetPermission(_collection2, _identity3);
            
            Assert.AreEqual(permission1, ReadWritePermission.ReadWrite);
            Assert.AreEqual(permission2, ReadWritePermission.None);
            Assert.AreEqual(permission3, ReadWritePermission.None);
        }
        
        [Test]
        public void SharedPermissionTest()
        {
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity3, ReadWritePermission.None);
            
            var permission1 = _permissions.GetPermission(_collection2, _identity1);
            var permission2 = _permissions.GetPermission(_collection2, _identity2);
            var permission3 = _permissions.GetPermission(_collection2, _identity3);
            
            Assert.AreEqual(permission1, ReadWritePermission.ReadWrite);
            Assert.AreEqual(permission2, ReadWritePermission.ReadWrite);
            Assert.AreEqual(permission3, ReadWritePermission.None);
        }
        
        [Test]
        public void SetPermissionMultipleTimesTest()
        {
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.Read);
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.None);
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            
            var permission1 = _permissions.GetPermission(_collection2, _identity1);
            var permission2 = _permissions.GetPermission(_collection2, _identity2);
            var permission3 = _permissions.GetPermission(_collection2, _identity3);
            
            Assert.AreEqual(permission1, ReadWritePermission.ReadWrite);
            Assert.AreEqual(permission2, ReadWritePermission.None);
            Assert.AreEqual(permission3, ReadWritePermission.None);
            
            Assert.AreEqual(1, _permissions.GetAll().Count());
            Assert.AreEqual(1, _permissions.GetAllIdentitiesWithPermission(_collection2).Count());
            Assert.AreEqual(_identity1, _permissions.GetAllIdentitiesWithPermission(_collection2).ToArray()[0]);
        }
        
        [Test]
        public void SetPermissionMultipleTimesTest2()
        {
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            var permission1 = _permissions.GetPermission(_collection2, _identity1);

            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.Read);
            var permission2 = _permissions.GetPermission(_collection2, _identity1);

            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.None);
            var permission3 = _permissions.GetPermission(_collection2, _identity1);
            
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            var permission4 = _permissions.GetPermission(_collection2, _identity1);
            
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.None);
            var permission5 = _permissions.GetPermission(_collection2, _identity1);

            Assert.AreEqual(ReadWritePermission.ReadWrite, permission1);
            Assert.AreEqual(ReadWritePermission.Read, permission2);
            Assert.AreEqual(ReadWritePermission.None, permission3);
            Assert.AreEqual(ReadWritePermission.ReadWrite, permission4);
            Assert.AreEqual(ReadWritePermission.None, permission5);

            var perms = _permissions.GetAll().ToArray();
            Assert.AreEqual(1, perms.Length);
            Assert.AreEqual(_collection2, perms[0].Item1.obj);
            Assert.AreEqual(_identity1, perms[0].Item2.ToArray()[0]);
            Assert.AreEqual(1, _permissions.GetAllIdentitiesWithPermission(_collection2).Count());
            Assert.AreEqual(_identity1, _permissions.GetAllIdentitiesWithPermission(_collection2).ToArray()[0]);
        }
        
        [Test]
        public void SetMultipleIdentitiesPermissionTest()
        {
            _permissions.SetPermission(_collection2, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity3, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity3, ReadWritePermission.None);
            
            var permission1 = _permissions.GetPermission(_collection2, _identity1);
            var permission2 = _permissions.GetPermission(_collection2, _identity2);
            var permission3 = _permissions.GetPermission(_collection2, _identity3);
            
            Assert.AreEqual(permission1, ReadWritePermission.ReadWrite);
            Assert.AreEqual(permission2, ReadWritePermission.ReadWrite);
            Assert.AreEqual(permission3, ReadWritePermission.None);
            
            Assert.AreEqual(2, _permissions.GetAll().Count());
            Assert.AreEqual(2, _permissions.GetAllIdentitiesWithPermission(_collection2).Count());
            Assert.AreEqual(_identity1, _permissions.GetAllIdentitiesWithPermission(_collection2).ToArray()[0]);
            Assert.AreEqual(_identity2, _permissions.GetAllIdentitiesWithPermission(_collection2).ToArray()[1]);
        }

        [Test]
        public void RemovePermissionTest()
        {
            _permissions.SetPermission(_collection2, _identity3, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity3, ReadWritePermission.None);
            
            var permission1 = _permissions.GetPermission(_collection2, _identity1);
            var permission2 = _permissions.GetPermission(_collection2, _identity2);
            var permission3 = _permissions.GetPermission(_collection2, _identity3);
            
            Assert.AreEqual(ReadWritePermission.None, permission1);
            Assert.AreEqual(ReadWritePermission.None, permission2);
            Assert.AreEqual(ReadWritePermission.None, permission3);
        }

        [Test]
        public void GetIdentitiesTest()
        {
            _permissions.SetPermission(_collection1, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity1, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity3, ReadWritePermission.Read);

            var id1Collections = _permissions.GetAllWithPermission(_identity1).ToArray();
            var id2Collections = _permissions.GetAllWithPermission(_identity2).ToArray();
            var id3Collections = _permissions.GetAllWithPermission(_identity3).ToArray();
            
            Assert.AreEqual(2, id1Collections.Length);
            Assert.AreEqual(_collection1, id1Collections[0].obj);            
            Assert.AreEqual(ReadWritePermission.ReadWrite, id1Collections[0].permission);            
            Assert.AreEqual(_collection3, id1Collections[1].obj);            
            Assert.AreEqual(ReadWritePermission.Read, id1Collections[1].permission);            
            
            Assert.AreEqual(1, id2Collections.Length);
            Assert.AreEqual(_collection1, id2Collections[0].obj);
            Assert.AreEqual(ReadWritePermission.Read, id2Collections[0].permission);
            
            Assert.AreEqual(1, id3Collections.Length);
            Assert.AreEqual(_collection3, id3Collections[0].obj);
            Assert.AreEqual(ReadWritePermission.Read, id3Collections[0].permission);
        }
        
        [Test]
        public void GetCollectionsTest()
        {
            _permissions.SetPermission(_collection1, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity1, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity3, ReadWritePermission.Read);

            var id1Identities = _permissions.GetAllIdentitiesWithPermission(_collection1).ToArray();
            var id2Identities = _permissions.GetAllIdentitiesWithPermission(_collection2).ToArray();
            var id3Identities = _permissions.GetAllIdentitiesWithPermission(_collection3).ToArray();
            
            Assert.AreEqual(2, id1Identities.Length);
            Assert.AreEqual(_identity1, id1Identities[0]);            
            Assert.AreEqual(_identity2, id1Identities[1]);            
            
            Assert.AreEqual(0, id2Identities.Length);
            
            Assert.AreEqual(2, id3Identities.Length);
            Assert.AreEqual(_identity1, id3Identities[0]);
            Assert.AreEqual(_identity3, id3Identities[1]);
        }

        [Test]
        public void RevokeAllForIdentity()
        {
            _permissions.SetPermission(_collection1, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity1, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity3, ReadWritePermission.Read);
            
            _permissions.RevokeAllForIdentity(_identity1);
            
            var id1Identities = _permissions.GetAllIdentitiesWithPermission(_collection1).ToArray();
            var id2Identities = _permissions.GetAllIdentitiesWithPermission(_collection2).ToArray();
            var id3Identities = _permissions.GetAllIdentitiesWithPermission(_collection3).ToArray();
            
            Assert.AreEqual(1, id1Identities.Length);
            Assert.AreEqual(_identity2, id1Identities[0]);            
            Assert.AreEqual(0, id2Identities.Length);
            Assert.AreEqual(1, id3Identities.Length);
            Assert.AreEqual(_identity3, id3Identities[0]);
        }

        [Test]
        public void RevokeAll()
        {
            _permissions.SetPermission(_collection1, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity1, ReadWritePermission.Read);
            _permissions.SetPermission(_collection3, _identity3, ReadWritePermission.Read);

            _permissions.RevokeAll();
            
            Assert.AreEqual(0, _permissions.GetAll().Count());
            Assert.AreEqual(0, _permissions.GetAllWithPermission(_identity1).Count());
            Assert.AreEqual(0, _permissions.GetAllIdentitiesWithPermission(_collection1).Count());
        }
        
                
        [Test]
        public void SetPermissionEventTest()
        {
            int eventFireCount = 0;
            PermissionChangedResult<MockCollection, Identity> changedResult = null;

            _permissions.AddEventListener(_identity2, data =>
            {
                changedResult = data;
                eventFireCount++;
            });
            
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);

            Assert.AreEqual(3, eventFireCount);
            Assert.AreEqual(_identity2, changedResult.identity);
            Assert.AreEqual(_collection1, changedResult.obj);
            Assert.AreEqual(ReadWritePermission.None, changedResult.permission);
        }
        
        [Test]
        public void SetPermissionEventTest2()
        {
            int eventFireCount = 0;
            PermissionChangedResult<MockCollection, Identity> changedResult = null;

            _permissions.AddEventListener(_identity2, data =>
            {
                changedResult = data;
                eventFireCount++;
            });
            
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);
            
            // These should be ignored by the event, as they're from another identity.
            _permissions.SetPermission(_collection1, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity3, ReadWritePermission.ReadWrite);

            Assert.AreEqual(3, eventFireCount);
            Assert.AreEqual(_identity2, changedResult.identity);
            Assert.AreEqual(_collection1, changedResult.obj);
            Assert.AreEqual(ReadWritePermission.None, changedResult.permission);
        }
        
        [Test]
        public void AddNewCollectionWithPermissionEventTest()
        {
            int eventFireCount = 0;
            PermissionChangedResult<MockCollection, Identity> changedResult = null;

            _permissions.SetPermission(_collection1, _identity1, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection1, _identity3, ReadWritePermission.ReadWrite);
            
            _permissions.AddEventListener(_identity2, data =>
            {
                changedResult = data;
                eventFireCount++;
            });
            
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);
            
            Assert.AreEqual(2, eventFireCount);
            Assert.AreEqual(_identity2, changedResult.identity);
            Assert.AreEqual(_collection1, changedResult.obj);
            Assert.AreEqual(ReadWritePermission.None, changedResult.permission);
        }
        
        [Test]
        public void RemoveEventListenerTest()
        {
            int eventFireCount = 0;
            PermissionChangedResult<MockCollection, Identity> changedResult = null;

            var callback1 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult = data;
                eventFireCount++;
            });
            _permissions.AddEventListener(_identity2, callback1);
            
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            
            var removed = _permissions.RemoveEventListener(_identity2, callback1);
            
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);
            
            Assert.AreEqual(1, eventFireCount);
            Assert.AreEqual(_identity2, changedResult.identity);
            Assert.AreEqual(_collection1, changedResult.obj);
            Assert.AreEqual(ReadWritePermission.ReadWrite, changedResult.permission);
            
            Assert.IsTrue(removed);
        }
        
        [Test]
        public void RemoveEventListenerTest2()
        {
            int eventFireCount1 = 0;
            int eventFireCount2 = 0;
            int eventFireCount3 = 0;
            PermissionChangedResult<MockCollection, Identity> changedResult1 = null;
            PermissionChangedResult<MockCollection, Identity> changedResult2 = null;
            PermissionChangedResult<MockCollection, Identity> changedResult3 = null;

            var callback1 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult1 = data;
                eventFireCount1++;
            });
            var callback2 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult2 = data;
                eventFireCount2++;
            });
            var callback3 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult3 = data;
                eventFireCount3++;
            });
            _permissions.AddEventListener(_identity2, callback1);
            _permissions.AddEventListener(_identity2, callback2);
            _permissions.AddEventListener(_identity2, callback3);
            
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            
            _permissions.RemoveEventListener(_identity2, callback1);
            
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            
            _permissions.RemoveEventListener(_identity2, callback2);

            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);
            
            Assert.AreEqual(1, eventFireCount1);
            Assert.AreEqual(2, eventFireCount2);
            Assert.AreEqual(3, eventFireCount3);
            Assert.AreEqual(_identity2, changedResult1.identity);
            Assert.AreEqual(_identity2, changedResult2.identity);
            Assert.AreEqual(_identity2, changedResult3.identity);
            Assert.AreEqual(_collection1, changedResult1.obj);
            Assert.AreEqual(_collection2, changedResult2.obj);
            Assert.AreEqual(_collection1, changedResult3.obj);
            Assert.AreEqual(ReadWritePermission.ReadWrite, changedResult1.permission);
            Assert.AreEqual(ReadWritePermission.Read, changedResult2.permission);
            Assert.AreEqual(ReadWritePermission.None, changedResult3.permission);
        }

        [Test]
        public void RemoveListenerFromNonExistingIdentityTest()
        {
            var callback1 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            { });
            
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);
            
            var removed1 = _permissions.RemoveEventListener(_identity2, callback1);
            var removed2 = _permissions.RemoveEventListener(_identity1, callback1);
            
            Assert.IsFalse(removed1);
            Assert.IsFalse(removed2);
        }

        [Test]
        public void RemoveAllEventListenersForIdentityTest()
        {
            int eventFireCount1 = 0;
            int eventFireCount2 = 0;
            int eventFireCount3 = 0;
            PermissionChangedResult<MockCollection, Identity> changedResult1 = null;
            PermissionChangedResult<MockCollection, Identity> changedResult2 = null;
            PermissionChangedResult<MockCollection, Identity> changedResult3 = null;

            var callback1 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult1 = data;
                eventFireCount1++;
            });
            var callback2 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult2 = data;
                eventFireCount2++;
            });
            var callback3 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            {
                changedResult3 = data;
                eventFireCount3++;
            });
            _permissions.AddEventListener(_identity2, callback1);
            _permissions.AddEventListener(_identity2, callback2);
            _permissions.AddEventListener(_identity2, callback3);
            
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.ReadWrite);
            
            _permissions.RemoveAllEventListenersForIdentity(_identity2);
            
            _permissions.SetPermission(_collection2, _identity2, ReadWritePermission.Read);
            _permissions.SetPermission(_collection1, _identity2, ReadWritePermission.None);
            
            Assert.AreEqual(1, eventFireCount1);
            Assert.AreEqual(1, eventFireCount2);
            Assert.AreEqual(1, eventFireCount3);
            Assert.AreEqual(_identity2, changedResult1.identity);
            Assert.AreEqual(_identity2, changedResult2.identity);
            Assert.AreEqual(_identity2, changedResult3.identity);
            Assert.AreEqual(_collection1, changedResult1.obj);
            Assert.AreEqual(_collection1, changedResult2.obj);
            Assert.AreEqual(_collection1, changedResult3.obj);
            Assert.AreEqual(ReadWritePermission.ReadWrite, changedResult1.permission);
            Assert.AreEqual(ReadWritePermission.ReadWrite, changedResult2.permission);
            Assert.AreEqual(ReadWritePermission.ReadWrite, changedResult3.permission);
        }
        
        [Test]
        public void RemoveEventListenersTest()
        {
            var callback1 = new NetworkPermissionsMap<MockCollection, Identity>.OnPermissionChangedDelegate(data =>
            { });
            
            var removed1 = _permissions.RemoveEventListener(_identity2, null);
            var removed2 = _permissions.RemoveEventListener(_identity2, callback1);
            
            Assert.IsFalse(removed1);
            Assert.IsFalse(removed2);
        }
        
        [Test]
        public void RemoveAllEventListenersTest()
        {
            _permissions.RemoveAllEventListenersForIdentity(_identity2);
            _permissions.RemoveAllEventListenersForIdentity(new Identity(){ID = Guid.NewGuid()});
        }
    }
}