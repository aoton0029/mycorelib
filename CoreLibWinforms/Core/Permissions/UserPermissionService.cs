using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class UserPermissionService
    {
        public static string FilePath = "UserPermission.json";
        private readonly Dictionary<string, UserPermissionProfile> _users = new Dictionary<string, UserPermissionProfile>();

        #region UserPermission
        public UserPermissionProfile RegisterNewUserPermission(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (_users.ContainsKey(userId))
                throw new InvalidOperationException($"User with ID {userId} already exists.");

            var permission = new UserPermissionProfile(userId);
            _users[userId] = permission;
            return permission;
        }

        public UserPermissionProfile GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (_users.TryGetValue(userId, out UserPermissionProfile user))
                return user;

            return null;
        }

        public void AssignRoleToUser(string userId, int roleId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.AssignRole(roleId);
        }

        public void UnassignRoleFromUser(string userId, int roleId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.UnassignRole(roleId);
        }

        public IEnumerable<UserPermissionProfile> GetAllUsers()
        {
            return _users.Values;
        }

        public void GrantAdditionalPermissionToUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.GrantAdditionalPermission(permissionId);
        }

        public void RevokeAdditionalPermissionFromUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.RevokeAdditionalPermission(permissionId);
        }

        public void DenyPermissionForUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.DenyPermission(permissionId);
        }

        public void RemoveDeniedPermissionForUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.RemoveDeniedPermission(permissionId);
        }

        public bool UserHasRole(string userId, int roleId)
        {
            var user = GetUser(userId);
            return user != null && user.HasRole(roleId);
        }

        public IEnumerable<int> GetUserRoleIds(string userId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            return user.AssignedRoleIds;
        }
        #endregion

        public void Save()
        {
            // ユーザー権限プロファイルをシリアライズ可能なデータに変換
            var userDataList = new List<UserPermissionData>();

            foreach (var user in _users.Values)
            {
                var userData = new UserPermissionData
                {
                    UserId = user.UserId,
                    AssignedRoleIds = user.AssignedRoleIds,
                    AdditionalPermissionIds = user.AdditionalPermissionIds,
                    DeniedPermissionIds = user.DeniedPermissionIds
                };

                userDataList.Add(userData);
            }

            // JSONにシリアライズ
            string jsonString = System.Text.Json.JsonSerializer.Serialize(userDataList, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            // ファイルに保存
            System.IO.File.WriteAllText(FilePath, jsonString);
        }

        public void Load()
        {
            // ファイルが存在しない場合は何もしない
            if (!System.IO.File.Exists(FilePath))
                return;

            try
            {
                // JSONファイルを読み込む
                string jsonString = System.IO.File.ReadAllText(FilePath);

                // JSONをデシリアライズ
                var userDataList = System.Text.Json.JsonSerializer.Deserialize<List<UserPermissionData>>(jsonString);

                // 既存のユーザーデータをクリア
                _users.Clear();

                // デシリアライズしたデータから UserPermissionProfile を作成
                if (userDataList != null)
                {
                    foreach (var userData in userDataList)
                    {
                        if (string.IsNullOrEmpty(userData.UserId))
                            continue;

                        // 新しいユーザープロファイルを作成
                        var userProfile = new UserPermissionProfile(userData.UserId);

                        // ロールを設定
                        if (userData.AssignedRoleIds != null && userData.AssignedRoleIds.Count > 0)
                            userProfile.FromRoleIds(userData.AssignedRoleIds);

                        // 追加権限を設定
                        if (userData.AdditionalPermissionIds != null && userData.AdditionalPermissionIds.Count > 0)
                            userProfile.FromAdditionalPermissionIds(userData.AdditionalPermissionIds);

                        // 拒否権限を設定
                        if (userData.DeniedPermissionIds != null && userData.DeniedPermissionIds.Count > 0)
                            userProfile.FromDeniedPermissionIds(userData.DeniedPermissionIds);

                        // ディクショナリに追加
                        _users[userData.UserId] = userProfile;
                    }
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                // JSONのパース中にエラーが発生した場合
                System.Diagnostics.Debug.WriteLine($"JSON deserialize error: {ex.Message}");
            }
            catch (System.IO.IOException ex)
            {
                // ファイル操作中にエラーが発生した場合
                System.Diagnostics.Debug.WriteLine($"File IO error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // その他のエラーが発生した場合
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            }

        }
    }

    public class UserPermissionData
    {
        public string UserId { get; set; }
        public List<int> AssignedRoleIds { get; set; } = new List<int>();
        public List<int> AdditionalPermissionIds { get; set; } = new List<int>();
        public List<int> DeniedPermissionIds { get; set; } = new List<int>();
        public UserPermissionData() { }
        public UserPermissionData(string userId)
        {
            UserId = userId;
        }
    }

    public class UserPermissionProfile
    {
        public string UserId { get; private set; }

        public BitArray AssignedRoleBitMask { get; set; }
        public BitArray AdditionalPermissionBitMask { get; set; }
        public BitArray DeniedPermissionBitMask { get; set; }
        public List<int> AssignedRoleIds => AssignedRoleBitMask.GetTrueIndices();
        public List<int> AdditionalPermissionIds => AdditionalPermissionBitMask.GetTrueIndices();
        public List<int> DeniedPermissionIds => DeniedPermissionBitMask.GetTrueIndices();

        public UserPermissionProfile(string userId, int capacity = 32)
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
