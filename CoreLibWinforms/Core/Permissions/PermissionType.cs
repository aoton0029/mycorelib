using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// ユーザーロールの基本インターフェース
    /// </summary>
    public interface IUserRole
    {
        /// <summary>
        /// ロールの数値値
        /// </summary>
        int Value { get; }

        /// <summary>
        /// ロールの名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 説明
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// 権限タイプの基本インターフェース
    /// </summary>
    public interface IPermission
    {
        /// <summary>
        /// 権限の数値値
        /// </summary>
        int Value { get; }

        /// <summary>
        /// 権限の名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 説明
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 他の権限と組み合わせることができるか
        /// </summary>
        bool IsCombineable { get; }
    }

    /// <summary>
    /// 汎用的なユーザーロールを表すクラス
    /// </summary>
    public class ApplicationRole : IUserRole
    {
        /// <summary>
        /// ロールの数値値
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// ロールの名前
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 新しいアプリケーションロールを作成します
        /// </summary>
        /// <param name="value">ロールの数値</param>
        /// <param name="name">ロールの名前</param>
        /// <param name="description">説明</param>
        public ApplicationRole(int value, string name, string description)
        {
            Value = value;
            Name = name;
            Description = description;
        }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is ApplicationRole role && Value == role.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(ApplicationRole left, ApplicationRole right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(ApplicationRole left, ApplicationRole right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// 汎用的な権限タイプを表すクラス
    /// </summary>
    public class ApplicationPermission : IPermission
    {
        /// <summary>
        /// 権限の数値値
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// 権限の名前
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 他の権限と組み合わせることができるか
        /// </summary>
        public bool IsCombineable { get; }

        /// <summary>
        /// 新しいアプリケーション権限を作成します
        /// </summary>
        /// <param name="value">権限の数値</param>
        /// <param name="name">権限の名前</param>
        /// <param name="description">説明</param>
        /// <param name="isCombineable">他の権限と組み合わせ可能かどうか</param>
        public ApplicationPermission(int value, string name, string description, bool isCombineable = true)
        {
            Value = value;
            Name = name;
            Description = description;
            IsCombineable = isCombineable;
        }

        /// <summary>
        /// 権限を組み合わせて新しい権限を作成します
        /// </summary>
        /// <param name="permissions">組み合わせる権限</param>
        /// <returns>権限の組み合わせ</returns>
        public static ApplicationPermission Combine(params ApplicationPermission[] permissions)
        {
            if (permissions == null || permissions.Length == 0)
                return null;

            int combinedValue = 0;
            var names = new List<string>();
            var descriptions = new List<string>();

            foreach (var perm in permissions)
            {
                if (perm == null) continue;
                if (!perm.IsCombineable)
                    throw new InvalidOperationException($"権限 '{perm.Name}' は組み合わせることができません。");

                combinedValue |= perm.Value;
                names.Add(perm.Name);
                descriptions.Add(perm.Description);
            }

            return new ApplicationPermission(
                combinedValue,
                string.Join("|", names),
                string.Join("、", descriptions),
                true
            );
        }

        /// <summary>
        /// この権限が指定された権限を含むかをチェックします
        /// </summary>
        /// <param name="permission">チェックする権限</param>
        /// <returns>権限を含む場合はtrue</returns>
        public bool HasPermission(ApplicationPermission permission)
        {
            return (Value & permission.Value) == permission.Value;
        }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            return obj is ApplicationPermission permission && Value == permission.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(ApplicationPermission left, ApplicationPermission right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(ApplicationPermission left, ApplicationPermission right)
        {
            return !(left == right);
        }

        public static ApplicationPermission operator |(ApplicationPermission left, ApplicationPermission right)
        {
            if (left is null) return right;
            if (right is null) return left;

            return Combine(left, right);
        }
    }
}
