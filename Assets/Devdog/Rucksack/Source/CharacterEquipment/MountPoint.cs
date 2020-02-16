using System;

namespace Devdog.Rucksack.CharacterEquipment
{
    public sealed class MountPoint<T> : IMountPoint<T>
        where T : class, IEquatable<T>, IEquippable<T>
    {
        public T currentMountedItem { get; private set; }
        public string name { get; set; }

        public bool hasMountedItem
        {
            get { return currentMountedItem != null; }
        }

        public void Mount(T item)
        {
            // Nothing to visually mount
            currentMountedItem = item;
        }

        public void Clear()
        {
            // Nothing to clear
            currentMountedItem = default(T);
        }
    }
}