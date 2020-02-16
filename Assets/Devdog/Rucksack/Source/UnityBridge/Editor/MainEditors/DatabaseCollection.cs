using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.Rucksack.Database;

namespace Devdog.Rucksack.Editor
{
    public class DatabaseCollection<T> : ICollection<T>
        where T: class, IIdentifiable
    {
        public IDatabase<T> database { get; set; }

        public bool IsReadOnly { get { return false; } }
        public int Count
        {
            get { return database.GetAll().result.Count(); }
        }


        public DatabaseCollection()
        {
            
        }

        public DatabaseCollection(IDatabase<T> database)
        {
            this.database = database;
        }

        public void Add(T item)
        {
            database.Set(item, item);
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(T item)
        {
            return database.Get(item).error == null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var items = database.GetAll().result.ToArray();
            Array.Copy(items, 0, array, arrayIndex, items.Length);
        }

        public bool Remove(T item)
        {
            return database.Remove(item).result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return database.GetAll().result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}