using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Ensures
{
    /// <summary>
    /// ローカライズ関連のヘルパークラス
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>
        /// アプリケーション全体のUICultureを設定
        /// </summary>
        public static void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        /// <summary>
        /// 日本語カルチャーを設定
        /// </summary>
        public static void SetJapaneseCulture()
        {
            SetCulture("ja-JP");
        }

        /// <summary>
        /// 英語カルチャーを設定
        /// </summary>
        public static void SetEnglishCulture()
        {
            SetCulture("en-US");
        }

        /// <summary>
        /// 現在のカルチャーが日本語かどうかを確認
        /// </summary>
        public static bool IsJapaneseCulture()
        {
            return CultureInfo.CurrentUICulture.Name.StartsWith("ja", StringComparison.OrdinalIgnoreCase);
        }
    }
}
