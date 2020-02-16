namespace Devdog.Rucksack.Collections
{
    public struct PermissionModel<T>
    {
        public readonly T obj;
        public readonly ReadWritePermission permission;

        public PermissionModel(T obj, ReadWritePermission permission)
        {
            this.obj = obj;
            this.permission = permission;
        }
    }
}