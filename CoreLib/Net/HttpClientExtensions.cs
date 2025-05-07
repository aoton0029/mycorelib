using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Net
{
    /// <summary>
    /// HttpClient拡張メソッド
    /// </summary>
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions _defaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        /// <summary>
        /// GETリクエストを送信し結果をデシリアライズ
        /// </summary>
        public static async Task<T?> GetFromJsonAsync<T>(
            this HttpClient client,
            string requestUri,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= _defaultJsonOptions;

            using var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(options, cancellationToken);
        }

        /// <summary>
        /// POSTリクエストを送信し結果をデシリアライズ
        /// </summary>
        public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
            this HttpClient client,
            string requestUri,
            TRequest requestData,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= _defaultJsonOptions;

            using var response = await client.PostAsJsonAsync(requestUri, requestData, options, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>(options, cancellationToken);
        }

        /// <summary>
        /// PUTリクエストを送信し結果をデシリアライズ
        /// </summary>
        public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
            this HttpClient client,
            string requestUri,
            TRequest requestData,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= _defaultJsonOptions;

            using var response = await client.PutAsJsonAsync(requestUri, requestData, options, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>(options, cancellationToken);
        }

        /// <summary>
        /// APIにBearerトークンを設定
        /// </summary>
        public static void SetBearerToken(this HttpClient client, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("トークンは必須です", nameof(token));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// APIにBasic認証を設定
        /// </summary>
        public static void SetBasicAuthentication(this HttpClient client, string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("ユーザー名は必須です", nameof(username));

            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        /// <summary>
        /// リクエストタイムアウト付きのGETリクエストを送信
        /// </summary>
        public static async Task<T?> GetFromJsonWithTimeoutAsync<T>(
            this HttpClient client,
            string requestUri,
            TimeSpan timeout,
            JsonSerializerOptions? options = null)
        {
            using var cts = new CancellationTokenSource(timeout);

            try
            {
                return await client.GetFromJsonAsync<T>(requestUri, options, cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"リクエストがタイムアウトしました: {requestUri}");
            }
        }

        /// <summary>
        /// リトライ付きのHTTPリクエスト送信
        /// </summary>
        public static async Task<HttpResponseMessage> SendWithRetryAsync(
            this HttpClient client,
            HttpRequestMessage request,
            int maxRetries = 3,
            TimeSpan? initialRetryDelay = null,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (maxRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetries));

            initialRetryDelay ??= TimeSpan.FromSeconds(1);
            var retryDelay = initialRetryDelay.Value;

            HttpResponseMessage? response = null;
            Exception? lastException = null;

            for (int retry = 0; retry <= maxRetries; retry++)
            {
                try
                {
                    // 元のリクエストをクローン（リクエストは複数回送信できないため）
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);

                    response = await client.SendAsync(clonedRequest, cancellationToken);

                    // 成功またはクライアントエラーの場合はリトライしない
                    if (response.IsSuccessStatusCode || (int)response.StatusCode < 500)
                    {
                        return response;
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;

                    // 最後の試行で例外が発生した場合は再スロー
                    if (retry == maxRetries)
                        throw;
                }

                // リトライする前に待機（指数バックオフ）
                if (retry < maxRetries)
                {
                    await Task.Delay(retryDelay, cancellationToken);
                    retryDelay = TimeSpan.FromMilliseconds(retryDelay.TotalMilliseconds * 2);
                }
            }

            // すべてのリトライが失敗した場合
            if (response != null)
                return response;

            throw new HttpRequestException("すべてのリトライ試行が失敗しました", lastException);
        }

        /// <summary>
        /// HttpRequestMessageのクローン作成
        /// </summary>
        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);

            // ヘッダーをコピー
            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // プロパティをコピー
            foreach (var property in request.Options)
            {
                clone.Options.Set(new HttpRequestOptionsKey<object>(property.Key), property.Value);
            }

            // コンテンツをコピー
            if (request.Content != null)
            {
                var bytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(bytes);

                // コンテンツヘッダーをコピー
                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return clone;
        }
    }
}
