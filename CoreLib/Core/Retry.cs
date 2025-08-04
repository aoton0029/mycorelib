using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core
{
    public static class Retry
    {
        /// <summary>
        /// The default timeout to use for retries without a given timeout. The default is 1000ms.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// The default interval to use for retries without a given interval. The default is 100ms.
        /// </summary>
        public static TimeSpan DefaultInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Retries while the given method evaluates to true and returns the value from the method.
        /// If it fails, it returns the default of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="retryMethod">The method which is retried.</param>
        /// <param name="checkMethod">The method which is used to decide if a retry is needed or if the value is correct.</param>
        /// <param name="timeout">The timeout when the retry aborts.</param>
        /// <param name="interval">The interval of retries.</param>
        /// <param name="throwOnTimeout">A flag indicating if it should throw on timeout.</param>
        /// <param name="ignoreException">A flag indicating if it should retry on an exception.</param>
        /// <param name="timeoutMessage">The message that should be added to the timeout exception in case a timeout occurs.</param>
        /// <param name="lastValueOnTimeout">A flag indicating if the last value should be returned on timeout. Returns the default if the value could never be fetched.</param>
        /// <param name="defaultOnTimeout">Allows to define a default value in case of a timeout.</param>
        /// <returns>The value from <paramref name="retryMethod"/> or the default of <typeparamref name="T"/>.</returns>
        public static RetryResult<T> While<T>(Func<T> retryMethod
                                            , Func<T, bool> checkMethod
                                            , TimeSpan? timeout = null
                                            , TimeSpan? interval = null
                                            , bool throwOnTimeout = false
                                            , bool ignoreException = false
                                            , string timeoutMessage = null
                                            , bool lastValueOnTimeout = false
                                            , T defaultOnTimeout = default(T)
                                            , int? maxRetries = null)
        {
            var retryResult = new RetryResult<T>();
            timeout = timeout ?? DefaultTimeout;
            interval = interval ?? DefaultInterval;
            var startTime = DateTime.UtcNow;
            Exception lastException = null;
            T lastValue = defaultOnTimeout;

            do
            {
                retryResult.RetryCount++;
                try
                {
                    lastValue = retryMethod();
                    if (!checkMethod(lastValue))
                    {
                        return retryResult.Finish(lastValue);
                    }
                }
                catch (Exception ex)
                {
                    if (!ignoreException)
                    {
                        throw;
                    }
                    lastException = ex;
                    retryResult.SetException(ex);
                }

                if (maxRetries.HasValue && retryResult.RetryCount >= maxRetries.Value)
                {
                    return retryResult.Finish(lastValue, true);
                }

                Thread.Sleep(interval.Value);
            }
            while (!IsTimeoutReached(startTime, timeout.Value));

            if (throwOnTimeout)
            {
                timeoutMessage = timeoutMessage ?? "Timeout occurred in retry";
                throw new TimeoutException(timeoutMessage, lastException);
            }
            return retryResult.Finish(lastValueOnTimeout ? lastValue : defaultOnTimeout, true);
        }

        public static RetryResult<bool> While(Action retryAction, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null)
        {
            return While(
                () =>
                {
                    retryAction();
                    return true; // Actionが成功すればtrueを返す
                },
                result => result, // 常にtrueなら終了
                timeout,
                interval,
                throwOnTimeout,
                ignoreException,
                timeoutMessage,
                lastValueOnTimeout: false,
                defaultOnTimeout: false
            );
        }

        /// <summary>
        /// Retries while the given method evaluates to true and returns the value from the method.
        /// If it fails, it returns the default of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="retryMethod">The method which is retried.</param>
        /// <param name="checkMethod">The method which is used to decide if a retry is needed or if the value is correct.</param>
        /// <param name="retrySettings">The settings to use for retrying.</param>
        /// <param name="lastValueOnTimeout">A flag indicating if the last value should be returned on timeout. Returns the default if the value could never be fetched.</param>
        /// <param name="defaultOnTimeout">Allows to define a default value in case of a timeout.</param>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <returns>The value from <paramref name="retryMethod"/> or the default of <typeparamref name="T"/>.</returns>
        public static RetryResult<T> While<T>(Func<T> retryMethod, Func<T, bool> checkMethod, RetrySettings retrySettings, bool lastValueOnTimeout = false, T defaultOnTimeout = default(T))
        {
            return While<T>(retryMethod, checkMethod, retrySettings?.Timeout, retrySettings?.Interval, retrySettings?.ThrowOnTimeout ?? false, retrySettings?.IgnoreException ?? false, retrySettings?.TimeoutMessage, lastValueOnTimeout, defaultOnTimeout);
        }

        /// <summary>
        /// Retries while the given method evaluates to false and returns the value from the method.
        /// If it fails, it returns the default of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <returns>The value from <paramref name="retryMethod"/> or the default of <typeparamref name="T"/>.</returns>
        public static RetryResult<T> WhileNot<T>(Func<T> retryMethod, Func<T, bool> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null, bool lastValueOnTimeout = false, T defaultOnTimeout = default(T))
        {
            return While(retryMethod, (x) => !checkMethod(x), timeout, interval, throwOnTimeout, ignoreException, timeoutMessage, lastValueOnTimeout, defaultOnTimeout);
        }

        /// <summary>
        /// Retries while the given method evaluates to false and returns the value from the method.
        /// If it fails, it returns the default of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="retryMethod">The method which is retried.</param>
        /// <param name="checkMethod">The method which is used to decide if a retry is needed or if the value is correct.</param>
        /// <param name="retrySettings">The settings to use for retrying.</param>
        /// <param name="lastValueOnTimeout">A flag indicating if the last value should be returned on timeout. Returns the default if the value could never be fetched.</param>
        /// <param name="defaultOnTimeout">Allows to define a default value in case of a timeout.</param>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <returns>The value from <paramref name="retryMethod"/> or the default of <typeparamref name="T"/>.</returns>
        public static RetryResult<T> WhileNot<T>(Func<T> retryMethod, Func<T, bool> checkMethod, RetrySettings retrySettings = null, bool lastValueOnTimeout = false, T defaultOnTimeout = default(T))
        {
            return While(retryMethod, (x) => !checkMethod(x), retrySettings, lastValueOnTimeout, defaultOnTimeout);
        }

        /// <summary>
        /// Retries while the given method evaluates to true.
        /// </summary>
        /// <returns>True if the retry completed successfully within the time and false otherwise.</returns>
        public static RetryResult<bool> WhileTrue(Func<bool> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null)
        {
            // Use the generic retry. To have the correct return value on success, we need to inverse the result of the check method.
            return While(() => !checkMethod(), r => !r, timeout, interval, throwOnTimeout, ignoreException, timeoutMessage: timeoutMessage);
        }

        /// <summary>
        /// Retries while the given method evaluates to false.
        /// </summary>
        /// <returns>True if the retry completed successfully within the time and false otherwise.</returns>
        public static RetryResult<bool> WhileFalse(Func<bool> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null)
        {
            // Use the generic retry. To have the correct return value on success, we need to inverse the result of the check method.
            return While(checkMethod, r => !r, timeout, interval, throwOnTimeout, ignoreException, timeoutMessage: timeoutMessage);
        }

        /// <summary>
        /// Retries while the given method evaluates to null.
        /// </summary>
        /// <returns>The value from <paramref name="checkMethod"/> or the default of <typeparamref name="T"/> in case of a timeout.</returns>
        public static RetryResult<T> WhileNull<T>(Func<T> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null)
        {
            return While(checkMethod, r => r == null, timeout, interval, throwOnTimeout, ignoreException, timeoutMessage: timeoutMessage);
        }

        /// <summary>
        /// Retries while the given method evaluates to not null.
        /// </summary>
        /// <returns>True if it evaluated to null within the time or false in case of a timeout.</returns>
        public static RetryResult<bool> WhileNotNull<T>(Func<T> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null)
        {
            return WhileTrue(() => checkMethod() != null, timeout, interval, throwOnTimeout, ignoreException, timeoutMessage);
        }

        /// <summary>
        /// Retries while return value from the given method evaluates to null or has no elements.
        /// </summary>
        public static RetryResult<T> WhileEmpty<T>(Func<T> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null) where T : IEnumerable
        {
            return While(checkMethod, r => r == null || !r.GetEnumerator().MoveNext(), timeout, interval, throwOnTimeout, ignoreException, timeoutMessage: timeoutMessage);
        }

        /// <summary>
        /// Retries while return value from the given method is null or an empty string.
        /// </summary>
        public static RetryResult<string> WhileEmpty(Func<string> checkMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, bool ignoreException = false, string timeoutMessage = null)
        {
            return While(checkMethod, String.IsNullOrEmpty, timeout, interval, throwOnTimeout, ignoreException, timeoutMessage: timeoutMessage);
        }

        /// <summary>
        /// Retries while the given method has an exception.
        /// </summary>
        /// <returns>True if the method completed without exception, false otherwise.</returns>
        public static RetryResult<bool> WhileException(Action retryMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, string timeoutMessage = null)
        {
            var success = false;
            var result = WhileTrue(() => { retryMethod(); success = true; return false; }, timeout, interval, ignoreException: true, throwOnTimeout: throwOnTimeout, timeoutMessage: timeoutMessage);
            result.Result = success;
            return result;
        }

        /// <summary>
        /// Retries while the given method has an exception and returns the value from the method.
        /// </summary>
        public static RetryResult<T> WhileException<T>(Func<T> retryMethod, TimeSpan? timeout = null, TimeSpan? interval = null, bool throwOnTimeout = false, string timeoutMessage = null)
        {
            var returnValue = default(T);
            var result = WhileTrue(() => { returnValue = retryMethod(); return false; }, timeout, interval, ignoreException: true, throwOnTimeout: throwOnTimeout, timeoutMessage: timeoutMessage);
            var newResult = new RetryResult<T>();
            if (result.HadException)
            {
                newResult.SetException(result.LastException);
            }
            newResult.Finish(returnValue, result.TimedOut);
            return newResult;
        }

        /// <summary>
        /// Method which checks if the timeout is reached.
        /// </summary>
        private static bool IsTimeoutReached(DateTime startTime, TimeSpan timeout)
        {
            // Check for infinite timeout
            if (timeout.TotalMilliseconds < 0)
            {
                return false;
            }
            return DateTime.UtcNow.Subtract(startTime) > timeout;
        }

        ///// <summary>
        ///// Allows searching with retrying for a list of <see cref="AutomationElement"/>s.
        ///// </summary>
        ///// <param name="searchMethod">The method used to search for the element list.</param>
        ///// <param name="retrySettings">The settings to use for retrying.</param>
        ///// <returns>The list of found elements.</returns>
        //public static AutomationElement[] Find(Func<AutomationElement[]> searchMethod, RetrySettings retrySettings)
        //{
        //    if (retrySettings == null)
        //    {
        //        return searchMethod();
        //    }
        //    return Retry.WhileEmpty(searchMethod, retrySettings.Timeout, retrySettings.Interval, retrySettings.ThrowOnTimeout, retrySettings.IgnoreException, retrySettings.TimeoutMessage).Result;
        //}

        ///// <summary>
        ///// Allows searching with retrying for an <see cref="AutomationElement"/>.
        ///// </summary>
        ///// <param name="searchMethod">The method used to search for the element.</param>
        ///// <param name="retrySettings">The settings to use for retrying.</param>
        ///// <returns>The found element.</returns>
        //public static AutomationElement Find(Func<AutomationElement> searchMethod, RetrySettings retrySettings)
        //{
        //    if (retrySettings == null)
        //    {
        //        return searchMethod();
        //    }
        //    return Retry.WhileNull(searchMethod, retrySettings.Timeout, retrySettings.Interval, retrySettings.ThrowOnTimeout, retrySettings.IgnoreException, retrySettings.TimeoutMessage).Result;
        //}
    }

    public class RetryResult<T>
    {
        /// <summary>
        /// Constructor for a new <see cref="RetryResult{T}"/>.
        /// </summary>
        public RetryResult() : this(DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Constructor for a new <see cref="RetryResult{T}"/> with a given start time.
        /// </summary>
        /// <param name="manualStartTime">The start time to set.</param>
        internal RetryResult(DateTime manualStartTime)
        {
            StartTime = manualStartTime;
        }

        /// <summary>
        /// Date and time when the retry was started.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Date and time when the retry finished or aborted.
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Flag which indicates if the retry finished because it reached the timeout.
        /// </summary>
        public bool TimedOut { get; private set; }

        /// <summary>
        /// Flag which indicates if the retry finished successfully.
        /// </summary>
        public bool Success => !TimedOut;

        /// <summary>
        /// Contains the last occured exception in the retry (if any). Only usefull if "ignoreException" is set to true on the retry.
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Flag which indicates if the retry had an exception or not.
        /// </summary>
        public bool HadException => LastException != null;

        /// <summary>
        /// Contains the final value returned by the retry.
        /// </summary>
        public T Result { get; internal set; }

        /// <summary>
        /// Time span how long the retry did run.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Contains the counter on how many iterations the retry did before returning.
        /// </summary>
        public long RetryCount { get; internal set; }

        /// <summary>
        /// Finishes the retry and sets the according values.
        /// </summary>
        /// <param name="result">The value to set as result.</param>
        /// <param name="timedOut">The flag which indicates if the retry timed out or not.</param>
        /// <returns>The object itself for fluent usage.</returns>
        internal RetryResult<T> Finish(T result, bool timedOut = false)
        {
            EndTime = DateTime.UtcNow;
            Result = result;
            TimedOut = timedOut;
            return this;
        }

        /// <summary>
        /// Sets the last exception.
        /// </summary>
        /// <param name="ex">The exception to set.</param>
        /// <returns>The object itself for fluent usage.</returns>
        internal RetryResult<T> SetException(Exception ex)
        {
            LastException = ex;
            return this;
        }
    }

    public class RetrySettings
    {
        /// <summary>
        /// The timeout when the retry will abort if it did not succeed.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// The interval of the retries.
        /// </summary>
        public TimeSpan? Interval { get; set; }

        /// <summary>
        /// A flag indicating if it should throw an <see cref="TimeoutException"/> if the timeout is reached.
        /// </summary>
        public bool ThrowOnTimeout { get; set; }

        /// <summary>
        /// A flag indicating if it should continue retrying when an exception occurs.
        /// </summary>
        public bool IgnoreException { get; set; }

        /// <summary>
        /// The message that should be added to the timeout exception in case a timeout occurs.
        /// </summary>
        public string TimeoutMessage { get; set; }
    }


}
