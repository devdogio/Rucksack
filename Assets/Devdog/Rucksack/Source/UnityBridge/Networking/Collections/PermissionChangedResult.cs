using System;

namespace Devdog.Rucksack.Collections
{
    public class PermissionChangedResult<TType, TIdentity> : EventArgs
    {
        public readonly TType obj;
        public readonly TIdentity identity;
        public readonly ReadWritePermission permission;

        public PermissionChangedResult(TType obj, TIdentity identity, ReadWritePermission permission)
        {
            this.obj = obj;
            this.identity = identity;
            this.permission = permission;
        }
    }
}