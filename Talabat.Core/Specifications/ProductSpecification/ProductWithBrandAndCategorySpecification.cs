using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Spacifications;

namespace Talabat.Core.Specifications.ProductSpecification
{
    public class ProductWithBrandAndCategorySpecification : BaseSpecification<Product>
    {
        public ProductWithBrandAndCategorySpecification(ProductSpecParams specParams) : base
            (p => (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                 (!specParams.BrandId.HasValue || p.BrandId == specParams.BrandId.Value) &&
                 (!specParams.CategoryId.HasValue || p.CategoryId == specParams.CategoryId.Value)
            )
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc": AddOrderBy(p => p.Price); break;
                    case "priceDesc": AddOrderByDescending(p => p.Price); break;
                    default: AddOrderBy(p => p.Name); break;
                }
            }
            else
            {
                AddOrderBy(p => p.Name);
            }

            ApplyPagination((specParams.PageIndex - 1) * (specParams.PageSize), specParams.PageSize);
        }

        public ProductWithBrandAndCategorySpecification(int id) : base(p => p.Id == id)
        {

            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Category);
        }
    }
}
