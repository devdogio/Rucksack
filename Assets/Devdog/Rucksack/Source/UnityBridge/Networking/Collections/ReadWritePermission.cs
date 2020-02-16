using System;

namespace Devdog.Rucksack.Collections
{
    [Flags]
    public enum ReadWritePermission : byte
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = 3,
    }

    public static class ReadWritePermissionExtensionMethods
    {
        public static bool CanWrite(this ReadWritePermission permission)
        {
            return permission.HasFlag(ReadWritePermission.Write);
        }

        public static bool CanRead(this ReadWritePermission permission)
        {
            return permission.HasFlag(ReadWritePermission.Read);
        }
    }
}