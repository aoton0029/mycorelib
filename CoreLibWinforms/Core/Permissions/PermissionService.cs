using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    // 権限定数 - 標準アクション定義
    public static class PermissionActions
    {
        public const string View = "View";       // 閲覧権限
        public const string Create = "Create";   // 作成権限
        public const string Edit = "Edit";       // 編集権限
        public const string Delete = "Delete";   // 削除権限
        public const string Approve = "Approve"; // 承認権限
        public const string Visible = "Visible"; // 表示権限（UIの表示/非表示）
        public const string Enable = "Enable";   // 有効化権限（UIの有効/無効）
    }

    // 権限定数 - 機能モジュール定義
    public static class PermissionModules
    {
        public const string User = "User";           // ユーザー管理モジュール
        public const string Customer = "Customer";   // 顧客管理モジュール
        public const string Invoice = "Invoice";     // 請求書モジュール
        public const string Report = "Report";       // レポートモジュール
        public const string Admin = "Admin";         // 管理者モジュール
        public const string Settings = "Settings";   // 設定モジュール
    }

    // 階層型権限の定義
    public class Permission
    {
        public string Id { get; set; }                    // 権限ID（ドット記法：Module.SubModule.Action）
        public string Description { get; set; }           // 権限の説明
        public bool IsGranted { get; set; } = false;      // 権限が付与されているか

        // 階層構造を分解するヘルパーメソッド
        public string[] GetHierarchy() => Id?.Split('.') ?? Array.Empty<string>();

        // 最後のセグメントがアクションを表す
        public string GetAction() => GetHierarchy().LastOrDefault() ?? string.Empty;

        // 最初のセグメントがメインモジュールを表す
        public string GetMainModule() => GetHierarchy().FirstOrDefault() ?? string.Empty;
    }

    // ユーザー権限クラス
    public class UserPermission
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> GrantedPermissions { get; set; } = new List<string>();
    }

    // 場所に基づく権限クラス
    public class LocationPermission
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public List<string> GrantedPermissions { get; set; } = new List<string>();
    }

    // コントロール権限 - フォームとコントロールの組み合わせに基づく権限
    public class ControlPermission
    {
        public string FormName { get; set; }
        public string ControlName { get; set; }
        public string RequiredPermission { get; set; }  // ドット記法の権限ID
    }

    // 権限管理システム
    public class PermissionService
    {
        private PermissionManager permissionManager;

        public PermissionService()
        {
            permissionManager = new PermissionManager();
        }

        // 権限チェック - ワイルドカードもサポート
        public bool IsPermitted(string userId, string locationId, string permissionId)
        {
            // ユーザーの権限を確認
            var userPerm = permissionManager.UserPermissions.FirstOrDefault(up => up.UserId == userId);
            if (userPerm == null)
                return false;

            // 場所の権限を確認
            var locPerm = permissionManager.LocationPermissions.FirstOrDefault(lp => lp.LocationId == locationId);
            if (locPerm == null)
                return false;

            // 完全一致または階層ワイルドカードでチェック
            return HasPermissionMatch(userPerm.GrantedPermissions, permissionId) &&
                   HasPermissionMatch(locPerm.GrantedPermissions, permissionId);
        }

        // コントロールの権限チェック
        public bool IsControlEnabled(string userId, string locationId, string formName, string controlName)
        {
            // コントロールに必要な権限を検索
            var controlPerm = permissionManager.ControlPermissions.FirstOrDefault(cp =>
                cp.FormName == formName && cp.ControlName == controlName);

            if (controlPerm == null)
                return true; // 権限設定がない場合はデフォルトで許可

            // 必要な権限を持っているか確認
            return IsPermitted(userId, locationId, controlPerm.RequiredPermission);
        }

        // コントロールがvisibleかどうかの権限チェック
        public bool IsControlVisible(string userId, string locationId, string formName, string controlName)
        {
            var controlPerm = permissionManager.ControlPermissions.FirstOrDefault(cp =>
                cp.FormName == formName && cp.ControlName == controlName);

            if (controlPerm == null)
                return true;

            // 権限IDに基づいたVisibleバージョンの権限名を作成する
            string visiblePermission = GetModuleVisiblePermission(controlPerm.RequiredPermission);

            // 表示権限をチェック
            return IsPermitted(userId, locationId, visiblePermission);
        }

        // 権限IDの末尾をVisibleに変更したバージョンを取得
        private string GetModuleVisiblePermission(string permissionId)
        {
            string[] segments = permissionId.Split('.');
            if (segments.Length == 0) return permissionId;

            segments[segments.Length - 1] = PermissionActions.Visible;
            return string.Join(".", segments);
        }

        // 権限IDの階層マッチング（ワイルドカード対応）
        private bool HasPermissionMatch(List<string> grantedPermissions, string requiredPermission)
        {
            // 完全一致
            if (grantedPermissions.Contains(requiredPermission))
                return true;

            // 上位階層にワイルドカードマッチがあるか確認
            var requiredSegments = requiredPermission.Split('.');

            // 例: Admin.Settings.Edit に対して、Admin.* や Admin.Settings.* などをチェック
            for (int i = 1; i <= requiredSegments.Length; i++)
            {
                var partialPath = string.Join(".", requiredSegments.Take(i));
                var wildcardPath = partialPath + ".*";

                if (grantedPermissions.Contains(wildcardPath))
                    return true;
            }

            return false;
        }

        public void Load()
        {
            permissionManager.Load();
        }
    }
    public class PermissionManager 
    {
        public static string FilePath = "../設定/permissions.json"; // 権限設定のJSONファイルパス

        public List<Permission> AvailablePermissions { get; set; } = new List<Permission>();
        public List<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public List<LocationPermission> LocationPermissions { get; set; } = new List<LocationPermission>();
        public List<ControlPermission> ControlPermissions { get; set; } = new List<ControlPermission>();

        // 階層型権限IDを作成するヘルパーメソッド
        public string CreatePermissionId(params string[] segments) => string.Join(".", segments);

        #region Permission
        // 権限の追加
        public Permission AddPermission(string id, string description)
        {
            var permission = new Permission
            {
                Id = id,
                Description = description
            };

            AvailablePermissions.Add(permission);
            return permission;
        }

        // 権限の削除
        public bool RemovePermission(string permissionId)
        {
            var permission = AvailablePermissions.FirstOrDefault(p => p.Id == permissionId);
            if (permission != null)
            {
                return AvailablePermissions.Remove(permission);
            }
            return false;
        }

        // 権限の取得
        public Permission GetPermission(string permissionId)
        {
            return AvailablePermissions.FirstOrDefault(p => p.Id == permissionId);
        }

        // モジュールに関連する全権限の取得
        public IEnumerable<Permission> GetModulePermissions(string moduleName)
        {
            return AvailablePermissions.Where(p => p.GetMainModule() == moduleName);
        }
        #endregion

        #region UserPermission
        // ユーザー権限の追加
        public UserPermission AddUserPermission(string userId, string userName)
        {
            var userPermission = new UserPermission
            {
                UserId = userId,
                UserName = userName
            };

            UserPermissions.Add(userPermission);
            return userPermission;
        }

        // ユーザー権限の取得
        public UserPermission GetUserPermission(string userId)
        {
            return UserPermissions.FirstOrDefault(up => up.UserId == userId);
        }

        // ユーザー権限の削除
        public bool RemoveUserPermission(string userId)
        {
            var userPermission = UserPermissions.FirstOrDefault(up => up.UserId == userId);
            if (userPermission != null)
            {
                return UserPermissions.Remove(userPermission);
            }
            return false;
        }

        // ユーザーに権限を付与
        public bool GrantPermissionToUser(string userId, string permissionId)
        {
            var userPermission = GetUserPermission(userId);
            if (userPermission != null && !userPermission.GrantedPermissions.Contains(permissionId))
            {
                userPermission.GrantedPermissions.Add(permissionId);
                return true;
            }
            return false;
        }

        // ユーザーから権限を削除
        public bool RevokePermissionFromUser(string userId, string permissionId)
        {
            var userPermission = GetUserPermission(userId);
            if (userPermission != null)
            {
                return userPermission.GrantedPermissions.Remove(permissionId);
            }
            return false;
        }
        #endregion

        #region LocationPermission
        // 場所権限の追加
        public LocationPermission AddLocationPermission(string locationId, string locationName)
        {
            var locationPermission = new LocationPermission
            {
                LocationId = locationId,
                LocationName = locationName
            };

            LocationPermissions.Add(locationPermission);
            return locationPermission;
        }

        // 場所権限の取得
        public LocationPermission GetLocationPermission(string locationId)
        {
            return LocationPermissions.FirstOrDefault(lp => lp.LocationId == locationId);
        }

        // 場所権限の削除
        public bool RemoveLocationPermission(string locationId)
        {
            var locationPermission = LocationPermissions.FirstOrDefault(lp => lp.LocationId == locationId);
            if (locationPermission != null)
            {
                return LocationPermissions.Remove(locationPermission);
            }
            return false;
        }

        // 場所に権限を付与
        public bool GrantPermissionToLocation(string locationId, string permissionId)
        {
            var locationPermission = GetLocationPermission(locationId);
            if (locationPermission != null && !locationPermission.GrantedPermissions.Contains(permissionId))
            {
                locationPermission.GrantedPermissions.Add(permissionId);
                return true;
            }
            return false;
        }

        // 場所から権限を削除
        public bool RevokePermissionFromLocation(string locationId, string permissionId)
        {
            var locationPermission = GetLocationPermission(locationId);
            if (locationPermission != null)
            {
                return locationPermission.GrantedPermissions.Remove(permissionId);
            }
            return false;
        }
        #endregion

        #region ControlPermission
        // コントロール権限の追加
        public ControlPermission AddControlPermission(string formName, string controlName, string requiredPermission)
        {
            var controlPermission = new ControlPermission
            {
                FormName = formName,
                ControlName = controlName,
                RequiredPermission = requiredPermission
            };

            ControlPermissions.Add(controlPermission);
            return controlPermission;
        }

        // コントロール権限の取得
        public ControlPermission GetControlPermission(string formName, string controlName)
        {
            return ControlPermissions.FirstOrDefault(cp => cp.FormName == formName && cp.ControlName == controlName);
        }

        // コントロール権限の削除
        public bool RemoveControlPermission(string formName, string controlName)
        {
            var controlPermission = GetControlPermission(formName, controlName);
            if (controlPermission != null)
            {
                return ControlPermissions.Remove(controlPermission);
            }
            return false;
        }

        // フォーム内のすべてのコントロール権限を取得
        public IEnumerable<ControlPermission> GetFormControlPermissions(string formName)
        {
            return ControlPermissions.Where(cp => cp.FormName == formName);
        }
        #endregion

        // JSONファイルに保存する
        public void Save(string filePath = null)
        {
            string path = filePath ?? FilePath;

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var permissionsData = new
            {
                AvailablePermissions,
                UserPermissions,
                LocationPermissions,
                ControlPermissions
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(permissionsData, options);
            File.WriteAllText(path, jsonString);
        }

        // JSONファイルから読み込む
        public void Load(string filePath = null)
        {
            string path = filePath ?? FilePath;

            if (!File.Exists(path))
                return;

            string jsonString = File.ReadAllText(path);

            try
            {
                var permissionsData = JsonSerializer.Deserialize<PermissionManager>(jsonString);

                if (permissionsData != null)
                {
                    this.AvailablePermissions = permissionsData.AvailablePermissions ?? new List<Permission>();
                    this.UserPermissions = permissionsData.UserPermissions ?? new List<UserPermission>();
                    this.LocationPermissions = permissionsData.LocationPermissions ?? new List<LocationPermission>();
                    this.ControlPermissions = permissionsData.ControlPermissions ?? new List<ControlPermission>();
                }
            }
            catch (JsonException ex)
            {
                // JSONファイルの形式が正しくない場合のエラー処理
                Console.WriteLine($"JSONファイルの読み込みエラー: {ex.Message}");
            }
        }

        // デフォルト権限の初期化
        public void InitializeDefaultPermissions()
        {
            // 標準権限の追加
            foreach (var module in GetModuleNames())
            {
                AddPermission($"{module}.{PermissionActions.View}", $"{module}の閲覧権限");
                AddPermission($"{module}.{PermissionActions.Create}", $"{module}の作成権限");
                AddPermission($"{module}.{PermissionActions.Edit}", $"{module}の編集権限");
                AddPermission($"{module}.{PermissionActions.Delete}", $"{module}の削除権限");
                AddPermission($"{module}.{PermissionActions.Approve}", $"{module}の承認権限");
                AddPermission($"{module}.{PermissionActions.Visible}", $"{module}の表示権限");
                AddPermission($"{module}.{PermissionActions.Enable}", $"{module}の有効化権限");
            }
        }

        // モジュール名の取得
        private IEnumerable<string> GetModuleNames()
        {
            return typeof(PermissionModules)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => (string)fi.GetValue(null))
                .ToList();
        }
    }

}
