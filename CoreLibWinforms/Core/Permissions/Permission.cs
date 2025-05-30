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
    public class Permission
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
        public static Permission Admin()
        {
            Permission adminPermission = new Permission(-1, "admin", 100);
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
    }

    // ユーザー権限を管理するクラス
    public class PermissionManager
    {
        public static int MaxPermissionId = 100;
        public static string PermissionsFilePath = "permissions.json";
        public static string UserRoleFilePath = "user_roles.json";

        // 利用可能な権限のリスト
        private List<Permission> _availablePermissions;

        // ユーザーIDと対応する権限のマッピング
        private Dictionary<string, Permission> _userPermissions;

        public PermissionManager()
        {
            _availablePermissions = new List<Permission>();
            _userPermissions = new Dictionary<string, Permission>();
            _availablePermissions.Add(Permission.Admin()); // デフォルトで管理者権限を追加
        }

        // 利用可能な権限を追加
        public void RegisterPermission(Permission permission)
        {
            if (permission != null && !_availablePermissions.Any(p => p.Id == permission.Id))
            {
                _availablePermissions.Add(permission);
            }
        }

        public void RegisterNewPermission(string name, )

        // IDで権限を取得
        public Permission GetPermissionById(int id)
        {
            return _availablePermissions.FirstOrDefault(p => p.Id == id);
        }

        // すべての権限を取得
        public List<Permission> GetAllPermissions()
        {
            return new List<Permission>(_availablePermissions);
        }

        public 

        // 権限IDが存在するかどうかを確認


        // ユーザーに権限を割り当てる


        // ユーザーに管理者権限を割り当てる


        // ユーザーに特定のIDの権限を付与する


        // ユーザーから特定のIDの権限を削除する


        // ユーザーが特定の権限IDを持っているかチェックする


        // ユーザーの権限を取得


        // 権限名を指定してユーザーに権限を付与する


        // 権限名を指定してユーザーに権限を削除する

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
    }
}
