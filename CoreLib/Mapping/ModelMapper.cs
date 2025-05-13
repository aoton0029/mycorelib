using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Mapping
{
    /// <summary>
    /// オブジェクト間のマッピングを管理するインターフェース
    /// </summary>
    public interface IModelMapper
    {
        /// <summary>
        /// ソースオブジェクトから新しい宛先オブジェクトにマッピング
        /// </summary>
        /// <typeparam name="TSource">ソースの型</typeparam>
        /// <typeparam name="TDestination">宛先の型</typeparam>
        /// <param name="source">ソースオブジェクト</param>
        /// <returns>マッピングされた新しい宛先オブジェクト</returns>
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();

        /// <summary>
        /// ソースオブジェクトから既存の宛先オブジェクトにマッピング
        /// </summary>
        /// <typeparam name="TSource">ソースの型</typeparam>
        /// <typeparam name="TDestination">宛先の型</typeparam>
        /// <param name="source">ソースオブジェクト</param>
        /// <param name="destination">宛先オブジェクト</param>
        /// <returns>マッピングされた宛先オブジェクト</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// ソースコレクションから新しい宛先コレクションにマッピング
        /// </summary>
        /// <typeparam name="TSource">ソースの型</typeparam>
        /// <typeparam name="TDestination">宛先の型</typeparam>
        /// <param name="sources">ソースコレクション</param>
        /// <returns>マッピングされた新しい宛先コレクション</returns>
        IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> sources) where TDestination : new();

        /// <summary>
        /// カスタムマッピング構成を作成
        /// </summary>
        /// <typeparam name="TSource">ソースの型</typeparam>
        /// <typeparam name="TDestination">宛先の型</typeparam>
        /// <returns>マッピング構成ビルダー</returns>
        IMapperConfigBuilder<TSource, TDestination> CreateMap<TSource, TDestination>() where TDestination : new();
    }

    /// <summary>
    /// マッピング構成ビルダーのインターフェース
    /// </summary>
    public interface IMapperConfigBuilder<TSource, TDestination>
    {
        /// <summary>
        /// プロパティのマッピングを設定
        /// </summary>
        /// <param name="destinationMember">宛先プロパティを選択する式</param>
        /// <param name="sourceMember">ソースプロパティを選択する式</param>
        /// <returns>マッピング構成ビルダー</returns>
        IMapperConfigBuilder<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Expression<Func<TSource, TMember>> sourceMember);

        /// <summary>
        /// マッピングから除外するプロパティを指定
        /// </summary>
        /// <param name="destinationMember">除外する宛先プロパティを選択する式</param>
        /// <returns>マッピング構成ビルダー</returns>
        IMapperConfigBuilder<TSource, TDestination> Ignore<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember);

        /// <summary>
        /// マッピング実行前に呼び出されるアクションを設定
        /// </summary>
        /// <param name="action">マッピング前に実行するアクション</param>
        /// <returns>マッピング構成ビルダー</returns>
        IMapperConfigBuilder<TSource, TDestination> BeforeMap(Action<TSource, TDestination> action);

        /// <summary>
        /// マッピング実行後に呼び出されるアクションを設定
        /// </summary>
        /// <param name="action">マッピング後に実行するアクション</param>
        /// <returns>マッピング構成ビルダー</returns>
        IMapperConfigBuilder<TSource, TDestination> AfterMap(Action<TSource, TDestination> action);
    }

    /// <summary>
    /// プロパティマッピング情報クラス
    /// </summary>
    internal class PropertyMapping
    {
        /// <summary>
        /// 宛先プロパティ
        /// </summary>
        public PropertyInfo DestinationProperty { get; }

        /// <summary>
        /// ソースプロパティ
        /// </summary>
        public PropertyInfo? SourceProperty { get; }

        /// <summary>
        /// カスタム値取得関数
        /// </summary>
        public Func<object, object?>? ValueGetter { get; }

        /// <summary>
        /// マッピングを無視するかどうか
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// プロパティ間のマッピングコンストラクタ
        /// </summary>
        public PropertyMapping(PropertyInfo destinationProperty, PropertyInfo sourceProperty)
        {
            DestinationProperty = destinationProperty;
            SourceProperty = sourceProperty;
            ValueGetter = null;
            Ignore = false;
        }

        /// <summary>
        /// カスタム値取得関数によるマッピングコンストラクタ
        /// </summary>
        public PropertyMapping(PropertyInfo destinationProperty, Func<object, object?> valueGetter)
        {
            DestinationProperty = destinationProperty;
            SourceProperty = null;
            ValueGetter = valueGetter;
            Ignore = false;
        }
    }

    /// <summary>
    /// マッピング構成情報クラス
    /// </summary>
    internal class MappingConfiguration
    {
        /// <summary>
        /// プロパティマッピングのリスト
        /// </summary>
        public List<PropertyMapping> PropertyMappings { get; } = new List<PropertyMapping>();

        /// <summary>
        /// マッピング前アクション
        /// </summary>
        public Action<object, object>? BeforeMapAction { get; set; }

        /// <summary>
        /// マッピング後アクション
        /// </summary>
        public Action<object, object>? AfterMapAction { get; set; }
    }

    /// <summary>
    /// マッピング構成ビルダー実装
    /// </summary>
    internal class MapperConfigBuilder<TSource, TDestination> : IMapperConfigBuilder<TSource, TDestination>
        where TDestination : new()
    {
        private readonly MappingConfiguration _config;
        private readonly ModelMapper _mapper;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MapperConfigBuilder(ModelMapper mapper, MappingConfiguration config)
        {
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// プロパティのマッピングを設定
        /// </summary>
        public IMapperConfigBuilder<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Expression<Func<TSource, TMember>> sourceMember)
        {
            var destProperty = GetPropertyInfo(destinationMember);
            var sourceProperty = GetPropertyInfo(sourceMember);

            if (destProperty != null && sourceProperty != null)
            {
                // 既存のマッピングを削除
                _config.PropertyMappings.RemoveAll(pm => pm.DestinationProperty.Name == destProperty.Name);

                // 新しいマッピングを追加
                _config.PropertyMappings.Add(new PropertyMapping(destProperty, sourceProperty));
            }

            return this;
        }

        /// <summary>
        /// マッピングから除外するプロパティを指定
        /// </summary>
        public IMapperConfigBuilder<TSource, TDestination> Ignore<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember)
        {
            var property = GetPropertyInfo(destinationMember);
            if (property != null)
            {
                // 既存のマッピングを探す
                var existingMapping = _config.PropertyMappings.FirstOrDefault(
                    pm => pm.DestinationProperty.Name == property.Name);

                if (existingMapping != null)
                {
                    existingMapping.Ignore = true;
                }
                else
                {
                    _config.PropertyMappings.Add(new PropertyMapping(property, (PropertyInfo)null!)
                    {
                        Ignore = true
                    });
                }
            }

            return this;
        }

        /// <summary>
        /// マッピング実行前に呼び出されるアクションを設定
        /// </summary>
        public IMapperConfigBuilder<TSource, TDestination> BeforeMap(Action<TSource, TDestination> action)
        {
            _config.BeforeMapAction = (src, dest) => action((TSource)src, (TDestination)dest);
            return this;
        }

        /// <summary>
        /// マッピング実行後に呼び出されるアクションを設定
        /// </summary>
        public IMapperConfigBuilder<TSource, TDestination> AfterMap(Action<TSource, TDestination> action)
        {
            _config.AfterMapAction = (src, dest) => action((TSource)src, (TDestination)dest);
            return this;
        }

        /// <summary>
        /// 式からプロパティ情報を取得
        /// </summary>
        private PropertyInfo? GetPropertyInfo<TObject, TMember>(Expression<Func<TObject, TMember>> expression)
        {
            if (expression.Body is MemberExpression memberExpr &&
                memberExpr.Member is PropertyInfo property)
            {
                return property;
            }
            return null;
        }
    }

    /// <summary>
    /// オブジェクト間のマッピングを提供するクラス
    /// </summary>
    public class ModelMapper : IModelMapper
    {
        private readonly Dictionary<TypePair, MappingConfiguration> _mappingConfigurations = new();

        /// <summary>
        /// ソースオブジェクトから新しい宛先オブジェクトにマッピング
        /// </summary>
        public TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var destination = new TDestination();
            return Map(source, destination);
        }

        /// <summary>
        /// ソースオブジェクトから既存の宛先オブジェクトにマッピング
        /// </summary>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            var typePair = new TypePair(typeof(TSource), typeof(TDestination));

            // カスタムマッピング設定を探す
            if (_mappingConfigurations.TryGetValue(typePair, out var config))
            {
                // マッピング前処理
                config.BeforeMapAction?.Invoke(source, destination);

                // カスタムマッピングによる処理
                MapWithConfiguration(source, destination, config);

                // マッピング後処理
                config.AfterMapAction?.Invoke(source, destination);
            }
            else
            {
                // 自動マッピング
                MapAutomatically(source, destination);
            }

            return destination;
        }

        /// <summary>
        /// ソースコレクションから新しい宛先コレクションにマッピング
        /// </summary>
        public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> sources)
            where TDestination : new()
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            var destinations = new List<TDestination>();
            foreach (var source in sources)
            {
                destinations.Add(Map<TSource, TDestination>(source));
            }
            return destinations;
        }

        /// <summary>
        /// カスタムマッピング構成を作成
        /// </summary>
        public IMapperConfigBuilder<TSource, TDestination> CreateMap<TSource, TDestination>()
            where TDestination : new()
        {
            var typePair = new TypePair(typeof(TSource), typeof(TDestination));
            var config = new MappingConfiguration();
            _mappingConfigurations[typePair] = config;

            return new MapperConfigBuilder<TSource, TDestination>(this, config);
        }

        /// <summary>
        /// 構成に基づいたマッピング処理
        /// </summary>
        private void MapWithConfiguration(object source, object destination, MappingConfiguration config)
        {
            foreach (var mapping in config.PropertyMappings)
            {
                if (mapping.Ignore)
                    continue;

                try
                {
                    object? value = null;

                    if (mapping.ValueGetter != null)
                    {
                        // カスタム値取得関数を使用
                        value = mapping.ValueGetter(source);
                    }
                    else if (mapping.SourceProperty != null && mapping.SourceProperty.CanRead)
                    {
                        // ソースプロパティから値を取得
                        value = mapping.SourceProperty.GetValue(source);
                    }

                    // 宛先プロパティに値を設定
                    if (mapping.DestinationProperty.CanWrite)
                    {
                        try
                        {
                            // 型変換を試みる
                            value = ConvertValue(value, mapping.DestinationProperty.PropertyType);
                            mapping.DestinationProperty.SetValue(destination, value);
                        }
                        catch
                        {
                            // 型変換エラーは無視
                        }
                    }
                }
                catch
                {
                    // マッピングエラーは無視して次のプロパティに進む
                }
            }
        }

        /// <summary>
        /// 自動プロパティマッピング処理
        /// </summary>
        private void MapAutomatically(object source, object destination)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var destProps = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var destProp in destProps)
            {
                var sourceProp = sourceType.GetProperty(destProp.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (sourceProp != null && sourceProp.CanRead)
                {
                    try
                    {
                        var value = sourceProp.GetValue(source);

                        // 型変換を試みる
                        value = ConvertValue(value, destProp.PropertyType);

                        destProp.SetValue(destination, value);
                    }
                    catch
                    {
                        // 型変換エラーは無視
                    }
                }
            }
        }

        /// <summary>
        /// 値を宛先の型に変換
        /// </summary>
        private object? ConvertValue(object? value, Type destinationType)
        {
            if (value == null)
                return destinationType.IsValueType ? Activator.CreateInstance(destinationType) : null;

            var sourceType = value.GetType();

            // 宛先タイプが既に互換性があるかどうかをチェック
            if (destinationType.IsAssignableFrom(sourceType))
                return value;

            // null許容型のハンドリング
            if (Nullable.GetUnderlyingType(destinationType) is Type underlyingType)
                destinationType = underlyingType;

            // 文字列変換
            if (destinationType == typeof(string))
                return value.ToString();

            // Enum変換
            if (destinationType.IsEnum && value is string stringValue)
                return Enum.Parse(destinationType, stringValue, true);

            // 基本型の変換
            try
            {
                return Convert.ChangeType(value, destinationType);
            }
            catch
            {
                // 変換失敗
                return null;
            }
        }

        /// <summary>
        /// 型ペアを表す構造体
        /// </summary>
        private readonly struct TypePair : IEquatable<TypePair>
        {
            public Type SourceType { get; }
            public Type DestinationType { get; }

            public TypePair(Type sourceType, Type destinationType)
            {
                SourceType = sourceType;
                DestinationType = destinationType;
            }

            public bool Equals(TypePair other)
            {
                return SourceType == other.SourceType && DestinationType == other.DestinationType;
            }

            public override bool Equals(object? obj)
            {
                return obj is TypePair other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(SourceType, DestinationType);
            }
        }
    }

    /// <summary>
    /// マッピング属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapFromAttribute : Attribute
    {
        /// <summary>
        /// ソースプロパティ名
        /// </summary>
        public string SourcePropertyName { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MapFromAttribute(string sourcePropertyName)
        {
            SourcePropertyName = sourcePropertyName;
        }
    }

    /// <summary>
    /// マッピング無視属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreMapAttribute : Attribute
    {
    }

    /// <summary>
    /// サービス登録拡張メソッド
    /// </summary>
    public static class ModelMapperExtensions
    {
        /// <summary>
        /// モデルマッパーをDIコンテナに登録
        /// </summary>
        public static IServiceCollection AddModelMapper(this IServiceCollection services)
        {
            services.AddSingleton<IModelMapper, ModelMapper>();
            return services;
        }
    }
}
