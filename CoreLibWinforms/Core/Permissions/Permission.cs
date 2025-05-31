using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Permissions
{
    public class Permission : ICloneable
    {
        // 権限のID（これはBitArrayの位置としても使う）
        public int Id { get; private set; }

        // 権限の名前
        public string Name { get; private set; }

        // 権限の組み合わせを表すビット配列
        public BitArray Permissions { get; private set; }

        public bool IsCombined { get; private set; }

        /// <summary>
        /// 基本権限コンストラクタ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Permission(int id, string name)
        {
            Id = id;
            Name = name;
            Permissions = new BitArray(PermissionManager.MaxPermissionId);
            IsCombined = false; // 権限が結合されていないことを示すフラグ
            Grant(id); // 自分自身の権限を付与
        }

        /// <summary>
        /// 特定の権限IDを持つ権限コンストラクタ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="permissionIds"></param>
        public Permission(int id, string name, IEnumerable<int> permissionIds)
        {
            Id = id;
            Name = name;
            Permissions = new BitArray(PermissionManager.MaxPermissionId);
            foreach (var permissionId in permissionIds)
            {
                if (permissionId >= 0 && permissionId < PermissionManager.MaxPermissionId)
                {
                    Permissions[permissionId] = true;
                }
            }
            IsCombined = true; // このコンストラクタは特定の権限IDを持つため、結合されているとみなす
        }

        // 静的な管理者権限生成メソッド
        public static Permission Admin(int id)
        {
            Permission adminPermission = new Permission(id, "admin");
            for (int i = 0; i < adminPermission.Permissions.Count; i++)
            {
                adminPermission.Grant(i);
            }
            return adminPermission;
        }

        // 特定の権限を付与する
        public void Grant(int permissionId)
        {
            if (permissionId >= 0 && permissionId < Permissions.Length)
            {
                Permissions[permissionId] = true;
            }
        }

        public void Grant(Permission other)
        {
            if (other == null)
                return;

            // 他の Permission オブジェクトが持つすべての権限を付与する
            Permissions.Or(other.Permissions);
        }

        // 特定の権限を削除する
        public void Revoke(int permissionId)
        {
            if (permissionId >= 0 && permissionId < Permissions.Length)
            {
                Permissions[permissionId] = false;
            }
        }

        public void Revoke(Permission other)
        {
            if (other == null)
                return;

            // 他の Permission オブジェクトが持つ権限を削除する
            // BitArrayの直接的なANDNOT操作がないため、一時的なBitArrayを作成して操作する
            BitArray temp = new BitArray(Permissions);
            temp.And(other.Permissions.Not());
            Permissions = temp;
        }

        // 特定の権限を持っているかチェックする
        public bool HasPermission(int permissionId)
        {
            return permissionId >= 0 && permissionId < Permissions.Length && Permissions[permissionId];
        }

        // 権限セットを他の権限セットと組み合わせる
        public void CombineWith(Permission other)
        {
            // BitArrayをORで結合
            Permissions.Or(other.Permissions);
        }

        public object Clone()
        {
            // 新しいPermissionオブジェクトを作成し、現在の状態をコピーする
            var clonedPermission = new Permission(Id, Name);
            clonedPermission.Permissions = new BitArray(Permissions);
            clonedPermission.IsCombined = IsCombined;
            return clonedPermission;
        }
    }

    // ユーザー権限を管理するクラス
    public class PermissionManager
    {
        public static int MaxPermissionId = 32;
        public static string PermissionsFilePath = "permissions.json";
        public static string UserRoleFilePath = "user_roles.json";


        private Dictionary<string, Permission> _availablePermissions;
        // ユーザーIDと対応する権限のマッピング
        private Dictionary<string, Permission> _userPermissions;

        public PermissionManager()
        {
            _availablePermissions = new Dictionary<string, Permission>();
            _userPermissions = new Dictionary<string, Permission>();

            // 管理者ロールを初期化
            var adminRole = Permission.Admin(_availablePermissions.Count);
            _availablePermissions.Add(adminRole.Name, adminRole);
        }

        // 利用可能な権限を追加
        public Permission RegisterPermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            if (_availablePermissions.ContainsKey(permission.Name))
                throw new ArgumentException($"Permission with name '{permission.Name}' already exists");

            _availablePermissions.Add(permission.Name, permission);
            return permission;
        }

        // 新しい権限を登録するメソッド（名前のみで新規作成、ID自動採番）
        public Permission RegisterNewPermission(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Permission name cannot be null or empty");

            if (_availablePermissions.ContainsKey(name))
                throw new ArgumentException($"Permission with name '{name}' already exists");

            // 新しいIDを割り当て
            int newId = _availablePermissions.Count;
            var permission = new Permission(newId, name);

            _availablePermissions.Add(name, permission);
            return permission;
        }

        // 新しい権限を登録するメソッド（特定のIDを持つ権限を作成、ID自動採番）
        public Permission RegisterNewPermission(string name, IEnumerable<int> permissionIds)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Permission name cannot be null or empty");

            if (permissionIds == null)
                throw new ArgumentNullException(nameof(permissionIds));

            if (_availablePermissions.ContainsKey(name))
                throw new ArgumentException($"Permission with name '{name}' already exists");

            // 新しいIDを割り当て
            int newId = _availablePermissions.Count;
            var permission = new Permission(newId, name, permissionIds);

            _availablePermissions.Add(name, permission);
            return permission;
        }

        // ユーザーに権限を割り当てる
        public void AssignPermissionToUser(string userId, Permission permission)
        {
            if (string.IsNullOrEmpty(userId) || permission == null)
                throw new ArgumentException("Invalid user ID or permission");
            if (_userPermissions.ContainsKey(userId))
            {
                _userPermissions[userId].CombineWith(permission);
            }
            else
            {
                _userPermissions[userId] = permission;
            }
        }

        // ユーザーに管理者権限を割り当てる(権限が存在する場合のみ)
        public void AssignAdminToUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (_availablePermissions.TryGetValue("admin", out Permission adminPermission))
            {
                AssignPermissionToUser(userId, adminPermission);
            }
            else
            {
                throw new InvalidOperationException("Admin permission is not available");
            }
        }

        // ユーザーに既存の特定のIDの権限を付与する(権限が存在する場合のみ。ユーザーが存在しない場合既存の権限を割り当てる)
        public void AssignPermissionToUserById(string userId, int permissionId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            // 特定の権限IDを持つPermissionオブジェクトを検索
            Permission foundPermission = null;
            foreach (var permission in _availablePermissions.Values)
            {
                if (permission.Id == permissionId)
                {
                    foundPermission = permission;
                    break;
                }
            }

            if (foundPermission == null)
                throw new ArgumentException($"Permission with ID {permissionId} does not exist");

            // 見つかった権限をユーザーに割り当て
            AssignPermissionToUser(userId, (Permission)foundPermission.Clone());
        }

        // ユーザーに既存の特定の権限を権限名から付与する(権限が存在する場合のみ。ユーザーが存在しない場合既存の権限を割り当てる)
        public void AssignPermissionToUserByName(string userId, string permissionName)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (string.IsNullOrEmpty(permissionName))
                throw new ArgumentException("Permission name cannot be null or empty");

            if (!_availablePermissions.TryGetValue(permissionName, out Permission permission))
                throw new ArgumentException($"Permission with name '{permissionName}' does not exist");

            // 見つかった権限をユーザーに割り当て
            AssignPermissionToUser(userId, (Permission)permission.Clone());
        }

        // ユーザーから特定の権限を削除する
        public void RevokePermissionFromUser(string userId, Permission permission)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            if (_userPermissions.TryGetValue(userId, out Permission userPermission))
            {
                userPermission.Revoke(permission);
            }
        }

        // ユーザーから特定のIDの権限を削除する
        public void RevokePermissionFromUserById(string userId, int permissionId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (_userPermissions.TryGetValue(userId, out Permission userPermission))
            {
                userPermission.Revoke(permissionId);
            }
        }


        // ユーザーから特定の権限を権限名から削除する
        public void RevokePermissionFromUserByName(string userId, string permissionName)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (string.IsNullOrEmpty(permissionName))
                throw new ArgumentException("Permission name cannot be null or empty");

            if (!_availablePermissions.TryGetValue(permissionName, out Permission permission))
                throw new ArgumentException($"Permission with name '{permissionName}' does not exist");

            if (_userPermissions.TryGetValue(userId, out Permission userPermission))
            {
                userPermission.Revoke(permission);
            }
        }

        // ユーザーが特定の権限IDを持っているかチェックする
        public bool UserHasPermissionId(string userId, int permissionId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (_userPermissions.TryGetValue(userId, out Permission userPermission))
            {
                return userPermission.HasPermission(permissionId);
            }
            return false;
        }

        // ユーザーが特定の権限を持っているかチェックする
        public bool UserHasPermission(string userId, Permission permission)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            if (_userPermissions.TryGetValue(userId, out Permission userPermission))
            {
                return permission.Permissions.SequenceEqual(userPermission.Permissions);
            }
            return false;
        }

        // ユーザーが特定の権限を持っているか権限名からチェックする
        public bool UserHasPermissionByName(string userId, string permissionName)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (string.IsNullOrEmpty(permissionName))
                throw new ArgumentException("Permission name cannot be null or empty");

            if (!_availablePermissions.TryGetValue(permissionName, out Permission permission))
                throw new ArgumentException($"Permission with name '{permissionName}' does not exist");

            return UserHasPermission(userId, permission);
        }

        // ユーザーの権限を取得
        public Permission GetUserPermission(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            if (_userPermissions.TryGetValue(userId, out Permission permission))
            {
                return permission;
            }
            return null;
        }

        // 登録済みの全権限を取得
        public List<Permission> GetAllPermissions()
        {
            return _availablePermissions.Values.ToList();
        }

        // IDから権限名を取得
        public string GetPermissionNameById(int id)
        {
            foreach (var permission in _availablePermissions.Values)
            {
                if (permission.Id == id)
                    return permission.Name;
            }
            return null;
        }

        // 権限の削除
        public bool RemovePermission(string name)
        {
            if (string.IsNullOrEmpty(name) || !_availablePermissions.ContainsKey(name))
                return false;

            _availablePermissions.Remove(name);
            return true;
        }
    }

    /// <summary>
    /// BitArrayクラスの拡張メソッドを提供します。
    /// </summary>
    public static class BitArrayExtensions
    {

        /// <summary>
        /// BitArray内に少なくとも1つ以上のtrueビットがあるかを確認します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>1つでもtrueビットがあればtrue、そうでなければfalse</returns>
        public static bool Any(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// BitArray内のすべてのビットがtrueかを確認します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>すべてのビットがtrueならtrue、そうでなければfalse</returns>
        public static bool All(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (!bitArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 指定した位置のビットを反転させます。
        /// </summary>
        /// <param name="bitArray">操作対象のBitArray</param>
        /// <param name="index">反転するビットのインデックス</param>
        /// <returns>操作後のBitArray（元のインスタンスと同じ）</returns>
        public static BitArray Flip(this BitArray bitArray, int index)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (index < 0 || index >= bitArray.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            bitArray[index] = !bitArray[index];
            return bitArray;
        }

        /// <summary>
        /// BitArray内のすべてのビットがfalseかどうかを判定します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>すべてのビットがfalseならtrue</returns>
        public static bool IsEmpty(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 2つのBitArrayが等しいかどうかを比較します。
        /// </summary>
        /// <param name="bitArray">比較元のBitArray</param>
        /// <param name="other">比較対象のBitArray</param>
        /// <returns>等しい場合はtrue、そうでなければfalse</returns>
        public static bool SequenceEqual(this BitArray bitArray, BitArray other)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (bitArray.Length != other.Length)
                return false;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i] != other[i])
                    return false;
            }
            return true;
        }
    }
}
