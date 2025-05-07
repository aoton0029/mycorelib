using CoreLib.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Specifications
{
    internal class _Sample
    {
        // サンプルエンティティ
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public bool IsActive { get; set; }
            public int CategoryId { get; set; }
            public Category? Category { get; set; }
        }

        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        // 基本仕様の例
        public class ActiveProductsSpecification : BaseSpecification<Product>
        {
            public ActiveProductsSpecification() : base(p => p.IsActive)
            {
                // 関連エンティティをインクルード
                AddInclude(p => p.Category);

                // 価格順にソート
                ApplyOrderBy(p => p.Price);
            }
        }

        // より複雑な仕様の例
        public class ProductByCategoryAndPriceRangeSpecification : BaseSpecification<Product>
        {
            public ProductByCategoryAndPriceRangeSpecification(
                int categoryId,
                decimal minPrice,
                decimal maxPrice,
                int pageIndex = 0,
                int pageSize = 10)
                : base(p => p.CategoryId == categoryId &&
                            p.Price >= minPrice &&
                            p.Price <= maxPrice &&
                            p.IsActive)
            {
                // 関連エンティティをインクルード
                AddInclude(p => p.Category);

                // 価格の昇順でソート
                ApplyOrderBy(p => p.Price);

                // ページング適用
                ApplyPaging(pageIndex, pageSize);

                // 追跡を無効化（読み取り専用）
                AsNoTrackingQuery();
            }
        }

        // ビルダーを使用した仕様構築の例
        public static class ProductSpecifications
        {
            public static ISpecification<Product> GetActiveProductsByCategory(int categoryId, int pageIndex, int pageSize)
            {
                return new SpecificationBuilder<Product>()
                    .Where(p => p.IsActive)
                    .Where(p => p.CategoryId == categoryId)
                    .Include(p => p.Category)
                    .OrderByAscending(p => p.Name)
                    .ThenBy(p => p.Price)
                    .Paginate(pageIndex, pageSize)
                    .WithNoTracking()
                    .Build();
            }

            public static ISpecification<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
            {
                return new SpecificationBuilder<Product>()
                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                    .OrderByDescending(p => p.Price)
                    .Build();
            }
        }

        // 使用例
        public class ProductService
        {
            private readonly IRepository<Product, int> _productRepository;

            public ProductService(IRepository<Product, int> productRepository)
            {
                _productRepository = productRepository;
            }

            // 仕様を使用した例
            public async Task<IEnumerable<Product>> GetActiveProductsAsync()
            {
                var spec = new ActiveProductsSpecification();
                return await _productRepository.FindAsync(spec);
            }

            // ビルダーを使用した例
            public async Task<IEnumerable<Product>> GetProductsByCategoryAndPriceRangeAsync(
                int categoryId,
                decimal minPrice,
                decimal maxPrice,
                int pageIndex,
                int pageSize)
            {
                var spec = new SpecificationBuilder<Product>()
                    .Where(p => p.CategoryId == categoryId)
                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                    .Where(p => p.IsActive)
                    .Include(p => p.Category)
                    .OrderByAscending(p => p.Price)
                    .Paginate(pageIndex, pageSize)
                    .Build();

                return await _productRepository.FindAsync(spec);
            }
        }
    }
}
