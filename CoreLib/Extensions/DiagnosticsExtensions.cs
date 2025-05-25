using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions
{
    /// <summary>
    /// 診断用拡張メソッド
    /// </summary>
    public static class DiagnosticsExtensions
    {
        /// <summary>
        /// コードブロックの実行時間を計測し、結果を返す
        /// </summary>
        public static TimeSpan Measure(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// 関数の実行時間を計測し、結果と共に返す
        /// </summary>
        public static (T Result, TimeSpan Elapsed) Measure<T>(Func<T> func)
        {
            var sw = Stopwatch.StartNew();
            var result = func();
            sw.Stop();
            return (result, sw.Elapsed);
        }

        /// <summary>
        /// 非同期処理の実行時間を計測し、結果を返す
        /// </summary>
        public static async Task<TimeSpan> MeasureAsync(Func<Task> asyncAction)
        {
            var sw = Stopwatch.StartNew();
            await asyncAction();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// 非同期関数の実行時間を計測し、結果と共に返す
        /// </summary>
        public static async Task<(T Result, TimeSpan Elapsed)> MeasureAsync<T>(Func<Task<T>> asyncFunc)
        {
            var sw = Stopwatch.StartNew();
            var result = await asyncFunc();
            sw.Stop();
            return (result, sw.Elapsed);
        }
    }
}
