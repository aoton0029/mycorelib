using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class UserPermissionProfile
    {
        public string UserId { get; private set; }

        public BitArray AssignedRoleBitMask { get; set; }
        public BitArray AdditionalPermissionBitMask { get; set; }
        public BitArray DeniedPermissionBitMask { get; set; }
        public List<int> AssignedRoleIds => AssignedRoleBitMask.GetTrueIndices();
        public List<int> AdditionalPermissionIds => AdditionalPermissionBitMask.GetTrueIndices();
        public List<int> DeniedPermissionIds => DeniedPermissionBitMask.GetTrueIndices();

        public UserPermissionProfile(string userId, int capacity=32)
        {
            UserId = userId;
            AssignedRoleBitMask = new BitArray(capacity);
            AdditionalPermissionBitMask = new BitArray(capacity);
            DeniedPermissionBitMask = new BitArray(capacity);
        }

        #region ロール
        public void AssignRole(int id)
        {
            EnsureCapacity_Role(id);
            AssignedRoleBitMask.Set(id, true);
        }

        public void UnassignRole(int id)
        {
            AssignedRoleBitMask.Set(id, false);
        }

        public bool HasRole(int id)
        {
            return AssignedRoleBitMask.Get(id);
        }

        public void FromRoleIds(IEnumerable<int> roleIds)
        {
            AssignedRoleBitMask.SetAll(false);
            foreach (var id in roleIds)
            {
                EnsureCapacity_Role(id);
                AssignedRoleBitMask.Set(id, true);
            }
        }

        public void EnsureCapacity_Role(int requireId)
        {
            if (requireId >= AssignedRoleBitMask.Length)
            {
                // 必要なサイズの2倍のサイズに拡張
                int newSize = Math.Max(requireId + 1, AssignedRoleBitMask.Length * 2);
                var newBitMask = new BitArray(newSize);
                // 既存のロールをコピー
                for (int i = 0; i < AssignedRoleBitMask.Length; i++)
                {
                    newBitMask[i] = AssignedRoleBitMask[i];
                }
                AssignedRoleBitMask = newBitMask;
            }
        }
        #endregion

        #region 追加権限
        public void GrantAdditionalPermission(int permId)
        {
            AdditionalPermissionBitMask.Set(permId, true);
            DeniedPermissionBitMask.Set(permId, false);
        }

        public void RevokeAdditionalPermission(int permId)
        {
            AdditionalPermissionBitMask.Set(permId, false);
        }

        public bool HasAdditionalPermission(int permId)
        {
            return AdditionalPermissionBitMask.Get(permId);
        }

        public void FromAdditionalPermissionIds(IEnumerable<int> permIds)
        {
            AdditionalPermissionBitMask.SetAll(false);
            DeniedPermissionBitMask.SetAll(false);
            foreach (var id in permIds)
            {
                EnsureCapacity_AdditionalPermission(id);
                AdditionalPermissionBitMask.Set(id, true);
            }
        }

        public void EnsureCapacity_AdditionalPermission(int requireId)
        {
            if (requireId >= AdditionalPermissionBitMask.Length)
            {
                // 必要なサイズの2倍のサイズに拡張
                int newSize = Math.Max(requireId + 1, AdditionalPermissionBitMask.Length * 2);
                var newBitMask = new BitArray(newSize);
                // 既存の追加権限をコピー
                for (int i = 0; i < AdditionalPermissionBitMask.Length; i++)
                {
                    newBitMask[i] = AdditionalPermissionBitMask[i];
                }
                AdditionalPermissionBitMask = newBitMask;
            }
        }
        #endregion

        #region 拒否権限
        public void DenyPermission(int permId)
        {
            DeniedPermissionBitMask.Set(permId, true);
            AdditionalPermissionBitMask.Set(permId, false);
        }

        public void RemoveDeniedPermission(int permId)
        {
            DeniedPermissionBitMask.Set(permId, false);
        }

        public bool HasDeniedPermission(int permId)
        {
            return DeniedPermissionBitMask.Get(permId);
        }

        public void FromDeniedPermissionIds(IEnumerable<int> permIds)
        {
            DeniedPermissionBitMask.SetAll(false);
            AdditionalPermissionBitMask.SetAll(false);
            foreach (var id in permIds)
            {
                EnsureCapacity_DeniedPermission(id);
                DeniedPermissionBitMask.Set(id, true);
            }
        }

        public void EnsureCapacity_DeniedPermission(int requireId)
        {
            if (requireId >= DeniedPermissionBitMask.Length)
            {
                // 必要なサイズの2倍のサイズに拡張
                int newSize = Math.Max(requireId + 1, DeniedPermissionBitMask.Length * 2);
                var newBitMask = new BitArray(newSize);
                // 既存の拒否権限をコピー
                for (int i = 0; i < DeniedPermissionBitMask.Length; i++)
                {
                    newBitMask[i] = DeniedPermissionBitMask[i];
                }
                DeniedPermissionBitMask = newBitMask;
            }
        }
        #endregion

    }

}
