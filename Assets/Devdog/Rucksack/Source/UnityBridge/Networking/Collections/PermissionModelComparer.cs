using System.Collections.Generic;

namespace Devdog.Rucksack.Collections
{
    public sealed class PermissionModelComparer<T> : IEqualityComparer<PermissionModel<T>>
    {
        public bool Equals(PermissionModel<T> x, PermissionModel<T> y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        /// <summary>
        /// Note: The permission is ignored in the hascode, this way a collection + permission's hash will ignore the permission. 
        /// When added to a haspmap it will overwrite previous entries.
        /// </summary>
        public int GetHashCode(PermissionModel<T> obj)
        {
            return obj.obj.GetHashCode();
        }
    }
}