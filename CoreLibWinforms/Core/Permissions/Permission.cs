using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Permissions
{
    // 権限を表すクラス
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Permission(int id, string name, string description = "")
        {
            Id = id;
            Name = name;
            Description = description;
        }

        // enum型から権限を作成するジェネリックファクトリメソッド
        public static Permission FromEnum<TEnum>(TEnum enumValue, string description = "") where TEnum : Enum
        {
            int id = Convert.ToInt32(enumValue);
            string name = enumValue.ToString();
            return new Permission(id, name, description);
        }
    }

    // ロールを表すクラス
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();

        public Role(int id, string name, string description = "")
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public void AddPermission(int permissionId)
        {
            if (!PermissionIds.Contains(permissionId))
            {
                PermissionIds.Add(permissionId);
            }
        }

        public void RemovePermission(int permissionId)
        {
            PermissionIds.Remove(permissionId);
        }

        // enum型から権限を追加するジェネリックメソッド
        public void AddPermission<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            AddPermission(Convert.ToInt32(enumValue));
        }

        // フラグ付きenum型から複数権限を追加するメソッド
        public void AddPermissions<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            int flagValue = Convert.ToInt32(enumValue);
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                int permValue = Convert.ToInt32(value);
                // 基本的な値だけを追加（組み合わせは除外）
                if (permValue != 0 && (flagValue & permValue) == permValue && IsPowerOfTwo(permValue))
                {
                    AddPermission(permValue);
                }
            }
        }

        // 2のべき乗かどうかをチェック（基本権限の判別用）
        private bool IsPowerOfTwo(int n)
        {
            return n != 0 && (n & (n - 1)) == 0;
        }

        // enum型から権限を削除するジェネリックメソッド
        public void RemovePermission<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            RemovePermission(Convert.ToInt32(enumValue));
        }

        // 組み合わせの演算を行わない形でenum値を取得するジェネリックメソッド
        public List<TEnum> GetEnumPermissions<TEnum>() where TEnum : Enum
        {
            var result = new List<TEnum>();
            foreach (int permId in PermissionIds)
            {
                if (Enum.IsDefined(typeof(TEnum), permId))
                {
                    result.Add((TEnum)Enum.ToObject(typeof(TEnum), permId));
                }
            }
            return result;
        }

        // 権限を持っているかチェックするジェネリックメソッド
        public bool HasPermission<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            return PermissionIds.Contains(Convert.ToInt32(enumValue));
        }

        // フラグ付き列挙型の組み合わせ権限をチェックするジェネリックメソッド
        public bool HasAllPermissions<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            int flagValue = Convert.ToInt32(enumValue);
            if (flagValue == 0) return true; // 権限なしは常にtrue

            // フラグを個々の基本権限に分解してチェック
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                int permValue = Convert.ToInt32(value);
                if (permValue != 0 && (flagValue & permValue) == permValue && !PermissionIds.Contains(permValue))
                {
                    return false; // 必要な権限が1つでも欠けていたらfalse
                }
            }
            return true;
        }
    }

    // ユーザーロールのマッピング
    public class UserRole
    {
        public string UserId { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();

        public UserRole(string userId)
        {
            UserId = userId;
        }

        public void AddRole(int roleId)
        {
            if (!RoleIds.Contains(roleId))
            {
                RoleIds.Add(roleId);
            }
        }

        public void RemoveRole(int roleId)
        {
            RoleIds.Remove(roleId);
        }
    }

    // 権限管理クラス
    public class PermissionManager
    {
        private static readonly string _permissionsFilePath = "permissions.json";
        private static readonly string _rolesFilePath = "roles.json";
        private static readonly string _userRolesFilePath = "userroles.json";

        private List<Permission> _permissions = new List<Permission>();
        private List<Role> _roles = new List<Role>();
        private List<UserRole> _userRoles = new List<UserRole>();

        private static Lazy<PermissionManager> _instance = new Lazy<PermissionManager>(() => new PermissionManager());
        public static PermissionManager Instance => _instance.Value;

        private PermissionManager() { }

        /// <summary>
        /// 特定のenum型の権限をすべて登録します
        /// </summary>
        /// <typeparam name="TEnum">enum型</typeparam>
        /// <param name="getDescription">説明を取得する関数（オプション）</param>
        public void RegisterEnumPermissions<TEnum>(Func<TEnum, string> getDescription = null) where TEnum : Enum
        {
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                int id = Convert.ToInt32(value);
                if (id == 0) continue; // 0は通常「なし」を意味するので登録しない

                string description = getDescription != null ? getDescription(value) : string.Empty;
                AddPermission(Permission.FromEnum(value, description));
            }
        }

        /// <summary>
        /// ユーザーが特定のenum権限を持っているか確認します
        /// </summary>
        public bool HasPermission<TEnum>(string userId, TEnum permission) where TEnum : Enum
        {
            var userRole = GetUserRole(userId);
            if (userRole == null) return false;

            int permissionId = Convert.ToInt32(permission);

            foreach (var roleId in userRole.RoleIds)
            {
                var role = GetRole(roleId);
                if (role != null)
                {
                    if (role.HasPermission(permissionId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// ユーザーが複合的なフラグ権限をすべて持っているか確認します
        /// </summary>
        public bool HasAllPermissions<TEnum>(string userId, TEnum permission) where TEnum : Enum
        {
            var userRole = GetUserRole(userId);
            if (userRole == null) return false;

            // ユーザーの全権限を集める
            HashSet<int> userPermissions = new HashSet<int>();

            foreach (var roleId in userRole.RoleIds)
            {
                var role = GetRole(roleId);
                if (role != null)
                {
                    foreach (var permId in role.PermissionIds)
                    {
                        userPermissions.Add(permId);
                    }
                }
            }

            // 必要な権限をビット分解して確認
            int flagValue = Convert.ToInt32(permission);
            if (flagValue == 0) return true; // 権限なしは常にtrue

            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                int permValue = Convert.ToInt32(value);
                if (permValue != 0 && (flagValue & permValue) == permValue && !userPermissions.Contains(permValue))
                {
                    return false; // 必要な権限が1つでも欠けていたらfalse
                }
            }

            return true;
        }

        // 全データの読み込み
        public void LoadAll()
        {
            LoadPermissions();
            LoadRoles();
            LoadUserRoles();
        }

        // 権限データの読み込み
        public void LoadPermissions()
        {
            if (File.Exists(_permissionsFilePath))
            {
                string json = File.ReadAllText(_permissionsFilePath);
                _permissions = JsonSerializer.Deserialize<List<Permission>>(json) ?? new List<Permission>();
            }
        }

        // ロールデータの読み込み
        public void LoadRoles()
        {
            if (File.Exists(_rolesFilePath))
            {
                string json = File.ReadAllText(_rolesFilePath);
                _roles = JsonSerializer.Deserialize<List<Role>>(json) ?? new List<Role>();
            }
        }

        // ユーザーロールデータの読み込み
        public void LoadUserRoles()
        {
            if (File.Exists(_userRolesFilePath))
            {
                string json = File.ReadAllText(_userRolesFilePath);
                _userRoles = JsonSerializer.Deserialize<List<UserRole>>(json) ?? new List<UserRole>();
            }
        }

        // 全データの保存
        public void SaveAll()
        {
            SavePermissions();
            SaveRoles();
            SaveUserRoles();
        }

        // 権限データの保存
        public void SavePermissions()
        {
            string json = JsonSerializer.Serialize(_permissions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_permissionsFilePath, json);
        }

        // ロールデータの保存
        public void SaveRoles()
        {
            string json = JsonSerializer.Serialize(_roles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_rolesFilePath, json);
        }

        // ユーザーロールデータの保存
        public void SaveUserRoles()
        {
            string json = JsonSerializer.Serialize(_userRoles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_userRolesFilePath, json);
        }

        // 権限の追加
        public void AddPermission(Permission permission)
        {
            _permissions.RemoveAll(p => p.Id == permission.Id);
            _permissions.Add(permission);
        }

        // ロールの追加
        public void AddRole(Role role)
        {
            _roles.RemoveAll(r => r.Id == role.Id);
            _roles.Add(role);
        }

        // ユーザーロールの追加
        public void AddUserRole(UserRole userRole)
        {
            _userRoles.RemoveAll(ur => ur.UserId == userRole.UserId);
            _userRoles.Add(userRole);
        }

        // 権限の取得
        public Permission GetPermission(int id)
        {
            return _permissions.FirstOrDefault(p => p.Id == id);
        }

        // ロールの取得
        public Role GetRole(int id)
        {
            return _roles.FirstOrDefault(r => r.Id == id);
        }

        // ユーザーロールの取得
        public UserRole GetUserRole(string userId)
        {
            return _userRoles.FirstOrDefault(ur => ur.UserId == userId);
        }

        // 全権限の取得
        public List<Permission> GetAllPermissions()
        {
            return _permissions.ToList();
        }

        // 全ロールの取得
        public List<Role> GetAllRoles()
        {
            return _roles.ToList();
        }

        // 全ユーザーロールの取得
        public List<UserRole> GetAllUserRoles()
        {
            return _userRoles.ToList();
        }
    }

}
