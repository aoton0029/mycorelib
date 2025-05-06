using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Core.Configuration
{
    /// <summary>
    /// 設定操作のヘルパークラス
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// ファイルから設定をロード
        /// </summary>
        public static async Task<T> LoadFromFileAsync<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
                return new T();

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }

        /// <summary>
        /// 設定をファイルに保存
        /// </summary>
        public static async Task SaveToFileAsync<T>(T settings, string filePath) where T : new()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(settings, options);
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// 設定値の検証
        /// </summary>
        public static ValidationResult Validate<T>(T settings, Action<T, ValidationResult> validator) where T : new()
        {
            var result = new ValidationResult();
            validator(settings, result);
            return result;
        }

        /// <summary>
        /// IConfigurationBuilderの拡張メソッド - 環境変数のロード
        /// </summary>
        public static IConfigurationBuilder AddStandardProviders(this IConfigurationBuilder builder, string environmentName, string basePath = null)
        {
            basePath ??= Directory.GetCurrentDirectory();

            return builder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }

    /// <summary>
    /// 設定値検証結果
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void ThrowIfInvalid()
        {
            if (!IsValid)
                throw new ConfigurationValidationException(string.Join(Environment.NewLine, Errors));
        }
    }

    /// <summary>
    /// 設定検証例外
    /// </summary>
    public class ConfigurationValidationException : Exception
    {
        public ConfigurationValidationException(string message) : base(message)
        {
        }
    }
}
