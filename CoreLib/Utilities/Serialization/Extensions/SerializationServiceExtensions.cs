using CoreLib.Core.Enums;
using CoreLib.Utilities.Serialization.Formats;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Extensions
{
    /// <summary>
    /// DIサービス登録用拡張メソッド
    /// </summary>
    public static class SerializationServiceExtensions
    {
        /// <summary>
        /// シリアライゼーションサービスを登録
        /// </summary>
        public static IServiceCollection AddCoreSerializationServices(this IServiceCollection services)
        {
            // デフォルトのシリアライザーを登録
            services.AddSingleton<ISerializer>(new JsonSerializer());
            services.AddSingleton<ISerializer>(new XmlSerializer());
            services.AddSingleton<ISerializer>(new BinarySerializer());

            // 指定したフォーマットに対応するシリアライザーをDIで解決できるように登録
            services.AddTransient<Func<DataFormat, ISerializer>>(provider => format =>
            {
                // 登録されている全てのISerializer実装を取得
                var serializers = provider.GetServices<ISerializer>();

                // 指定フォーマットに対応するシリアライザーを探す
                foreach (var serializer in serializers)
                {
                    if (serializer.Format == format)
                        return serializer;
                }

                throw new NotSupportedException($"指定されたフォーマット '{format}' はサポートされていません。");
            });

            return services;
        }
    }
}
