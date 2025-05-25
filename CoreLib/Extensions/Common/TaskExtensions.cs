using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.Common
{
    /// <summary>
    /// Task関連の拡張メソッド
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Taskにタイムアウトを設定
        /// </summary>
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            using var timeoutCancellationTokenSource = new CancellationTokenSource();

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task;  // 正常に完了した場合は結果を返す
            }
            else
            {
                throw new TimeoutException($"操作がタイムアウトしました: {timeout.TotalSeconds}秒");
            }
        }

        /// <summary>
        /// 安全にタスクを実行（例外を指定された方法で処理）
        /// </summary>
        public static async Task<Result<T>> SafeExecuteAsync<T>(this Task<T> task, Func<Exception, string>? errorMessageFactory = null)
        {
            try
            {
                var result = await task;
                return Result<T>.Success(result);
            }
            catch (Exception ex)
            {
                var errorMessage = errorMessageFactory?.Invoke(ex) ?? ex.Message;
                return Result<T>.Failure(errorMessage, ex);
            }
        }

        /// <summary>
        /// タスクを一定時間ごとにリトライ
        /// </summary>
        public static async Task<T> RetryAsync<T>(this Func<Task<T>> taskFactory, int maxRetries, TimeSpan delay)
        {
            Exception? lastException = null;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await taskFactory();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (i < maxRetries - 1)
                        await Task.Delay(delay);
                }
            }

            throw new AggregateException($"リトライ回数（{maxRetries}回）を超えました", lastException!);
        }

        /// <summary>
        /// タスクをバックグラウンドで実行し、完了を待たない
        /// </summary>
        public static void FireAndForget(this Task task, Action<Exception>? errorHandler = null)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && errorHandler != null)
                {
                    errorHandler(t.Exception!.InnerException ?? t.Exception!);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    /// <summary>
    /// 処理結果を表す汎用クラス
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }
        public Exception? Exception { get; }

        private Result(bool isSuccess, T? value, string? errorMessage, Exception? exception)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null, null);
        public static Result<T> Failure(string errorMessage, Exception? exception = null) =>
            new Result<T>(false, default, errorMessage, exception);
    }
}
