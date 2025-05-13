using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Configuration
{
    internal class _Sample
    {
        /// <summary>
        /// 拡張されたアプリケーション設定
        /// </summary>
        public class AppSettings
        {
            public string ApplicationName { get; set; } = string.Empty;
            public string Environment { get; set; } = "Development";
            public string Version { get; set; } = "1.0.0";
            public bool EnabledCache { get; set; } = true;
            public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
            public int RetryCount { get; set; } = 3;
            public LogSettings LogSettings { get; set; } = new();
            public DatabaseSettings DatabaseSettings { get; set; } = new();
            public SecuritySettings SecuritySettings { get; set; } = new();
            public NotificationSettings NotificationSettings { get; set; } = new();
            public Dictionary<string, string> CustomSettings { get; set; } = new();

            public bool IsDevelopment => Environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
            public bool IsProduction => Environment.Equals("Production", StringComparison.OrdinalIgnoreCase);
            public bool IsStaging => Environment.Equals("Staging", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 拡張されたログ設定
        /// </summary>
        public class LogSettings
        {
            public string LogLevel { get; set; } = "Information";
            public bool EnableConsoleLogging { get; set; } = true;
            public bool EnableFileLogging { get; set; } = false;
            public string LogFilePath { get; set; } = "logs/app.log";
            public int RetainedFileCountLimit { get; set; } = 31;
            public long FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024; // 10MB
            public bool IncludeScopes { get; set; } = true;
            public bool UseUtcTimestamp { get; set; } = true;
        }

        /// <summary>
        /// データベース設定
        /// </summary>
        public class DatabaseSettings
        {
            public string ConnectionString { get; set; } = string.Empty;
            public string ProviderName { get; set; } = "SqlServer";
            public int CommandTimeout { get; set; } = 30;
            public bool EnableDetailedErrors { get; set; } = false;
            public bool EnableSensitiveDataLogging { get; set; } = false;
            public int MaxRetryCount { get; set; } = 3;
            public int MaxRetryDelay { get; set; } = 30;
            public bool EnableAutoMigration { get; set; } = false;
        }

        /// <summary>
        /// セキュリティ設定
        /// </summary>
        public class SecuritySettings
        {
            public JwtSettings JwtSettings { get; set; } = new();
            public PasswordSettings PasswordSettings { get; set; } = new();
            public CorsSettings CorsSettings { get; set; } = new();
            public bool RequireHttps { get; set; } = true;
            public bool EnableXssProtection { get; set; } = true;
            public bool EnableCsrfProtection { get; set; } = true;
        }

        /// <summary>
        /// JWT設定
        /// </summary>
        public class JwtSettings
        {
            public string SecretKey { get; set; } = string.Empty;
            public string Issuer { get; set; } = string.Empty;
            public string Audience { get; set; } = string.Empty;
            public int ExpiryMinutes { get; set; } = 60;
            public bool ValidateIssuerSigningKey { get; set; } = true;
            public bool ValidateIssuer { get; set; } = true;
            public bool ValidateAudience { get; set; } = true;
            public bool ValidateLifetime { get; set; } = true;
            public int ClockSkewMinutes { get; set; } = 5;
        }

        /// <summary>
        /// パスワード設定
        /// </summary>
        public class PasswordSettings
        {
            public int RequiredLength { get; set; } = 8;
            public bool RequireDigit { get; set; } = true;
            public bool RequireLowercase { get; set; } = true;
            public bool RequireUppercase { get; set; } = true;
            public bool RequireNonAlphanumeric { get; set; } = true;
            public int RequiredUniqueChars { get; set; } = 4;
            public int MaxFailedAttempts { get; set; } = 5;
            public int LockoutMinutes { get; set; } = 30;
            public int PasswordExpiryDays { get; set; } = 90;
        }

        /// <summary>
        /// CORS設定
        /// </summary>
        public class CorsSettings
        {
            public bool AllowAnyOrigin { get; set; } = false;
            public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
            public bool AllowAnyMethod { get; set; } = false;
            public string[] AllowedMethods { get; set; } = Array.Empty<string>();
            public bool AllowAnyHeader { get; set; } = false;
            public string[] AllowedHeaders { get; set; } = Array.Empty<string>();
            public bool AllowCredentials { get; set; } = false;
            public int PreflightMaxAgeSeconds { get; set; } = 600;
        }

        /// <summary>
        /// 通知設定
        /// </summary>
        public class NotificationSettings
        {
            public EmailSettings EmailSettings { get; set; } = new();
            public SmsSettings SmsSettings { get; set; } = new();
            public PushNotificationSettings PushNotificationSettings { get; set; } = new();
            public bool EnableBackgroundService { get; set; } = true;
            public int ProcessingIntervalSeconds { get; set; } = 60;
            public int MaxRetryCount { get; set; } = 3;
        }

        /// <summary>
        /// メール設定
        /// </summary>
        public class EmailSettings
        {
            public string SmtpServer { get; set; } = string.Empty;
            public int Port { get; set; } = 25;
            public bool EnableSsl { get; set; } = true;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FromEmail { get; set; } = string.Empty;
            public string FromName { get; set; } = string.Empty;
            public int ConnectionTimeout { get; set; } = 10;
            public bool UseDefaultCredentials { get; set; } = false;
        }

        /// <summary>
        /// SMS設定
        /// </summary>
        public class SmsSettings
        {
            public string Provider { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string ApiSecret { get; set; } = string.Empty;
            public string FromNumber { get; set; } = string.Empty;
            public int Timeout { get; set; } = 10;
        }

        /// <summary>
        /// プッシュ通知設定
        /// </summary>
        public class PushNotificationSettings
        {
            public string Provider { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string AppId { get; set; } = string.Empty;
            public bool SandboxMode { get; set; } = false;
        }

    }
}
