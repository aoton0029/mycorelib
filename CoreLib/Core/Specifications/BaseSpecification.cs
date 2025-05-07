using CoreLib.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Specifications
{
    /// <summary>
    /// 仕様パターンのインターフェース
    /// </summary>
    /// <typeparam name="T">エンティティの型</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// フィルター式
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// インクルード式のリスト（関連エンティティの取得用）
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// 文字列型のインクルードパス
        /// </summary>
        List<string> IncludeStrings { get; }

        /// <summary>
        /// ソート式（昇順）
        /// </summary>
        Expression<Func<T, object>>? OrderBy { get; }

        /// <summary>
        /// ソート式（降順）
        /// </summary>
        Expression<Func<T, object>>? OrderByDescending { get; }

        /// <summary>
        /// 追加のソート式（昇順）リスト
        /// </summary>
        List<Expression<Func<T, object>>> ThenByList { get; }

        /// <summary>
        /// 追加のソート式（降順）リスト
        /// </summary>
        List<Expression<Func<T, object>>> ThenByDescendingList { get; }

        /// <summary>
        /// グループ化式
        /// </summary>
        Expression<Func<T, object>>? GroupBy { get; }

        /// <summary>
        /// ページングの有効化
        /// </summary>
        bool IsPagingEnabled { get; }

        /// <summary>
        /// ページサイズ
        /// </summary>
        int Take { get; }

        /// <summary>
        /// スキップ件数
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// 追跡の無効化
        /// </summary>
        bool AsNoTracking { get; }
    }

    /// <summary>
    /// 仕様パターンの基底実装
    /// </summary>
    /// <typeparam name="T">エンティティの型</typeparam>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        /// <summary>
        /// フィルター式
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; private set; }

        /// <summary>
        /// インクルード式のリスト
        /// </summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// 文字列型のインクルードパス
        /// </summary>
        public List<string> IncludeStrings { get; } = new List<string>();

        /// <summary>
        /// ソート式（昇順）
        /// </summary>
        public Expression<Func<T, object>>? OrderBy { get; private set; }

        /// <summary>
        /// ソート式（降順）
        /// </summary>
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        /// <summary>
        /// 追加のソート式（昇順）リスト
        /// </summary>
        public List<Expression<Func<T, object>>> ThenByList { get; } = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// 追加のソート式（降順）リスト
        /// </summary>
        public List<Expression<Func<T, object>>> ThenByDescendingList { get; } = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// グループ化式
        /// </summary>
        public Expression<Func<T, object>>? GroupBy { get; private set; }

        /// <summary>
        /// ページングの有効化
        /// </summary>
        public bool IsPagingEnabled { get; private set; } = false;

        /// <summary>
        /// ページサイズ
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// スキップ件数
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// 追跡の無効化
        /// </summary>
        public bool AsNoTracking { get; private set; } = false;

        /// <summary>
        /// デフォルトのコンストラクタ（すべてのエンティティを取得）
        /// </summary>
        protected BaseSpecification()
        {
            Criteria = _ => true;
        }

        /// <summary>
        /// フィルター付きコンストラクタ
        /// </summary>
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// インクルード式を追加
        /// </summary>
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// 文字列型のインクルードパスを追加
        /// </summary>
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        /// <summary>
        /// ソート式を設定（昇順）
        /// </summary>
        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        /// <summary>
        /// ソート式を設定（降順）
        /// </summary>
        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        /// <summary>
        /// 追加のソート式を追加（昇順）
        /// </summary>
        protected virtual void ApplyThenBy(Expression<Func<T, object>> thenByExpression)
        {
            ThenByList.Add(thenByExpression);
        }

        /// <summary>
        /// 追加のソート式を追加（降順）
        /// </summary>
        protected virtual void ApplyThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
        {
            ThenByDescendingList.Add(thenByDescendingExpression);
        }

        /// <summary>
        /// グループ化式を設定
        /// </summary>
        protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        /// <summary>
        /// ページングを適用
        /// </summary>
        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        /// <summary>
        /// ページングを適用（ページ番号指定）
        /// </summary>
        protected virtual void ApplyPaging(int pageIndex, int pageSize, bool applySkip = true)
        {
            if (pageSize <= 0)
                throw new ArgumentException("ページサイズは1以上である必要があります", nameof(pageSize));

            if (pageIndex < 0)
                throw new ArgumentException("ページインデックスは0以上である必要があります", nameof(pageIndex));

            Take = pageSize;
            Skip = applySkip ? pageIndex * pageSize : 0;
            IsPagingEnabled = true;
        }

        /// <summary>
        /// エンティティの追跡を無効化
        /// </summary>
        protected virtual void AsNoTrackingQuery()
        {
            AsNoTracking = true;
        }

        /// <summary>
        /// 仕様を満たすか判定する式を直接取得
        /// </summary>
        public Func<T, bool> GetCriteriaCompiled()
        {
            return Criteria.Compile();
        }
    }


    /// <summary>
    /// 汎用的なSpecification実装
    /// </summary>
    internal class GenericSpecification<T> : BaseSpecification<T> where T : class
    {
        public GenericSpecification(Expression<Func<T, bool>> criteria) : base(criteria)
        {
        }
    }

    /// <summary>
    /// ページングを適用したSpecification実装
    /// </summary>
    internal class PagedSpecification<T> : BaseSpecification<T> where T : class
    {
        public PagedSpecification(Expression<Func<T, bool>> criteria, int pageIndex, int pageSize)
            : base(criteria)
        {
            ApplyPaging((pageIndex - 1) * pageSize, pageSize);
        }
    }

    /// <summary>
    /// 順序付けを適用したSpecification実装
    /// </summary>
    internal class OrderedSpecification<T> : BaseSpecification<T> where T : class
    {
        public OrderedSpecification(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, bool descending = false)
            : base(criteria)
        {
            if (descending)
                ApplyOrderByDescending(orderBy);
            else
                ApplyOrderBy(orderBy);
        }
    }

}
