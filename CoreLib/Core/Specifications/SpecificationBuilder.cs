using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Specifications
{
    /// <summary>
    /// 仕様ビルダー - 流暢なAPIで仕様を構築
    /// </summary>
    /// <typeparam name="T">エンティティの型</typeparam>
    public class SpecificationBuilder<T> : ISpecification<T>
    {
        /// <summary>
        /// フィルター式
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; private set; } = x => true;

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
        /// デフォルトコンストラクタ
        /// </summary>
        public SpecificationBuilder() { }

        /// <summary>
        /// 条件を適用
        /// </summary>
        public SpecificationBuilder<T> Where(Expression<Func<T, bool>> criteria)
        {
            if (Criteria == x => true)
            {
                Criteria = criteria;
            }
            else
            {
                // パラメータ式を取得
                var parameter = Expression.Parameter(typeof(T), "x");

                // オリジナルの式のパラメータを置き換え
                var visitor1 = new ParameterReplacerVisitor(Criteria.Parameters[0], parameter);
                var left = visitor1.Visit(Criteria.Body);

                var visitor2 = new ParameterReplacerVisitor(criteria.Parameters[0], parameter);
                var right = visitor2.Visit(criteria.Body);

                // AND演算子で結合
                var combinedBody = Expression.AndAlso(left, right);

                // 新しいラムダ式を構築
                Criteria = Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
            }

            return this;
        }

        /// <summary>
        /// OR条件を適用
        /// </summary>
        public SpecificationBuilder<T> Or(Expression<Func<T, bool>> criteria)
        {
            // パラメータ式を取得
            var parameter = Expression.Parameter(typeof(T), "x");

            // オリジナルの式のパラメータを置き換え
            var visitor1 = new ParameterReplacerVisitor(Criteria.Parameters[0], parameter);
            var left = visitor1.Visit(Criteria.Body);

            var visitor2 = new ParameterReplacerVisitor(criteria.Parameters[0], parameter);
            var right = visitor2.Visit(criteria.Body);

            // OR演算子で結合
            var combinedBody = Expression.OrElse(left, right);

            // 新しいラムダ式を構築
            Criteria = Expression.Lambda<Func<T, bool>>(combinedBody, parameter);

            return this;
        }

        /// <summary>
        /// インクルード式を追加
        /// </summary>
        public SpecificationBuilder<T> Include(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
            return this;
        }

        /// <summary>
        /// 文字列型のインクルードパスを追加
        /// </summary>
        public SpecificationBuilder<T> Include(string includeString)
        {
            IncludeStrings.Add(includeString);
            return this;
        }

        /// <summary>
        /// ソート式を設定（昇順）
        /// </summary>
        public SpecificationBuilder<T> OrderByAscending(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            return this;
        }

        /// <summary>
        /// ソート式を設定（降順）
        /// </summary>
        public SpecificationBuilder<T> OrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
            return this;
        }

        /// <summary>
        /// 追加のソート式を追加（昇順）
        /// </summary>
        public SpecificationBuilder<T> ThenBy(Expression<Func<T, object>> thenByExpression)
        {
            if (OrderBy == null && OrderByDescending == null)
                throw new InvalidOperationException("最初にOrderByAscendingまたはOrderByDescendingを呼び出す必要があります");

            ThenByList.Add(thenByExpression);
            return this;
        }

        /// <summary>
        /// 追加のソート式を追加（降順）
        /// </summary>
        public SpecificationBuilder<T> ThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
        {
            if (OrderBy == null && OrderByDescending == null)
                throw new InvalidOperationException("最初にOrderByAscendingまたはOrderByDescendingを呼び出す必要があります");

            ThenByDescendingList.Add(thenByDescendingExpression);
            return this;
        }

        /// <summary>
        /// グループ化式を設定
        /// </summary>
        public SpecificationBuilder<T> GroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
            return this;
        }

        /// <summary>
        /// ページングを適用
        /// </summary>
        public SpecificationBuilder<T> Paginate(int pageIndex, int pageSize)
        {
            if (pageSize <= 0)
                throw new ArgumentException("ページサイズは1以上である必要があります", nameof(pageSize));

            if (pageIndex < 0)
                throw new ArgumentException("ページインデックスは0以上である必要があります", nameof(pageIndex));

            Skip = pageIndex * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
            return this;
        }

        /// <summary>
        /// ページングを適用（スキップと取得件数で直接指定）
        /// </summary>
        public SpecificationBuilder<T> PaginateExplicit(int skip, int take)
        {
            if (take <= 0)
                throw new ArgumentException("取得件数は1以上である必要があります", nameof(take));

            if (skip < 0)
                throw new ArgumentException("スキップ件数は0以上である必要があります", nameof(skip));

            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
            return this;
        }

        /// <summary>
        /// エンティティの追跡を無効化
        /// </summary>
        public SpecificationBuilder<T> WithNoTracking()
        {
            AsNoTracking = true;
            return this;
        }

        /// <summary>
        /// 仕様オブジェクトを構築
        /// </summary>
        public ISpecification<T> Build()
        {
            return this;
        }
    }

    /// <summary>
    /// 式ツリーのパラメータを置き換えるビジター
    /// </summary>
    internal class ParameterReplacerVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacerVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}
