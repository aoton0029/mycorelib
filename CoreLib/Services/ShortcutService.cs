using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    public interface IShortcutService
    {
        Task<IReadOnlyDictionary<string, ShortcutBinding>> GetShortcutsAsync();
        Task<bool> UpdateShortcutAsync(string commandName, ShortcutBinding binding);
        Task<bool> ResetToDefaultsAsync();
        Task<bool> ImportShortcutsAsync(string filePath);
        Task<bool> ExportShortcutsAsync(string filePath);
        bool IsShortcutPressed(string commandName, KeyEventArgs e);
        string GetShortcutDisplayText(string commandName);
    }

    public class ShortcutBinding
    {
        public Key Key { get; set; }
        public ModifierKeys Modifiers { get; set; }
        public string DisplayText => GetDisplayText(Key, Modifiers);

        public static string GetDisplayText(Key key, ModifierKeys modifiers)
        {
            var parts = new List<string>();

            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                parts.Add("Ctrl");

            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                parts.Add("Alt");

            if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                parts.Add("Shift");

            if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
                parts.Add("Win");

            if (key != Key.None)
                parts.Add(GetKeyDisplayName(key));

            return string.Join("+", parts);
        }

        private static string GetKeyDisplayName(Key key)
        {
            // 特殊キーの表示名をカスタマイズ
            return key switch
            {
                Key.OemPlus => "+",
                Key.OemMinus => "-",
                Key.OemQuestion => "?",
                Key.OemComma => ",",
                Key.OemPeriod => ".",
                Key.OemSemicolon => ";",
                Key.OemQuotes => "\"",
                Key.OemOpenBrackets => "[",
                Key.OemCloseBrackets => "]",
                Key.OemBackslash => "\\",
                _ => key.ToString()
            };
        }
    }

    public class ShortcutService : IShortcutService
    {
        private readonly ILogger<ShortcutService> _logger;
        private readonly string _shortcutsFilePath;
        private Dictionary<string, ShortcutBinding> _shortcuts;
        private readonly Dictionary<string, ShortcutBinding> _defaultShortcuts;

        public ShortcutService(ILogger<ShortcutService> logger)
        {
            _logger = logger;
            _shortcutsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "YourApp",
                "shortcuts.json");

            // デフォルトのショートカット定義
            _defaultShortcuts = new Dictionary<string, ShortcutBinding>
            {
                ["New"] = new ShortcutBinding { Key = Key.N, Modifiers = ModifierKeys.Control },
                ["Open"] = new ShortcutBinding { Key = Key.O, Modifiers = ModifierKeys.Control },
                ["Save"] = new ShortcutBinding { Key = Key.S, Modifiers = ModifierKeys.Control },
                ["SaveAs"] = new ShortcutBinding { Key = Key.S, Modifiers = ModifierKeys.Control | ModifierKeys.Shift },
                ["Print"] = new ShortcutBinding { Key = Key.P, Modifiers = ModifierKeys.Control },
                ["Exit"] = new ShortcutBinding { Key = Key.F4, Modifiers = ModifierKeys.Alt },
                ["Cut"] = new ShortcutBinding { Key = Key.X, Modifiers = ModifierKeys.Control },
                ["Copy"] = new ShortcutBinding { Key = Key.C, Modifiers = ModifierKeys.Control },
                ["Paste"] = new ShortcutBinding { Key = Key.V, Modifiers = ModifierKeys.Control },
                ["Undo"] = new ShortcutBinding { Key = Key.Z, Modifiers = ModifierKeys.Control },
                ["Redo"] = new ShortcutBinding { Key = Key.Y, Modifiers = ModifierKeys.Control },
                ["Find"] = new ShortcutBinding { Key = Key.F, Modifiers = ModifierKeys.Control },
                ["Replace"] = new ShortcutBinding { Key = Key.H, Modifiers = ModifierKeys.Control },
                ["SelectAll"] = new ShortcutBinding { Key = Key.A, Modifiers = ModifierKeys.Control },
                ["ZoomIn"] = new ShortcutBinding { Key = Key.OemPlus, Modifiers = ModifierKeys.Control },
                ["ZoomOut"] = new ShortcutBinding { Key = Key.OemMinus, Modifiers = ModifierKeys.Control },
                ["Help"] = new ShortcutBinding { Key = Key.F1, Modifiers = ModifierKeys.None }
            };
        }

        public async Task<IReadOnlyDictionary<string, ShortcutBinding>> GetShortcutsAsync()
        {
            if (_shortcuts == null)
            {
                await LoadShortcutsAsync();
            }

            return _shortcuts;
        }

        public async Task<bool> UpdateShortcutAsync(string commandName, ShortcutBinding binding)
        {
            if (_shortcuts == null)
            {
                await LoadShortcutsAsync();
            }

            try
            {
                // 既存のコマンドかチェック
                if (!_shortcuts.ContainsKey(commandName))
                {
                    _logger.LogWarning("未知のコマンド名のショートカットを更新しようとしました: {Command}", commandName);
                    return false;
                }

                // 重複チェック（同じショートカットの組み合わせが別のコマンドに割り当てられていないか）
                var conflictCommand = _shortcuts.FirstOrDefault(s =>
                    s.Key != commandName &&
                    s.Value.Key == binding.Key &&
                    s.Value.Modifiers == binding.Modifiers);

                if (conflictCommand.Key != null)
                {
                    _logger.LogWarning(
                        "ショートカットの競合: {Shortcut} は既に {ExistingCommand} に割り当てられています",
                        binding.DisplayText, conflictCommand.Key);
                    return false;
                }

                // ショートカットを更新
                _shortcuts[commandName] = binding;
                _logger.LogInformation("ショートカットを更新しました: {Command} -> {Shortcut}", commandName, binding.DisplayText);

                // 保存
                await SaveShortcutsAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ショートカットの更新中にエラーが発生しました: {Command}", commandName);
                return false;
            }
        }

        public async Task<bool> ResetToDefaultsAsync()
        {
            try
            {
                _shortcuts = new Dictionary<string, ShortcutBinding>(_defaultShortcuts);
                await SaveShortcutsAsync();
                _logger.LogInformation("ショートカットをデフォルトにリセットしました");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ショートカットのリセット中にエラーが発生しました");
                return false;
            }
        }

        public async Task<bool> ImportShortcutsAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("インポートファイルが見つかりません: {Path}", filePath);
                    return false;
                }

                var json = await File.ReadAllTextAsync(filePath);
                var importedShortcuts = JsonSerializer.Deserialize<Dictionary<string, ShortcutBindingDto>>(json);

                if (importedShortcuts == null || !importedShortcuts.Any())
                {
                    _logger.LogWarning("インポートファイルに有効なショートカット定義がありません");
                    return false;
                }

                // インポートデータを変換
                var newShortcuts = new Dictionary<string, ShortcutBinding>();
                foreach (var (command, dto) in importedShortcuts)
                {
                    // コマンドがデフォルト定義に存在するかチェック
                    if (!_defaultShortcuts.ContainsKey(command))
                    {
                        _logger.LogWarning("未知のコマンドをスキップします: {Command}", command);
                        continue;
                    }

                    newShortcuts[command] = new ShortcutBinding
                    {
                        Key = dto.Key,
                        Modifiers = dto.Modifiers
                    };
                }

                _shortcuts = newShortcuts;
                await SaveShortcutsAsync();

                _logger.LogInformation("ショートカットを正常にインポートしました: {Count}個", _shortcuts.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ショートカットのインポート中にエラーが発生しました: {Path}", filePath);
                return false;
            }
        }

        public async Task<bool> ExportShortcutsAsync(string filePath)
        {
            try
            {
                if (_shortcuts == null)
                {
                    await LoadShortcutsAsync();
                }

                // DTOに変換
                var dtoShortcuts = _shortcuts.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new ShortcutBindingDto
                    {
                        Key = kvp.Value.Key,
                        Modifiers = kvp.Value.Modifiers,
                        DisplayText = kvp.Value.DisplayText
                    });

                var json = JsonSerializer.Serialize(dtoShortcuts, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);

                _logger.LogInformation("ショートカットを正常にエクスポートしました: {Path}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ショートカットのエクスポート中にエラーが発生しました: {Path}", filePath);
                return false;
            }
        }

        public bool IsShortcutPressed(string commandName, KeyEventArgs e)
        {
            if (_shortcuts == null || !_shortcuts.TryGetValue(commandName, out var binding))
            {
                return false;
            }

            return e.Key == binding.Key && e.KeyboardDevice.Modifiers == binding.Modifiers;
        }

        public string GetShortcutDisplayText(string commandName)
        {
            if (_shortcuts == null || !_shortcuts.TryGetValue(commandName, out var binding))
            {
                return string.Empty;
            }

            return binding.DisplayText;
        }

        private async Task LoadShortcutsAsync()
        {
            try
            {
                if (File.Exists(_shortcutsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_shortcutsFilePath);
                    var dtoShortcuts = JsonSerializer.Deserialize<Dictionary<string, ShortcutBindingDto>>(json);

                    _shortcuts = dtoShortcuts.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new ShortcutBinding { Key = kvp.Value.Key, Modifiers = kvp.Value.Modifiers }
                    );

                    _logger.LogInformation("ショートカット設定を読み込みました: {Count}個", _shortcuts.Count);
                }
                else
                {
                    _logger.LogInformation("ショートカット設定ファイルが見つからないため、デフォルト設定を使用します");
                    _shortcuts = new Dictionary<string, ShortcutBinding>(_defaultShortcuts);
                    await SaveShortcutsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ショートカット設定の読み込み中にエラーが発生しました");
                _shortcuts = new Dictionary<string, ShortcutBinding>(_defaultShortcuts);
            }
        }

        private async Task SaveShortcutsAsync()
        {
            try
            {
                var directoryName = Path.GetDirectoryName(_shortcutsFilePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // DTOに変換
                var dtoShortcuts = _shortcuts.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new ShortcutBindingDto
                    {
                        Key = kvp.Value.Key,
                        Modifiers = kvp.Value.Modifiers,
                        DisplayText = kvp.Value.DisplayText
                    });

                var json = JsonSerializer.Serialize(dtoShortcuts, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_shortcutsFilePath, json);

                _logger.LogDebug("ショートカット設定を保存しました: {Path}", _shortcutsFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ショートカット設定の保存中にエラーが発生しました");
            }
        }

        // シリアライズ用DTO
        private class ShortcutBindingDto
        {
            public Key Key { get; set; }
            public ModifierKeys Modifiers { get; set; }
            public string DisplayText { get; set; }
        }
    }
}
