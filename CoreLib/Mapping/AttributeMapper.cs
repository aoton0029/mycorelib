using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Mapping
{
    /// <summary>
    /// 属性ベースのマッピングを提供するクラス
    /// </summary>
    public class AttributeMapper
    {
        private readonly IModelMapper _mapper;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AttributeMapper(IModelMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 型間のマッピング設定を登録
        /// </summary>
        public void RegisterMap<TSource, TDestination>() where TDestination : new()
        {
            ConfigureMapping<TSource, TDestination>();
        }

        /// <summary>
        /// ソースオブジェクトから宛先オブジェクトへのマッピング
        /// </summary>
        public TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// ソースオブジェクトから既存の宛先オブジェクトへのマッピング
        /// </summary>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }

        /// <summary>
        /// ソースコレクションから宛先コレクションへのマッピング
        /// </summary>
        public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> sources)
            where TDestination : new()
        {
            return _mapper.MapCollection<TSource, TDestination>(sources);
        }

        /// <summary>
        /// 属性に基づいたマッピング設定を構成
        /// </summary>
        private void ConfigureMapping<TSource, TDestination>() where TDestination : new()
        {
            var mapBuilder = _mapper.CreateMap<TSource, TDestination>();
            var destProperties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var destProperty in destProperties)
            {
                // マッピング無視属性があるか確認
                if (destProperty.GetCustomAttribute<IgnoreMapAttribute>() != null)
                {
                    // リフレクションを使用して適切なIgnoreメソッドを呼び出す
                    InvokeGenericMethod(mapBuilder, "Ignore", destProperty);
                    continue;
                }

                // カスタムマッピング属性があるか確認
                var mapFromAttr = destProperty.GetCustomAttribute<MapFromAttribute>();
                if (mapFromAttr != null)
                {
                    var sourceProperty = typeof(TSource).GetProperty(mapFromAttr.SourcePropertyName);
                    if (sourceProperty != null)
                    {
                        // リフレクションを使用して適切なForMemberメソッドを呼び出す
                        InvokeForMemberMethod(mapBuilder, destProperty, sourceProperty);
                    }
                }
            }
        }

        /// <summary>
        /// リフレクションを使用してIgnoreメソッドを呼び出す
        /// </summary>
        private void InvokeGenericMethod(IMapperConfigBuilder<TSource, TDestination> builder, string methodName,
            PropertyInfo destProperty)
        {
            var method = builder.GetType().GetMethod(methodName);
            if (method != null)
            {
                var genericMethod = method.MakeGenericMethod(destProperty.PropertyType);
                var lambda = CreatePropertyExpression(typeof(TDestination), destProperty.Name, destProperty.PropertyType);
                genericMethod.Invoke(builder, new[] { lambda });
            }
        }

        /// <summary>
        /// リフレクションを使用してForMemberメソッドを呼び出す
        /// </summary>
        private void InvokeForMemberMethod<TSource, TDestination>(IMapperConfigBuilder<TSource, TDestination> builder,
            PropertyInfo destProperty, PropertyInfo sourceProperty)
        {
            var method = builder.GetType().GetMethod("ForMember");
            if (method != null)
            {
                var genericMethod = method.MakeGenericMethod(destProperty.PropertyType);
                var destLambda = CreatePropertyExpression(typeof(TDestination), destProperty.Name, destProperty.PropertyType);
                var sourceLambda = CreatePropertyExpression(typeof(TSource), sourceProperty.Name, sourceProperty.PropertyType);
                genericMethod.Invoke(builder, new[] { destLambda, sourceLambda });
            }
        }

        /// <summary>
        /// プロパティアクセス式を動的に生成
        /// </summary>
        private static object CreatePropertyExpression(Type objectType, string propertyName, Type propertyType)
        {
            // パラメータ式: x
            var parameter = System.Linq.Expressions.Expression.Parameter(objectType, "x");

            // プロパティアクセス式: x.Property
            var property = System.Linq.Expressions.Expression.Property(parameter, propertyName);

            // ラムダ式: x => x.Property
            var lambda = System.Linq.Expressions.Expression.Lambda(property, parameter);

            return lambda;
        }
    }

    /// <summary>
    /// 属性マッパーのサービス登録拡張メソッド
    /// </summary>
    public static class AttributeMapperExtensions
    {
        /// <summary>
        /// 属性マッパーをDIコンテナに登録
        /// </summary>
        public static IServiceCollection AddAttributeMapper(this IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IModelMapper)))
            {
                services.AddModelMapper();
            }

            services.AddSingleton<AttributeMapper>();
            return services;
        }

        /// <summary>
        /// モデルマッピングサービス一式をDIコンテナに登録
        /// </summary>
        public static IServiceCollection AddModelMapping(this IServiceCollection services)
        {
            services.AddModelMapper();
            services.AddAttributeMapper();
            return services;
        }
    }
}
