using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Navigations
{
    public interface IPage
    {
        void OnPageShown(NavigationContext context);
        void OnPageLeave(NavigationContext context);
    }

    /// <summary>
    /// ナビゲーションのコンテキストを表すクラス
    /// </summary>
    public class NavigationContext
    {
        public Type? PrevPage { get; set; }
        public Type? CurrentPage { get; set; }
        public Type? NextPage { get; set; }
        public NavigationParameter TempData { get; set; }
    }

    /// <summary>
    /// 画面間で一次的なデータを渡すためのパラメータクラス
    /// </summary>
    public class NavigationParameter : Dictionary<string, object>
    {
        public NavigationParameter() { }
        public NavigationParameter(string key, object value)
        {
            this[key] = value;
        }
        public T? GetValue<T>(string key)
        {
            if (TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }
        public void SetValue<T>(string key, T value)
        {
            this[key] = value;
        }
    }

    /// <summary>
    /// ナビゲーションの結果を表すクラス
    /// </summary>
    public class NavigationResult
    {
        public bool ShouldClose { get; set; }               // ナビゲーションを終了して画面を閉じるか
        public Type? RedirectToPage { get; set; }           // 終了後、遷移すべきページ（nullで何もしない）
        public NavigationParameter TempData { get; set; }             // 次ページへ渡すデータ（RedirectToPageがある場合）

        public static NavigationResult Close() => new() { ShouldClose = true };
        public static NavigationResult None() => new() { ShouldClose = false };
        public static NavigationResult Redirect<T>(NavigationParameter data) => new()
        {
            RedirectToPage = typeof(T),
            TempData = data
        };
    }


    /// <summary>
    /// ナビゲーションイベント引数クラス
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        public NavigationContext Context { get; set; }
        public Type FromPage { get; set; }
        public Type ToPage { get; set; }
        public NavigationParameter Parameter { get; set; }
        public bool Cancel { get; set; }
    }

    public class NavigationService
    {
        private readonly Dictionary<Type, UserControl> _caches = new();
        private Control _currentPage;
        private Control _control;

        public event EventHandler<NavigationEventArgs> Navigating;
        public event EventHandler<NavigationEventArgs> Navigated;

        public NavigationService(Control container)
        {
            _control = container;
        }

        public void RegisterPage<TPage>(UserControl page)
        {
            RegisterPage(typeof(TPage), page);
        }

        public void RegisterPage(Type type, UserControl page)
        {
            if (!_caches.ContainsKey(type))
            {
                _caches[type] = page;
            }
        }

        public void NavigateTo(NavigationParameter parameters = null)
        {
            if (_currentPage != null && _currentPage is UserControl control)
            {
                NavigationTo(control, parameters);
            }
        }

        public void NavigateTo<TPage>(NavigationParameter parameters = null) where TPage : UserControl
        {
            NavigateTo(typeof(TPage), parameters);
        }

        public void NavigateTo(Type pageType, NavigationParameter parameters = null)
        {
            if (!_caches.TryGetValue(pageType, out UserControl page))
            {
                throw new InvalidOperationException($"ページタイプ {pageType.Name} は登録されていません。RegisterPage() メソッドで登録してください。");
            }

            NavigationTo(page, parameters);
        }

        public void NavigationTo(UserControl page, NavigationParameter parameters = null)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            Type currentPageType = _currentPage?.GetType();
            Type nextPageType = page.GetType();

            var context = new NavigationContext
            {
                PrevPage = currentPageType,
                CurrentPage = currentPageType,
                NextPage = nextPageType,
                TempData = parameters ?? new NavigationParameter()
            };

            // ナビゲーション前のイベント発火
            var args = new NavigationEventArgs
            {
                Context = context,
                FromPage = currentPageType,
                ToPage = nextPageType,
                Parameter = context.TempData,
                Cancel = false
            };

            Navigating?.Invoke(this, args);

            if (args.Cancel)
                return;

            // 現在のページに離脱通知
            if (_currentPage is IPage currentIPage)
            {
                currentIPage.OnPageLeave(context);
            }

            InternalNavigateTo(page, context);

            // ナビゲーション後のイベント発火
            args.Cancel = false;
            context.CurrentPage = nextPageType;
            context.NextPage = null;
            Navigated?.Invoke(this, args);
        }

        protected void InternalNavigateTo(UserControl page, NavigationContext context)
        {
            if (_control.Controls.Count > 0)
            {
                _control.Controls.Clear();
            }

            page.Dock = DockStyle.Fill;
            _control.Controls.Add(page);
            _currentPage = page;

            // 新しいページに表示通知
            if (page is IPage iPage)
            {
                iPage.OnPageShown(context);
            }
        }
    }
}
