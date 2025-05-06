using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO.Formats
{
    /// <summary>
    /// HTML操作ユーティリティ
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// HTMLをエンコードする
        /// </summary>
        public static string Encode(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return WebUtility.HtmlEncode(html);
        }

        /// <summary>
        /// HTMLをデコードする
        /// </summary>
        public static string Decode(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return WebUtility.HtmlDecode(html);
        }

        /// <summary>
        /// HTMLからテキストを抽出（タグ除去）
        /// </summary>
        public static string ExtractText(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // HTMLタグを除去
            var tagRemoved = Regex.Replace(html, @"<[^>]+>|&nbsp;", " ");
            // 連続する空白を1つに
            tagRemoved = Regex.Replace(tagRemoved, @"\s+", " ");
            // 前後の空白を除去
            return tagRemoved.Trim();
        }

        /// <summary>
        /// HTMLファイルからテキストを抽出
        /// </summary>
        public static async Task<string> ExtractTextFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("HTMLファイルが見つかりません", filePath);

            string html = await File.ReadAllTextAsync(filePath);
            return ExtractText(html);
        }

        /// <summary>
        /// HTMLからメタデータを抽出
        /// </summary>
        public static Dictionary<string, string> ExtractMetaTags(string html)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(html))
                return result;

            // titleタグの取得
            var titleMatch = Regex.Match(html, @"<title>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (titleMatch.Success)
            {
                result["title"] = titleMatch.Groups[1].Value.Trim();
            }

            // metaタグの取得
            var metaMatches = Regex.Matches(html, @"<meta\s+(?:name|property)=""(.*?)""\s+content=""(.*?)""[^>]*>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in metaMatches)
            {
                string name = match.Groups[1].Value.Trim();
                string content = match.Groups[2].Value.Trim();
                result[name] = content;
            }

            return result;
        }

        /// <summary>
        /// HTML内の指定タグを取得
        /// </summary>
        public static List<string> ExtractTags(string html, string tagName)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(html) || string.IsNullOrEmpty(tagName))
                return result;

            var matches = Regex.Matches(html, $@"<{tagName}[^>]*>(.*?)</{tagName}>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }

        /// <summary>
        /// HTML内のリンク(aタグ)を抽出
        /// </summary>
        public static Dictionary<string, string> ExtractLinks(string html)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(html))
                return result;

            var matches = Regex.Matches(html, @"<a\s+(?:[^>]*?\s+)?href=""([^""]*)""\s*(?:[^>]*?)>(.*?)</a>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                string url = match.Groups[1].Value;
                string text = ExtractText(match.Groups[2].Value);

                if (!result.ContainsKey(url)) // 同じURLが複数ある場合は最初のもののみ
                    result[url] = text;
            }

            return result;
        }

        /// <summary>
        /// HTML内の画像(imgタグ)を抽出
        /// </summary>
        public static Dictionary<string, string> ExtractImages(string html)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(html))
                return result;

            var matches = Regex.Matches(html, @"<img\s+(?:[^>]*?\s+)?src=""([^""]*)""\s*(?:[^>]*?\s+)?alt=""([^""]*)""[^>]*>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                string src = match.Groups[1].Value;
                string alt = match.Groups[2].Value;

                if (!result.ContainsKey(src)) // 同じsrcが複数ある場合は最初のもののみ
                    result[src] = alt;
            }

            return result;
        }

        /// <summary>
        /// 指定された文字列をHTMLタグで囲む
        /// </summary>
        public static string WrapWithTag(string content, string tag, Dictionary<string, string>? attributes = null)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(tag))
                return content;

            var sb = new StringBuilder("<" + tag);

            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    sb.Append($" {attr.Key}=\"{Encode(attr.Value)}\"");
                }
            }

            sb.Append(">");
            sb.Append(content);
            sb.Append("</" + tag + ">");

            return sb.ToString();
        }

        /// <summary>
        /// テキストをHTMLのpタグに変換（改行をbrタグに）
        /// </summary>
        public static string TextToHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // テキストをHTMLエンコード
            string encoded = Encode(text);
            // 改行をbrタグに変換
            string withBreaks = encoded.Replace("\r\n", "<br>").Replace("\n", "<br>");
            // pタグで囲む
            return $"<p>{withBreaks}</p>";
        }

        /// <summary>
        /// 相対URLを絶対URLに変換
        /// </summary>
        public static string ResolveRelativeUrl(string baseUrl, string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return baseUrl;

            if (Uri.TryCreate(relativeUrl, UriKind.Absolute, out _))
                return relativeUrl; // 既に絶対URLの場合

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
                throw new ArgumentException("ベースURLが無効です", nameof(baseUrl));

            Uri.TryCreate(baseUri, relativeUrl, out var resolvedUri);
            return resolvedUri?.ToString() ?? relativeUrl;
        }

        /// <summary>
        /// HTMLに対してシンプルな検証を行う
        /// </summary>
        public static bool ValidateBasicHtml(string html, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrEmpty(html))
            {
                errors.Add("HTMLが空です");
                return false;
            }

            // 開始タグと終了タグのバランスをチェック
            var tags = new Dictionary<string, int>
            {
                { "html", 0 },
                { "head", 0 },
                { "body", 0 },
                { "div", 0 },
                { "p", 0 },
                { "span", 0 },
                { "a", 0 },
                { "table", 0 },
                { "tr", 0 },
                { "td", 0 },
                { "th", 0 },
                { "ul", 0 },
                { "ol", 0 },
                { "li", 0 }
            };

            // 開始タグをカウント
            foreach (var tag in tags.Keys.ToList())
            {
                var startMatches = Regex.Matches(html, $@"<{tag}(\s|>)", RegexOptions.IgnoreCase);
                var endMatches = Regex.Matches(html, $@"</{tag}>", RegexOptions.IgnoreCase);

                tags[tag] = startMatches.Count - endMatches.Count;

                if (tags[tag] != 0)
                {
                    errors.Add($"{tag}タグが{Math.Abs(tags[tag])}個{(tags[tag] > 0 ? "多い" : "少ない")}です");
                }
            }

            // 基本的なHTML構造の存在チェック
            if (!html.Contains("<html", StringComparison.OrdinalIgnoreCase))
                errors.Add("htmlタグがありません");

            if (!html.Contains("<head", StringComparison.OrdinalIgnoreCase))
                errors.Add("headタグがありません");

            if (!html.Contains("<body", StringComparison.OrdinalIgnoreCase))
                errors.Add("bodyタグがありません");

            if (!Regex.IsMatch(html, @"<!DOCTYPE\s+html>", RegexOptions.IgnoreCase))
                errors.Add("DOCTYPE宣言がありません");

            return errors.Count == 0;
        }

        /// <summary>
        /// HTMLを整形して読みやすくする
        /// </summary>
        public static string FormatHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            var result = new StringBuilder();
            int indentLevel = 0;
            bool inTag = false;
            bool inContent = false;
            var contentBuilder = new StringBuilder();

            for (int i = 0; i < html.Length; i++)
            {
                char c = html[i];

                // タグの開始
                if (c == '<')
                {
                    // コンテンツがあれば追加
                    if (inContent && contentBuilder.Length > 0)
                    {
                        string content = contentBuilder.ToString().Trim();
                        if (!string.IsNullOrEmpty(content))
                        {
                            result.AppendLine();
                            result.Append(new string(' ', indentLevel * 2));
                            result.Append(content);
                        }
                        contentBuilder.Clear();
                    }

                    inTag = true;
                    inContent = false;

                    // 終了タグか確認
                    if (i + 1 < html.Length && html[i + 1] == '/')
                    {
                        indentLevel = Math.Max(0, indentLevel - 1);
                    }

                    result.AppendLine();
                    result.Append(new string(' ', indentLevel * 2));
                    result.Append(c);
                }
                // タグの終了
                else if (c == '>')
                {
                    result.Append(c);
                    inTag = false;
                    inContent = true;

                    // 自己完結タグでなければインデントを増やす
                    if (i > 0 && html[i - 1] != '/' && i > 1 && html[i - 2] != '!')
                    {
                        // タグ名を取得して自己完結タグをチェック
                        int tagStart = html.LastIndexOf('<', i) + 1;
                        string tagName = string.Empty;
                        for (int j = tagStart; j < i; j++)
                        {
                            if (char.IsWhiteSpace(html[j]) || html[j] == '>')
                            {
                                tagName = html.Substring(tagStart, j - tagStart);
                                break;
                            }
                        }

                        // img, br, hr, input などは自己完結タグなのでインデントを増やさない
                        if (!string.Equals(tagName, "img", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(tagName, "br", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(tagName, "hr", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(tagName, "input", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(tagName, "meta", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(tagName, "link", StringComparison.OrdinalIgnoreCase))
                        {
                            indentLevel++;
                        }
                    }
                }
                else if (inTag)
                {
                    // タグ内の文字
                    result.Append(c);
                }
                else if (inContent)
                {
                    // コンテンツ内の文字
                    contentBuilder.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 指定されたURLからHTMLを取得し、処理を行う
        /// </summary>
        public static async Task<string> FetchAndProcessHtmlAsync(string url, Func<string, string> processor)
        {
            using var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CoreLib HTML Processor");

            try
            {
                string html = await client.GetStringAsync(url);
                return processor(html);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"HTMLの取得と処理に失敗しました: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// HTMLファイルを読み込み、処理を行い、結果を保存
        /// </summary>
        public static async Task ProcessHtmlFileAsync(string inputPath, string outputPath, Func<string, string> processor)
        {
            if (!File.Exists(inputPath))
                throw new FileNotFoundException("入力ファイルが見つかりません", inputPath);

            try
            {
                string html = await File.ReadAllTextAsync(inputPath);
                string processed = processor(html);

                // 出力ディレクトリの作成
                string? directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(outputPath, processed);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"HTMLファイルの処理に失敗しました: {ex.Message}", ex);
            }
        }
    }
}
