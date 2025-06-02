using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core
{
    public class DualListManager : INotifyPropertyChanged
    {
        private readonly string _idColumn;
        private readonly string _nameColumn;
        private readonly DataTable _table;
        private string _filterExpression = string.Empty;

        // プロパティ変更通知のためのイベント
        public event PropertyChangedEventHandler? PropertyChanged;

        // フィルター条件を保持するプロパティ
        public string FilterExpression
        {
            get => _filterExpression;
            set
            {
                if (_filterExpression != value)
                {
                    _filterExpression = value;
                    OnPropertyChanged(nameof(AvailableView));
                }
            }
        }

        // フィルター条件を組み合わせたRowFilterを作成
        private string GetAvailableRowFilter()
        {
            string baseFilter = "IsSelected = false";
            if (string.IsNullOrWhiteSpace(_filterExpression))
                return baseFilter;

            return $"{baseFilter} AND ({_filterExpression})";
        }

        // AvailableViewプロパティを修正してフィルターを適用
        public DataView AvailableView => new DataView(_table, GetAvailableRowFilter(), _nameColumn, DataViewRowState.CurrentRows);

        public DataView SelectedView => new DataView(_table, "IsSelected = true", "Order ASC", DataViewRowState.CurrentRows);

        public DualListManager(DataTable table, string idColumn, string nameColumn, IEnumerable<string>? preselectedIds = null)
        {
            _idColumn = idColumn;
            _nameColumn = nameColumn;
            _table = table;

            if (!_table.Columns.Contains("IsSelected"))
                _table.Columns.Add("IsSelected", typeof(bool));

            if (!_table.Columns.Contains("Order"))
                _table.Columns.Add("Order", typeof(int));

            // 選択状態の初期化
            preselectedIds ??= Enumerable.Empty<string>();
            int order = 0;

            foreach (DataRow row in _table.Rows)
            {
                var id = row[_idColumn]?.ToString();
                bool isSelected = preselectedIds.Contains(id);
                row["IsSelected"] = isSelected;
                row["Order"] = isSelected ? order++ : DBNull.Value;
            }
        }

        // プロパティ変更通知を発行するメソッド
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // フィルターを適用するメソッド
        public void ApplyFilter(string filterExpression)
        {
            FilterExpression = filterExpression;
        }

        // 特定の列に対するフィルターを適用するヘルパーメソッド
        public void ApplyColumnFilter(string columnName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                FilterExpression = string.Empty;
            }
            else
            {
                // DataViewのフィルター構文に従ってフィルタ式を作成
                FilterExpression = $"{columnName} LIKE '%{value.Replace("'", "''")}%'";
            }
        }

        public void Select(string id)
        {
            var row = FindRowById(id);
            if (row != null && !(bool)row["IsSelected"])
            {
                row["IsSelected"] = true;
                row["Order"] = SelectedView.Count;
                OnPropertyChanged(nameof(AvailableView));
                OnPropertyChanged(nameof(SelectedView));
            }
        }

        public void Deselect(string id)
        {
            var row = FindRowById(id);
            if (row != null && (bool)row["IsSelected"])
            {
                row["IsSelected"] = false;
                row["Order"] = DBNull.Value;
                Reorder();
                OnPropertyChanged(nameof(AvailableView));
                OnPropertyChanged(nameof(SelectedView));
            }
        }

        public void MoveUp(string id) => Move(id, -1);
        public void MoveDown(string id) => Move(id, 1);

        private void Move(string id, int delta)
        {
            var rows = SelectedView.Cast<DataRowView>().ToList();
            int index = rows.FindIndex(r => r[_idColumn]?.ToString() == id);
            int newIndex = index + delta;

            if (index < 0 || newIndex < 0 || newIndex >= rows.Count) return;

            var rowA = rows[index];
            var rowB = rows[newIndex];

            var tmp = rowA["Order"];
            rowA["Order"] = rowB["Order"];
            rowB["Order"] = tmp;

            SelectedView.Sort = "Order ASC";
            OnPropertyChanged(nameof(SelectedView));
        }

        public List<string> GetSelectedIdsInOrder()
        {
            return SelectedView.Cast<DataRowView>()
                .OrderBy(r => r["Order"])
                .Select(r => r[_idColumn]?.ToString() ?? "")
                .ToList();
        }

        private DataRow? FindRowById(string id)
        {
            return _table.AsEnumerable()
                .FirstOrDefault(r => r[_idColumn]?.ToString() == id);
        }

        private void Reorder()
        {
            var rows = _table.Select("IsSelected = true").OrderBy(r => r["Order"]).ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i]["Order"] = i;
            }
        }

        // オプション：保存・読み込み（選択IDのみ）
        public void SaveSelectedIds(string path)
        {
            var ids = GetSelectedIdsInOrder();
            File.WriteAllText(path, JsonSerializer.Serialize(ids));
        }

        public static List<string> LoadSelectedIds(string path)
        {
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path)) ?? new()
                : new();
        }
    }

    /// <summary>
    /// 汎用リストアイテムラッパークラス
    /// </summary>
    /// <typeparam name="T">ラップするアイテムの型</typeparam>
    public class ListItem<T>
    {
        // 元のデータ
        public T Item { get; }

        // 選択状態
        public bool IsSelected { get; set; }

        // 選択された順序
        public int? Order { get; set; }

        public ListItem(T item)
        {
            Item = item;
            IsSelected = false;
            Order = null;
        }
    }

    /// <summary>
    /// クラスのリストを使用したデュアルリストマネージャー
    /// </summary>
    /// <typeparam name="T">リストで管理するアイテムの型</typeparam>
    public class GenericDualListManager<T> : INotifyPropertyChanged
    {
        private readonly List<ListItem<T>> _items;
        private readonly Func<T, string> _idSelector;
        private readonly Func<T, string> _nameSelector;
        private Func<T, bool> _filterPredicate = _ => true;
        private string _filterText = string.Empty;

        // プロパティ変更通知のためのイベント
        public event PropertyChangedEventHandler? PropertyChanged;

        // フィルターテキストを保持するプロパティ
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged(nameof(AvailableItems));
                }
            }
        }

        // 利用可能アイテムのリスト (選択されていないアイテム + フィルタ適用)
        public IEnumerable<ListItem<T>> AvailableItems =>
            _items.Where(x => !x.IsSelected && _filterPredicate(x.Item))
                 .OrderBy(x => _nameSelector(x.Item));

        // 選択済みアイテムのリスト
        public IEnumerable<ListItem<T>> SelectedItems =>
            _items.Where(x => x.IsSelected)
                 .OrderBy(x => x.Order);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="items">管理するアイテムのリスト</param>
        /// <param name="idSelector">IDを取得するセレクタ関数</param>
        /// <param name="nameSelector">名前を取得するセレクタ関数</param>
        /// <param name="preselectedIds">事前に選択するID一覧</param>
        public GenericDualListManager(
            IEnumerable<T> items,
            Func<T, string> idSelector,
            Func<T, string> nameSelector,
            IEnumerable<string>? preselectedIds = null)
        {
            _idSelector = idSelector;
            _nameSelector = nameSelector;
            _items = new List<ListItem<T>>();

            // アイテムをラップして追加
            foreach (var item in items)
            {
                _items.Add(new ListItem<T>(item));
            }

            // 選択状態の初期化
            preselectedIds ??= Enumerable.Empty<string>();
            int order = 0;

            foreach (var wrapper in _items)
            {
                var id = _idSelector(wrapper.Item);
                if (preselectedIds.Contains(id))
                {
                    wrapper.IsSelected = true;
                    wrapper.Order = order++;
                }
            }
        }

        // プロパティ変更通知を発行するメソッド
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// フィルター条件を設定
        /// </summary>
        /// <param name="predicate">フィルターに使用する述語関数</param>
        public void ApplyFilter(Func<T, bool> predicate)
        {
            _filterPredicate = predicate ?? (_ => true);
            OnPropertyChanged(nameof(AvailableItems));
        }

        /// <summary>
        /// プロパティに対してテキストフィルターを適用
        /// </summary>
        /// <param name="propertySelector">検索対象プロパティを選択するセレクタ</param>
        /// <param name="value">検索テキスト</param>
        public void ApplyTextFilter(Func<T, string> propertySelector, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _filterPredicate = _ => true;
            }
            else
            {
                var searchText = value.ToLower();
                _filterPredicate = item => propertySelector(item).ToLower().Contains(searchText);
            }

            FilterText = value;
        }

        /// <summary>
        /// アイテムを選択状態にする
        /// </summary>
        /// <param name="id">選択するアイテムのID</param>
        public void Select(string id)
        {
            var item = FindItemById(id);
            if (item != null && !item.IsSelected)
            {
                item.IsSelected = true;
                item.Order = SelectedItems.Count();
                OnPropertyChanged(nameof(AvailableItems));
                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        /// <summary>
        /// アイテムの選択を解除する
        /// </summary>
        /// <param name="id">選択解除するアイテムのID</param>
        public void Deselect(string id)
        {
            var item = FindItemById(id);
            if (item != null && item.IsSelected)
            {
                item.IsSelected = false;
                item.Order = null;
                Reorder();
                OnPropertyChanged(nameof(AvailableItems));
                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        public void MoveUp(string id) => Move(id, -1);
        public void MoveDown(string id) => Move(id, 1);

        private void Move(string id, int delta)
        {
            var items = SelectedItems.ToList();
            int index = items.FindIndex(i => _idSelector(i.Item) == id);
            int newIndex = index + delta;

            if (index < 0 || newIndex < 0 || newIndex >= items.Count) return;

            var itemA = items[index];
            var itemB = items[newIndex];

            // 順序入れ替え
            var temp = itemA.Order;
            itemA.Order = itemB.Order;
            itemB.Order = temp;

            OnPropertyChanged(nameof(SelectedItems));
        }

        /// <summary>
        /// 選択されたIDを順番に取得
        /// </summary>
        public List<string> GetSelectedIdsInOrder()
        {
            return SelectedItems
                .OrderBy(i => i.Order)
                .Select(i => _idSelector(i.Item))
                .ToList();
        }

        /// <summary>
        /// 選択された項目を順番に取得
        /// </summary>
        public List<T> GetSelectedItemsInOrder()
        {
            return SelectedItems
                .OrderBy(i => i.Order)
                .Select(i => i.Item)
                .ToList();
        }

        private ListItem<T>? FindItemById(string id)
        {
            return _items.FirstOrDefault(i => _idSelector(i.Item) == id);
        }

        private void Reorder()
        {
            var selectedItems = _items.Where(i => i.IsSelected).OrderBy(i => i.Order).ToList();
            for (int i = 0; i < selectedItems.Count; i++)
            {
                selectedItems[i].Order = i;
            }
        }

        /// <summary>
        /// 選択IDをファイルに保存
        /// </summary>
        public void SaveSelectedIds(string path)
        {
            var ids = GetSelectedIdsInOrder();
            File.WriteAllText(path, JsonSerializer.Serialize(ids));
        }

        /// <summary>
        /// ファイルから選択IDを読み込み
        /// </summary>
        public static List<string> LoadSelectedIds(string path)
        {
            return File.Exists(path)
                ? JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path)) ?? new()
                : new();
        }
    }
}
