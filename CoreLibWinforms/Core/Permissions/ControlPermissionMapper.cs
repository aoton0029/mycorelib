using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{


    public class ControlPermissionMapper
    {
        public static string FilePath = "ControlPermissionMap.json";
        /// <summary>
        /// 権限に対してコントロールの権限設定をマッピングする辞書
        /// </summary>
        private readonly Dictionary<int, List<ControlPermissionSettings>> _controlPermissionMaps;

        public ControlPermissionMapper()
        {
            _controlPermissionMaps = new Dictionary<int, List<ControlPermissionSettings>>();
        }

        /// <summary>
        /// コントロールに権限を関連付ける
        /// </summary>
        public void RegisterControl(int permissionId, string controlName, bool affectVisibility = true, bool affectEnabled = true)
        {
            // 指定された権限IDに対するリストがまだ存在しない場合は作成
            if (!_controlPermissionMaps.ContainsKey(permissionId))
            {
                _controlPermissionMaps[permissionId] = new List<ControlPermissionSettings>();
            }

            // 既存の設定を確認
            var existingSetting = GetControlPermissionSettings(permissionId, controlName);
            if (existingSetting != null)
            {
                // 既存の設定を更新
                existingSetting.AffectVisibility = affectVisibility;
                existingSetting.AffectEnabled = affectEnabled;
            }
            else
            {
                // 新しい設定を追加
                _controlPermissionMaps[permissionId].Add(new ControlPermissionSettings
                {
                    ControlName = controlName,
                    PermissionId = permissionId,
                    AffectVisibility = affectVisibility,
                    AffectEnabled = affectEnabled
                });
            }
        }

        public void UnregisterControl(int permissionId, string controlName)
        {
            if (_controlPermissionMaps.TryGetValue(permissionId, out var settingsList))
            {
                // 指定されたコントロール名に一致する設定を削除
                var settingToRemove = settingsList.FirstOrDefault(s => s.ControlName == controlName);
                if (settingToRemove != null)
                {
                    settingsList.Remove(settingToRemove);

                    // リストが空になった場合は、辞書からキーを削除
                    if (settingsList.Count == 0)
                    {
                        _controlPermissionMaps.Remove(permissionId);
                    }
                }
            }
        }

        public ControlPermissionSettings GetControlPermissionSettings(int permissionId, string controlName)
        {
            if (_controlPermissionMaps.TryGetValue(permissionId, out var settingsList))
            {
                return settingsList.FirstOrDefault(s => s.ControlName == controlName);
            }
            return null;
        }

        public List<ControlPermissionSettings> GetControlPermissionSettings(int permissionId)
        {
            if (_controlPermissionMaps.TryGetValue(permissionId, out var settingsList))
            {
                return settingsList;
            }
            return new List<ControlPermissionSettings>();
        }

        /// <summary>
        /// コントロール名でマッピングを検索するためのメソッド
        /// </summary>
        /// <param name="controlName">コントロール名</param>
        /// <returns>そのコントロールに対する権限設定のリスト</returns>
        public List<ControlPermissionSettings> GetControlPermissionSettingsByControlName(string controlName)
        {
            var result = new List<ControlPermissionSettings>();

            foreach (var settingsList in _controlPermissionMaps.Values)
            {
                result.AddRange(settingsList.Where(s => s.ControlName == controlName));
            }

            return result;
        }

        /// <summary>
        /// すべてのコントロール権限マッピングをコントロール名をキーにして取得
        /// </summary>
        /// <returns>コントロール名と権限設定のディクショナリ</returns>
        public Dictionary<string, List<ControlPermissionSettings>> GetAllMappings()
        {
            // コントロール名をキーとするディクショナリ
            var result = new Dictionary<string, List<ControlPermissionSettings>>();

            // 各権限IDと設定リストのペアについて処理
            foreach (var kvp in _controlPermissionMaps)
            {
                foreach (var setting in kvp.Value)
                {
                    // コントロール名が既にキーとして存在するか確認
                    if (!result.TryGetValue(setting.ControlName, out var settingsList))
                    {
                        // 存在しない場合は新しいリストを作成
                        settingsList = new List<ControlPermissionSettings>();
                        result[setting.ControlName] = settingsList;
                    }

                    // 設定を追加
                    settingsList.Add(setting);
                }
            }

            return result;
        }

        /// <summary>
        /// コントロール権限マッピングをJSONファイルに保存
        /// </summary>
        public void Save()
        {
            try
            {
                // すべての設定を平坦なリストに変換
                var allSettings = new List<ControlPermissionSettings>();

                foreach (var kvp in _controlPermissionMaps)
                {
                    allSettings.AddRange(kvp.Value);
                }

                // JSONオプションを設定（整形出力）
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // JSONにシリアライズ
                string jsonString = JsonSerializer.Serialize(allSettings, options);

                // ファイルに保存
                File.WriteAllText(FilePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"コントロール権限マッピングの保存中にエラーが発生しました: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// JSONファイルからコントロール権限マッピングを読み込み
        /// </summary>
        public void Load()
        {
            // ファイルが存在しない場合は何もしない
            if (!File.Exists(FilePath))
                return;

            try
            {
                // ファイルからJSONを読み込み
                string jsonString = File.ReadAllText(FilePath);

                // JSONをControlPermissionSettingsのリストにデシリアライズ
                var settings = JsonSerializer.Deserialize<List<ControlPermissionSettings>>(jsonString);

                // 既存のマッピングをクリア
                _controlPermissionMaps.Clear();

                // デシリアライズした設定を辞書に再構成
                if (settings != null)
                {
                    foreach (var setting in settings)
                    {
                        if (!_controlPermissionMaps.TryGetValue(setting.PermissionId, out var settingsList))
                        {
                            settingsList = new List<ControlPermissionSettings>();
                            _controlPermissionMaps[setting.PermissionId] = settingsList;
                        }

                        settingsList.Add(setting);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"コントロール権限マッピングの読み込み中にエラーが発生しました: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// コントロールの権限設定を保持するクラス
    /// </summary>
    public class ControlPermissionSettings
    {
        /// <summary>
        /// コントロールの名前
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 権限ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 表示/非表示を制御するか
        /// </summary>
        public bool AffectVisibility { get; set; } = true;

        /// <summary>
        /// 有効/無効を制御するか
        /// </summary>
        public bool AffectEnabled { get; set; } = true;
    }
}
