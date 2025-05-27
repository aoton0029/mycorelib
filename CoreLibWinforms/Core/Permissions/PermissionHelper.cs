using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// 権限関連のヘルパーメソッドを提供するクラス
    /// </summary>
    public static class PermissionHelper
    {
        private static readonly Dictionary<IPermission, string> _permissionDescriptionCache = new();
        private static readonly Dictionary<IUserRole, string> _userRoleDescriptionCache = new();

        /// <summary>
        /// 列挙値からDescription属性の値を取得します
        /// </summary>
        /// <param name="value">列挙値</param>
        /// <returns>説明文字列、見つからない場合は列挙値の名前</returns>
        public static string GetDescriptionFromEnum(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : value.ToString();
        }

        /// <summary>
        /// 特定のロールが別のロールよりも高い権限を持つかをチェック
        /// </summary>
        /// <param name="higherRole">上位と思われるロール</param>
        /// <param name="lowerRole">下位と思われるロール</param>
        /// <returns>higherRoleがlowerRoleより高い権限を持つ場合true</returns>
        public static bool IsHigherRole(IUserRole higherRole, IUserRole lowerRole)
        {
            if (higherRole == null || lowerRole == null)
                return false;

            return higherRole.Value > lowerRole.Value;
        }

        /// <summary>
        /// 指定された機能に対する権限マトリックスを生成します
        /// </summary>
        /// <param name="featureId">機能ID</param>
        /// <param name="roles">チェックするロールのリスト</param>
        /// <returns>ロールごとの権限マップ</returns>
        public static Dictionary<IUserRole, IPermission> GeneratePermissionMatrix(string featureId, List<IUserRole> roles)
        {
            var result = new Dictionary<IUserRole, IPermission>();
            var permManager = PermissionManager.Instance;

            foreach (var role in roles)
            {
                var permission = permManager.GetPermission(featureId, role);
                if (permission != null)
                {
                    result[role] = permission;
                }
            }

            return result;
        }

        /// <summary>
        /// 文字列から権限オブジェクトに変換します（アプリケーション固有の実装が必要）
        /// </summary>
        /// <param name="permissionName">権限名（複数の権限はカンマ区切り）</param>
        /// <param name="permissionResolver">権限名を権限オブジェクトに変換する関数</param>
        /// <returns>対応する権限オブジェクト</returns>
        public static IPermission ParsePermission(string permissionName, Func<string, ApplicationPermission> permissionResolver)
        {
            if (string.IsNullOrWhiteSpace(permissionName) || permissionResolver == null)
            {
                return null;
            }

            var permissionNames = permissionName.Split(',', '|', ';')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p));

            ApplicationPermission result = null;

            foreach (var name in permissionNames)
            {
                var permission = permissionResolver(name);
                if (permission != null)
                {
                    if (result == null)
                    {
                        result = permission;
                    }
                    else
                    {
                        result = result | permission;
                    }
                }
            }

            return result;
        }
    }

}
