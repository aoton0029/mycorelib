using CoreLib.Utilities.IO.Formats;
using CoreLibWinforms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Histories
{
    /// <summary>
    /// 履歴サービス - 複数の履歴タイプを管理します
    /// </summary>
    public class HistoryService
    {
        private readonly Dictionary<string, HistoryCollection> _historyCollections = new Dictionary<string, HistoryCollection>();

        /// <summary>
        /// デフォルトの履歴最大数
        /// </summary>
        public const int DefaultMaxItems = 20;

        /// <summary>
        /// デフォルトの履歴保存パス
        /// </summary>
        public static string DefaultHistoryFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "YourAppName",
            "Histories",
            "history.json");

        /// <summary>
        /// 履歴データの保存先ファイルパス
        /// </summary>
        public string HistoryFilePath { get; set; } = DefaultHistoryFilePath;

        /// <summary>
        /// 指定した履歴タイプの履歴コレクションを取得します
        /// </summary>
        /// <param name="historyType">履歴タイプの識別子</param>
        /// <returns>履歴コレクション</returns>
        public HistoryCollection GetHistory(string historyType)
        {
            if (!_historyCollections.ContainsKey(historyType))
            {
                _historyCollections[historyType] = new HistoryCollection(DefaultMaxItems);
            }

            return _historyCollections[historyType];
        }

        /// <summary>
        /// 指定した履歴タイプの履歴コレクションを作成または取得します
        /// </summary>
        /// <param name="historyType">履歴タイプの識別子</param>
        /// <param name="maxItems">最大履歴数</param>
        /// <returns>履歴コレクション</returns>
        public HistoryCollection GetOrCreateHistory(string historyType, int maxItems = DefaultMaxItems)
        {
            if (!_historyCollections.ContainsKey(historyType))
            {
                _historyCollections[historyType] = new HistoryCollection(maxItems);
            }

            return _historyCollections[historyType];
        }

        /// <summary>
        /// 指定した履歴タイプに項目を追加します
        /// </summary>
        /// <param name="historyType">履歴タイプの識別子</param>
        /// <param name="item">追加する項目</param>
        public void AddItem(string historyType, object item)
        {
            GetHistory(historyType).AddItem(item);
        }

        /// <summary>
        /// 指定した履歴タイプの履歴をクリアします
        /// </summary>
        /// <param name="historyType">履歴タイプの識別子</param>
        public void ClearHistory(string historyType)
        {
            if (_historyCollections.ContainsKey(historyType))
            {
                _historyCollections[historyType].Clear();
            }
        }

        /// <summary>
        /// すべての履歴をクリアします
        /// </summary>
        public void ClearAllHistory()
        {
            foreach (var collection in _historyCollections.Values)
            {
                collection.Clear();
            }
        }

        /// <summary>
        /// 特定の履歴タイプの最大項目数を設定します
        /// </summary>
        /// <param name="historyType">履歴タイプの識別子</param>
        /// <param name="maxItems">最大項目数</param>
        public void SetMaxItems(string historyType, int maxItems)
        {
            GetOrCreateHistory(historyType).MaxItems = maxItems;
        }

        /// <summary>
        /// 履歴データをJSONファイルに保存します
        /// </summary>
        /// <returns>保存に成功したかどうか</returns>
        public async Task<bool> SaveToJsonAsync()
        {
            try
            {
                var historyData = new Dictionary<string, SerializableHistoryCollection>();

                foreach (var kvp in _historyCollections)
                {
                    historyData[kvp.Key] = new SerializableHistoryCollection
                    {
                        MaxItems = kvp.Value.MaxItems,
                        Items = kvp.Value.Items.Select(i => new HistoryItemData { Value = i?.ToString() }).ToList()
                    };
                }

                await JsonHelper.SaveToFileAsync(historyData, HistoryFilePath);
                return true;
            }
            catch (Exception ex)
            {
                // 実際のアプリケーションではログ出力などを行うことを推奨
                System.Diagnostics.Debug.WriteLine($"履歴の保存に失敗: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// JSONファイルから履歴データを読み込みます
        /// </summary>
        /// <returns>読み込みに成功したかどうか</returns>
        public async Task<bool> LoadFromJsonAsync()
        {
            try
            {
                if (!File.Exists(HistoryFilePath))
                    return false;

                var historyData = await JsonHelper.LoadFromFileAsync<Dictionary<string, SerializableHistoryCollection>>(HistoryFilePath);
                if (historyData == null)
                    return false;

                // 既存のコレクションをクリア
                _historyCollections.Clear();

                // 読み込んだデータを復元
                foreach (var kvp in historyData)
                {
                    var collection = new HistoryCollection(kvp.Value.MaxItems);
                    foreach (var item in kvp.Value.Items.Where(i => i?.Value != null))
                    {
                        collection.AddItem(item.Value);
                    }
                    _historyCollections[kvp.Key] = collection;
                }

                return true;
            }
            catch (Exception ex)
            {
                // 実際のアプリケーションではログ出力などを行うことを推奨
                System.Diagnostics.Debug.WriteLine($"履歴の読み込みに失敗: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// アプリケーション終了時に履歴を保存します
        /// </summary>
        public async Task SaveOnExitAsync()
        {
            await SaveToJsonAsync();
        }

        /// <summary>
        /// アプリケーション起動時に履歴を読み込みます
        /// </summary>
        public async Task LoadOnStartupAsync()
        {
            await LoadFromJsonAsync();
        }
    }

    /// <summary>
    /// JSONシリアライズ用の履歴コレクションデータ
    /// </summary>
    [Serializable]
    public class SerializableHistoryCollection
    {
        /// <summary>
        /// 最大履歴数
        /// </summary>
        public int MaxItems { get; set; }

        /// <summary>
        /// 履歴項目リスト
        /// </summary>
        public List<HistoryItemData> Items { get; set; } = new List<HistoryItemData>();
    }

    /// <summary>
    /// JSONシリアライズ用の履歴項目データ
    /// </summary>
    [Serializable]
    public class HistoryItemData
    {
        /// <summary>
        /// 履歴項目の値（文字列表現）
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 履歴コレクション - 特定のタイプの履歴項目を保持します
    /// </summary>
    public class HistoryCollection
    {
        private readonly List<object> _items = new List<object>();
        private int _maxItems;

        /// <summary>
        /// 履歴の項目リスト
        /// </summary>
        public IReadOnlyList<object> Items => _items.AsReadOnly();

        /// <summary>
        /// 履歴に保持できる最大項目数
        /// </summary>
        public int MaxItems
        {
            get => _maxItems;
            set
            {
                _maxItems = value;
                TrimExcess();
            }
        }

        /// <summary>
        /// 履歴コレクションを初期化します
        /// </summary>
        /// <param name="maxItems">最大項目数</param>
        public HistoryCollection(int maxItems)
        {
            _maxItems = maxItems;
        }

        /// <summary>
        /// 履歴に項目を追加します
        /// </summary>
        /// <param name="item">追加する項目</param>
        public void AddItem(object item)
        {
            // 既存の同じ項目があれば削除（重複排除）
            _items.RemoveAll(i => i.Equals(item));

            // 新しい項目を最初に追加
            _items.Insert(0, item);

            // 上限を超える場合、古い項目を削除
            TrimExcess();
        }

        /// <summary>
        /// 最大数を超えた履歴項目を削除します
        /// </summary>
        private void TrimExcess()
        {
            if (_items.Count > _maxItems)
            {
                _items.RemoveRange(_maxItems, _items.Count - _maxItems);
            }
        }

        /// <summary>
        /// 履歴をクリアします
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// 指定したインデックスの履歴項目を取得します
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>履歴項目</returns>
        public object GetItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                return _items[index];
            }

            return null;
        }
    }

    /// <summary>
    /// FormHistorySelectionDropdownと履歴サービスの連携用ヘルパークラス
    /// </summary>
    public static class HistoryDropdownHelper
    {
        /// <summary>
        /// 履歴サービスから履歴を取得してドロップダウンに表示します
        /// </summary>
        /// <param name="dropdown">履歴ドロップダウンコントロール</param>
        /// <param name="historyService">履歴サービス</param>
        /// <param name="historyType">履歴タイプ</param>
        /// <param name="position">表示位置</param>
        /// <param name="targetControl">基準となるコントロール</param>
        public static void ShowHistory(
            FormHistorySelectionDropdown dropdown,
            HistoryService historyService,
            string historyType,
            Position position,
            Control targetControl)
        {
            var history = historyService.GetHistory(historyType);
            dropdown.ShowHistory(position, targetControl, history.Items as IList<object>);

            // 履歴クリアイベントの処理
            dropdown.HistoryCleared += (sender, e) => {
                historyService.ClearHistory(historyType);
            };
        }

        /// <summary>
        /// 履歴サービスに新しい履歴項目を追加します
        /// </summary>
        /// <param name="historyService">履歴サービス</param>
        /// <param name="historyType">履歴タイプ</param>
        /// <param name="item">追加する項目</param>
        public static void AddHistoryItem(
            HistoryService historyService,
            string historyType,
            object item)
        {
            historyService.AddItem(historyType, item);
        }

        /// <summary>
        /// 履歴選択ドロップダウンの設定を履歴サービスと同期します
        /// </summary>
        /// <param name="dropdown">履歴ドロップダウンコントロール</param>
        /// <param name="historyService">履歴サービス</param>
        /// <param name="historyType">履歴タイプ</param>
        public static void SyncDropdownSettings(
            FormHistorySelectionDropdown dropdown,
            HistoryService historyService,
            string historyType)
        {
            // ドロップダウンの表示最大数を履歴サービスの最大数と同期
            historyService.SetMaxItems(historyType, dropdown.MaxVisibleItems);
        }
    }
}
