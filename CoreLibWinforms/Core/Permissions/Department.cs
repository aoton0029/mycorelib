using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// 組織の部署を表すクラス
    /// </summary>
    public class Department
    {
        // 部署ID
        public string Id { get; private set; }

        // 部署名
        public string Name { get; set; }

        // 親部署のID（最上位部署はnull）
        public string? ParentDepartmentId { get; set; }

        // 子部署のコレクション
        private readonly List<Department> _childDepartments = new();
        public IReadOnlyList<Department> ChildDepartments => _childDepartments.AsReadOnly();

        // 部署に所属するユーザーIDのリスト
        private readonly HashSet<string> _memberUserIds = new();
        public IReadOnlyCollection<string> MemberUserIds => _memberUserIds;

        /// <summary>
        /// 部署コンストラクタ
        /// </summary>
        /// <param name="id">部署ID</param>
        /// <param name="name">部署名</param>
        /// <param name="parentDepartmentId">親部署ID（オプション）</param>
        public Department(string id, string name, string? parentDepartmentId = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Department ID cannot be empty", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name cannot be empty", nameof(name));

            Id = id;
            Name = name;
            ParentDepartmentId = parentDepartmentId;
        }

        /// <summary>
        /// 子部署を追加
        /// </summary>
        /// <param name="child">追加する子部署</param>
        public void AddChildDepartment(Department child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            // 循環参照のチェック
            if (IsDescendantOf(child.Id))
                throw new InvalidOperationException("Circular reference detected in department hierarchy");

            child.ParentDepartmentId = Id;
            if (!_childDepartments.Contains(child))
                _childDepartments.Add(child);
        }

        /// <summary>
        /// 子部署を削除
        /// </summary>
        /// <param name="departmentId">削除する子部署のID</param>
        /// <returns>削除が成功したかどうか</returns>
        public bool RemoveChildDepartment(string departmentId)
        {
            var child = _childDepartments.FirstOrDefault(d => d.Id == departmentId);
            if (child != null)
            {
                child.ParentDepartmentId = null;
                return _childDepartments.Remove(child);
            }
            return false;
        }

        /// <summary>
        /// ユーザーを部署に追加
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>追加が成功したかどうか</returns>
        public bool AddMember(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            return _memberUserIds.Add(userId);
        }

        /// <summary>
        /// ユーザーを部署から削除
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除が成功したかどうか</returns>
        public bool RemoveMember(string userId)
        {
            return _memberUserIds.Remove(userId);
        }

        /// <summary>
        /// ユーザーが部署のメンバーかどうかを確認
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>メンバーであればtrue</returns>
        public bool IsMember(string userId)
        {
            return _memberUserIds.Contains(userId);
        }

        /// <summary>
        /// 指定した部署が自分の子孫であるかどうかを確認
        /// </summary>
        /// <param name="departmentId">確認する部署ID</param>
        /// <returns>子孫であればtrue</returns>
        private bool IsDescendantOf(string departmentId)
        {
            if (Id == departmentId)
                return true;

            foreach (var child in _childDepartments)
            {
                if (child.IsDescendantOf(departmentId))
                    return true;
            }

            return false;
        }
    }


    /// <summary>
    /// 部署を管理するクラス
    /// </summary>
    public class DepartmentManager
    {
        private readonly Dictionary<string, Department> _departments = new();

        /// <summary>
        /// 部署を作成
        /// </summary>
        /// <param name="id">部署ID</param>
        /// <param name="name">部署名</param>
        /// <param name="parentDepartmentId">親部署ID（オプション）</param>
        /// <returns>作成された部署</returns>
        public Department CreateDepartment(string id, string name, string? parentDepartmentId = null)
        {
            if (_departments.ContainsKey(id))
                throw new ArgumentException($"Department with ID '{id}' already exists", nameof(id));

            var department = new Department(id, name, parentDepartmentId);
            _departments.Add(id, department);

            // 親部署があれば、その子部署として登録
            if (!string.IsNullOrWhiteSpace(parentDepartmentId) && _departments.TryGetValue(parentDepartmentId, out var parentDepartment))
            {
                parentDepartment.AddChildDepartment(department);
            }

            return department;
        }

        /// <summary>
        /// 部署を取得
        /// </summary>
        /// <param name="id">部署ID</param>
        /// <returns>部署</returns>
        public Department GetDepartment(string id)
        {
            if (!_departments.TryGetValue(id, out var department))
                throw new KeyNotFoundException($"Department with ID '{id}' not found");

            return department;
        }

        /// <summary>
        /// 部署の存在確認
        /// </summary>
        /// <param name="id">部署ID</param>
        /// <returns>存在すればtrue</returns>
        public bool DepartmentExists(string id)
        {
            return _departments.ContainsKey(id);
        }

        /// <summary>
        /// 部署の削除
        /// </summary>
        /// <param name="id">削除する部署ID</param>
        public void DeleteDepartment(string id)
        {
            if (!_departments.TryGetValue(id, out var department))
                throw new KeyNotFoundException($"Department with ID '{id}' not found");

            // 子部署の親部署参照をクリア
            foreach (var child in department.ChildDepartments.ToList())
            {
                department.RemoveChildDepartment(child.Id);
            }

            // 親部署がある場合は、親からの参照も削除
            if (!string.IsNullOrWhiteSpace(department.ParentDepartmentId) &&
                _departments.TryGetValue(department.ParentDepartmentId, out var parent))
            {
                parent.RemoveChildDepartment(id);
            }

            _departments.Remove(id);
        }

        /// <summary>
        /// すべての部署を取得
        /// </summary>
        /// <returns>部署のコレクション</returns>
        public IEnumerable<Department> GetAllDepartments()
        {
            return _departments.Values;
        }

        /// <summary>
        /// ルート部署（親を持たない部署）のみを取得
        /// </summary>
        /// <returns>ルート部署のコレクション</returns>
        public IEnumerable<Department> GetRootDepartments()
        {
            return _departments.Values.Where(d => string.IsNullOrWhiteSpace(d.ParentDepartmentId));
        }

        /// <summary>
        /// ユーザーが所属するすべての部署を取得
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザーが所属する部署のコレクション</returns>
        public IEnumerable<Department> GetUserDepartments(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Enumerable.Empty<Department>();

            return _departments.Values.Where(d => d.IsMember(userId));
        }

        /// <summary>
        /// ユーザーを部署に追加
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="departmentId">部署ID</param>
        public void AddUserToDepartment(string userId, string departmentId)
        {
            var department = GetDepartment(departmentId);
            department.AddMember(userId);
        }

        /// <summary>
        /// ユーザーを部署から削除
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="departmentId">部署ID</param>
        public void RemoveUserFromDepartment(string userId, string departmentId)
        {
            var department = GetDepartment(departmentId);
            department.RemoveMember(userId);
        }
    }
}
