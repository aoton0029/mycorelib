using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Mapping
{
    internal class _Sample
    {
        // ビルダーAPIを使用したマッピング設定
        public class MappingExample
        {
            private readonly IModelMapper _mapper;

            public MappingExample(IModelMapper mapper)
            {
                _mapper = mapper;

                // 初期化時にマッピング設定
                _mapper.CreateMap<Product, ProductDto>()
                    .ForMember(dest => dest.ProductPrice, src => src.Price)
                    .ForMember(dest => dest.IsAvailable, src => src.IsActive)
                    .Ignore(dest => dest.InternalNote)
                    .AfterMap((src, dest) =>
                    {
                        dest.LastUpdated = DateTime.UtcNow;
                        dest.FormattedPrice = $"¥{src.Price:N0}";
                    });

                _mapper.CreateMap<UserEntity, UserViewModel>()
                    .ForMember(dest => dest.FullName, src => src.DisplayName)
                    .ForMember(dest => dest.EmailAddress, src => src.Email);
            }

            public ProductDto MapProduct(Product product)
            {
                return _mapper.Map<Product, ProductDto>(product);
            }

            public List<UserViewModel> GetUserViewModels(IEnumerable<UserEntity> users)
            {
                return _mapper.MapCollection<UserEntity, UserViewModel>(users).ToList();
            }
        }

        // 属性を使用したマッピング
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int StockCount { get; set; }
            public string SKU { get; set; }    // 内部管理用コード
        }

        public class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; }

            [MapFrom("Description")]
            public string ProductDescription { get; set; }

            [MapFrom("Price")]
            public decimal ProductPrice { get; set; }

            [MapFrom("IsActive")]
            public bool IsAvailable { get; set; }

            public DateTime CreatedAt { get; set; }

            [IgnoreMap]
            public string InternalNote { get; set; }

            public string FormattedPrice { get; set; }

            public DateTime LastUpdated { get; set; }
        }

        // 属性ベースのマッピング使用例
        public class AttributeMappingExample
        {
            private readonly AttributeMapper _mapper;

            public AttributeMappingExample(AttributeMapper mapper)
            {
                _mapper = mapper;

                // マッピング設定を登録
                _mapper.RegisterMap<Product, ProductDto>();
            }

            public ProductDto MapProduct(Product product)
            {
                var dto = _mapper.Map<Product, ProductDto>(product);

                // マッピング後の追加処理
                dto.FormattedPrice = $"¥{product.Price:N0}";
                dto.LastUpdated = DateTime.UtcNow;

                return dto;
            }
        }

        // DI登録サンプル
        public void ConfigureServices(IServiceCollection services)
        {
            // モデルマッピングサービス一式を登録
            services.AddModelMapping();

            // または個別に登録
            // services.AddModelMapper();
            // services.AddAttributeMapper();
        }
    }
}
