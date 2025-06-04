using CoreLibWinforms.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BitArray Permissions { get; private set; }

        public Role(string name, int initialCapacity = 32)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));

            Name = name;
            Permissions = new BitArray(initialCapacity);
        }

        public void GrantPermission(Permission permission)
        {
            EnsureCapacity(permission.Id + 1);
            Permissions[permission.Id] = true;
        }

        public void RevokePermission(Permission permission)
        {
            if (permission.Id < Permissions.Length)
            {
                Permissions[permission.Id] = false;
            }
        }

        public bool HasPermission(Permission permission)
        {
            return permission.Id < Permissions.Length && Permissions[permission.Id];
        }

        private void EnsureCapacity(int requiredLength)
        {
            if (requiredLength > Permissions.Length)
            {
                // 必要なサイズの2倍のサイズに拡張
                int newSize = Math.Max(requiredLength, Permissions.Length * 2);
                var newPermissions = new BitArray(newSize);

                // 既存の権限をコピー
                for (int i = 0; i < Permissions.Length; i++)
                {
                    newPermissions[i] = Permissions[i];
                }

                Permissions = newPermissions;
            }
        }
    }
}
